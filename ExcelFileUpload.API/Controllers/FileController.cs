using ExcelFileUpload.API.Models.Domain;
using ExcelFileUpload.API.Models.DTO;
using ExcelFileUpload.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Diagnostics;

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
        public async Task<IActionResult> Upload(IFormFile files) {

            var watch = new Stopwatch();
            watch.Start();

            var fileDTO = new ExcelFileDTO() {
                FormFile = files,
                FileName = files.FileName
            };

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
                 
                var response = await fileRepository.Upload(file);
                
                watch.Stop();
                
                var completionTime = watch.ElapsedMilliseconds / 60000.0;

                response.CompletionTime = completionTime;
                     
                return Ok(response);
          
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
