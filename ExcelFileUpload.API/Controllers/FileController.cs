using ExcelFileUpload.API.Models.Domain;
using ExcelFileUpload.API.Models.DTO;
using ExcelFileUpload.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFileUpload.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase {
        private readonly IFileRepository fileRepository;

        public FileController(IFileRepository fileRepository) {
            this.fileRepository = fileRepository;
        }


        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ExcelFileDTO fileDTO) {
            // Validate file 
            ValidateFileUpload(fileDTO);

            // If no errors then upload to server (self)
            if(ModelState.IsValid) {
                var file = new ExcelFile {
                    FormFile = fileDTO.FormFile,
                    FileName = fileDTO.FileName,
                    FileDescription = fileDTO.FileDescription,
                    FileExtension = Path.GetExtension(fileDTO.FormFile.FileName),
                    FileSizeInBytes = fileDTO.FormFile.Length,
                };

                var products = await fileRepository.Upload(file);

                if(products!=null) {

                    return Ok(products);
                }
                else {
                    return NotFound("No products found in the uploaded Excel file.");
                }
            }


            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ExcelFileDTO fileDTO) {
            var allowedExtensions = new string[] { ".xls", ".xlsx" };

            // Getting extension from file
            if(!allowedExtensions.Contains(Path.GetExtension(fileDTO.FormFile.FileName))) {
                ModelState.AddModelError("file", "Unsupported file extension");
            }

            // If file bigger than 10mb
            if (fileDTO.FormFile.Length > 10485760) {
                ModelState.AddModelError("file", "File size is more than 10mb");
            }
        }

    }
}
