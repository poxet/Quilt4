namespace Quilt4.Web.Models
{
    public class AdminViewModels
    {
        public string DbType { get; set; }
        public bool DbOnline { get; set; }
        public string DbName { get; set; }
        public string DbServer { get; set; }

        public string SupportEmailAdress { get; set; }
        public string SmtpServerAdress { get; set; }
        public string SmtpServerPort { get; set; }
        public string SendEMailEnabled { get; set; }
        public string EMailConfirmationEnabled { get; set; }
        
        public bool InfluxDbEnabled { get; set; }
        public bool InfluxDbOnline { get; set; }
        public string InfluxDbUrl { get; set; }
        public string InfluxDbVersion { get; set; }
        public string InfluxDbName { get; set; }
    }
}