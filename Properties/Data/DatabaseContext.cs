using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using MyApi.Models;
using Npgsql;
using Newtonsoft.Json.Linq;
using MyApi.Helper;

namespace MyApi.Data{
    public class DatabaseContext{
        private DBSetting? _dbSetting = null;
        private string? _connectionString = null;
        IConfiguration? _config = null;
        private readonly Regex regParameters = new Regex(@"@\w+", RegexOptions.Compiled);

        // public DatabaseContext(IOptions<DBSetting> dbSetting){
        //     AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        //     this._dbSetting = dbSetting.Value;
        // }
        public DatabaseContext(IConfiguration config){
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            this._config = config;
        }

        public IDbConnection CreateConnection(){
            if (this._dbSetting != null)
                this._connectionString = $"Host={_dbSetting.server};Database={_dbSetting.Database};Username={_dbSetting.UserId};Password={_dbSetting.Password};Port={_dbSetting.Port}";
            else if (this._config != null)
                this._connectionString = _config?.GetSection("ConnectionStrings").GetChildren()?.FirstOrDefault(li => li.Key == "PostgressConnectionDb")?.Value;

            var conn =  new NpgsqlConnection(connectionString: this._connectionString);
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
            }
            catch (System.Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
            return conn;
        }

        public IDbCommand CreateCommand(
            IDbConnection conn, 
            string sql,
            object[]? paramvalues, 
            string[]? paramNames, 
            CommandType commandType = CommandType.Text){
            var cmd = new NpgsqlCommand(cmdText: sql, connection: (NpgsqlConnection) conn);
            try
            {
                cmd.CommandType = commandType;

                if (paramNames != null && paramvalues != null){
                    for(var i= 0 ; i< paramNames.Length; i++) {
                        var dp = cmd.CreateParameter();
                        dp.ParameterName = paramNames[i];
                        dp.Value = paramvalues[i];
                        cmd.Parameters.Add(dp);
                    }
                }    
                else if (paramNames == null && paramvalues != null){
                    var parameters = regParameters.Matches(cmd.CommandText);
                    var index = 0;
                    foreach(var el in parameters){
                        var dp = cmd.CreateParameter();
                        dp.ParameterName = el.ToString();
                        dp.Value = paramvalues[index++];
                        cmd.Parameters.Add(dp);
                    }
                }
            }
            catch (System.Exception ex)
            {
                cmd.Dispose();
                throw new Exception(ex.Message);
            } 
            return cmd;
        }

        public IDbCommand CreateCommand(
            IDbConnection conn, 
            string sql,
            object[]? paramvalues){
            return CreateCommand(
                conn: conn,
                sql: sql,
                paramvalues: paramvalues,
                paramNames: null);
        }

        public IDbCommand CreateCommand(
            IDbConnection conn, 
            string sql){
            return CreateCommand(
                conn: conn,
                sql: sql,
                paramvalues: null,
                paramNames: null);
        }

        public IList<T> ExecuteReader<T>(
            string sql,
            string[]? paramNames,
            object[]? paramValues,
            Func<IDataReader, T> fetcher,
            CommandType commandType = CommandType.Text){
            var list = new List<T>();
            try
            {
                using (var conn = this.CreateConnection()){
                    using (var cmd = this.CreateCommand(
                        conn: conn, 
                        sql: sql,
                        paramNames: paramNames,
                        paramvalues: paramValues,
                        commandType: commandType)){
                            using (var reader = cmd.ExecuteReader()){
                                while(reader.Read()){
                                    var t = fetcher(reader);
                                    list.Add(t);
                                }
                            }
                        }
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public IList<IDictionary> ExecuteReader(
            string sql,
            string[]? paramNames,
            object[]? paramValues){

            string[]? fieldNames = null;

            return this.ExecuteReader(
                sql: sql,
                paramNames: paramNames,
                paramValues: paramValues,
                fetcher: (reader) => {

                    if (fieldNames == null){
                        fieldNames = new string[reader.FieldCount];
                        for (var i = 0; i < reader.FieldCount; i++) {
                            fieldNames[i] = reader.GetName(i);
                        }
                    }

                    IDictionary item = new Hashtable();

                    for (int i = 0; i < fieldNames.Length; i++){
                      var value = reader[i];
                        item[fieldNames[i]] = value is DBNull ? null: value;
                    }
                    return item;
                }
            );
        }

        public IList<T> ExecuteReader<T>(
            string sql,
            object[]? paramValues,
            Func<IDataReader, T> fetcher){

            return this.ExecuteReader(
                sql: sql,
                paramValues: paramValues,
                paramNames: null,
                fetcher: fetcher
            );
        }

        public IList<T> ExecuteReader<T>(
            string sql,
            Func<IDataReader, T> fetcher){

            return this.ExecuteReader(
                sql: sql,
                paramValues: null,
                paramNames: null,
                fetcher: fetcher
            );
        }

        public int ExecuteNonQuery(
            string sql,
            string[]? paramNames,
            object[]? paramValues){
            try
            {
                using (var conn = this.CreateConnection()){
                    using (var cmd = this.CreateCommand(
                        conn: conn,
                        sql: sql,
                        paramNames: paramNames,
                        paramvalues: paramValues
                    )){
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
        }

        public int ExecuteNonQuery(
            string sql,
            object[]? paramValues){
            try
            {
                using (var conn = this.CreateConnection()){
                    using (var cmd = this.CreateCommand(
                        conn: conn,
                        sql: sql,
                        paramvalues: paramValues,
                        paramNames: null
                    )){
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
        }
   } 
}