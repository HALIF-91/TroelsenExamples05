using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace FillDataSetUsingSqlDataAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***** Fun with Data Adapters ********\n");
            // Жестко закодированная строка соединения
            string cnStr = "Integrated Security=SSPI;Initial Catalog=AutoLot;" +
                @"Data Source=(local)\SQLEXPRESS";

            // Объект DataSet создается вызывающим процессом
            DataSet ds = new DataSet("AutoLot");

            // Указать адаптеру текст команды Select и строку соединения
            SqlDataAdapter dAdapt = new SqlDataAdapter("Select * from Inventory", cnStr);

            // Отобразить имена столбцов базы данных на дружественные к пользователю имена
            DataTableMapping custMap = dAdapt.TableMappings.Add("Inventory", "Current Inventory");
            custMap.ColumnMappings.Add("CarID", "Car ID");
            custMap.ColumnMappings.Add("PetName", "Name of Car");
            
            // Заполнить DataSet новой таблицей по имени Inventory
            dAdapt.Fill(ds, "Inventory");

            // Отобразить содержимое DataSet с использованием вспомогательного \
            // метода, созданного ранее в данной главе
            PrintDataSet(ds);
            Console.ReadLine();
        }
        static void PrintTable(DataTable dt)
        {
            // Получтить объект DataTableReader
            DataTableReader dtReader = dt.CreateDataReader();

            // DataTableReader работает в точности как DataReader
            while (dtReader.Read())
            {
                for (int i = 0; i < dtReader.FieldCount; i++)
                {
                    Console.Write("{0}\t", dtReader.GetValue(i).ToString().Trim());
                }
                Console.WriteLine();
            }
            dtReader.Close();
        }
        static void PrintDataSet(DataSet ds)
        {
            // Вывести имя DataSet и любые расширенные свойства
            Console.WriteLine("DataSet is named: {0}", ds.DataSetName);
            foreach (System.Collections.DictionaryEntry de in ds.ExtendedProperties)
            {
                Console.WriteLine("Key = {0}, Value = {1}", de.Key, de.Value);
            }
            Console.WriteLine();

            // Вывести каждую таблицу
            foreach (DataTable dt in ds.Tables)
            {
                Console.WriteLine("=> {0} Table:", dt.TableName);

                // Вывести имена столбцов
                for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
                {
                    Console.Write(dt.Columns[curCol].ColumnName + "\t");
                }
                Console.WriteLine("\n--------------------------------------");

                // Вывести DataTable
                //for (int curRow = 0; curRow < dt.Rows.Count; curRow++)
                //{
                //    for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
                //    {
                //        Console.Write(dt.Rows[curRow][curCol].ToString() + "\t");
                //    }
                //    Console.WriteLine();
                //}

                // Вызвать вспомогательный метод
                PrintTable(dt);
            }
        }
    }
}
