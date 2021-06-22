using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToDoApp
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (Login.Text.Trim().Length != 0)
            {
                using (SqlConnection connection = new SqlConnection(Data.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand($"SELECT * FROM [Users] WHERE [Login] = @login AND [Password] = @password", connection);
                    command.Parameters.AddWithValue("@login", Login.Text);
                    command.Parameters.AddWithValue("@password", Password.Password);
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            connection.Close();
                            MainWindow m = new MainWindow(Login.Text);
                            m.Show();
                            Close();
                        }
                    }
                }
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Registration r = new Registration();
            r.ShowDialog();
        }
    }
}
