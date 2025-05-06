using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace MokkivarausApp.Data
{
    public class DatabaseService
    {
        private string connectionString = "server=127.0.0.1;port=3307;database=vn;user=root;password=Ruutti;";

        public async Task<DataTable> GetDataAsync(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
            }

            return dt;
        }
    }
}
