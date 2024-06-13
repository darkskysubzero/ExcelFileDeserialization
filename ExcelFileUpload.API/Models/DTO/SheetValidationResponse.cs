using ExcelFileUpload.API.Models.Data;

namespace ExcelFileUpload.API.Models.DTO {
    public class SheetValidationResponse {
        public List<Position>? SheetData { get; set; } = new List<Position>();
        public bool IsSheetValid { get; set; } = false;
        public List<string>? Errors { get; set; } = new List<string>();
    }
}
