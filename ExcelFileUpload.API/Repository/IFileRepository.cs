using ExcelFileUpload.API.Models.Data;
using ExcelFileUpload.API.Models.Domain;
using ExcelFileUpload.API.Models.DTO;

namespace ExcelFileUpload.API.Repository {
    public interface IFileRepository {
        Task<SheetValidationResponse> Upload(ExcelFile file);
    }
}
