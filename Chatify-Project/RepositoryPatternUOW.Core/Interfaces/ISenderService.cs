using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.Interfaces
{
    public interface ISenderService
    {
        public Task SendAsync(string to, string subject, string body);
    }
}
