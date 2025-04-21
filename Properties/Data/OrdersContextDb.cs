using System.Data;
using Microsoft.Extensions.Options;
using MyApi.Helper;
using MyApi.Models;

namespace MyApi.Data{
    public class OrdersContextDb{
        DatabaseContext? _dbContext;

        public OrdersContextDb(IConfiguration configuration){
            this._dbContext = new DatabaseContext(configuration);
        }

        public IEnumerable<Order> GetOrders(){
            string sql = @"select * from get_orders()"; 
            List<Order> orders = new List<Order>();
            
            var db = this._dbContext?.ExecuteReader<Order>(
                sql: sql,
                fetcher: (reader) => {
                    var order = new Order{
                                Id = reader.Get<int>("id"),
                                ProductId = reader.Get<int>("product_id"),
                                Description =  reader.Get<string>("description"),
                                Quantity = reader.Get<int>("quantity"),
                                Amount = reader.Get<decimal>("amount"),
                                Total = reader.Get<decimal>("total"),
                                OrderDate = reader.Get<DateTime>("order_date")
                            };
                    orders.Add(order);
                    return order;
            });

            return orders;
        }

        public IEnumerable<Order> GetInterests(){
            string sql = @"select * from get_interests()"; 
            List<Order> orders = new List<Order>();
            
            var db = this._dbContext?.ExecuteReader<Order>(
                sql: sql,
                fetcher: (reader) => {
                    var order = new Order{
                                Id = reader.Get<int>("id"),
                                ProductId = reader.Get<int>("product_id"),
                                Description =  reader.Get<string>("description"),
                                Quantity = reader.Get<int>("quantity"),
                                Amount = reader.Get<decimal>("amount"),
                                Price = reader.Get<decimal>("price"),
                                Interest = reader.Get<decimal>("interest"),
                                Total = reader.Get<decimal>("total"),
                                OrderDate = reader.Get<DateTime>("order_date")
                            };
                    orders.Add(order);
                    return order;
            });

            return orders;
        }

        public IEnumerable<Inventory> GetInventory(){
            string sql = @"select * from get_inventory()"; 
            List<Inventory> items = new List<Inventory>();
            
            var db = this._dbContext?.ExecuteReader<Inventory>(
                sql: sql,
                fetcher: (reader) => {
                    var item = new Inventory{
                                ProductId = reader.Get<int>("product_id"),
                                ProductName =  reader.Get<string>("product_name"),
                                Description =  reader.Get<string>("description"),
                                Quantity = reader.Get<int>("quantity"),
                                OrderQuantity = reader.Get<int>("order_quantity"),
                                Price = reader.Get<decimal>("price"),
                                InventoryQuantity = reader.Get<int>("inventory_quantity"),
                                Total = reader.Get<decimal>("total"),
                                OrderDate = reader.Get<DateTime>("order_date")
                            };
                    items.Add(item);
                    return item;
            });

            return items;
        }

        public Order? GetOrder(int id){
            string sql = @"select * from get_order(@v_id)"; 
            
            return this._dbContext?.ExecuteReader<Order>(
                sql: sql,
                paramValues: new object[]{id},
                fetcher: (reader) => {
                    return new Order{
                                Id = reader.Get<int>("id"),
                                ProductId = reader.Get<int>("product_id"),
                                Description =  reader.Get<string>("description"),
                                Quantity = reader.Get<int>("quantity"),
                                Amount = reader.Get<decimal>("amount"),
                                Total = reader.Get<decimal>("total"),
                                OrderDate = reader.Get<DateTime>("order_date")
                            };
            }).FirstOrDefault();
        }

        public void InsertOrder(Order order){
            string sql = @"select insert_order(@v_product_id, @v_quantity, @v_amount, @v_order_date)"; 

            this._dbContext?.ExecuteNonQuery(
                sql: sql,
                paramValues: new object[]{
                    order.ProductId,
                    order.Quantity,
                    order.Amount,
                    order.OrderDate
                }
            );
        }

        public void DeleteOrder(int id){
            string sql = @"select delete_order(@v_id)"; 

            this._dbContext?.ExecuteNonQuery(
                sql: sql,
                paramValues: new object[]{id}
            );
        }

        public void UpdateOrder(Order order){
            string sql = @"select update_order(@v_id, @v_product_id, @v_quantity, @v_amount, @v_order_date)"; 

            this._dbContext?.ExecuteNonQuery(
                sql: sql,
                paramValues: new object[]{
                    order.Id,
                    order.ProductId,
                    order.Quantity,
                    order.Amount,
                    order.OrderDate
                }
            );
        }

    }
}