using System;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using MokkivarausApp.Models;

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

                if (table != "varauksen_palvelut" && table != "posti") columns.RemoveAt(0);

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

        public async void AddData(string table, List<string> values, List<string> columns)
        {
            try
            {
                if (!(await GetTableNames()).Exists(x => x == table)) throw new Exception("The database does not contain a table with the specified name");

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

        public async Task<Asiakas> GetOrCreateCustomerByName(string name)
        {
            Asiakas customer = await GetCustomerByNameAsync(name);
            if (customer.Empty()) return CreateCustomer(name);
            return customer;
        }

        public Asiakas CreateCustomer(string name)
        {
            Asiakas customer = new Asiakas();

            try
            {
                string[] cleanName = name.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

                customer.Postinro = "00000";
                customer.Etunimi = cleanName[0];
                customer.Sukunimi = cleanName[1];

                AddData("asiakas",
                    new List<string> { customer.Postinro, customer.Etunimi, customer.Sukunimi },
                    new List<string> { "postinro, etunimi, sukunimi" }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                Trace.WriteLine("Database Error: " + ex.Message);
            }

            return customer;
        }

        /*
        public async Task<Asiakas> CreateCustomer(List<string> values)
        {

        }

        public async Task<Asiakas> CreateCustomer(List<Asiakas> customer)
        {

        }
        */

        public async Task<List<Varaus>> GetReservationsByCustomerIDAsync(uint customerID)
        {
            List<Varaus> reservations = [];

            try
            {
                string sqlQuery = @"SELECT varaus.*, mokki.mokkinimi, CONCAT(asiakas.etunimi, ' ', asiakas.sukunimi) AS asiakasnimi
                                    FROM ((varaus
                                    INNER JOIN mokki ON varaus.mokki_id = mokki.mokki_id)
                                    INNER JOIN asiakas ON varaus.asiakas_id = asiakas.asiakas_id)
                                    WHERE asiakas.asiakas_id = @CustomerID
                                    ORDER BY varaus.varattu_alkupvm;";

                MySqlCommand cmd = new MySqlCommand(sqlQuery);

                cmd.Parameters.AddWithValue("@CustomerID", customerID);

                DataTable dt = await GetDataAsync(cmd);

                foreach (DataRow row in dt.Rows)
                {
                    Varaus newReservation = new();

                    newReservation.VarausId = Convert.ToUInt32(row["varaus_id"]);
                    newReservation.AsiakasId = Convert.ToUInt32(row["asiakas_id"]);
                    newReservation.MokkiId = Convert.ToUInt32(row["mokki_id"]);
                    newReservation.VarattuPvm = Convert.ToDateTime(row["varattu_pvm"]);
                    newReservation.VahvistusPvm = Convert.ToDateTime(row["vahvistus_pvm"]);
                    newReservation.VarattuAlkuPvm = Convert.ToDateTime(row["varattu_alkupvm"]);
                    newReservation.VarattuLoppuPvm = Convert.ToDateTime(row["varattu_loppupvm"]);
                    newReservation.MokkiNimi = row["mokkinimi"].ToString();
                    newReservation.AsiakasNimi = row["asiakasnimi"].ToString();

                    reservations.Add(newReservation);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                Trace.WriteLine("Database Error: " + ex.Message);
            }

            return reservations;
        }

        public async Task<Asiakas> GetCustomerByNameAsync(string name)
        {
            Asiakas newCustomer = new Asiakas();

            try
            {
                string[] cleanName = name.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                string sqlQuery = "SELECT * FROM asiakas WHERE etunimi = @Etunimi AND sukunimi = @Sukunimi LIMIT 1";

                MySqlCommand cmd = new MySqlCommand(sqlQuery);

                cmd.Parameters.AddWithValue("@Etunimi", cleanName[0]);
                cmd.Parameters.AddWithValue("@Sukunimi", cleanName[1]);

                DataTable dt = await GetDataAsync(cmd);

                if (dt.Rows.Count < 1) return newCustomer;

                DataRow dr = dt.Rows[0];

                newCustomer.AsiakasId = Convert.ToUInt32(dr["asiakas_id"]);
                newCustomer.Postinro = dr["postinro"].ToString();
                newCustomer.Etunimi = dr["etunimi"].ToString();
                newCustomer.Sukunimi = dr["sukunimi"].ToString();
                newCustomer.Lahiosoite = dr["lahiosoite"].ToString();
                newCustomer.Email = dr["email"].ToString();
                newCustomer.Puhelinnro = dr["puhelinnro"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                Trace.WriteLine("Database Error: " + ex.Message);
            }

            return newCustomer;
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