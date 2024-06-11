using ExcelFileUpload.API.Models.Domain;

namespace ExcelFileUpload.API.Repository {
    public interface IFileRepository {
        Task<ExcelFile> Upload(ExcelFile file);
    }
}
