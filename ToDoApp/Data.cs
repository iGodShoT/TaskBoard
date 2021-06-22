using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ToDoApp
{
    public static class Data
    {
        public const string ConnectionString = @"Data Source=95.165.158.109\GODSHOT;Initial Catalog=todo;Persist Security Info=True;User ID=Test;Password=123";
        public static bool Validate(this TextBox control) => control.Text.Trim().Length != 0;
        public static bool Validate(this PasswordBox control) => control.Password.Trim().Length != 0;
        public static void Update(this List<ColumnDefinition> list, Grid grid)
        {
            list.Clear();
            list.AddRange(grid.ColumnDefinitions);
        }
        public static string GetUserID(string login)
        {
            string userID = "";
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand($"select [ID] from Users where [Login] = '{login}'", sqlConnection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                         userID = Convert.ToString(reader.GetInt32(0));
                return userID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
            finally
            {
                sqlConnection.Close();
            }
        }

    }
}
