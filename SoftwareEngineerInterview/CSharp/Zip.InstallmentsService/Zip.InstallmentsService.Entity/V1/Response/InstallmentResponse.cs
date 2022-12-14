using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Zip.InstallmentsService.Entity.V1.Response
{
    public class InstallmentResponse
    {
        public Guid Id { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        [JsonIgnore]
        public Guid CreatedBy { get; set; }
    }
}
