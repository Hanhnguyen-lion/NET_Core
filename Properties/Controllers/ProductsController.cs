using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers{
    [Route("api/shopping/products")]
    [ApiController]
    public class ProductsController: ControllerBase{

        ProductsContextDb db; 
        public ProductsController(){
            db = new ProductsContextDb(); 
            // products = db.GetProducts().ToList<Product>();
        }

        [HttpGet] 
        public ActionResult<IEnumerable<Product>> Get(){
            List<Product> products = db.GetProducts().ToList<Product>();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id){
            var product = db.GetProduct(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Product product){
            db.InsertProduct(product);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult<IEnumerable<Product>> Put(int id, [FromBody] Product product){
            db.UpdateProduct(product: product);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id){
            db.DeleteProduct(id);
            return Ok();
        }
    }
}