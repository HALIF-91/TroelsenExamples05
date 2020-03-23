using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SimpleDataSet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***** Fun with DataSets *********\n");
            // Создать объект DataSet и добавить несколько свойств
            DataSet carsInventoryDS = new DataSet("Car Inventory");

            carsInventoryDS.ExtendedProperties["TimeStamp"] = DateTime.Now;
            carsInventoryDS.ExtendedProperties["DataSetID"] = Guid.NewGuid();
            carsInventoryDS.ExtendedProperties["Company"] =
                "Mikko's Hot Tub Super Store";

            FillDataSet(carsInventoryDS);
            PrintDataSet(carsInventoryDS);

            SaveAndLoadAsXml(carsInventoryDS);
            SaveAndLoadAsBinary(carsInventoryDS);
            Console.ReadLine();
        }
        static void SaveAndLoadAsBinary(DataSet carsInventoryDS)
        {
            // Установить флаг двоичной сериализации
            carsInventoryDS.RemotingFormat = SerializationFormat.Binary;

            // Сохранить этот DataSet в двоичном виде
            FileStream fs = new FileStream("BinaryCars.bin", FileMode.Create);
            BinaryFormatter bFormat = new BinaryFormatter();
            bFormat.Serialize(fs, carsInventoryDS);
            fs.Close();

            // Очистить DataSet
            carsInventoryDS.Clear();

            // Загрузить DataSet из двочиного файла
            fs = new FileStream("BinaryCars.bin", FileMode.Open);
            DataSet data = (DataSet)bFormat.Deserialize(fs);
        }
        static void SaveAndLoadAsXml(DataSet carsInventoryDS)
        {
            // Сохранить этот DataSet в виде XML
            carsInventoryDS.WriteXml("carsDataSet.xml");
            carsInventoryDS.WriteXmlSchema("carsDataSet.xsd");

            // Очистить DataSet
            carsInventoryDS.Clear();

            // Загрузить DataSet из файла XML
            carsInventoryDS.ReadXml("carsDataSet.xml");
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

        static void FillDataSet(DataSet ds)
        {
            // Создать столбцы данных, которые отображаются на "реальные"
            // столбцы в таблице Inventory из базы данных AutoLot
            DataColumn carIDColumn = new DataColumn("CarID", typeof(int));
            carIDColumn.Caption = "Car ID";
            carIDColumn.ReadOnly = true;
            carIDColumn.AllowDBNull = false;
            carIDColumn.Unique = true;
            carIDColumn.AutoIncrement = true;
            carIDColumn.AutoIncrementSeed = 0;
            carIDColumn.AutoIncrementStep = 1;

            DataColumn carMakeColumn = new DataColumn("Make", typeof(string));
            DataColumn carColorColumn = new DataColumn("Color", typeof(string));
            DataColumn carPetNameColumn = new DataColumn("PetName", typeof(string));
            carPetNameColumn.Caption = "Pet Name";

            // Добавить объекты DataColumn в DataTable
            DataTable inventoryTable = new DataTable("Inventory");
            inventoryTable.Columns.AddRange(new DataColumn[]
            {
                carIDColumn, carMakeColumn, carColorColumn, carPetNameColumn
            });

            // Задать первичный ключ этой таблицы
            inventoryTable.PrimaryKey = new DataColumn[] { inventoryTable.Columns[0] };

            // Добавить несколько строк в таблицу Inventory
            DataRow carRow = inventoryTable.NewRow();
            carRow["Make"] = "BMW";
            carRow["Color"] = "Black";
            carRow["PetName"] = "Hamlet";
            inventoryTable.Rows.Add(carRow);

            carRow = inventoryTable.NewRow();
            // Столбец 0 - это автоинкрементное поле идентификатора
            // поэтому начать заполнение со столбца 1
            carRow[1] = "Saab";
            carRow[2] = "Red";
            carRow[3] = "Sea Breeze";
            inventoryTable.Rows.Add(carRow);

            // Наконец, добавить таблицу в DataSet
            ds.Tables.Add(inventoryTable);
        }
        static void ManipulateDataRowState()
        {
            // Создать объект temp типа DataTable для тестирования
            DataTable temp = new DataTable("Temp");
            temp.Columns.Add(new DataColumn("TempColumn", typeof(int)));

            // RowState = Detached (т.е. пока еще не является частью DataTable)
            DataRow row = temp.NewRow();
            Console.WriteLine("After calling NewRow(): {0}", row.RowState);

            // RowState = Added
            temp.Rows.Add(row);
            Console.WriteLine("After calling Rows.Add(): {0}", row.RowState);

            // RowState = Added
            row["TempColumn"] = 10;
            Console.WriteLine("After first assignment: {0}", row.RowState);

            // RowState = Unchanged
            temp.AcceptChanges();
            Console.WriteLine("After calling AcceptChanges: {0}", row.RowState);

            // RowState = Modified
            row["TempColumn"] = 11;
            Console.WriteLine("After first assignment: {0}", row.RowState);

            // RowState = Deleted
            temp.Rows[0].Delete();
            Console.WriteLine("After calling Delete: {0}", row.RowState);
        }
    }
}
