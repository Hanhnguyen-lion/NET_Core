using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyApi.Models;
using Npgsql;

namespace MyApi.Data{
    public class OrdersContextDb{
        string? _connString = null;

        public OrdersContextDb(){
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            _connString = OrdersContextDb.ConnectionStringSection("PostgressConnectionDb");
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

        public IEnumerable<Order> GetOrders(){
            string sql = @"select * from get_orders()"; 
            List<Order> orders = new List<Order>();
            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    using(var reader = cmd.ExecuteReader()){
                        while(reader.Read()){
                            orders.Add(new Order{
                                Id = reader.GetInt32("id"),
                                ProductId = reader.GetInt32("product_id"),
                                Description = reader.GetString("description"),
                                Quantity = reader.GetInt32("quantity"),
                                Amount = reader.GetDecimal("amount"),
                                Total = reader.GetDecimal("total"),
                                OrderDate = reader.GetDateTime("order_date")
                            });
                        }
                    }
                }
            }

            return orders;
        }
        public Order? GetOrder(int id){
            string sql = @"select * from get_order(:v_id)"; 
            Order? order = null;

            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("v_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    cmd.Parameters[0].Value = id;

                    using(var reader = cmd.ExecuteReader()){
                        while(reader.Read()){
                        order = new Order{
                                Id = reader.GetInt32("id"),
                                ProductId = reader.GetInt32("product_id"),
                                Description = reader.GetString("description"),
                                Quantity = reader.GetInt32("quantity"),
                                Amount = reader.GetDecimal("amount"),
                                Total = reader.GetDecimal("total"),
                                Interest = reader.GetDecimal("interest"),
                                OrderDate = reader.GetDateTime("order_date")
                            };
                        }
                    }
                }
            }

            return order;
        }

        public void InsertOrder(Order order){
            string sql = @"select insert_order(:v_product_id, :v_quantity, :v_amount, :v_order_date)"; 
            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("v_product_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters.Add(new NpgsqlParameter("v_quantity", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters.Add(new NpgsqlParameter("v_amount", NpgsqlTypes.NpgsqlDbType.Numeric));
                    cmd.Parameters.Add(new NpgsqlParameter("v_order_date", NpgsqlTypes.NpgsqlDbType.Date));

                    cmd.Parameters[0].Value = order.ProductId;
                    cmd.Parameters[1].Value = order.Quantity;
                    cmd.Parameters[2].Value = order.Amount;
                    cmd.Parameters[3].Value = order.OrderDate;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteOrder(int id){
            string sql = @"select delete_order(:v_id)"; 
            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("v_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters[0].Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateOrder(Order order){
            string sql = @"select update_order(:v_id, :v_product_id, :v_quantity, :v_amount, :v_order_date)"; 
            using (var conn = new NpgsqlConnection(connectionString: this._connString)){
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn)){
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("v_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters.Add(new NpgsqlParameter("v_product_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters.Add(new NpgsqlParameter("v_quantity", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters.Add(new NpgsqlParameter("v_amount", NpgsqlTypes.NpgsqlDbType.Numeric));
                    cmd.Parameters.Add(new NpgsqlParameter("v_order_date", NpgsqlTypes.NpgsqlDbType.Date));
                    cmd.Parameters[0].Value = order.Id;
                    cmd.Parameters[1].Value = order.ProductId;
                    cmd.Parameters[2].Value = order.Quantity;
                    cmd.Parameters[3].Value = order.Amount;
                    cmd.Parameters[4].Value = order.OrderDate;
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}