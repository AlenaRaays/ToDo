using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToDo.AppData;
using ToDo.Helpers;

namespace ToDo.Pages
{
    public partial class AiPage : Page
    {
        private ObservableCollection<ChatMessage> _messages;

        public AiPage()
        {
            InitializeComponent();
            _messages = new ObservableCollection<ChatMessage>
            {
                new ChatMessage { Text = "Привет! Я твой ИИ-помощник ToDo. Могу составить план на день или помочь побороть прокрастинацию. Задавай вопрос!", IsUser = false }
            };
            ChatItemsControl.ItemsSource = _messages;
        }

        private async void SendPromptBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AiPromptTxt.Text)) return;

            string userText = AiPromptTxt.Text.Trim();
            _messages.Add(new ChatMessage { Text = userText, IsUser = true });
            AiPromptTxt.Clear();

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == UserSession.CurrentUserId);
                if (user != null)
                {
                    user.AiRequestCount++;
                    db.SaveChanges();
                }
            }

            _messages.Add(new ChatMessage { Text = "Нейросеть генерирует ответ...", IsUser = false });
            ChatItemsControl.Items.Refresh();

            await Task.Delay(1500); // Имитация ожидания сервера

            _messages.RemoveAt(_messages.Count - 1); 
            _messages.Add(new ChatMessage { Text = $"Отличный запрос: '{userText}'. Чтобы всё успеть, разбей задачи на мелкие шаги и включи таймер Помодоро на 25 минут!", IsUser = false });
        }
    }

    public class ChatMessage
    {
        public string? Text { get; set; }
        public bool IsUser { get; set; }
        public HorizontalAlignment Alignment => IsUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }
}