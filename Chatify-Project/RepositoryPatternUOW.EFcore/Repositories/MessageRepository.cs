using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.Models;
using RepositoryPatternUOW.EFcore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            if(await context.Set<Message>().AnyAsync())
            return await context.Set<Message>().MaxAsync(x => x.Id);
            return 0;
        }
        
        public async Task MakeAllReadInGroup(string groupId)
        {
           await context.Messages.Where(x=>x.GroupId==groupId).ExecuteUpdateAsync(x=>x.SetProperty(x=>x.IsRead,true));
           
        }
        
    }
}
