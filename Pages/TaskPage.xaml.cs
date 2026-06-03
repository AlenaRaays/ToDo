using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using ToDo.AppData;
using ToDo.Entities;
using ToDo.Helpers;

namespace ToDo.Pages
{
    public partial class TasksPage : Page
    {
        public TasksPage()
        {
            InitializeComponent();
            LoadTasksFromDb();
            InitializeComboBoxes();
        }
        private void LoadTasksFromDb()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var tasksFromDb = db.TodoTasks
                        .Where(t => t.UserId == UserSession.CurrentUserId)
                        .ToList();
                }
                UpdateTaskSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}");
            }
        }

        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            string taskTitle = NewTaskTxt.Text.Trim();

            if (string.IsNullOrEmpty(taskTitle))
            {
                MessageBox.Show("Введите название задачи!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? selectedCategoryId = CategoryComboBox.SelectedValue as int?;
            int? selectedPriorityId = PriorityComboBox.SelectedValue as int?;

            try
            {
                using (var db = new AppDbContext())
                {
                    var newTask = new TodoTask
                    {
                        Title = taskTitle,
                        Description = "",
                        CreatedAt = DateTime.Now,
                        UserId = UserSession.CurrentUserId,
                        StatusId = 1, 
                        CategoryId = selectedCategoryId,
                        PriorityId = selectedPriorityId ?? 2 
                    };

                    db.TodoTasks.Add(newTask);
                    db.SaveChanges();


                    NewTaskTxt.Clear();
                    CategoryComboBox.SelectedIndex = -1;
                    PriorityComboBox.SelectedIndex = -1;
                }

                UpdateTaskSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить задачу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is TodoTask taskToDelete)
            {
                using (var db = new AppDbContext())
                {
                    var item = db.TodoTasks.Find(taskToDelete.Id);
                    if (item != null)
                    {
                        db.TodoTasks.Remove(item);
                        db.SaveChanges();
                    }
                }
                UpdateTaskSource();
            }
        }

        private void TaskCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TodoTask clickedTask)
            {
                using (var db = new AppDbContext())
                {
                    var dbTask = db.TodoTasks.Find(clickedTask.Id);
                    if (dbTask != null)
                    {
                        dbTask.StatusId = (checkBox.IsChecked == true) ? 3 : 1;
                        db.SaveChanges();
                    }
                }
                UpdateTaskSource();
            }
        }

        private void Filter_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTaskSource();
        }

        private void UpdateTaskSource()
        {
            if (TasksListBox == null) return;

            try
            {
                using (var db = new AppDbContext())
                {
                    // 1. Базовый запрос
                    IQueryable<TodoTask> query = db.TodoTasks.Where(t => t.UserId == UserSession.CurrentUserId);

                    // 2. Фильтрация
                    if (FilterActive.IsChecked == true)
                        query = query.Where(t => t.StatusId != 3);
                    else if (FilterCompleted.IsChecked == true)
                        query = query.Where(t => t.StatusId == 3);

                    // 3. Сортировка
                    switch (SortComboBox.SelectedIndex)
                    {
                        case 0:
                            query = query.OrderByDescending(t => t.CreatedAt);
                            break;
                        case 1:
                            query = query.OrderBy(t => t.CreatedAt);
                            break;
                        case 2:
                            query = query.OrderBy(t => t.Title);
                            break;
                        default:
                            query = query.OrderByDescending(t => t.CreatedAt);
                            break;
                    }

                    // 4. Вывод
                    TasksListBox.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
            }
        }
        private void InitializeComboBoxes()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    CategoryComboBox.ItemsSource = db.Categories.ToList();
                    PriorityComboBox.ItemsSource = db.TaskPriorities.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочников: {ex.Message}");
            }
        }
    }
}