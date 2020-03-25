using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace MultitabledDataSetApp
{
    public partial class MainForm : Form
    {
        // Объект DataSet уровня формы
        private DataSet autoLotDS = new DataSet("AutoLot");

        // Использовать построители команд для упрощения конфигурирования адаптеров данных
        private SqlCommandBuilder sqlCBInventory;
        private SqlCommandBuilder sqlCBCustomers;
        private SqlCommandBuilder sqlCBOrders;

        // Адаптеры данных (для каждой таблицы)
        private SqlDataAdapter invTableAdapter;
        private SqlDataAdapter custTableAdapter;
        private SqlDataAdapter ordersTableAdapter;

        // Строка соединения уровня формы
        private string cnStr = string.Empty;
        public MainForm()
        {
            InitializeComponent();

            // Получить строку соединения из файла *.config
            cnStr = ConfigurationManager.ConnectionStrings["AutoLotSqlProvider"].ConnectionString;

            // Создать адаптеры
            invTableAdapter = new SqlDataAdapter("Select * from Inventory", cnStr);
            custTableAdapter = new SqlDataAdapter("Select * from Customers", cnStr);
            ordersTableAdapter = new SqlDataAdapter("Select * from Orders", cnStr);

            // Автоматически сгенерировать команды
            sqlCBInventory = new SqlCommandBuilder(invTableAdapter);
            sqlCBCustomers = new SqlCommandBuilder(custTableAdapter);
            sqlCBOrders = new SqlCommandBuilder(ordersTableAdapter);

            // Заполнить таблицы в DataSet
            invTableAdapter.Fill(autoLotDS, "Inventory");
            custTableAdapter.Fill(autoLotDS, "Customers");
            ordersTableAdapter.Fill(autoLotDS, "Orders");

            // Построить отношения между таблицами
            BuildTableRelationShip();

            // Привязать к сеткам
            dataGridViewInventory.DataSource = autoLotDS.Tables["Inventory"];
            dataGridViewCustomers.DataSource = autoLotDS.Tables["Customers"];
            dataGridViewOrders.DataSource = autoLotDS.Tables["Orders"];
        }

        private void BuildTableRelationShip()
        {
            // Создать объект отношения между данными CustomerOrder
            DataRelation dr = new DataRelation("CustomerOrder",
                autoLotDS.Tables["Customers"].Columns["CustID"],
                autoLotDS.Tables["Orders"].Columns["CustID"]);
            autoLotDS.Relations.Add(dr);

            // Создание объекта отношения между данными InventoryOrder
            dr = new DataRelation("InventoryOrder",
                autoLotDS.Tables["Inventory"].Columns["CarID"],
                autoLotDS.Tables["Orders"].Columns["CarID"]);
            autoLotDS.Relations.Add(dr);
        }

        private void btnUpdateDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                invTableAdapter.Update(autoLotDS, "Inventory");
                custTableAdapter.Update(autoLotDS, "Customers");
                ordersTableAdapter.Update(autoLotDS, "Orders");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGetOrderInfo_Click(object sender, EventArgs e)
        {
            string strOrderInfo = string.Empty;
            DataRow[] drsCust = null;
            DataRow[] drsOrder = null;

            // Получить идентификатор клиента из текстового поля
            int custID = int.Parse(this.txtCustID.Text);

            // На основе custID получить подходящую строку из таблицы Customers
            drsCust = autoLotDS.Tables["Customers"].Select(string.Format("CustID={0}", custID));
            strOrderInfo += string.Format("Customer {0}: {1} {2}\n",
                drsCust[0]["CustID"].ToString(),
                drsCust[0]["FirstName"].ToString(),
                drsCust[0]["LastName"].ToString());

            // Перейти из таблицы Customers в таблицу Orders
            drsOrder = drsCust[0].GetChildRows(autoLotDS.Relations["CustomerOrder"]);

            // Проход в цикле по всем заказам этого клиента
            foreach (DataRow order in drsOrder)
            {
                strOrderInfo += string.Format("----\nOrder Number: {0}\n",
                    order["OrderID"]);

                // Получить автомобиль, на который ссылается этот заказ
                DataRow[] drsInv = order.GetParentRows(
                    autoLotDS.Relations["InventoryOrder"]);

                // Получить информацию для (ОДНОГО) автомобиля из этого заказа
                DataRow car = drsInv[0];
                strOrderInfo += string.Format("Make: {0}\n", car["Make"]); // Марка
                strOrderInfo += string.Format("Color: {0}\n", car["Color"]); // Цвет
                strOrderInfo += string.Format("PetName: {0}\n", car["PetName"]); // Друж. имя
            }
            MessageBox.Show(strOrderInfo, "Order Details");
        }
    }
}
