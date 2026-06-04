using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ToDo.AppData;
using ToDo.Entities;
using ToDo.Helpers;

namespace ToDo.Pages
{
    public partial class HabitsPage : Page
    {
        public HabitsPage()
        {
            InitializeComponent();
            LoadHabitsFromDb();
        }

        private void LoadHabitsFromDb()
        {
            try
            {
                using (var db = new AppDbContext())
                {

                    var habitsFromDb = db.Habits
                        .Where(h => h.UserId == Helpers.UserSession.CurrentUserId)
                        .ToList();

                    HabitsListBox.ItemsSource = new ObservableCollection<Habit>(habitsFromDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void AddHabitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewHabitTxt.Text)) return;

            using (var db = new AppDbContext())
            {
                var newHabit = new Habit
                {
                    Title = NewHabitTxt.Text.Trim(),
                    Description = "",
                    Streak = 0,
                    UserId = UserSession.CurrentUserId,
                    CreatedAt = DateTime.Now
                };

                db.Habits.Add(newHabit);
                db.SaveChanges();
            }

            NewHabitTxt.Clear();
            LoadHabitsFromDb();
        }

        private void CompleteHabitBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Habit habit)
            {
                using (var db = new AppDbContext())
                {
                    var dbHabit = db.Habits.Find(habit.Id);
                    if (dbHabit != null)
                    {
                        if (dbHabit.LastCompletedDate?.Date != DateTime.Today)
                        {
                            dbHabit.Streak++;
                            dbHabit.LastCompletedDate = DateTime.Now;
                            db.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Вы уже засчитали эту привычку сегодня!");
                        }
                    }
                }
                LoadHabitsFromDb();
            }
        }
        private void DeleteHabitBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Habit habit)
            {
                using (var db = new AppDbContext())
                {
                    var dbHabit = db.Habits.Find(habit.Id);
                    if (dbHabit != null)
                    {
                        db.Habits.Remove(dbHabit);
                        db.SaveChanges();
                    }
                }
                LoadHabitsFromDb();
            }
        }

        public (int total, int completed, double percentage) GetTaskProgress()
        {
            using (var db = new AppDbContext())
            {
                var tasks = db.TodoTasks.Where(t => t.UserId == UserSession.CurrentUserId).ToList();

                int total = tasks.Count;
                if (total == 0) return (0, 0, 0);

                int completed = tasks.Count(t => t.IsCompleted); // Предполагаю, что у тебя есть bool IsCompleted
                double percentage = (double)completed / total * 100;

                return (total, completed, percentage);
            }
        }
    }
}