using System;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace MokkivarausApp.Data
{
    public class DatabaseService
    {
        private string connectionString = "server=127.0.0.1;port=3307;database=vn;user=root;password=Ruutti;";

        public async void AddData(string table, List<string> values)
        {
            try
            {
                if (!(await GetTableNames()).Exists(x => x == table)) throw new Exception("The database does not contain a table with the specified name");

                List<string> columns = await GetTableColumns(table);

                if(table != "varauksen_palvelut" && table != "posti") columns.RemoveAt(0);

                if (columns.Count != values.Count) throw new Exception("Different amount of Columns and Values");

                string columnString = string.Join(", ", columns);
                string valueString = string.Join("', '", values);
                string sqlQuery = "INSERT INTO " + table + " (" + columnString + ") VALUES ('" + valueString + "');";

                MySqlCommand cmd = new MySqlCommand(sqlQuery);

                DatabaseNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Trace.WriteLine(ex.Message);
            }
        }

        public async Task<List<string>> GetTableColumns(string tableName)
        {
            List<string> tableColumns = new List<string>();

            try
            {
                string sqlQuery = "SHOW COLUMNS FROM " + tableName + ";";

                DataTable dt = await GetDataAsync(sqlQuery);

                foreach (DataRow row in dt.Rows)
                {
                    tableColumns.Add(row["Field"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                Trace.WriteLine("Database Error: " + ex.Message);
            }

            return tableColumns;
        }

        public async Task<List<string>> GetTableNames()
        {
            List<string> tableNames = new List<string>();

            try
            {
                DataTable dt = await GetDataAsync("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' && TABLE_SCHEMA = 'vn';");

                foreach (DataRow row in dt.Rows)
                {
                    tableNames.Add(row["TABLE_NAME"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                Trace.WriteLine("Database Error: " + ex.Message);
            }

            return tableNames;
        }

        public async Task<DataTable> GetDataAsync(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }

                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                Trace.WriteLine("Database Error: " + ex.Message);
            }

            return dt;
        }

        public async Task<DataTable> GetDataAsync(MySqlCommand cmd)
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (cmd.Connection = connection)
                    {
                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }

                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                Trace.WriteLine("Database Error: " + ex.Message);
            }

            return dt;
        }

        public async void DatabaseNonQuery(MySqlCommand cmd)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    cmd.Connection = connection;

                    await cmd.ExecuteNonQueryAsync();

                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                Trace.WriteLine("Database Error: " + ex.Message);
            }
        }
    }
}