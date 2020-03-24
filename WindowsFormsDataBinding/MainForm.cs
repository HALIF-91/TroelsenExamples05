using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System;

namespace WindowsFormsDataBinding
{
    public partial class MainForm : Form
    {
        // Коллекция объектов Car
        List<Car> listCars = null;
        // Складская информация
        DataTable inventoryTable = new DataTable();
        // Представление DataView
        DataView yugosOnlyView;
        public MainForm()
        {
            InitializeComponent();
            // Заполнить список автомобилями
            listCars = new List<Car>
            {
                new Car { ID = 100, PetName = "Chucky", Make = "BMW", Color = "Green" },
                new Car { ID = 101, PetName = "Tiny", Make = "Yugo", Color = "White" },
                new Car { ID = 102, PetName = "Ami", Make = "Jeep", Color = "Tan" },
                new Car { ID = 103, PetName = "Pain Inducer", Make = "Caravan", Color = "Pink" },
                new Car { ID = 104, PetName = "Fred", Make = "BMW", Color = "Green" },
                new Car { ID = 105, PetName = "Sidd", Make = "BMW", Color = "Black" },
                new Car { ID = 106, PetName = "Mel", Make = "Firebird", Color = "Red" },
                new Car { ID = 107, PetName = "Sarah", Make = "Colt", Color = "Black" }
            };

            // Создать таблицу данных
            CreateDataTable();
            //ShowCarsWithIdGreaterThan();

            // Создать представление
            CreateDataView();
        }
        private void CreateDataTable()
        {
            // Создать схему таблицы
            DataColumn carIDColumn = new DataColumn("ID", typeof(int));
            DataColumn carMakeColumn = new DataColumn("Make", typeof(string));
            DataColumn carColorColumn = new DataColumn("Color", typeof(string));
            DataColumn carPetNameColumn = new DataColumn("PetName", typeof(string));
            carPetNameColumn.Caption = "Pet Name";
            inventoryTable.Columns.AddRange(new DataColumn[]
            {
                carIDColumn, carMakeColumn, carColorColumn, carPetNameColumn
            });

            // Пройти по List<T> для создания строк
            foreach (Car c in listCars)
            {
                DataRow newRow = inventoryTable.NewRow();
                newRow["ID"] = c.ID;
                newRow["Make"] = c.Make;
                newRow["Color"] = c.Color;
                newRow["PetName"] = c.PetName;
                inventoryTable.Rows.Add(newRow);
            }

            // Привязать DataTable к CarInventoryGridView
            carInventoryGridView.DataSource = inventoryTable;
        }
        private void CreateDataView()
        {
            // Установить таблицу, которая используется для создания этого представления
            yugosOnlyView = new DataView(inventoryTable);

            // Сконфигурировать представление с помощью фильтра
            yugosOnlyView.RowFilter = "Make='Yugo'";

            // Привязать к новому элементу
            dataGridYugosView.DataSource = yugosOnlyView;
        }
        // Удалить эту строку из DataRowCollection
        private void btnRemoveRow_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Найти правильную строку для удаления
                DataRow[] rowToDelete = inventoryTable.Select(string.Format("ID={0}", int.Parse(txtRowToRemove.Text)));

                // Удалить ее
                rowToDelete[0].Delete();
                inventoryTable.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDisplayMakes_Click(object sender, EventArgs e)
        {
            // Построить фильтр на основе пользовательского ввода
            string filterStr = string.Format("Make='{0}'", txtMakeToView.Text);

            // Найти все строки, удовлетворяющие фильтру
            DataRow[] makes = inventoryTable.Select(filterStr, "PetName DESC");
            //DataRow[] makes = inventoryTable.Select(filterStr);

            if (makes.Length == 0)
                MessageBox.Show("Sorry, no cars...", "Selection error!");
            else
            {
                string strMake = "";
                for (int i = 0; i < makes.Length; i++)
                {
                    // Получить значение PetName из текущей строки
                    strMake += makes[i]["PetName"] + "\n";
                }

                // Вывести имена всех найденных автомобилей указанной марки
                MessageBox.Show(strMake, string.Format("We have {0}s named:", txtMakeToView.Text));
            }
        }
        private void ShowCarsWithIdGreaterThan()
        {
            // Вывести дружественные имена всех автомобилей со значением ID больше 105
            DataRow[] properIDs;
            string newFilterStr = "ID > 105";
            properIDs = inventoryTable.Select(newFilterStr);
            string strIDs = null;
            for (int i = 0; i < properIDs.Length; i++)
            {
                DataRow temp = properIDs[i];
                strIDs += temp["PetName"] + " is ID " + temp["ID"] + "\n";
            }
            MessageBox.Show(strIDs, "Pet names of cars where ID > 105");
        }
        // Найти с помощью фильтра все строки, которые нужно отредактировать
        private void btnChangeMakes_Click(object sender, EventArgs e)
        {
            // Подтвердить выбор
            if (DialogResult.Yes == MessageBox.Show("Are you sure??? BMWs are much nicer than Yugos!", "Please Confirm!", MessageBoxButtons.YesNo))
            {
                // Построить фильтр
                string filterStr = "Make='BMW'";
                string strMake = string.Empty;

                // Найти все строки, удовлетворяющие фильтру
                DataRow[] makes = inventoryTable.Select(filterStr);

                // Заменить все BMW на Yugo
                for (int i = 0; i < makes.Length; i++)
                {
                    makes[i]["Make"] = "Yugo";
                }
            }
        }
    }
}
