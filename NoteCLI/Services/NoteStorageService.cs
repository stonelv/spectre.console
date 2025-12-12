using NoteCLI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NoteCLI.Services
{
    public class NoteStorageService
    {
        private readonly string _storagePath;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public NoteStorageService(string storagePath = "notes.json")
        {
            _storagePath = storagePath;
            InitializeStorage();
        }

        private void InitializeStorage()
        {
            if (!File.Exists(_storagePath))
            {
                File.WriteAllText(_storagePath, "[]");
            }
        }

        public List<Note> GetAllNotes()
        {
            string json = File.ReadAllText(_storagePath);
            return JsonSerializer.Deserialize<List<Note>>(json, _jsonOptions);
        }

        public Note GetNoteById(Guid id)
        {
            List<Note> notes = GetAllNotes();
            return notes.Find(note => note.Id == id);
        }

        public void SaveNote(Note note)
        {
            List<Note> notes = GetAllNotes();
            Note existingNote = notes.Find(n => n.Id == note.Id);

            if (existingNote != null)
            {
                existingNote.Title = note.Title;
                existingNote.Content = note.Content;
                existingNote.Category = note.Category;
                existingNote.Tags = note.Tags;
                existingNote.UpdatedAt = DateTime.Now;
            }
            else
            {
                note.Id = Guid.NewGuid();
                note.CreatedAt = DateTime.Now;
                note.UpdatedAt = DateTime.Now;
                notes.Add(note);
            }

            string json = JsonSerializer.Serialize(notes, _jsonOptions);
            File.WriteAllText(_storagePath, json);
        }

        public void DeleteNote(Guid id)
        {
            List<Note> notes = GetAllNotes();
            notes.RemoveAll(note => note.Id == id);
            string json = JsonSerializer.Serialize(notes, _jsonOptions);
            File.WriteAllText(_storagePath, json);
        }
    }
}