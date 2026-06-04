using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using ToDo.AppData;
using ToDo.Entities;
using ToDo.Helpers;

namespace ToDo.Pages
{
    public partial class TasksPage : Page
    {
        private int _editingTaskId = 0;
        public TasksPage()
        {
            InitializeComponent();
            UpdateTaskSource();
            LoadCategories();
            InitializeComboBoxes();
        }

        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            string taskTitle = NewTaskTxt.Text.Trim();

            if (string.IsNullOrEmpty(taskTitle))
            {
                MessageBox.Show("Введите название задачи!", "Предупреждение");
                return;
            }

            int? selectedCategoryId = CategoryComboBox.SelectedValue as int?;
            int? selectedPriorityId = PriorityComboBox.SelectedValue as int?;

            string description = DescriptionTxt.Text.Trim();

            DateTime? dueTime = DueTimePick.SelectedTime;
            DateTime? dueDate = DueDatePick.SelectedDate;

            DateTime combinedDateTime = new DateTime(
                dueDate.Value.Year, dueDate.Value.Month, dueDate.Value.Day,
                dueTime.Value.Hour, dueTime.Value.Minute, dueTime.Value.Second
            );

            try
            {
                using (var db = new AppDbContext())
                {
                    if (_editingTaskId != 0)
                    {
                        var taskToUpdate = db.TodoTasks.Find(_editingTaskId);
                        if (taskToUpdate != null)
                        {
                            taskToUpdate.Title = NewTaskTxt.Text;
                            taskToUpdate.Description = DescriptionTxt.Text;
                            taskToUpdate.CategoryId = (int?)CategoryComboBox.SelectedValue;
                            taskToUpdate.PriorityId = (int?)PriorityComboBox.SelectedValue ?? 2;
                            taskToUpdate.DueDate = combinedDateTime;

                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var newTask = new TodoTask
                        {
                            Title = taskTitle,
                            Description = description,
                            DueDate = combinedDateTime,
                            CreatedAt = DateTime.Now,
                            UserId = UserSession.CurrentUserId,
                            StatusId = 1,
                            CategoryId = selectedCategoryId,
                            PriorityId = selectedPriorityId ?? 2
                        };
                        db.TodoTasks.Add(newTask);
                        db.SaveChanges();


                        AddTaskBtn.Content = "Добавить";
                        _editingTaskId = 0;
                        NewTaskTxt.Clear();
                        DescriptionTxt.Clear();
                        DueDatePick.SelectedDate = null;
                        CategoryComboBox.SelectedIndex = -1;
                        PriorityComboBox.SelectedIndex = -1;

                    }
                }
                UpdateTaskSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
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

        private void LoadCategories()
        {
            using (var db = new AppDbContext())
            {
                var categories = db.Categories.ToList();

                categories.Insert(0, new Category { Name = "Все категории", Id = 0 });

                CategoryFilter.ItemsSource = categories;
                CategoryFilter.SelectedIndex = 0;
            }
        }

        private void Filter_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTaskSource();
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTaskSource();
        }
        private void SearchTxt_TextChanged(object sender, TextChangedEventArgs e)
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
                    IQueryable<TodoTask> query = db.TodoTasks.Where(t => t.UserId == UserSession.CurrentUserId);

                    string searchText = SearchTxt.Text.Trim().ToLower();
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query = query.Where(t => t.Title.ToLower().Contains(searchText));
                    }

                    if (FilterActive.IsChecked == true)
                        query = query.Where(t => t.StatusId != 3);
                    else if (FilterCompleted.IsChecked == true)
                        query = query.Where(t => t.StatusId == 3);

                    if (CategoryFilter.SelectedItem is Category selectedCategory && selectedCategory.Id != 0)
                    {
                        query = query.Where(t => t.CategoryId == selectedCategory.Id);
                    }

                    switch (SortComboBox.SelectedIndex)
                    {
                        case 1: query = query.OrderBy(t => t.CreatedAt); break;
                        case 2: query = query.OrderBy(t => t.Title); break;
                        default: query = query.OrderByDescending(t => t.CreatedAt); break;
                    }

                    TasksListBox.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка: {ex.Message}");
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

        private void TasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (TasksListBox.SelectedItem is TodoTask selectedTask)
            {
                _editingTaskId = selectedTask.Id;

                NewTaskTxt.Text = selectedTask.Title;
                DescriptionTxt.Text = selectedTask.Description;
                CategoryComboBox.SelectedValue = selectedTask.CategoryId;
                PriorityComboBox.SelectedValue = selectedTask.PriorityId;
                DueDatePick.SelectedDate = selectedTask.DueDate;

                AddTaskBtn.Content = "Сохранить изменения";
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            AddTaskBtn.Content = "Добавить";
            _editingTaskId = 0;
            NewTaskTxt.Clear();
            DescriptionTxt.Clear();
            DueDatePick.SelectedDate = null;
            CategoryComboBox.SelectedIndex = -1;
            PriorityComboBox.SelectedIndex = -1;
        }
    }
}