﻿using ExcelFileUpload.API.Models.Domain;
using ClosedXML;
using ExcelFileUpload.API.Models.Data;
using ClosedXML.Excel;

namespace ExcelFileUpload.API.Repository {
    public class FileRepository : IFileRepository {

        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FileRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor) {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Position>?> Upload(ExcelFile file) {
              
            // Read file content into a memory stream
            using (MemoryStream memoryStream = new MemoryStream()) {
                await file.FormFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Importing excel data from memory stream
                var positions = ImportExcel<Position>(memoryStream, "Data");
                 
                return positions;
            } 
        }

        public List<T> ImportExcel<T>(Stream fileStream, string sheetName) {
            List<T> list = new List<T>();
            Type typeOfObject = typeof(T);

            using (IXLWorkbook workbook = new XLWorkbook(fileStream)) {
                var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == sheetName);
                if (worksheet == null) throw new ArgumentException($"Sheet {sheetName} not found.");

                // Adding a Id column manually because missing in spreadsheet
                var missingCell = worksheet.Cell(2, 1);
                if(string.IsNullOrWhiteSpace(missingCell.Value.ToString())) {
                    missingCell.Value = "Id";
                }

                var properties = typeOfObject.GetProperties();
                 
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

                foreach (IXLRow row in worksheet.RowsUsed().Skip(2)) { // Skip header rows
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

        public async Task CopyFile(ExcelFile file) {
            // Creating a path
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Files", $"{file.FileName}{file.FileExtension}");

            // File stream
            using var stream = new FileStream(localFilePath, FileMode.Create);

            await file.FormFile.CopyToAsync(stream);
        }

         
    }
}
