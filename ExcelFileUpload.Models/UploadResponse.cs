using ExcelFileUpload.API.Models.Data;

namespace ExcelFileUpload.API.Models.DTO {
    public class UploadResponse {
        public List<Position>? Positions { get; set; }
        public List<string>? Errors { get; set; }
        public Dictionary<int, string>? DataErrors { get; set; }
        public bool IsFileValid { get; set; }
        public double CompletionTime { get; set; }

    }
}
