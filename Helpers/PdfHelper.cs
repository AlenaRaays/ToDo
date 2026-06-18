using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Linq;
using ToDo.Entities;

namespace ToDo.Helpers
{
    public class PdfHelper
    {
        public static void GenerateTaskPdf(string filePath, TodoTask task)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            string priorityName = "Не указан";

            using (var db = new ToDo.AppData.AppDbContext())
            {
                var priority = db.TaskPriorities.FirstOrDefault(p => p.Id == task.PriorityId);
                if (priority != null)
                {
                    priorityName = priority.Name;
                }
            }

                Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(3, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));


                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("Моя задача").FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                            column.Item().Text(System.DateTime.Now.ToString("dd MMMM yyyy")).FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                    });


                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        // Название задачи
                        column.Item().BorderBottom(1).PaddingBottom(5).Text(task.Title).FontSize(18).Bold();

                        column.Item().PaddingTop(10).Text("Описание:").SemiBold();
                        column.Item().Text(string.IsNullOrWhiteSpace(task.Description) ? "Нет описания" : task.Description);

                        // Информация о задаче в таблице
                        column.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns => { columns.ConstantColumn(150); columns.RelativeColumn(); });

                            AddRow(table, "Дата выполнения:", task.DueDate?.ToString("dd.MM.yyyy HH:mm") ?? "Не указана");
                            AddRow(table, "Статус:", task.StatusId == 3 ? "Выполнено" : "В процессе");
                            AddRow(table, "Приоритет:", priorityName);
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Сгенерировано в приложении ToDo");
                    });
                });
            }).GeneratePdf(filePath);
        }

        private static void AddRow(TableDescriptor table, string label, string value)
        {
            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(label).SemiBold();
            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(value);
        }
    }
}