using BlazorApp.Server.DbContext;
using BlazorApp.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("data")]
    public class ExampleTextDataController : ControllerBase
    {
        private readonly BlazorAppDbContext _context;

        public ExampleTextDataController(BlazorAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<ExampleTextDataDto>> Index()
        {
            var result = _context.ExampleTextData
                .OrderByDescending(etd => etd.Created)
                .ToList();

            return result
                .Select(r => new ExampleTextDataDto(r))
                .ToList();
        }

        [HttpPost]
        public async Task<CreatedResult> Create([FromBody] ExampleTextDataDto dto)
        {
            var model = dto.ToModel();
            _context.ExampleTextData.Add(model);
            await _context.SaveChangesAsync();

            return Created("data", model.Id);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var result = _context.ExampleTextData.SingleOrDefault(etd => etd.Id == id);
            if (result == null)
            {
                return new NotFoundResult();
            }

            _context.ExampleTextData.Remove(result);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}