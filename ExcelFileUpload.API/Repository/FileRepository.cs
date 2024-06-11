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

        public async Task<ExcelFile> Upload(ExcelFile file) {
              
            // Read file content into a memory stream
            using (MemoryStream memoryStream = new MemoryStream()) {
                await file.FormFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Importing excel data from memory stream
                var products = ImportExcel<Product>(memoryStream, "Products");

                products.ForEach(x => Console.WriteLine(x.Name));

                return file;
            }
             


        }

        public List<T> ImportExcel<T>(Stream fileStream, string sheetName) {

            List<T> list = new List<T>();
            Type typeOfObject = typeof(T);

            using(IXLWorkbook workbook = new XLWorkbook(fileStream)) {

                var worksheet = workbook.Worksheets.Where(w=>w.Name==sheetName).First();
                var properties = typeOfObject.GetProperties();

                // Header column texts
                var columns = worksheet.FirstRow().Cells().Select((v, i) => new { Value = v.Value, Index = i + 1 });

                // Skip first row because header
                foreach(IXLRow row in worksheet.RowsUsed().Skip(1)){
                    T obj = (T)Activator.CreateInstance(typeOfObject);

                    foreach (var property in properties) {
                        int colIndex = columns.SingleOrDefault(c => c.Value.ToString() == property.Name.ToString()).Index;
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
                            // You can log a warning or take other actions here
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