using System;
using System.Data.SqlClient;
using System.Windows;

namespace ToDoApp
{
    /// <summary>
    ///     Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var result = Name.Validate() & Surname.Validate() & Login.Validate() & Password.Validate();
            if (result)
            {
                var connection = new SqlConnection(Data.ConnectionString);
                try
                {
                    connection.Open();
                    var command =
                        new SqlCommand(
                            $"INSERT INTO [Users] ([Login], [Password], [Name], [Surname]) values ('{Login.Text}', '{Password.Password}', '{Name.Text}', '{Surname.Text}')",
                            connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Повторяющееся значение ключа"))
                    {
                        Message("Ваш логин не уникален, попробуйте заново");
                        Login.Text = "";
                    }
                }
                finally
                {
                    connection.Close();
                    Close();
                }
            }
        }

        private void Message(string message)
        {
            var duration = 4;
            SnackbarOne.MessageQueue?.Enqueue(
                message,
                null,
                null,
                null,
                false,
                true,
                TimeSpan.FromSeconds(duration));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}