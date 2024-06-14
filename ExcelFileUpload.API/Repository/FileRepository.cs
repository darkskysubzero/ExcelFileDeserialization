using ClosedXML;
using ExcelFileUpload.API.Models.Data;
using ClosedXML.Excel;
using ExcelFileUpload.API.Models.DTO;
using ExcelFileUpload.API.Models;

namespace ExcelFileUpload.API.Repository
{
    public class FileRepository : IFileRepository {

        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FileRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor) {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<SheetValidationResponse> Upload(ExcelFile file) {
              
            // Read file content into a memory stream
            using (MemoryStream memoryStream = new MemoryStream()) {
                await file.FormFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Importing excel data from memory stream
                var sheetResponse = ImportExcel(memoryStream, "Data");
                 
                return sheetResponse;
            } 
        }

        public SheetValidationResponse ImportExcel(Stream fileStream, string sheetName) {
            // Parsing Validations
            /*
             * 01 - Invalid sheet name
             * 02 - Columns match
             * 03 - Able to parse every row 
             */

            SheetValidationResponse response = new SheetValidationResponse();

            Type typeOfObject = typeof(Models.Data.Position);

            using (IXLWorkbook workbook = new XLWorkbook(fileStream)) {
                var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == sheetName);

                // 01 - Invalid sheet name
                if (worksheet == null) { 
                    response.Errors!.Add($"Invalid Sheet : Sheet  \"{sheetName}\" not found.");
                    return response; 
                }

                // Adding a Id column manually because missing in spreadsheet
                var missingCell = worksheet.Cell(2, 1);
                if(string.IsNullOrWhiteSpace(missingCell.Value.ToString())) {
                    missingCell.Value = "Id";
                }

                var properties = typeOfObject.GetProperties();
                 
                // 02 - Column count does not match
                if (worksheet.LastColumnUsed().ColumnNumber() != 31) {
                    response.Errors!.Add("Invalid Sheet : Sheet column count does not match");
                    return response;
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
                      {"PositionType","Position Type:  \r\nPermanent / Fixed Term Contract" },
                      {"PositionActiveDate", "Position Active Date"},
                      {"PoolManager", "Pool Manager"},
                      {"TopSiteManager", "TopSite Manager"},
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

                // 03 - checking if every row is converted properly
                response.IsSheetValid = true;

                foreach (IXLRow row in worksheet.RowsUsed().Skip(2)) { // Skip header rows
                    Models.Data.Position obj = (Models.Data.Position)Activator.CreateInstance(typeOfObject);


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
                            // Create an an error message here
                            response.Errors!.Add($"Conversion Failed : Row[{row.RowNumber()}] - Col[{colIndex}] Cell[{row.Cell(colIndex)}]");
                            response.IsSheetValid = false;
                        }
                    }

                    response.SheetData!.Add(obj);
                }
            }

            return response;
        }

        public async Task CopyFile(ExcelFile file) {
            // Creating a path
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Files", $"{file.FileName}{file.FileExtension}");

            // File stream
            using var stream = new FileStream(localFilePath, FileMode.Create);

            await file.FormFile.CopyToAsync(stream);
        }

         
    }
}
