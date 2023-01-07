namespace Zip.InstallmentsService.Entity.Common
{
    /// <summary>
    /// Data structure which defines all the properties for request validation
    /// </summary>
    public class ValidateRequest
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
