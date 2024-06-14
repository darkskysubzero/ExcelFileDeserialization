using ExcelFileUpload.API.Models.Domain;

namespace ExcelFileUpload.API.Repository {
    public interface IPositionRepository {
        Task<List<Position>?> GetAllPositionsAsync();
    }
}
