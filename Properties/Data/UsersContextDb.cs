using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Storage;
using MyApi.Helper;
using MyApi.Models;

namespace MyApi.Data{
    public class UsersContextDb{
        
        DatabaseContext _dbContext;
        
        public UsersContextDb(IConfiguration configuration){
            this._dbContext = new DatabaseContext(configuration);
        }

        public IEnumerable<User> GetUsers(){
            IList<User> users = new List<User>();
            var engine = this._dbContext.ExecuteReader(
                sql: "SELECT * FROM get_users()",
                fetcher: (reader) =>{
                    var user = new User{
                        Id = reader.Get<int>("id"),
                        Email = reader.Get<string>("email"),
                        Password = reader.Get<string>("password"),
                        Firstname = reader.Get<string>("firstname"),
                        Lastname = reader.Get<string>("lastname")
                    };
                    users.Add(user);
                    return user;
                }
            );
            return users;
        }

        public User? GetUser(string email){
            return this._dbContext.ExecuteReader(
                sql: "SELECT * FROM get_user(@v_email)",
                paramValues: new object[]{email},
                fetcher: (reader) =>{
                    return new User{
                        Id = reader.Get<int>("id"),
                        Email = reader.Get<string>("email"),
                        Password = reader.Get<string>("password"),
                        Firstname = reader.Get<string>("firstname"),
                        Lastname = reader.Get<string>("lastname")
                    };
                }
            ).FirstOrDefault();
        }

        public Client? GetClient(string clientId){
            return _dbContext.ExecuteReader(

                sql: "select * from get_client(@v_clientid)",
                paramValues: new object[]{clientId},
                fetcher: (reader) =>{
                    return new Client{
                        ClientId = reader.Get<string>("clientid"),
                        Name = reader.Get<string>("name"),
                        ClientURL = reader.Get<string>("clienturl"),
                    };
                }

            ).FirstOrDefault();
        }

        public IList<SigningKey> SigningKeys{
            get{
                IList<SigningKey> signingKeys = new List<SigningKey>();
                var engine = _dbContext.ExecuteReader(

                    sql: "select * from get_signingkeys()",
                    fetcher: (reader) =>{
                        var signingKey = new SigningKey{
                            Id = reader.Get<int>("id"),
                            CreatedAt = reader.Get<DateTime>("created_at"),
                            ExpiresAt = reader.Get<DateTime>("expires_at"),
                            KeyId = reader.Get<string>("key_id"),
                            IsActive = reader.Get<bool>("is_active"),
                            PublicKey = reader.Get<string>("public_key"),
                            PrivateKey = reader.Get<string>("private_key")
                        };
                        signingKeys.Add(signingKey);
                        return signingKey;
                    }
                );
                return signingKeys;
            }
        }

        public void InsertUser(User user){
            this._dbContext.ExecuteNonQuery(
                sql: "SELECT insert_user(@v_email, @v_firstname, @v_lastname, @v_password)",
                paramValues: new object[]{
                    user.Email ?? "",
                    user.Firstname ?? "",
                    user.Lastname ?? "",
                    user.Password ?? ""
                });
        }

        public void UpdateUser(User user){
            this._dbContext.ExecuteNonQuery(
                sql: "SELECT update_user(@v_email, @v_firstname, @v_lastname, @v_password)",
                paramValues: new object[]{
                    user.Email ?? "",
                    user.Firstname ?? "",
                    user.Lastname ?? "",
                    user.Password ?? ""
                });
        }

        public void DeleteUser(string email){
            this._dbContext.ExecuteNonQuery(
                sql: "SELECT delete_user(@v_email)",
                paramValues: new object[]{email});
        }

        public void InsertSigningKey(SigningKey signingKey){
            this._dbContext.ExecuteNonQuery(
                sql: @"SELECT insert_signingkey(
                                                    @v_key_id, 
                                                    @v_private_key, 
                                                    @v_public_key, 
                                                    @v_is_active, 
                                                    @v_created_at, 
                                                    @v_expires_at)",
                paramValues: new object[]{
                    signingKey.KeyId ?? "",
                    signingKey.PrivateKey ?? "",
                    signingKey.PublicKey ?? "",
                    signingKey.IsActive,
                    signingKey.CreatedAt,
                    signingKey.ExpiresAt
                });
        }

        public void UpdateSigningKey(SigningKey signingKey){
            this._dbContext.ExecuteNonQuery(
                sql: @"SELECT update_signingkey(
                                                    @v_key_id, 
                                                    @v_private_key, 
                                                    @v_public_key, 
                                                    @v_is_active, 
                                                    @v_created_at, 
                                                    @v_expires_at)",
                paramValues: new object[]{
                    signingKey.KeyId ?? "",
                    signingKey.PrivateKey ?? "",
                    signingKey.PublicKey ?? "",
                    signingKey.IsActive,
                    signingKey.CreatedAt,
                    signingKey.ExpiresAt
                });
        }

        public void InsertClient(Client client){
            this._dbContext.ExecuteNonQuery(
                sql: @"SELECT insert_client(
                                                @v_id, 
                                                @v_clientid, 
                                                @v_name, 
                                                @v_clienturl)",
                paramValues: new object[]{
                    client.Id,
                    client.ClientId??"",
                    client.Name ?? "",
                    client.ClientURL ?? ""
                });
        }

    }
}