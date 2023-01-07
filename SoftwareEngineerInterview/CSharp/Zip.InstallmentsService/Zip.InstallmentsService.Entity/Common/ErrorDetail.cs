using System.Text.Json;

namespace Zip.InstallmentsService.Entity.Common
{
    /// <summary>
    /// Data structure which defines all the properties for error details
    /// </summary
    public class ErrorDetail
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
