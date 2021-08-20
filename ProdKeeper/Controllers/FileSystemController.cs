using Microsoft.AspNetCore.Mvc;
using ProdKeeper.Entity.Data;
using ProdKeeper.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProdKeeper.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class FileSystemController : ControllerBase
    {
        ApplicationDbContext _context;
        public FileSystemController(ApplicationDbContext context)
        {
            this._context = context;
        }
        // GET: api/<FileSystemController>
        [HttpGet("{*path}")]
        public IEnumerable<string> Get()
        {
            var path=this.ControllerContext.RouteData.Values["path"].ToString();
            FileSystemService fs= new FileSystemService(new FileSystemOption( this._context));
            var val = fs.GetFiles(path);
            var val2 = fs.GetFolders(path);
            return null;
            
        }

        // GET api/<FileSystemController>/5


        // POST api/<FileSystemController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FileSystemController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FileSystemController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
