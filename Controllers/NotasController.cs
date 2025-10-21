using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeScribe.Api.Data;
using SafeScribe.Api.DTOs;
using SafeScribe.Api.Models;

namespace SafeScribe.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotasController : ControllerBase
    {
        private readonly AppDbContext _db;
        public NotasController(AppDbContext db) => _db = db;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                               ?? User.FindFirstValue(ClaimTypes.Name) 
                                               ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        private string CurrentRole => User.FindFirstValue(ClaimTypes.Role) ?? "Leitor";

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> Create(NoteCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                              ?? User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = int.Parse(userIdClaim)
            };
            _db.Notes.Add(note);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = note.Id }, 
                new NoteResponseDto { Id = note.Id, Title = note.Title, Content = note.Content, CreatedAt = note.CreatedAt, UserId = note.UserId });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var note = await _db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
            if (note == null) return NotFound();

            var isAdmin = CurrentRole == nameof(Role.Admin);
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
            var isOwner = note.UserId == int.Parse(userIdClaim);

            if (!isAdmin && !isOwner) return Forbid();

            return Ok(new NoteResponseDto { Id = note.Id, Title = note.Title, Content = note.Content, CreatedAt = note.CreatedAt, UserId = note.UserId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, NoteUpdateDto dto)
        {
            var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id);
            if (note == null) return NotFound();

            var isAdmin = CurrentRole == nameof(Role.Admin);
            var userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isOwner = note.UserId == userIdClaim;

            if (!isAdmin && !isOwner) return Forbid();

            note.Title = dto.Title;
            note.Content = dto.Content;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id);
            if (note == null) return NotFound();

            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
