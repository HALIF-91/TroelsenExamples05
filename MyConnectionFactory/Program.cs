using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System;
using System.Configuration;

namespace MyConnectionFactory
{
    // Список возможных поставщиков
    enum DataProvider
    {
        SqlServer, OleDb, Odbc, None
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****** Very Simple Connection Factory ******\n");

            // Прочитать ключ provider
            string dataProvString = ConfigurationManager.AppSettings["provider"];

            // Преобразовать строку в перечисление
            DataProvider dp = DataProvider.None;

            if (Enum.IsDefined(typeof(DataProvider), dataProvString))
                dp = (DataProvider)Enum.Parse(typeof(DataProvider), dataProvString);
            else
                Console.WriteLine("Sorry, no provider exists!"); // Поставщик не существует

            // Получить конкретное подключение
            IDbConnection myCn = GetConnection(DataProvider.SqlServer);
            Console.WriteLine("Your connection is a {0}", myCn.GetType().Name);

            // Открыть, использовать и закрыть подключение
            
            Console.ReadLine();
        }
        // Этот метод возвращает конкретный объект подключения
        // на основе значения перечисления DataProvider
        private static IDbConnection GetConnection(DataProvider dp)
        {
            IDbConnection conn = null;
            switch (dp)
            {
                case DataProvider.SqlServer: 
                    conn = new SqlConnection(); break;
                case DataProvider.OleDb: 
                    conn = new OleDbConnection(); break;
                case DataProvider.Odbc: 
                    conn = new OdbcConnection(); break;
            }
            return conn;
        }
    }
}
