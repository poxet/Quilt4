namespace Quilt4.Web.Models
{
    public class SendEmailViewModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool EmailEnabled { get; set; }

    }
}