using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _config;
        public DataContextDapper(IConfiguration config)
        {
            _config=config;
        }
        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }
        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSQL(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql) > 0;
        }
        public int ExecuteSQLwithRow(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql);
        }

        public bool ExecuteSQLwithParameters(string sql,DynamicParameters sqlParameter)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql, sqlParameter) > 0;

            // SqlCommand sqlCommandwithParam = new SqlCommand(sql);
            // foreach(SqlParameter i in sqlParameter)
            // {
            //     sqlCommandwithParam.Parameters.Add(i);
            // }

            // SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            // dbConnection.Open();

            // sqlCommandwithParam.Connection =dbConnection;

            // int rowsAffected = sqlCommandwithParam.ExecuteNonQuery();

            // dbConnection.Close();

            // return rowsAffected>0;
        }        
        public IEnumerable<T> LoadDatawithParameters<T>(string sql, DynamicParameters sqlParameter)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql, sqlParameter);
        }
        public T LoadDataSinglewithParameters<T>(string sql, DynamicParameters sqlParameter)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql, sqlParameter);
        }



    }
}