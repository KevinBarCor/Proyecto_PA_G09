namespace UamHelpDeskPA.Mvc.Models
{
    public class MySessionViewModel
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? RevokedReason { get; set; }
    }
}
