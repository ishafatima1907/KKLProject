using KKLProject.DBContext;
using KKLProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text;

namespace KKLProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompressedDataController : ControllerBase
    {
        private readonly KKLDbContext _context;

        public CompressedDataController(KKLDbContext context)
        {
            _context = context;
        }

        //[HttpPost("store-compressed-json")]
        //public async Task<IActionResult> StoreCompressedJson([FromBody] string jsonPayload)
        //{
        //    byte[] compressedData = Compress(jsonPayload);
        //    var compressedJsonData = new CompressedData { CompressedJson = compressedData };
        //    _context.CompressedData.Add(compressedJsonData);
        //    await _context.SaveChangesAsync();
        //    return Ok(new { Message = "Compressed JSON data saved successfully", Id = compressedJsonData.Id });
        //}

        //private byte[] Compress(string data)
        //{
        //    using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
        //    using (var compressedStream = new MemoryStream())
        //    {
        //        using (var compressorStream = new GZipStream(compressedStream, CompressionMode.Compress))
        //        {
        //            uncompressedStream.CopyTo(compressorStream);
        //        }
        //        return compressedStream.ToArray();
        //    }
        //}

        [HttpPost("upload-compressed-json")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCompressedJson(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Read the file content
            using (var streamReader = new StreamReader(jsonFile.OpenReadStream()))
            {
                string jsonPayload = await streamReader.ReadToEndAsync();

                // Compress the JSON payload
                byte[] compressedData = Compress(jsonPayload);

                // Store the compressed data in the database
                var compressedJsonData = new CompressedData { CompressedJson = compressedData };
                _context.CompressedData.Add(compressedJsonData);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Compressed JSON data saved successfully", Id = compressedJsonData.Id });
            }
        }


        private byte[] Compress(string data)
        {
            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            using (var compressedStream = new MemoryStream())
            {
                using (var compressorStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    uncompressedStream.CopyTo(compressorStream);
                }
                return compressedStream.ToArray();
            }
        }


        [HttpGet("get-compressed-json/{id}")]
        public async Task<IActionResult> GetCompressedJson(int id)
        {
            var compressedData = await _context.CompressedData.FindAsync(id);

            if (compressedData == null)
            {
                return NotFound("Data not found.");


            }

            // Decompress the stored data
            string decompressedJson = Decompress(compressedData.CompressedJson);
            return Ok(decompressedJson);
        }

        private string Decompress(byte[] compressedData)
        {
            using (var compressedStream = new MemoryStream(compressedData))
            using (var decompressedStream = new MemoryStream())
            {
                using (var decompressorStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    decompressorStream.CopyTo(decompressedStream);
                }
                return Encoding.UTF8.GetString(decompressedStream.ToArray());
            }
        }

    }
}