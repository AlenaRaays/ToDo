using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ToDo.AppData;
using ToDo.Helpers;

namespace ToDo.Pages
{
    public partial class Authorize : Page
    {
        public Authorize()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTxt.Text.Trim();
            string password = PasswordTxt.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);

                if (user != null && Helpers.HashPasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    UserSession.CurrentUserId = user.Id;
                    UserSession.CurrentUsername = user.Username;

                    if (NavigationService != null)
                    {
                        NavigationService.Navigate(new MainWorkspacePage());
                    }

                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            // Переключаем Frame на новую страницу регистрации
            NavigationService.Navigate(new RegisterPage());
        }
    }
}
