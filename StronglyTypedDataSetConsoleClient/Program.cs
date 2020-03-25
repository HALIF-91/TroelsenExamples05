using System;
using AutoLotDAL;
using AutoLotDAL.AutoLotDataSetTableAdapters;

namespace StronglyTypedDataSetConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***** Fun with Strongly Typed DataSets *****\n");

            AutoLotDataSet.InventoryDataTable table = new AutoLotDataSet.InventoryDataTable();

            InventoryTableAdapter dAdapt = new InventoryTableAdapter();

            dAdapt.Fill(table);
            PrintInventory(table);

            // Добавить строки, обновить и вывести повторно
            AddRecords(table, dAdapt);
            table.Clear();
            dAdapt.Fill(table);
            PrintInventory(table);

            // Удалить строки, обновить и вывести повторно
            RemoveRecords(table, dAdapt);
            table.Clear();
            dAdapt.Fill(table);
            PrintInventory(table);

            CallStoredProc();
            Console.ReadLine();
        }

        private static void CallStoredProc()
        {
            try
            {
                QueriesTableAdapter q = new QueriesTableAdapter();
                Console.Write("Enter ID of car to look up: ");
                string carID = Console.ReadLine();
                string carName = "";
                q.GetPetName(int.Parse(carID), ref carName);

                Console.WriteLine("CarID {0} has the name of {1}", carID, carName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void RemoveRecords(AutoLotDataSet.InventoryDataTable dt, InventoryTableAdapter dAdapt)
        {
            try
            {
                AutoLotDataSet.InventoryRow rowToDelete = dt.FindByCarID(999);
                dAdapt.Delete(rowToDelete.CarID, rowToDelete.Make, rowToDelete.Color, rowToDelete.PetName);
                rowToDelete = dt.FindByCarID(777);
                dAdapt.Delete(rowToDelete.CarID, rowToDelete.Make, rowToDelete.Color,
                    rowToDelete.PetName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void AddRecords(AutoLotDataSet.InventoryDataTable dt, InventoryTableAdapter dAdapt)
        {
            try
            {
                // Получить из таблицы новую строго типизированную строку
                AutoLotDataSet.InventoryRow newRow = dt.NewInventoryRow();

                // Заполнить строку данными
                newRow.CarID = 999;
                newRow.Color = "Purple";
                newRow.Make = "BMW";
                newRow.PetName = "Saku";

                // Вставить новую строку
                dt.AddInventoryRow(newRow);

                // Добавить еще одну строку, используя перегруженный метод добавления
                dt.AddInventoryRow(777, "Yugo", "Green", "Zippy");

                // Обновить базу данных
                dAdapt.Update(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void PrintInventory(AutoLotDataSet.InventoryDataTable dt)
        {
            // Вывести имена столбцов
            for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
            {
                Console.Write(dt.Columns[curCol].ColumnName + "\t");
            }
            Console.WriteLine("\n--------------------------------------");

            // Вывести данные
            for (int curRow = 0; curRow < dt.Rows.Count; curRow++)
            {
                for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
                {
                    Console.Write(dt.Rows[curRow][curCol].ToString() + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}