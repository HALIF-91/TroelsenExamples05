using System;
using System.Data;
using System.Linq;
using AutoLotDAL;
using AutoLotDAL.AutoLotDataSetTableAdapters;

namespace LinqToDataSetApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****** LINQ over DataSet ******\n");

            // Получить строго типизированный объект DataTable, содержащий
            // текущие данные таблицы Inventory из базы данных AutoLot
            AutoLotDataSet dal = new AutoLotDataSet();
            InventoryTableAdapter da = new InventoryTableAdapter();
            AutoLotDataSet.InventoryDataTable data = da.GetData();

            PrintAllCarIDs(data);
            ShowBlackCars(data);
            BuildDataTableFromQuery(data);
            Console.ReadLine();
        }
        static void PrintAllCarIDs(DataTable data)
        {
            // Получить перечислимую версию DataTable
            EnumerableRowCollection enumData = data.AsEnumerable();

            // Вывести значения идентификаторов автомобилей
            foreach (DataRow r in enumData)
            {
                Console.WriteLine("Car ID = {0}", r["CarID"]);
            }
            Console.WriteLine();
        }
        static void ShowBlackCars(DataTable data)
        {
            // Проецировать новый результирующий набор, содержащий
            // идентификатор/цвет для строк, в которых Color = Red
            var cars = from car in data.AsEnumerable()
                       where (string)car["Color"] == "Black"
                       select new
                       {
                           ID = car.Field<int>("CarID"),
                           Make = car.Field<string>("Make")
                       };
            Console.WriteLine("Here are black cars we have in stock:");
            foreach (var item in cars)
            {
                Console.WriteLine("-> CarID = {0} is {1}", item.ID, item.Make);
            }
            Console.WriteLine();
        }
        static void BuildDataTableFromQuery(DataTable data)
        {
            var cars = from car in data.AsEnumerable()
                       where car.Field<int>("CarID") > 5
                       select car;

            // Использовать этот результирующий набор для построения нового объекта DataTable
            DataTable newTable = cars.CopyToDataTable();

            // Выести содержимое DataTable
            for (int curRow = 0; curRow < newTable.Rows.Count; curRow++)
            {
                for (int curCol = 0; curCol < newTable.Columns.Count; curCol++)
                {
                    Console.Write(newTable.Rows[curRow][curCol].ToString().Trim() + "\t");
                }
                Console.WriteLine();
            }

            /*
             * Предположим, что myDataGrid - это объект сетки графического
             * пользовательского интерфейса
             myDataGrid.DataSource = (from car in data.AsEnumerable()
                       where car.Field<int>("CarID") > 5
                       select car).CopyToDataTable();
             */
        }
    }
}
