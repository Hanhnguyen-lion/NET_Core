using Microsoft.Extensions.Options;
using MyApi.Helper;
using MyApi.Models;

namespace MyApi.Data{
    public class ProductsContextDb{
        DatabaseContext? _dbContext;

        public ProductsContextDb(IConfiguration configuration){
            this._dbContext = new DatabaseContext(config: configuration);
        }

        public IEnumerable<Product> GetProducts(){
            string sql = @"select * from get_products()"; 
            List<Product> products = new List<Product>();

            var db = this._dbContext?.ExecuteReader<Product>(
                sql: sql,
                fetcher: (reader) => {
                    var product = new Product{
                                ProductId = reader.Get<int>("product_id"),
                                ProductName = (string) reader["product_name"],
                                Description =  reader.Get<string>("description"),
                                Quantity = reader.Get<int>("quantity"),
                                Price = reader.Get<decimal>("price"),
                                Total = reader.Get<decimal>("total"),
                                OrderDate = reader.Get<DateTime>("order_date")
                            };
                    products.Add(product);
                    return product;
                });

            return products;
        }
        public Product? GetProduct(int productId){
            string sql = @"select * from get_product(@v_product_id)"; 

            return this._dbContext?.ExecuteReader(
                sql: sql,
                paramValues: new object[]{productId},
                fetcher: (reader) => {
                    return new Product{
                                ProductId = reader.Get<int>("product_id"),
                                ProductName = reader.Get<string>("product_name"),
                                Description = reader.Get<string>("description"),
                                Quantity = reader.Get<int>("quantity"),
                                Price = reader.Get<decimal>("price"),
                                Total = reader.Get<decimal>("total"),
                                OrderDate = reader.Get<DateTime>("order_date")
                            };
                }).FirstOrDefault();

        }

        public void InsertProduct(Product product){
            string sql = @"select insert_product(@v_product_name, @v_description, @v_quantity, @v_price, @v_order_date)"; 

            this._dbContext?.ExecuteNonQuery(
                sql: sql,
                paramValues: new object[]{
                    product.ProductName,
                    product.Description,
                    product.Quantity,
                    product.Price,
                    product.OrderDate
                }
            );
        }

        public void DeleteProduct(int productId){
            string sql = @"select delete_product(@v_product_id)"; 
            this._dbContext?.ExecuteNonQuery(
                sql: sql,
                paramValues: new object[]{productId}
            );
        }

        public void UpdateProduct(Product product){
            string sql = @"select update_product(@v_product_id, @v_product_name, @v_description, @v_quantity, @v_price, @v_order_date)"; 
            this._dbContext?.ExecuteNonQuery(
                sql: sql,
                paramValues: new object[]{
                    product.ProductId,
                    product.ProductName,
                    product.Description,
                    product.Quantity,
                    product.Price,
                    product.OrderDate
                }
            );
        }

    }
}