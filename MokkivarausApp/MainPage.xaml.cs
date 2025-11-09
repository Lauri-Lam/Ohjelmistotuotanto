namespace MokkivarausApp
{
    using MokkivarausApp.Data;
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public partial class MainPage : ContentPage
    {
        DatabaseService dbS = new DatabaseService();

        public MainPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            DataTable dt = await dbS.GetDataAsync("SELECT * FROM alue");

            foreach (DataRow row in dt.Rows)
            {
                Trace.WriteLine(row["nimi"]);
                Trace.WriteLine(row["alue_id"]);
            }
        }
    }
}
