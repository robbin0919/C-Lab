using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
namespace HelloWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        // GET: api/<HelloWorldController> 
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")  , "HelloWorld" };
        }
        // GET api/<HelloWorldController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        // POST api/<HelloWorldController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
        // PUT api/<HelloWorldController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        // DELETE api/<HelloWorldController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}