using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Proyecto3.Controllers
{
    [Route("api")]
    [ApiController]
    public class HuffmanController : ControllerBase
    {
        // GET: api/Huffman
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Jose Daniel Giron", "Cristian Josue Barrientos" };
        }

        // POST: api/Huffman
        [HttpPost("compress/{name}")]
        public IActionResult Compress([FromBody] string name, IFormFile file)
        {
            return Ok();
        }

        // PUT: api/Huffman/5
        [HttpPost("decompress")]
        public IActionResult Decompress([FromBody] IFormFile file)
        {
            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpGet("compressions")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
