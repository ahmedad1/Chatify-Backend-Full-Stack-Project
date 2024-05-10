using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.Models;
using RepositoryPattern.EFcore.Repositories;
using RepositoryPatternUOW.Core.Models;
using RepositoryPatternUOW.EFcore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.EFcore
{
    public class UnitOfWork:IUnitOfWork
    {
        AppDbContext context;
        public IBaseRepository<User> UserRepository{get;}
        public IBaseRepository<UserConnection> UserConnectionRepository{get;}
        public IBaseRepository<Group> GroupRepository { get; }
        public IBaseRepository<VerificationCode> VerificationCodeRepository { get; }
        public IBaseRepository<IdentityTokenVerification> IdentityTokenVerificationRepository { get; }
        public IBaseRepository<RefreshToken> RefreshTokenRepository { get; }

        public IBaseRepository<Message> MessageRepository { get; }

        public IBaseRepository<FriendRequest> FriendRequestRepository {get;}

        public UnitOfWork(AppDbContext context) {
            this.context = context;
            UserRepository=new BaseRepository<User>(context);
            UserConnectionRepository=new BaseRepository<UserConnection>(context);
            GroupRepository=new BaseRepository<Group>(context);
            VerificationCodeRepository=new BaseRepository<VerificationCode>(context);
            IdentityTokenVerificationRepository=new BaseRepository<IdentityTokenVerification>(context);
            RefreshTokenRepository=new BaseRepository<RefreshToken>(context);
            FriendRequestRepository = new BaseRepository<FriendRequest>(context);

    }
    public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public void SetLazyLoading(bool IsEnabled)
        {
            context.ChangeTracker.LazyLoadingEnabled = IsEnabled;
        }
    }
}
