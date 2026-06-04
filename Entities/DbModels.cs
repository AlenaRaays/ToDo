using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Entities
{
    //1. Роли
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }

    // 2. Пользователи
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Логин должен быть от 4 до 20 символов")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        public int AiRequestCount { get; set; }
        public int RoleId { get; set; } = 2;

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        // Навигационные свойства
        public virtual ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();
        public virtual ICollection<Habit> Habits { get; set; } = new List<Habit>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual UserSetting? Setting { get; set; }
    }

    // 3. Настройки пользователя (тема, язык и т.д.)
    public class UserSetting
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public string Theme { get; set; } = "Light"; // Light / Dark
        public string Language { get; set; } = "Ru";
    }

    // 4. Статусы задач (Новая, В процессе, Выполнена)
    public class TaskStatusM
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }

    // 5. Приоритеты задач (Низкий, Средний, Высокий)
    public class TaskPriority
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        public string? ColorHex { get; set; } // Цвет для UI
    }

    // 6. Категории (Работа, Учеба, Личное)
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; } // Иконка для интерфейса
    }

    // 7. Сами Задачи
    public class TodoTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; } // Дедлайн
        
        [NotMapped]
        public bool IsCompleted
        {
            get => StatusId == 3;
            set => StatusId = value ? 3 : 1;
        }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual TaskStatusM? Status { get; set; }
        public int PriorityId { get; set; }
        [ForeignKey("PriorityId")]
        public virtual TaskPriority? Priority { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<TodoTaskTag> TaskTags { get; set; } = new List<TodoTaskTag>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }


    // 8. Теги (Метки для задач)
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<TodoTaskTag> TaskTags { get; set; } = new List<TodoTaskTag>();
    }

    // 9. Связующая таблица Многие-ко-Многим (Задача <-> Тег)
    public class TodoTaskTag
    {
        [Key]
        public int Id { get; set; }

        public int TodoTaskId { get; set; }
        [ForeignKey("TodoTaskId")]
        public virtual TodoTask? TodoTask { get; set; }

        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag? Tag { get; set; }
    }

    // 10. Комментарии к задачам
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int TodoTaskId { get; set; }
        [ForeignKey("TodoTaskId")]
        public virtual TodoTask? TodoTask { get; set; }
    }

    // 11. Вложения (Файлы/ссылки к задачам)
    public class Attachment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FilePathOrUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public int TodoTaskId { get; set; }
        [ForeignKey("TodoTaskId")]
        public virtual TodoTask? TodoTask { get; set; }
    }

    // 12. Напоминания для задач
    public class Reminder
    {
        [Key]
        public int Id { get; set; }
        public DateTime ReminderTime { get; set; }
        public bool IsTriggered { get; set; } = false;

        public int TodoTaskId { get; set; }
        [ForeignKey("TodoTaskId")]
        public virtual TodoTask? TodoTask { get; set; }
    }

    // 13. Системные уведомления для пользователя
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }

    // 14. Фишка: Привычки (Трекер привычек)
    public class Habit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Streak { get; set; }
        public DateTime? LastCompletedDate {  get; set; }
        public DateTime? CreatedAt { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<HabitLog> HabitLogs { get; set; } = new List<HabitLog>();
    }

    // 15. Логи привычек (Отметки по дням: выполнено/нет)
    public class HabitLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime LogDate { get; set; }
        public bool IsCompleted { get; set; } = true;

        public int HabitId { get; set; }
        [ForeignKey("HabitId")]
        public virtual Habit? Habit { get; set; }
    }
}
