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
                    var allTasks = db.TodoTasks.Where(t => t.UserId == currentUserId).ToList();

                    var user = db.Users.FirstOrDefault(u => u.Id == currentUserId);
                    int aiCount = user?.AiRequestCount ?? 0;

                    int total = allTasks.Count;
                    int completed = allTasks.Count(t => t.StatusId == 3);

                    TotalTasksTxt.Text = total.ToString();
                    TotalHabitsTxt.Text = db.Habits.Count(h => h.UserId == currentUserId).ToString();
                    TotalAiRequestsTxt.Text = aiCount.ToString();

                    double percentage = total == 0 ? 0 : ((double)completed / total) * 100;

                    TasksProgressBar.Value = percentage;
                    ProgressText.Text = $"Выполнено: {completed} из {total} задач ({Math.Round(percentage)}%)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить аналитику: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAnalyticsFromDb();
        }
    }
}