using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.Models;
using RepositoryPatternUOW.EFcore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.EFcore.Repositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
       

        public MessageRepository( AppDbContext context):base(context)
        { }

        public async Task<int?> GetLastId()
        {
            return await context.Set<Message>().MaxAsync(x => x.Id);
        }
    }
}
