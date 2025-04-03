using Microsoft.AspNetCore.Authorization;
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

        public ProductsController(IConfiguration configuration){
            db = new ProductsContextDb(configuration); 
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
            var editProduct = db.GetProduct(id);
            if (editProduct == null){
                return NotFound();
            }
            else{
                product.ProductId = editProduct.ProductId;
                db.UpdateProduct(product: product);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id){
            var product = db.GetProduct(id);
            if (product == null)
                return NotFound();
            else
                db.DeleteProduct(id);
                return Ok();
        }
    }
}