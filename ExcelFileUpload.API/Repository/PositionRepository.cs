using ExcelFileUpload.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ExcelFileUpload.API.Repository {
    public class PositionRepository : IPositionRepository {
        private readonly ExcelFileDbtestContext context;

        public PositionRepository(ExcelFileDbtestContext context) {
            this.context = context;
        }

        public async Task<List<Position>?> GetAllPositionsAsync() {
            return await context.Positions.ToListAsync();
        }
    }
}
