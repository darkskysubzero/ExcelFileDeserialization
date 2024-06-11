namespace ExcelFileUpload.API.Models.DTO {
    public class ExcelFileDTO {
        public IFormFile FormFile { get; set; }
        public string FileName { get; set; }
        public string? FileDescription { get; set; }
    }
}
