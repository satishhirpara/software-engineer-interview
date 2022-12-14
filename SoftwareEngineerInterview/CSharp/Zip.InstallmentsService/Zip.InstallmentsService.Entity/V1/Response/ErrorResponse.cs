using System;
using System.Collections.Generic;
using System.Text;
using Zip.InstallmentsService.Entity.Common;

namespace Zip.InstallmentsService.Entity.V1.Response
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
