using System;
using System.Security.Claims;
using animalFinder.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReqFile = animalFinder.DTO.API.File;

namespace animalFinder.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Upload([FromBody] ReqFile reqFile)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;

                var bytes = Convert.FromBase64String(reqFile.Base64);
                bytes = _fileService.AsJpeg(bytes);
                bytes = _fileService.Resize(bytes, 500);
                bytes = _fileService.Compress(bytes);

                var fileGuid = _fileService.Upload(userId, bytes, reqFile.Type);
                return Ok(fileGuid.ToString());
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                var fileTuple = _fileService.Get(new Guid(id));

                return Ok(Convert.ToBase64String(fileTuple.Item1));
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
