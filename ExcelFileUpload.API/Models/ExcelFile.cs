using System.ComponentModel.DataAnnotations;

namespace ExcelFileUpload.API.Models
{
    public class ExcelFile
    {
        public int Id { get; set; }
        public IFormFile FormFile { get; set; }
        [Required]
        public string FileName { get; set; }
        public string? FileDescription { get; set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
        public string FilePath { get; set; }

    }
}
