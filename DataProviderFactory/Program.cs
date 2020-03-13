using System;
using System.Configuration;
using System.Data.Common;

namespace DataProviderFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**** Fun with Data Provider Factories *****\n");

            // Получить строку соединения и поставщика из файла *.config
            string dp = ConfigurationManager.AppSettings["provider"];
            string cnStr = ConfigurationManager.ConnectionStrings["AutoLotSqlProvider"].ConnectionString;
            //string cnStr = ConfigurationManager.AppSettings["connectStr"];

            // Получить фабрику поставщиков
            DbProviderFactory df = DbProviderFactories.GetFactory(dp);

            // Получить объект подключения
            using (DbConnection cn = df.CreateConnection())
            {
                Console.WriteLine("Your connection object is a: {0}", cn.GetType().Name);
                cn.ConnectionString = cnStr;
                cn.Open();

                // Создать объект команды
                DbCommand cmd = df.CreateCommand();
                Console.WriteLine("Your command object is a: {0}", cmd.GetType().Name);
                cmd.Connection = cn;
                cmd.CommandText = "Select * from Inventory";

                // Вывести данные с помощью объекта чтения данных
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    Console.WriteLine("Your data reader object is a: {0}", dr.GetType().Name);
                    Console.WriteLine("\n***** Current Inventory ******");
                    while(dr.Read())
                        Console.WriteLine("-> Car #{0} is a {1}.", dr["CarID"], dr["Make"]);
                }
            }
            Console.ReadLine();
        }
    }
}
