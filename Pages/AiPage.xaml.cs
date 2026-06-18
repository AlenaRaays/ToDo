using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ToDo.AppData;
using ToDo.Entities;
using ToDo.Helpers;

namespace ToDo.Pages
{
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(); }
        }

        public bool IsUser { get; set; }
        public HorizontalAlignment Alignment => IsUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public partial class AiPage : Page
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; set; } = new ObservableCollection<ChatMessage>();
        private readonly AiService _aiService = new AiService();
        

        public AiPage()
        {
            InitializeComponent();
            ChatItemsControl.ItemsSource = ChatMessages;
        }

        private async void SendPromptBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                int id = UserSession.CurrentUserId;
                var user = db.Users.FirstOrDefault(u => u.Id == id);
                user.AiRequestCount++;
            }


            string prompt = QuestionTxt.Text.Trim();
            if (string.IsNullOrEmpty(prompt)) return;

            ChatMessages.Add(new ChatMessage { Text = prompt, IsUser = true });
            QuestionTxt.Clear();
            QuestionTxt.IsEnabled = false; 
            SendPromptBtn.IsEnabled = false;
            

            var botMessage = new ChatMessage { Text = "...", IsUser = false };
            ChatMessages.Add(botMessage);

            try
            {
                string response = await _aiService.GetResponseAsync(prompt);
                botMessage.Text = response;
            }
            catch (Exception ex)
            {
                botMessage.Text = $"Ошибка подключения: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            finally
            {
                QuestionTxt.IsEnabled = true;
                SendPromptBtn.IsEnabled = true;
            }
        }
    }
}