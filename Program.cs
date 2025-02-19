using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using ToDo_LIst.Models;
//using SQLite;


namespace ToDo_LIst
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.


            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());


        //    DataManager<TodoItem> dm = new();
        //    dm.DropTable();

        }
    }
}