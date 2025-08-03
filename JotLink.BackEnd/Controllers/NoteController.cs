using Microsoft.AspNetCore.Mvc;
using JotLink.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JotLink.BackEnd.Controllers
{
    [ApiController]
    public class NoteController : ControllerBase
    {
        private static readonly Dictionary<Guid, Note> Notes = new();
        private static readonly Dictionary<string, Note> NotesByPublicId = new();

        [HttpPost("notes")]
        public IActionResult CreateNote([FromBody] NoteDTO dto)
        {
            var note = new Note
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                PublicId = string.IsNullOrEmpty(dto.PublicId) ? GenerateShortId() : dto.PublicId
            };

            Notes[note.Id] = note;
            NotesByPublicId[note.PublicId] = note;

            return Ok(ToDTO(note));
        }

        [HttpGet("n/{publicId}")]
        public IActionResult GetNoteByPublicId(string publicId)
        {
            if (!NotesByPublicId.TryGetValue(publicId, out var note))
                return NotFound();

            return Ok(ToDTO(note));
        }

        [HttpPut("notes/{id}")]
        public IActionResult UpdateNote(Guid id, [FromBody] NoteDTO dto)
        {
            if (!Notes.TryGetValue(id, out var existingNote))
                return NotFound();

            existingNote.Title = dto.Title;
            existingNote.Content = dto.Content;
            existingNote.LastModified = DateTime.UtcNow;

            Notes[id] = existingNote;

            return Ok(ToDTO(existingNote));
        }

        private static NoteDTO ToDTO(Note note)
        {
            return new NoteDTO
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                LastModified = note.LastModified,
                PublicId = note.PublicId
            };
        }

        private static string GenerateShortId(int length = 8)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class Note
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public string PublicId { get; set; } = "";
    }
}