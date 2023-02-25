using BlazorApp.Server.Infrastructure;
using BlazorApp.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("files")]
    public class StorageAccountController : ControllerBase
    {
        private readonly AzureStorageService _storageService;

        public StorageAccountController(AzureStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet]
        public async Task<List<FileDto>> Index()
        {
            return await _storageService.GetFileListAsync();
        }

        [HttpPost]
        public async Task<CreatedResult> Create(IFormFile file)
        {
            await _storageService.StoreFileAsync(file.OpenReadStream(), file.FileName);

            return Created("files", file.FileName);
        }

        [HttpGet("{filename}")]
        public async Task<FileStreamResult> DownloadFile(string fileName)
        {
            var stream = await _storageService.GetFileAsync(fileName);
            return new FileStreamResult(stream, "application/octet-stream");
        }


        [HttpDelete("{filename}")]
        public async Task<IActionResult> Remove(string fileName)
        {
            await _storageService.DeleteFileAsync(fileName);
            return Ok();
        }

    }
}