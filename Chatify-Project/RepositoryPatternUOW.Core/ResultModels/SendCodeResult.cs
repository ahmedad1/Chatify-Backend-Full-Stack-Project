using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.ResultModels
{
    public class SendCodeResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpDate { get; set; }

        public SendCodeResult(bool success, string? token=null, DateTime? expDate=null)
        {
            Success = success;
            Token = token;
            ExpDate = expDate;
        }

    }
}
