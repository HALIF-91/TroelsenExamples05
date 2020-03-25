using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridViewDataDesigner
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "inventoryDataSet.Inventory". При необходимости она может быть перемещена или удалена.
            this.inventoryTableAdapter.Fill(this.inventoryDataSet.Inventory);

        }

        private void btnUpdateInventory_Click(object sender, EventArgs e)
        {
            try
            {
                // Сохрнаить изменения, внесенные в таблицу Inventory, в базе данных
                this.inventoryTableAdapter.Fill(this.inventoryDataSet.Inventory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Получить актуальную копию для сетки
            this.inventoryTableAdapter.Fill(this.inventoryDataSet.Inventory);
        }
    }
}
