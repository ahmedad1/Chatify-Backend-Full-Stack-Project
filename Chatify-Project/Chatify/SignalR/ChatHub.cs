using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RepositoryPattern.Core.Interfaces;
using RepositoryPatternUOW.Core.Models;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;

namespace Chatify.SignalR
{
    [Authorize]
    public class ChatHub(IUnitOfWork unitOfWork):Hub
    {
        
        public override async Task OnConnectedAsync()
        {
            var userId =int.Parse(JwtHandler.ExtractPayload(Context.GetHttpContext()!.Request)[JwtRegisteredClaimNames.NameId].ToString()!);   
            var user=await unitOfWork.UserRepository.GetByIdAsync(userId);
            user!.UserConnections!.Add(new() { ConnectionId = Context.ConnectionId });
            var groups = user.Groups?.Select(x=>x.Id).ToList();
            if (groups is not null)
            {
                for (int i = 0; i < groups.Count(); i++)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, groups[i]);

                }
                if (groups.Any())
                {
                    await Task.WhenAll(Clients.Groups(groups).SendAsync("isActive", user.UserName, user.FirstName, user.LastName),
                         unitOfWork.SaveChangesAsync());
                }
                else
                    await unitOfWork.SaveChangesAsync();
                
                
            }else 
                await unitOfWork.SaveChangesAsync();
            await base.OnConnectedAsync();
        }
        public async Task SendMessage(string message,string groupId)
        {
            var payload = JwtHandler.ExtractPayload(Context.GetHttpContext()!.Request);
            var userName =payload[JwtRegisteredClaimNames.UniqueName].ToString()!;
            var userId =int.Parse(payload[JwtRegisteredClaimNames.NameId].ToString()!);
            var group=await unitOfWork.GroupRepository.GetByIdAsync(groupId);
            if (groupId is null)
            {
                Context.Abort();
                return;
            }
            var receiverId = group.Users.FirstOrDefault(x => x.Id != userId)!;
            if (receiverId is null) {
                unitOfWork.GroupRepository.Remove(group);
                await unitOfWork.SaveChangesAsync();
                Context.Abort();
                return;
            }
            await unitOfWork.MessageRepository.AddAsync(new() {GroupId=groupId,MessageText=message,SenderId=userId,ReceiverId=receiverId.Id });
            await Task.WhenAll(unitOfWork.SaveChangesAsync(),Clients.OthersInGroup(groupId).SendAsync("newMessage",userName,message));

        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId =int.Parse(JwtHandler.ExtractPayload(Context.GetHttpContext()!.Request)[JwtRegisteredClaimNames.NameId].ToString()!);
            var user=await unitOfWork.UserRepository.GetByIdAsync(userId);
            var groups = user!.Groups!.Select(x=>x.Id);
            await Task.WhenAll(Clients.Groups(groups).SendAsync("isNotActive", user.UserName),
             unitOfWork.UserConnectionRepository.ExecuteDeleteAsync(x => x.ConnectionId == Context.ConnectionId));
            await base.OnDisconnectedAsync(exception);
        }
    }
}
