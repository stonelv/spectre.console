using NoteCLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoteCLI.Services
{
    public class NoteDataGenerator
    {
        private static readonly List<string> Categories = new List<string> { "工作", "学习", "生活", "娱乐", "健康" };
        private static readonly List<string> AllTags = new List<string> { "重要", "紧急", "待办", "阅读", "写作", "编程", "健身", "旅行" };

        public List<Note> GenerateMockNotes(int count = 20)
        {
            var notes = new List<Note>();
            var random = new Random();

            for (int i = 1; i <= count; i++)
            {
                var category = Categories[random.Next(Categories.Count)];
                var tagCount = random.Next(1, 4);
                var tags = AllTags.OrderBy(x => random.Next()).Take(tagCount).ToList();

                notes.Add(new Note
                {
                    Id = Guid.NewGuid(),
                    Title = $"{category}笔记{i}",
                    Content = $"这是{category}类别的第{i}条笔记内容，包含{string.Join(", ", tags)}等标签。",
                    Category = category,
                    Tags = tags,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(30)),
                    UpdatedAt = DateTime.Now.AddDays(-random.Next(30))
                });
            }

            return notes;
        }
    }
}