namespace Quilt4.Web.DataTransfer
{
    public class RegisterCompatibilityRequest
    {
        public string SupportToolkitNameVersion { get; set; }
        public bool Compatible { get; set; }
        public string Message { get; set; }
    }
}