using Microsoft.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToDo.AppData;
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
        public AiPage()
        {
            InitializeComponent();
            ChatItemsControl.ItemsSource = ChatMessages;
        }

        private async void SendPromptBtn_Click(object sender, RoutedEventArgs e)
        {
            string prompt = QuestionTxt.Text.Trim();
            if (string.IsNullOrEmpty(prompt)) return;

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
                // Просто выводим сообщение об ошибке
                botMessage.Text = $"Ошибка: {ex.Message}";

                // Это по-прежнему будет работать и писать детали в окно Output в Visual Studio
                System.Diagnostics.Debug.WriteLine($"Детали ошибки: {ex.ToString()}");
            }
        }
    }
}
