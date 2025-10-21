namespace SafeScribe.Api.DTOs
{
    public class NoteResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
    }
}
