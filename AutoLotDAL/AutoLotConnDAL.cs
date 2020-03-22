using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AutoLotConnectedLayer
{
    public class InventoryDAL
    {
        // Этот член будет использоваться всеми методами
        private SqlConnection sqlCn = null;
        public void OpenConnection(string connectionString)
        {
            sqlCn = new SqlConnection();
            sqlCn.ConnectionString = connectionString;
            sqlCn.Open();
        }
        public void CloseConnection()
        {
            sqlCn.Close();
        }
        public void InsertAuto(NewCar car)
        {
            // Сформировать SQL-оператор
            string sql = string.Format("Insert into Inventory" +
                "(CarID, Make, Color, PetName) Values" +
                "('{0}', '{1}', '{2}', '{3}')", car.CarID, car.Make, car.Color, car.PetName);

            // Выполнить SQL оператор с применением нашего подключения
            using (SqlCommand cmd = new SqlCommand(sql, this.sqlCn))
            {
                cmd.ExecuteNonQuery();
            }
        }
        public void InsertAuto(int id, string color, string make, string petName)
        {
            // Обратите внимание на "заполнители" в SQL запросе
            string sql = string.Format("Insert into Inventory " +
                "(CarID, Make, Color, PetName) values " +
                "(@CarID, @Make, @Color, @PetName)");

            // Эта команда будет иметь внутренние параметры
            using (SqlCommand cmd = new SqlCommand(sql, this.sqlCn))
            {
                // Заполнить коллекцию параметров
                SqlParameter param = new SqlParameter();
                param.ParameterName = "@CarID";
                param.Value = id;
                param.SqlDbType = SqlDbType.Int;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@Make";
                param.Value = make;
                param.SqlDbType = SqlDbType.Char;
                param.Size = 10;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@Color";
                param.Value = color;
                param.SqlDbType = SqlDbType.Char;
                param.Size = 10;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@PetName";
                param.Value = petName;
                param.SqlDbType = SqlDbType.Char;
                param.Size = 10;
                cmd.Parameters.Add(param);

                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteCar(int id)
        {
            // Получить идентификатор удаляемого автомобиля, затем выполнить удаление
            string sql = string.Format("Delete from Inventory where CarID = '{0}'", id);

            using (SqlCommand cmd = new SqlCommand(sql, this.sqlCn))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Exception error = new Exception("Sorry! That car is on order!", ex);
                    throw error;
                }
            }
        }
        public void UpdateCarPetName(int id, string newPetName)
        {
            // Получить идентификатор модифицируемого автомобиля и новое дружественное имя
            string sql = string.Format("Update Inventory Set PetName = '{0}' Where CarID = '{1}'", newPetName, id);

            using (SqlCommand cmd = new SqlCommand(sql, this.sqlCn))
            {
                cmd.ExecuteNonQuery();
            }
        }
        public List<NewCar> GetAllInventoryAsList()
        {
            // Здесь будут храниться записи
            List<NewCar> inv = new List<NewCar>();

            // Подготовить объект команды
            string sql = "Select * from Inventory";

            using(SqlCommand cmd = new SqlCommand(sql, this.sqlCn))
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    inv.Add(new NewCar
                    {
                        CarID = (int)dr["CarID"],
                        Color = (string)dr["Color"],
                        Make = (string)dr["Make"],
                        PetName = (string)dr["PetName"]
                    });
                }
                dr.Close();
            }
            return inv;
        }
        public DataTable GetAllInventoryAsDataTable()
        {
            // Здесь будут храниться записи
            DataTable inv = new DataTable();

            // Подготовить объект команды
            string sql = "Select * from Inventory";
            using(SqlCommand cmd = new SqlCommand(sql, this.sqlCn))
            {
                SqlDataReader dr = cmd.ExecuteReader();

                // Заполнить DataTable данными из объекта чтения и выполнить очистку
                inv.Load(dr);
                dr.Close();
            }
            return inv;
        }
        public string LookUpPetName(int carID)
        {
            string carPetName = string.Empty;

            // Установить имя хранимой процедуры
            using (SqlCommand cmd = new SqlCommand("GetPetName", this.sqlCn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                // Входной параметр
                SqlParameter param = new SqlParameter();
                param.ParameterName = "@carID";
                param.SqlDbType = SqlDbType.Int;
                param.Value = carID;

                // По умолчанию параметры считаются входными (Input), но все же для ясности
                param.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(param);

                // Выходной паратметр
                param = new SqlParameter();
                param.ParameterName = "@petName";
                param.SqlDbType = SqlDbType.Char;
                param.Size = 10;
                param.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(param);

                // Выполнить хранимую процедуру
                cmd.ExecuteNonQuery();

                // Возвратить выходной параметр
                carPetName = (string)cmd.Parameters["@petName"].Value;
            }
            return carPetName;
        }
    }
}
