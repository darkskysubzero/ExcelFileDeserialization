using ExcelFileUpload.API.Models.Data;

namespace ExcelFileUpload.API.Models.DTO {
    public class UploadResponse {
        public List<Position> Positions { get; set; }
        public double CompletionTime { get; set; }
    }
}
