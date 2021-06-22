using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ToDoApp
{
    public partial class MainWindow : Window
    {
        private readonly string _userID;

        public MainWindow()
        {
            InitializeComponent();
            Columns = new List<ColumnDefinition>();
        }

        public MainWindow(string login)
        {
            InitializeComponent();
            Columns = new List<ColumnDefinition>();
            _userID = Data.GetUserID(login);
            GetColumnsFromDatabase();
        }

        public List<ColumnDefinition> Columns { get; }


        private void AddColumnButton_Click(object sender, RoutedEventArgs e)
        {
            InsertColumnIntoDatabase("Column");
            var id = GetColumnID();
            var columnDefinition = new ColumnDefinition {Width = GridLength.Auto};
            MainGrid.ColumnDefinitions.Add(columnDefinition);
            Columns.Update(MainGrid);
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical, Name = $"StackPanel{Grid.GetColumn(AddColumnButton)}"
            };
            var taskWindow = new TaskWindow(this, stackPanel, columnDefinition, _userID, id);
            stackPanel.Children.Add(taskWindow);
            MainGrid.Children.Add(stackPanel);
            Grid.SetColumn(stackPanel, MainGrid.Children.Count - 1);
        }

        private void InsertColumnIntoDatabase(string columnName)
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                sqlConnection.Open();
                var sqlCommand =
                    new SqlCommand($"INSERT INTO [Columns] ([Name], [UserID]) values ('{columnName}', '{_userID}')",
                        sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private string GetColumnID()
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                var id = "";
                sqlConnection.Open();
                var sqlCommand =
                    new SqlCommand($"SELECT TOP 1 [ID] FROM [Columns] WHERE [UserID] = '{_userID}' ORDER BY [ID] DESC",
                        sqlConnection);
                var dataReader = sqlCommand.ExecuteReader();
                if (dataReader.HasRows)
                    while (dataReader.Read())
                        id = dataReader.GetInt32(0).ToString();
                return id;
            }
            catch
            {
                return "";
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void GetColumnsFromDatabase()
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                var id = "";
                sqlConnection.Open();
                var sqlCommand = new SqlCommand($"SELECT [ID], [Name] FROM [Columns] WHERE [UserID] = '{_userID}'",
                    sqlConnection);
                var dataReader = sqlCommand.ExecuteReader();
                if (dataReader.HasRows)
                    while (dataReader.Read())
                    {
                        id = dataReader.GetInt32(0).ToString();
                        var columnDefinition = new ColumnDefinition {Width = GridLength.Auto};
                        MainGrid.ColumnDefinitions.Add(columnDefinition);
                        Columns.Update(MainGrid);
                        var stackPanel = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Name = $"StackPanel{Grid.GetColumn(AddColumnButton)}"
                        };
                        var taskWindow = new TaskWindow(this, stackPanel, columnDefinition, _userID, id);
                        stackPanel.Children.Add(taskWindow);
                        MainGrid.Children.Add(stackPanel);
                        Grid.SetColumn(stackPanel, MainGrid.Children.Count - 1);
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}