using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.ResultModels
{
    public record SignUpResult(bool Success,
                               bool HasRepeatedEmail ,
                               bool HasRepeatedUserName);

  
}
