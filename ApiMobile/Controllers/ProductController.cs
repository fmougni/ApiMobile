using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace ApiMobile.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;
       
        public ProductController(IConfiguration config)
        {   
            _config = config;
            payetonkawaDBContext.connectionString = _config.GetConnectionString("DefaultConnection");

        }
        // GET: ProductController
        [HttpGet(Name = "GetProducts")]
        public async Task<string> GetAsync(string token)
        {
            if ( await payetonkawaDBContext.VerifyToken(token))
            {

                return payetonkawaDBContext.SelectAllProduct();
            }
            return "Accès refusé";
        }
  
        // GET: ProductController/{id}
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<ActionResult<string>> GetByIdAsync(int id,string token)
        {
            if (await payetonkawaDBContext.VerifyToken(token))
            {
                return payetonkawaDBContext.SelectProductById(id);
            }
            return "Accès refusé";
        }
       

    }

}
