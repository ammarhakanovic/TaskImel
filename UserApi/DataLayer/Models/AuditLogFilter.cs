namespace UserApi.DataLayer.Models
{
    public class AuditLogFilter
    {
        public string? ChangedBy { get; set; }
        public string? Action { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
