using RepositoryPattern.Core.Models;
using RepositoryPatternUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IBaseRepository<User>UserRepository { get; }
        IBaseRepository<UserConnection> UserConnectionRepository { get; }
        IBaseRepository<Group> GroupRepository { get; }
        IBaseRepository<VerificationCode> VerificationCodeRepository { get; }
        IBaseRepository<IdentityTokenVerification> IdentityTokenVerificationRepository { get; }
        IBaseRepository<RefreshToken> RefreshTokenRepository { get; }
        IMessageRepository MessageRepository { get; }
        IBaseRepository<FriendRequest> FriendRequestRepository { get; }

        public void SetLazyLoading(bool IsEnabled);
        public Task SaveChangesAsync();
    }
}
