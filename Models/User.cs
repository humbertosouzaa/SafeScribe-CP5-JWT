namespace SafeScribe.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public Role Role { get; set; }
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
