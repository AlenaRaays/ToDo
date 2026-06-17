using Microsoft.Extensions.AI;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToDo.AppData;
using ToDo.Helpers;

namespace ToDo.Pages
{
    public class ChatMessage
    {
        // Обязательно добавьте { get; set; }, чтобы можно было менять текст
        public string Text { get; set; }
        public bool IsUser { get; set; }

        // Свойство для выравнивания (вычисляемое)
        public HorizontalAlignment Alignment => IsUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }

    public partial class AiPage : Page
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; set; } = new ObservableCollection<ChatMessage>();
        public AiPage()
        {
            InitializeComponent();
            ChatItemsControl.ItemsSource = ChatMessages;
        }

        private async void SendPromptBtn_Click(object sender, RoutedEventArgs e)
        {
            string prompt = QuestionTxt.Text.Trim();
            if (string.IsNullOrEmpty(prompt)) return;

            // 1. Добавляем сообщение пользователя и очищаем поле
            ChatMessages.Add(new ChatMessage { Text = prompt, IsUser = true });
            QuestionTxt.Clear();

            // 2. Добавляем временное сообщение бота
            var botMessage = new ChatMessage { Text = "Печатаю...", IsUser = false };
            ChatMessages.Add(botMessage);

            try
            {
                AiService ai = new AiService();
                string response = await ai.GetResponseAsync(prompt);
                botMessage.Text = response; // Обновляем текст
            }
            catch (Exception ex)
            {
                botMessage.Text = "Ошибка: " + ex.Message;
            }
        }
    }
}
