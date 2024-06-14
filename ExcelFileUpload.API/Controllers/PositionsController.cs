using ExcelFileUpload.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFileUpload.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase {
        
        private readonly IPositionRepository repository;

        public PositionsController(IPositionRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var positions = await repository.GetAllPositionsAsync();
            return Ok(positions);
        }
    }
}
