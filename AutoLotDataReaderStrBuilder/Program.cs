using System;
using System.Data.SqlClient;

namespace AutoLotDataReaderStrBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****** Fun with Data Readers *******\n");

            // Создать строку соединения с помощью объекта построителя
            SqlConnectionStringBuilder cnStrBuilder = new SqlConnectionStringBuilder();

            cnStrBuilder.InitialCatalog = "AutoLot";
            cnStrBuilder.DataSource = @"(local)\SQLEXPRESS";
            cnStrBuilder.ConnectTimeout = 30;
            cnStrBuilder.IntegratedSecurity = true;

            // Создать и открыть подключение
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = cnStrBuilder.ConnectionString;
                cn.Open();

                ShowConnectionStatus(cn);
                SelectOneData(cn);
                SelectMoreData(cn);
            }
            Console.ReadLine();
        }

        #region Получение одного результирующего набора
        static void SelectOneData(SqlConnection cn)
        {
            // Создать объект команды SQL
            string strSQL = "Select * From Inventory";
            SqlCommand myCommand = new SqlCommand(strSQL, cn);

            // Получить объект чтения данных с помощью ExecuteReader()
            using (SqlDataReader myDataReader = myCommand.ExecuteReader())
            {
                // Организовать цикл по результатам
                while (myDataReader.Read())
                {
                    Console.WriteLine("***** Record *******");
                    for (int i = 0; i < myDataReader.FieldCount; i++)
                    {
                        Console.WriteLine("{0} = {1} ",
                            myDataReader.GetName(i),
                            myDataReader.GetValue(i).ToString());
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine("********************************************");
        }
        #endregion

        #region Получение нескольких результирующих наборов
        static void SelectMoreData(SqlConnection cn)
        {
            string strSQL = "Select * from Inventory;Select * from Customers";
            SqlCommand command = new SqlCommand(strSQL, cn);

            using(SqlDataReader reader = command.ExecuteReader())
            {
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("***** Record *******");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine("{0} = {1} ",
                                reader.GetName(i),
                                reader.GetValue(i).ToString());
                        }
                        Console.WriteLine();
                    }
                } while (reader.NextResult());
            }
        }
        #endregion

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
