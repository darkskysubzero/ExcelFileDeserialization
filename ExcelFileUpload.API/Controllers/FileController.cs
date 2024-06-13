using ExcelFileUpload.API.Models.Domain;
using ExcelFileUpload.API.Models.DTO;
using ExcelFileUpload.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Upload([FromForm] ExcelFileDTO fileDTO) {

            var watch = new Stopwatch();
            watch.Start();

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

                var sheetResponse = await fileRepository.Upload(file);

                return Ok(sheetResponse);
                //if(sheetResponse.SheetData != null) {

                //    watch.Stop();
                //    var completionTime = watch.ElapsedMilliseconds / 60000.0;

                //    var response = new UploadResponse {
                //        Positions = positions,
                //        CompletionTime = completionTime
                //    };

                //    return Ok(response);
                //}
                //else {
                //    return NotFound("No positions found in the uploaded Excel file.");
                //}
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
