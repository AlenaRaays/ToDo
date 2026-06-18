using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ToDo.AppData;
using ToDo.Entities;
using ToDo.Helpers;
using ToDo.Pages;

namespace ToDo
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new User
            {
                Username = LoginTxt.Text.Trim(),
                Email = EmailTxt.Text.Trim(),
                PasswordHash = HashPasswordHelper.HashPassword(PasswordBox.Password)
            };

            var context = new System.ComponentModel.DataAnnotations.ValidationContext(newUser);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(newUser, context, results, true))
            {
                MessageBox.Show(results[0].ErrorMessage);
                return;
            }


            using (var db = new AppDbContext())
            {
                db.Users.Add(newUser);
                db.SaveChanges();
                MessageBox.Show("Регистрация успешна!");
                NavigationService.Navigate(new Authorize());
            }
        }
        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Authorize());
        }
    }
}