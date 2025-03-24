using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyApi.Models;
using Npgsql;

namespace MyApi.Data{
    public class ProductsContextDb{
        string? _connString = null;

        public ProductsContextDb(){
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            _connString = ProductsContextDb.ConnectionStringSection("PostgressConnectionDb");
        }

        static string ConnectionStringSection(string connSetting, string appSettings="appSettings.json")
        {
            var objBuilder = new ConfigurationBuilder()
                                                        .SetBasePath(Directory.GetCurrentDirectory())
                                                        .AddJsonFile(appSettings, optional: false, reloadOnChange: true);
            IConfiguration conManager = objBuilder.Build();
        
            var conn = conManager.GetConnectionString(connSetting);

            if (conn == null)
            {
                conn = "";
            }

            return conn.ToString();
        }

        public IEnumerable<Product> GetProducts(){
            string sql = @"select * from get_products()"; 
            List<Product> products = new List<Product>();
            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    using(var reader = cmd.ExecuteReader()){
                        while(reader.Read()){
                            products.Add(new Product{
                                ProductId = reader.GetInt32("product_id"),
                                ProductName = reader.GetString("product_name"),
                                Description = reader.GetString("description"),
                                Quantity = reader.GetInt32("quantity"),
                                Price = reader.GetDecimal("price"),
                                Total = reader.GetDecimal("total"),
                                OrderDate = reader.GetDateTime("order_date")
                            });
                        }
                    }
                }
            }

            return products;
        }
        public Product? GetProduct(int productId){
            string sql = @"select * from get_product(:v_product_id)"; 
            Product? product = null;

            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("v_product_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    cmd.Parameters[0].Value = productId;

                    using(var reader = cmd.ExecuteReader()){
                        while(reader.Read()){
                        product = new Product{
                                ProductId = reader.GetInt32("product_id"),
                                ProductName = reader.GetString("product_name"),
                                Description = reader.GetString("description"),
                                Quantity = reader.GetInt32("Quantity"),
                                Price = reader.GetDecimal("price"),
                                Total = reader.GetDecimal("total"),
                                OrderDate = reader.GetDateTime("order_date")
                            };
                        }
                    }
                }
            }

            return product;
        }

        public void InsertProduct(Product product){
            string sql = @"select insert_product(:v_product_name, :v_description, :v_quantity, :v_price, :v_order_date)"; 
            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("v_product_name", NpgsqlTypes.NpgsqlDbType.Varchar));
                    cmd.Parameters.Add(new NpgsqlParameter("v_description", NpgsqlTypes.NpgsqlDbType.Varchar));
                    cmd.Parameters.Add(new NpgsqlParameter("v_quantity", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters.Add(new NpgsqlParameter("v_price", NpgsqlTypes.NpgsqlDbType.Numeric));
                    cmd.Parameters.Add(new NpgsqlParameter("v_order_date", NpgsqlTypes.NpgsqlDbType.Date));

                    cmd.Parameters[0].Value = product.ProductName;
                    cmd.Parameters[1].Value = product.Description;
                    cmd.Parameters[2].Value = product.Quantity;
                    cmd.Parameters[3].Value = product.Price;
                    cmd.Parameters[4].Value = product.OrderDate;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProduct(int productId){
            string sql = @"select delete_product(:v_product_id)"; 
            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("v_product_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters[0].Value = productId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProduct(Product product){
            string sql = @"select update_product(:v_product_id, :v_product_name, :v_description, :v_quantity, :v_price, :v_order_date)"; 
            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("v_product_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters.Add(new NpgsqlParameter("v_product_name", NpgsqlTypes.NpgsqlDbType.Varchar));
                    cmd.Parameters.Add(new NpgsqlParameter("v_description", NpgsqlTypes.NpgsqlDbType.Varchar));
                    cmd.Parameters.Add(new NpgsqlParameter("v_quantity", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters.Add(new NpgsqlParameter("v_price", NpgsqlTypes.NpgsqlDbType.Numeric));
                    cmd.Parameters.Add(new NpgsqlParameter("v_order_date", NpgsqlTypes.NpgsqlDbType.Date));
                    cmd.Parameters[0].Value = product.ProductId;
                    cmd.Parameters[1].Value = product.ProductName;
                    cmd.Parameters[2].Value = product.Description;
                    cmd.Parameters[3].Value = product.Quantity;
                    cmd.Parameters[4].Value = product.Price;
                    cmd.Parameters[5].Value = product.OrderDate;
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}