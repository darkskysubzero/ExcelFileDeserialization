using ExcelFileUpload.API.Models.Domain;
using ClosedXML;
using ExcelFileUpload.API.Models.Data;
using ClosedXML.Excel;
using Azure;
using ExcelFileUpload.API.Models.DTO;

namespace ExcelFileUpload.API.Repository {
    public class FileRepository : IFileRepository {

        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FileRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor) {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<UploadResponse> Upload(ExcelFile file) {
            // Directory path
            var directoryPath = Path.Combine(webHostEnvironment.ContentRootPath, "Files");

            try {
                // Delete all files in the directory
                var files = Directory.GetFiles(directoryPath);
                foreach (var filePath in files) {
                    File.Delete(filePath);
                }

                // Create a new file path
                var localFilePath = Path.Combine(directoryPath, $"{file.FileName}");

                // Open a FileStream to write the uploaded file to the server
                using (FileStream fileStream = new FileStream(localFilePath, FileMode.Create)) {
                    // Copy uploaded file content to FileStream asynchronously
                    await file.FormFile.CopyToAsync(fileStream);

                    // Reset fileStream position to the beginning
                    fileStream.Seek(0, SeekOrigin.Begin);

                    // Import Excel data from fileStream
                    List<string> errors;
                    var positions = ImportExcel<Position>(fileStream, "Data", out errors);
                     

                    UploadResponse response = new UploadResponse() {
                        Positions = positions,
                        Errors = errors,
                        CompletionTime = 0.00
                    };

                    return response;
                }


            }
            catch (Exception ex) {
                Console.WriteLine($"Error uploading or processing file: {ex.Message}");
                return null;
            }
        }

        public List<T> ImportExcel<T>(Stream fileStream, string sheetName, out List<string> errors) {
            List<T> list = new List<T>();
            errors = new List<string>();
            Type typeOfObject = typeof(T);

            using (IXLWorkbook workbook = new XLWorkbook(fileStream)) {
                var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == sheetName);
                if (worksheet == null) {
                    errors.Add($"Sheet '{sheetName}' not found.");
                    return list; // Return empty list if sheet not found
                }

                // Adding a Id column manually because missing in spreadsheet
                var missingCell = worksheet.Cell(2, 1);
                if(string.IsNullOrWhiteSpace(missingCell.Value.ToString())) {
                    missingCell.Value = "Id";
                }

                // Renaming cells
                worksheet.Cell(2, 22).Value = "Position Type"; 
                worksheet.Cell(2, 25).Value = "Top Site Manager";

                var properties = typeOfObject.GetProperties();

                int columnCount = worksheet.LastColumnUsed().ColumnNumber();
                // Validate column count
                if (columnCount != 31) {
                    errors.Add("Invalid Sheet: Sheet column count does not match expected 31 columns.");
                    return list;
                }


                // Converting columns to dictionary (value and index)
                var columns = worksheet.Row(2).Cells().Select((v, i) => new { Value = v.Value, Index = i + 1 })
                .ToDictionary(c => c.Value.ToString(), c => c.Index);
                 

                // Define a dictionary to map property names to column names
                Dictionary<string, string> propertyNameToColumnName = new Dictionary<string, string> {
                      {"Id", "Id"},
                      {"OrgNumber", "Org Number"},
                      {"Client", "Client"},
                      {"Site", "Site"},
                      {"Building", "Building"},
                      {"Area", "Area"},
                      {"ProcessName", "Process Name"},
                      {"PositionTitle", "Position Title"},
                      {"PositionDescription", "Position Description"},
                      {"EmployeeNumber", "Employee Number (payroll number)"},
                      {"EmployeeName", "Name"},
                      {"EmployeeSurname", "Surname"},
                      {"EmployeeDisplayName", "Known as Name"},
                      {"EmployeeIDNumber", "ID Number"},
                      {"BaseShift", "Base Shift"},
                      {"NewBaseShift", "New Base Shift"},
                      {"ShiftStartEndTime", "Shift Start & End Time"},
                      {"ShiftType", "Shift Type: Variable Fixed Rotating"},
                      {"SalaryWage", "Salary / Wage"},
                      {"ReportsToPosition", "Reports to position"},
                      {"TAManager", "T&A Manager"},
                      {"PositionType","Position Type" },
                      {"PositionActiveDate", "Position Active Date"},
                      {"PoolManager", "Pool Manager"},
                      {"TopSiteManager", "Top Site Manager"},
                      {"PrismaUser", "PRISMA USER (YES/NO)"},
                      {"EmailToCreatePrismaUsers", "Email / to create Prisma users"},
                      {"ConfirmAttendance", "Confirm Attendance (YES/NO)"},
                      {"WorkOnOrgchart", "Work on the Org Chart (YES/NO)"},
                      {"CompleteParticipateWorkflow", "Complete/participate in workflow (YES/NO)"},
                      {"MHERequest", "MHE Req."}
                  };

                // Cache the column indices for properties
                Dictionary<string, int> propertyToColumnIndex = properties
                    .Where(p => propertyNameToColumnName.ContainsKey(p.Name))
                    .ToDictionary(
                        p => p.Name,
                        p => columns.ContainsKey(propertyNameToColumnName[p.Name]) ? columns[propertyNameToColumnName[p.Name]] : 1
                    );

                foreach (IXLRow row in worksheet.RowsUsed().Skip(1)) { // Skip header rows
                    T obj = (T)Activator.CreateInstance(typeOfObject);

                    foreach (var property in properties) {

                        var colIndex = propertyToColumnIndex[property.Name];

                        var cellValue = row.Cell(colIndex).Value;
                        var targetType = property.PropertyType;

                        // Convert cell value to property type
                        object convertedValue = null;
                        if (targetType == typeof(string)) {
                            convertedValue = cellValue.ToString();
                        }
                        else if (targetType == typeof(int)) {
                            int intValue;
                            if (int.TryParse(cellValue.ToString(), out intValue)) {
                                convertedValue = intValue;
                            }
                        }
                        else if (targetType == typeof(decimal)) {
                            decimal decimalValue;
                            if (decimal.TryParse(cellValue.ToString(), out decimalValue)) {
                                convertedValue = decimalValue;
                            }
                        }
                        else if (targetType == typeof(DateTime)) {
                            DateTime dateTimeValue;
                            if (DateTime.TryParse(cellValue.ToString(), out dateTimeValue)) {
                                convertedValue = dateTimeValue;
                            }
                        }

                        // Add more conditions for other types as needed...

                        if (convertedValue != null) {
                            property.SetValue(obj, convertedValue);
                        }
                        else {
                            // Handle unsupported types or failed conversions 
                            Console.WriteLine("Conversion failed!");
                        }
                    }

                    list.Add(obj);
                }
            }

            return list;
        }

        public async Task<FileStream> CopyFile(ExcelFile file) {
            // Creating a path
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Files", $"{file.FileName}{file.FileExtension}");

            // File stream
            using var stream = new FileStream(localFilePath, FileMode.Create);

            await file.FormFile.CopyToAsync(stream);

            return stream;
        }

         
    }
}
