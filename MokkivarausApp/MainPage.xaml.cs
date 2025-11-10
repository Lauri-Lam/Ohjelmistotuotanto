namespace MokkivarausApp
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using MokkivarausApp.Data;
    using MySql.Data.MySqlClient;

    public partial class MainPage : ContentPage
    {
        DatabaseService dbS = new DatabaseService();

        public MainPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData(string table = "alue")
        {
            DataTable dt = await dbS.GetDataAsync("SELECT * FROM " + table);

            foreach (DataRow row in dt.Rows)
            {
                Trace.WriteLine(row);
            }
        }

        private void AddCustomer(string postinro, string etunimi, string sukunimi, string lahiosoite, string email, string puhelinnro)
        {
            string sqlQuery = "INSERT INTO asiakas (postinro, etunimi, sukunimi, lahiosoite, email, puhelinnro) VALUES (@Postinro, @Etunimi, @Sukunimi, @Lahiosoite, @Email, @Puhelinnro)";

            MySqlCommand cmd = new MySqlCommand(sqlQuery);

            cmd.Parameters.AddWithValue("@Postinro", postinro);
            cmd.Parameters.AddWithValue("@Etunimi", etunimi);
            cmd.Parameters.AddWithValue("@Sukunimi", sukunimi);
            cmd.Parameters.AddWithValue("@Lahiosoite", lahiosoite);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Puhelinnro", puhelinnro);

            dbS.DatabaseNonQuery(cmd);
        }

        private void AddReservation(string asiakas_id, string mokki_id, string varattu_pvm, string vahvistus_pvm, string varattu_alkupvm, string varattu_loppupvm)
        {
            string sqlQuery = "INSERT INTO varaus (asiakas_id, mokki_id, varattu_pvm, vahvistus_pvm, varattu_alkupvm, varattu_loppupvm) VALUES (@Asiakas_id, @Mokki_id, @Varattu_pvm, @Vahvistus_pvm, @Varattu_alkupvm, @Varattu_loppupvm)";

            MySqlCommand cmd = new MySqlCommand(sqlQuery);

            cmd.Parameters.AddWithValue("@Asiakas_id", asiakas_id);
            cmd.Parameters.AddWithValue("@Mokki_id", mokki_id);
            cmd.Parameters.AddWithValue("@Varattu_pvm", varattu_pvm);
            cmd.Parameters.AddWithValue("@Vahvistus_pvm", vahvistus_pvm);
            cmd.Parameters.AddWithValue("@Varattu_alkupvm", varattu_alkupvm);
            cmd.Parameters.AddWithValue("@Varattu_loppupvm", varattu_loppupvm);

            dbS.DatabaseNonQuery(cmd);
        }
    }
}
