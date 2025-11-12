namespace MokkivarausApp
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using MokkivarausApp.Data;

    public partial class MainPage : ContentPage
    {
        DatabaseService databaseService = new DatabaseService();

        public MainPage()
        {
            InitializeComponent();
            ConnectionCheck();
        }

        private async void ConnectionCheck()
        {
            foreach (string tableName in await databaseService.GetTableNames()) Trace.WriteLine(tableName);
        }
    }
}