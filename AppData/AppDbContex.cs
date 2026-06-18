using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToDo.Entities;

namespace ToDo.AppData
{
    public class AppDbContext : DbContext
    {
        public DbSet <User> Users { get; set; }
        public DbSet <Role> Roles { get; set; }
        public DbSet <UserSetting> UserSettings { get; set; }
        public DbSet <TaskStatusM> TaskStatusesM { get; set; }
        public DbSet <TaskPriority> TaskPriorities { get; set; }
        public DbSet <Category> Categories { get; set; }
        public DbSet <TodoTask> TodoTasks { get; set; }
        public DbSet <Tag> Tags { get; set; }
        public DbSet <TodoTaskTag> TodoTaskTags { get; set; }
        public DbSet <Comment> Comments { get; set; }
        public DbSet <Attachment> Attachments { get; set; }
        public DbSet <Reminder> Reminders { get; set; }
        public DbSet <Notification> Notifications { get; set; }
        public DbSet <Habit> Habits { get; set; }
        public DbSet <HabitLog> HabitLogs { get; set; }
        

        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=ALENA\SQLEXPRESS;Database=ToDo;
                Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Администратор" },
                new Role { Id = 2, Name = "Пользователь" }
            );

            modelBuilder.Entity<TaskStatusM>().HasData(
                new TaskStatusM { Id = 1, Name = "Новая" },
                new TaskStatusM { Id = 2, Name = "В работе" },
                new TaskStatusM { Id = 3, Name = "Выполнена" }
            );

            modelBuilder.Entity<TaskPriority>().HasData(
                new TaskPriority { Id = 1, Name = "Низкий", ColorHex = "#808080" },  // Серый
                new TaskPriority { Id = 2, Name = "Средний", ColorHex = "#007ACC" }, // Синий
                new TaskPriority { Id = 3, Name = "Высокий", ColorHex = "#E51400" }  // Красный
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Работа", Icon = "Briefcase" },
                new Category { Id = 2, Name = "Учеба", Icon = "School" },
                new Category { Id = 3, Name = "Личное", Icon = "Account" }
            );
        }
    }
}
