using NoteCLI.Models;
using System;
using System.Collections.Generic;

namespace NoteCLI.Services
{
    public class NoteDataGenerator
    {
        private readonly string[] _categories = { "工作", "学习", "生活", "娱乐", "健康" };
        private readonly string[] _tags = { "重要", "紧急", "待办", "已完成", "想法" };
        private readonly string[] _titles = {
            "项目进度会议",
            "C# 高级编程学习",
            "周末旅行计划",
            "新电影推荐",
            "健身计划",
            "团队建设活动",
            "算法优化方案",
            "读书笔记",
            "音乐播放列表",
            "健康饮食"
        };
        private readonly string[] _contents = {
            "讨论项目进展情况，分配下一步任务。",
            "学习泛型和LINQ查询的高级用法。",
            "计划周末去海边度假，需要提前预订酒店。",
            "最近上映的科幻电影口碑不错，值得一看。",
            "每周三次有氧运动，每次30分钟。",
            "组织团队户外拓展活动，增强团队凝聚力。",
            "优化搜索算法，提高性能。",
            "《人月神话》读书笔记，记录关键观点。",
            "整理个人音乐收藏，创建新的播放列表。",
            "制定健康饮食计划，减少糖分摄入。"
        };

        public List<Note> GenerateSampleNotes(int count = 20)
        {
            List<Note> notes = new List<Note>();
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                Note note = new Note
                {
                    Id = Guid.NewGuid(),
                    Title = _titles[random.Next(_titles.Length)] + " " + (i + 1),
                    Content = _contents[random.Next(_contents.Length)],
                    Category = _categories[random.Next(_categories.Length)],
                    CreatedAt = DateTime.Now.AddDays(-random.Next(30)),
                    UpdatedAt = DateTime.Now.AddDays(-random.Next(30))
                };

                // 随机选择1-3个标签
                List<string> selectedTags = new List<string>();
                int tagCount = random.Next(1, 4);
                for (int j = 0; j < tagCount; j++)
                {
                    string tag = _tags[random.Next(_tags.Length)];
                    if (!selectedTags.Contains(tag))
                    {
                        selectedTags.Add(tag);
                    }
                }
                note.Tags = selectedTags;

                notes.Add(note);
            }

            return notes;
        }
    }
}