using ExcelFileUpload.API.Models.Data;
using ExcelFileUpload.API.Models.Domain;

namespace ExcelFileUpload.API.Repository {
    public interface IFileRepository {
        Task<List<Product>?> Upload(ExcelFile file);
    }
}
