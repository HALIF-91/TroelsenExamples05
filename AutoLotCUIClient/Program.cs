using System;
using AutoLotConnectedLayer;
using System.Configuration;
using System.Data;

namespace AutoLotCUIClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****** The AutoLot Console UI *******\n");

            // Получить строку соединения из App.config
            string cnStr = ConfigurationManager.ConnectionStrings["AutoLotSqlProvider"].ConnectionString;
            bool userDone = false;
            string userCommand = "";

            // Создать объект InventoryDAL
            InventoryDAL invDAL = new InventoryDAL();
            invDAL.OpenConnection(cnStr);

            // Продолжать запрашивать у пользователя ввод вплоть до получения команды Q

            try
            {
                ShowInstructions();
                do
                {
                    Console.Write("\nPlease enter your command: ");
                    userCommand = Console.ReadLine();
                    Console.WriteLine();
                    switch (userCommand.ToUpper())
                    {
                        case "I":
                            InsertNewCar(invDAL); break;
                        case "U":
                            UpdateCarPetName(invDAL); break;
                        case "D":
                            DeleteCar(invDAL); break;
                        case "L":
                            ListInventory(invDAL); break;
                        case "S":
                            ShowInstructions(); break;
                        case "P":
                            LookUpPetName(invDAL); break;
                        case "Q":
                            userDone = true; break;
                        default:
                            Console.WriteLine("Bad data! Try again"); break;
                    }
                } while (!userDone);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                invDAL.CloseConnection();
            }
        }

        private static void LookUpPetName(InventoryDAL invDAL)
        {
            // Получить идентификатор автомобиля для поиска дружественного имени
            Console.Write("Enter ID of car to look up: ");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("PetName of {0} is {1}", id, invDAL.LookUpPetName(id).TrimEnd());
        }

        private static void UpdateCarPetName(InventoryDAL invDAL)
        {
            // Сначала получить пользовательские данные
            int CarID;
            string newCarPetName;

            Console.Write("Enter Car ID: ");
            CarID = int.Parse(Console.ReadLine());
            Console.Write("Enter new Pet name: ");
            newCarPetName = Console.ReadLine();

            // Теперь передать информацию библиотеке доступа к данным
            invDAL.UpdateCarPetName(CarID, newCarPetName);
        }

        private static void InsertNewCar(InventoryDAL invDAL)
        {
            // Сначала получить пользовательские данные
            int newCarID;
            string newCarColor, newCarMake, newCarPetName;
            Console.Write("Enter Car ID: ");
            newCarID = int.Parse(Console.ReadLine());

            Console.Write("Enter Car color: ");
            newCarColor = Console.ReadLine();

            Console.Write("Enter Car make: ");
            newCarMake = Console.ReadLine();

            Console.Write("Enter Pet name: ");
            newCarPetName = Console.ReadLine();

            // Теперь передать информацию библиотеке доступа к данным
            NewCar car = new NewCar
            {
                CarID = newCarID,
                Color = newCarColor,
                Make = newCarMake,
                PetName = newCarPetName
            };

            invDAL.InsertAuto(car);
        }

        private static void DeleteCar(InventoryDAL invDAL)
        {
            // Получить идентификатор удаляемого автомобиля
            Console.Write("Enter ID of Car to delete: ");
            int id = int.Parse(Console.ReadLine());

            // На случай нарушения ссылочной целостности
            try
            {
                invDAL.DeleteCar(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ShowInstructions()
        {
            Console.WriteLine("I: inserts a new car"); // вставить новый автомобиль
            Console.WriteLine("U: updates an existing car"); // обновить сущ. автомобиль
            Console.WriteLine("D: deletes an existing car"); // удалить сущ. автомобиль
            Console.WriteLine("L: lists current inventory"); // вывести текущие запасы
            Console.WriteLine("S: shows these instructions"); // вывести эти инструкции
            Console.WriteLine("P: looks up pet name"); // найти дружественное имя автомобиля
            Console.WriteLine("Q: quits program"); // завершить программу
        }

        static void ListInventory(InventoryDAL invDAL)
        {
            // получить список автомобилей на складе
            DataTable dt = invDAL.GetAllInventoryAsDataTable();
            // передать DataTable вспомогательной функции для отображения
            DisplayTable(dt);
        }

        static void DisplayTable(DataTable dt)
        {
            // Вывести имена столбцов
            for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
            {
                Console.Write(dt.Columns[curCol].ColumnName + "\t");
            }
            Console.WriteLine("\n-----------------------------------");

            // вывести содердимое DataTable
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
