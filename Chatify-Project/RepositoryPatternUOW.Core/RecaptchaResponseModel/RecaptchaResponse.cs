using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.RecaptchaResponseModel
{
    public class RecaptchaResponse
    {
       
            public bool success { get; set; }
            public DateTime challenge_ts { get; set; }
            public string hostname { get; set; }
            public float score { get; set; }
            public string action { get; set; }


    }
}
