using System;
using AutoLotConnectedLayer;

namespace AdoNetTransaction
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****** Simple Transaction Example ******\n");
            // Простой способ разрешить или запретить успешное выполнение транзакции
            bool throwEx = true;
            string userAnswer = string.Empty;

            Console.Write("Do you want to throw an exception (Y or N): ");
            userAnswer = Console.ReadLine();

            if (userAnswer.ToLower() == "n")
                throwEx = false;

            InventoryDAL dal = new InventoryDAL();
            dal.OpenConnection(@"Data Source=(local)\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=AutoLot");

            // Обработать клиента с идентификатором 333
            dal.ProcessCreditRisk(throwEx, 333);
            Console.WriteLine("Check CreditRisk table for results");
            Console.ReadLine();
        }
    }
}
