using NoteCLI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NoteCLI.Services
{
    public class NoteStorageService
    {
        private readonly string _storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "notes.json");

        public NoteStorageService()
        {
            if (!File.Exists(_storagePath))
            {
                File.WriteAllText(_storagePath, "[]");
            }
        }

        public List<Note> GetAllNotes()
        {
            var json = File.ReadAllText(_storagePath);
            return JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
        }

        public Note GetNoteById(Guid id)
        {
            return GetAllNotes().FirstOrDefault(n => n.Id == id);
        }

        public void SaveNote(Note note)
        {
            var notes = GetAllNotes();
            var existingNote = notes.FirstOrDefault(n => n.Id == note.Id);

            if (existingNote != null)
            {
                notes.Remove(existingNote);
            }

            notes.Add(note);
            SaveAllNotes(notes);
        }

        public void DeleteNote(Guid id)
        {
            var notes = GetAllNotes();
            var note = notes.FirstOrDefault(n => n.Id == id);
            if (note != null)
            {
                notes.Remove(note);
                SaveAllNotes(notes);
            }
        }

        private void SaveAllNotes(List<Note> notes)
        {
            var json = JsonSerializer.Serialize(notes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_storagePath, json);
        }
    }
}