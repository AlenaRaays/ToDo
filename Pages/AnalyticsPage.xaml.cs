using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ToDo.AppData;
using ToDo.Helpers;
// using ToDo.Data; // Подключи папку, где лежит твой AppDbContext!

namespace ToDo.Pages
{
    public partial class AnalyticsPage : Page
    {
        public AnalyticsPage()
        {
            InitializeComponent();

            // Подписываемся на событие загрузки страницы
            this.Loaded += AnalyticsPage_Loaded;
        }

        private void AnalyticsPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadAnalyticsFromDb();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить аналитику: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAnalyticsFromDb()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    int currentUserId = UserSession.CurrentUserId;

                    int tasksCount = db.TodoTasks.Where(t => t.UserId == currentUserId).Count();

                    int habitsCount = db.Habits.Where(h => h.UserId == currentUserId).Count();

                    int aiRequestsCount = 0;

                    TotalTasksTxt.Text = tasksCount.ToString();
                    TotalHabitsTxt.Text = habitsCount.ToString();
                    TotalAiRequestsTxt.Text = aiRequestsCount.ToString();

                    StudyProgress.Value = CalculateCategoryProgress(db, "Учеба", currentUserId);
                    SportProgress.Value = CalculateCategoryProgress(db, "Спорт", currentUserId);
                    ChoresProgress.Value = CalculateCategoryProgress(db, "Быт", currentUserId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить аналитику: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private double CalculateCategoryProgress(AppDbContext db, string categoryName, int userId)
        {
            var userCategoryTasks = db.TodoTasks
                .Where(t => t.UserId == userId && t.Category != null && t.Category.Name == categoryName);

            int total = userCategoryTasks.Count();

            if (total == 0) return 0;
            
            int completed = userCategoryTasks.Where(t => t.StatusId == 3).Count();
            return ((double)completed / total) * 100;
        }
    }
}