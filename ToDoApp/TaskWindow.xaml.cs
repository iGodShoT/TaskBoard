using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ToDoApp
{
    public partial class TaskWindow : UserControl
    {
        private readonly ColumnDefinition _columnDefinition;
        private readonly string _id;
        private readonly MainWindow _mainWindow;
        private readonly StackPanel _stackPanel;
        private readonly List<Item> _tasks;
        private readonly string _userId;

        public TaskWindow(MainWindow mainWindow, StackPanel stackPanel, ColumnDefinition columnDefinition,
            string userId, string id)
        {
            InitializeComponent();
            ItemListView.ItemsSource = _tasks;
            _tasks = new List<Item>();
            ItemListView.Items.Refresh();
            _mainWindow = mainWindow;
            _stackPanel = stackPanel;
            _columnDefinition = columnDefinition;
            _userId = userId;
            _id = id;
            TableName.Text = GetColumnName();
            GetTasksFromDatabase();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (_tasks.Count == 0)
            {
                InsertTaskIntoDatabase(" ");
                _tasks.Add(new Item(Convert.ToInt32(GetTaskId()), ""));
                ItemListView.ItemsSource = _tasks;
                ItemListView.Items.Refresh();
            }
            else
            {
                if (_tasks[_tasks.Count - 1].Value == "") return;
                InsertTaskIntoDatabase(" ");
                _tasks.Add(new Item(Convert.ToInt32(GetTaskId()), ""));
                ItemListView.ItemsSource = _tasks;
                ItemListView.Items.Refresh();
            }
        }

        private string GetTaskId()
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                var id = "";
                sqlConnection.Open();
                var sqlCommand =
                    new SqlCommand(
                        $"SELECT TOP 1 [ID] FROM [Tasks] WHERE [UserID] = '{_userId}' AND [ColumnID] = '{_id}' ORDER BY [ID] DESC",
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

        private void InsertTaskIntoDatabase(string name)
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                sqlConnection.Open();
                var sqlCommand =
                    new SqlCommand(
                        $"INSERT INTO [Tasks] ([Name], [UserID], [ColumnID]) values ('{name}','{_userId}','{_id}')",
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

        private void GetTasksFromDatabase()
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                sqlConnection.Open();
                var sqlCommand =
                    new SqlCommand(
                        $"SELECT [ID], [Name] FROM [Tasks] WHERE [UserID] = '{_userId}' AND [ColumnID] = '{_id}'",
                        sqlConnection);
                var dataReader = sqlCommand.ExecuteReader();
                if (dataReader.HasRows)
                    while (dataReader.Read())
                    {
                        var item = new Item(dataReader.GetInt32(0), dataReader.GetString(1));
                        _tasks.Add(item);
                        ItemListView.ItemsSource = _tasks;
                        ItemListView.Items.Refresh();
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

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            _tasks[_tasks.FindIndex(x => x.Index == (int) textBox.Tag)].Value = textBox.Text;
            _tasks[_tasks.FindIndex(x => x.Index == (int) textBox.Tag)].Edit = false;
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                sqlConnection.Open();
                var sqlCommand =
                    new SqlCommand(
                        $"UPDATE [Tasks] SET [Name] = '{_tasks.Find(x => x.Index == (int) textBox.Tag).Value}' WHERE [ID] = '{_tasks.Find(x => x.Index == (int) textBox.Tag).Index}'",
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

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TextBox_LostFocus(sender, e);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainGrid.Children.Remove(_stackPanel);
            _mainWindow.MainGrid.ColumnDefinitions.Remove(_columnDefinition);
            DeleteTasksFromColumn();
            DeleteColumn();
        }

        private void DeleteTasksFromColumn()
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand($"DELETE FROM [Tasks] WHERE [ColumnID] = '{_id}'", sqlConnection);
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

        private void DeleteColumn()
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand($"DELETE FROM [Columns] WHERE [ID] = '{_id}'", sqlConnection);
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


        private void ItemListView_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(Item))) return;
            var received = (Item) e.Data.GetData(typeof(Item));
            _tasks.Add(received);
            ItemListView.ItemsSource = _tasks;
            ItemListView.Items.Refresh();
            InsertTaskIntoDatabase(received.Value);
        }

        private string GetColumnName()
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                var columnName = "";
                sqlConnection.Open();
                var sqlCommand = new SqlCommand($"SELECT [Name] FROM [Columns] WHERE [ID] = '{_id}'", sqlConnection);
                var dataReader = sqlCommand.ExecuteReader();
                if (dataReader.HasRows)
                    while (dataReader.Read())
                        columnName = dataReader.GetString(0);
                return columnName;
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


        private void TextBlock_Edit_ON_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            Enumerable.Range(0, _tasks.Count).Where(x => x != (int) (sender as TextBlock)?.Tag).ToList()
                .ForEach(x => _tasks[x].Edit = false);
            _tasks[_tasks.FindIndex(x => x.Index == (int) (sender as TextBlock)?.Tag)].Edit = true;
        }

        private void TextBlock_DragDrop_ON_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right) return;

            Item item = default;
            try
            {
                if (!(sender is TextBlock textBlock) || textBlock.Text == "") return;
                DragDrop.DoDragDrop(textBlock, _tasks.Find(x => x.Index == (int) textBlock.Tag), DragDropEffects.Move);
                item = _tasks.Find(x => x.Index == (int) textBlock.Tag);
                _tasks.RemoveAt(_tasks.FindIndex(x => x.Index == (int) textBlock.Tag));
                ItemListView.Items.Refresh();
            }
            catch
            {
                if (item != default)
                {
                    _tasks.Add(item);
                    ItemListView.ItemsSource = _tasks;
                    ItemListView.Items.Refresh();
                }
            }
            SqlConnection sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand($"DELETE FROM [Tasks] WHERE [ID] = '{item.Index}'", sqlConnection);
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

        private void TableName_Edit_OFF_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var sqlConnection = new SqlConnection(Data.ConnectionString);
            try
            {
                sqlConnection.Open();
                var sqlCommand =
                    new SqlCommand($"UPDATE [Columns] SET [Name] = '{(sender as TextBox)?.Text}' WHERE [ID] = '{_id}'",
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
    }
}