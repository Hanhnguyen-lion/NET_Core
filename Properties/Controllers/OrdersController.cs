using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers{
    [Route("api/shopping/orders")]
    [ApiController]
    public class OrdersController: ControllerBase{

        OrdersContextDb db; 
        public OrdersController(IConfiguration configuration){
            db = new OrdersContextDb(configuration); 
        }

        [HttpGet] 
        public ActionResult<IEnumerable<Order>> Get(){
            List<Order> orders = db.GetOrders().ToList<Order>();
            return Ok(orders);
        }

        [HttpGet("GetInterests")] 
        public ActionResult<IEnumerable<Order>> GetInterests(){
            List<Order> orders = db.GetInterests().ToList<Order>();
            return Ok(orders);
        }

        [HttpGet("Inventories")] 
        public ActionResult<IEnumerable<Inventory>> GetInventories(){
            List<Inventory> items = db.GetInventory().ToList<Inventory>();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public ActionResult<Order> Get(int id){
            var order = db.GetOrder(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Order order){
            db.InsertOrder(order);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult<IEnumerable<Order>> Put(int id, [FromBody] Order order){
            db.UpdateOrder(order: order);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id){
            db.DeleteOrder(id);
            return Ok();
        }
    }
}