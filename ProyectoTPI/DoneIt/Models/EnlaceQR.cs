namespace DoneIt.Models
{
    public class EnlaceQR
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
