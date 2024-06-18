using ExcelFileUpload.API.Models.Data;
using ExcelFileUpload.API.Models.Domain;

namespace ExcelFileUpload.API.Repository {
    public interface IFileRepository {
        Task<List<Position>?> Upload(ExcelFile file);
    }
}
