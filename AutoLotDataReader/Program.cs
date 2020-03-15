using System;
using System.Data.SqlClient;

namespace AutoLotDataReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****** Fun with Data Readers *******\n");

            // Создать и открыть подключение
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString =
                    @"Data Source=(local)\SQLEXPRESS;Integrated Security=SSPI;" +
                    "Initial Catalog=AutoLot;Connect Timeout=30";
                cn.Open();

                ShowConnectionStatus(cn);

                // Создать объект команды SQL
                string strSQL = "Select * From Inventory";
                SqlCommand myCommand = new SqlCommand(strSQL, cn);

                // Получить объект чтения данных с помощью ExecuteReader()
                using(SqlDataReader myDataReader = myCommand.ExecuteReader())
                {
                    // Организовать цикл по результатам
                    while (myDataReader.Read())
                    {
                        Console.WriteLine("-> Make: {0}, PetName: {1}, Color: {2}",
                            myDataReader["Make"].ToString(),    // либо [0]
                            myDataReader["PetName"].ToString(), // либо [1]
                            myDataReader["Color"].ToString());  // либо [2]
                    }
                }
            }
            Console.ReadLine();
        }

        static void ShowConnectionStatus(SqlConnection cn)
        {
            // Вывести различные сведения о текущем объекте подключения
            Console.WriteLine("***** Info about your connection *******");
            Console.WriteLine("Database Location: {0}", cn.DataSource); // местоположение базы данных
            Console.WriteLine("Database name: {0}", cn.Database);       // имя базы данных
            Console.WriteLine("Timeout: {0}", cn.ConnectionTimeout);    // Таймаут
            Console.WriteLine("Connection state: {0}\n", cn.State);     // Состояние
        }
    }
}
