using KKLProject.DBContext;
using KKLProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KKLProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JsonDataController : ControllerBase
    {
        private readonly KKLDbContext _context;

        public JsonDataController(KKLDbContext context)
        {
            _context = context;
        }

        [HttpPost("store-json")]
        public async Task<IActionResult> StoreJson([FromBody] string jsonPayload)
        {
            var jsonData = new JsonData { LargeJson = jsonPayload };
            _context.JsonData.Add(jsonData);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "JSON data saved successfully", Id = jsonData.Id });
        }

        [HttpPost("upload-json-file")]
        public async Task<IActionResult> UploadJsonFile(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var streamReader = new StreamReader(jsonFile.OpenReadStream()))
            {
                // Read the content of the file
                string jsonPayload = await streamReader.ReadToEndAsync();

                // Deserialize if needed or store directly
                var jsonData = new JsonData { LargeJson = jsonPayload };

                _context.JsonData.Add(jsonData);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "JSON data saved successfully", Id = jsonData.Id });
            }
        }
    }

}
