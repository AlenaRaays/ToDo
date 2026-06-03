using System.Windows;
using System.Windows.Controls;
using ToDo.Helpers;

namespace ToDo.Pages
{
    public partial class MainWorkspacePage : Page
    {
        public MainWorkspacePage()
        {
            InitializeComponent();
            ContentFrame.Navigate(new TasksPage());

            this.Loaded += MainWorkspacePage_Loaded;

            UserName.Text = $"Здравствуйте, {UserSession.CurrentUsername}";
        }

        private void MainWorkspacePage_Loaded(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.MinWidth = 1000;
                parentWindow.MinHeight = 650;

                parentWindow.Width = 1280;
                parentWindow.Height = 800;

                parentWindow.ResizeMode = ResizeMode.CanResize;
                parentWindow.WindowState = WindowState.Maximized;
            }
        }

        private void NavigationMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContentFrame == null || TxtHeaderTitle == null) return;

            if (NavigationMenu.SelectedItem == MenuTasks)
            {
                TxtHeaderTitle.Text = "Мои Задачи";
                ContentFrame.Navigate(new TasksPage());
            }
            else if (NavigationMenu.SelectedItem == MenuHabits)
            {
                TxtHeaderTitle.Text = "Трекер привычек";
                ContentFrame.Navigate(new HabitsPage()); 
            }
            else if (NavigationMenu.SelectedItem == MenuAI)
            {
                TxtHeaderTitle.Text = "ИИ Ассистент";
                ContentFrame.Navigate(new AiPage());
            }
            else if (NavigationMenu.SelectedItem == MenuAnalytics)
            {
                TxtHeaderTitle.Text = "Аналитика";
                ContentFrame.Navigate(new AnalyticsPage());
            }
        }
        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.MinWidth = 0;
                parentWindow.MinHeight = 0;

                parentWindow.ResizeMode = ResizeMode.NoResize;
                parentWindow.WindowState = WindowState.Normal;

                parentWindow.Width = 450;
                parentWindow.Height = 650;

                parentWindow.Left = (SystemParameters.PrimaryScreenWidth - parentWindow.Width) / 2;
                parentWindow.Top = (SystemParameters.PrimaryScreenHeight - parentWindow.Height) / 2;
            }

            if (NavigationService != null)
            {
                NavigationService.Navigate(new Authorize());
            }
        }
    }
}