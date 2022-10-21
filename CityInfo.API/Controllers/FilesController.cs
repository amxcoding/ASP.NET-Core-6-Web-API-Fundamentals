using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            this._fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
        }

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            // Others Ways to send a file: 
            // FileContentResult
            // FileStreamResult
            // PhysicalFileResult
            // VirtualFileResult

            // Change file settings so it is coppied to the output directory
            // For demo the path is hardcoded, real life scenario -> use file id to fetch correct file path
            var pathToFile = "./Resources/creating-the-api-and-returning-resources-slides.pdf";

            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound();
            }

            if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType)) // out asigns value to contentType if found
            {
                contentType = "application/octet-stream"; // if cant be determined set it it to this
            }

            var file = System.IO.File.ReadAllBytes (pathToFile);
            return File(file, contentType, Path.GetFileName(pathToFile));

        }
    }
}
