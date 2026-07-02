namespace UamHelpDeskPA.Api.Models
{
    public class PendingSession
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string SessionToken { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public User User { get; set; } = null!;
    }
}