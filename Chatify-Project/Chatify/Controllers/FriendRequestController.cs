using Chatify.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using RepositoryPattern.Core.DTOs;
using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.Models;
using RepositoryPattern.Core.ResultModels;
using RepositoryPatternUOW.Core.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Chatify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendRequestController(IUnitOfWork unitOfWork, IHubContext<ChatHub> chatHub) : ControllerBase
    {
        [HttpGet("page/{pageNum}")]
        public async Task<IActionResult> Get(int pageNum)
        {
            int id = int.Parse(JwtHandler.ExtractPayload(Request)[JwtRegisteredClaimNames.NameId].ToString()!);

            var result =await unitOfWork.FriendRequestRepository.GetWhere(x => x.RecipientId == id, pageNum, ["Sender"]);
            return Ok(result.Select(x => new { x.Sender.FirstName, x.Sender.LastName, x.Sender.UserName }));

        }
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(FriendRequestDto friendRequestDto)
        {
            var payload = JwtHandler.ExtractPayload(Request);
            int id = int.Parse(payload[JwtRegisteredClaimNames.NameId].ToString()!);
            string userName = payload[JwtRegisteredClaimNames.UniqueName].ToString()!;
            string firstName = payload[JwtRegisteredClaimNames.Name].ToString()!;
            string lastName = payload[JwtRegisteredClaimNames.FamilyName].ToString()!;
     
            var recevier = await unitOfWork.UserRepository.GetOneByAsync(x => x.UserName == friendRequestDto.UserName);
            if (recevier is null) return BadRequest();
            if (recevier!.Groups!.Any(x => x.Users.Any(x => x.UserName == userName))
                ||await unitOfWork.FriendRequestRepository.ExistsAsync(x=>x.SenderId==id&&x.RecipientId==recevier.Id||x.SenderId==recevier.Id&&x.RecipientId==id)
                )
                return BadRequest();

            var request = new FriendRequest() { RecipientId = recevier.Id, SenderId =id };
            await unitOfWork.FriendRequestRepository.AddAsync(request);
            var connectionIdsOfReceipant = (await unitOfWork.UserConnectionRepository.GetWhere(x => x.User.UserName == friendRequestDto.UserName)).ToList();
            if (!connectionIdsOfReceipant.IsNullOrEmpty())
            {
                for (int i = 0; i < connectionIdsOfReceipant.Count(); i++) {
                    await chatHub.Clients.Client(connectionIdsOfReceipant[i].ConnectionId).SendAsync("friendRequest", userName,firstName,lastName);
                }

                await unitOfWork.SaveChangesAsync();
            }
            else
                await unitOfWork.SaveChangesAsync();
            return Ok();

        }
        [HttpDelete]
        public async Task<IActionResult>CancelRequest(FriendRequestDto friendRequestDto)
        {
            var payload = JwtHandler.ExtractPayload(Request);
            int id = int.Parse(payload[JwtRegisteredClaimNames.NameId].ToString()!);
            var userName = payload[JwtRegisteredClaimNames.UniqueName];
            var recipient = await unitOfWork.UserRepository.GetOneByAsync(x => x.UserName == friendRequestDto.UserName);
            if (recipient is null)
                return NotFound();
            var result = await unitOfWork.FriendRequestRepository.ExecuteDeleteAsync(x => x.SenderId == id && x.RecipientId == recipient.Id);
            if (result == 0)
                return NotFound();
            var connectionIdsOfReceipant = (await unitOfWork.UserConnectionRepository.GetWhere(x => x.User.UserName == friendRequestDto.UserName)).ToList();
            if (!connectionIdsOfReceipant.IsNullOrEmpty())
                for (int i = 0; i < connectionIdsOfReceipant.Count(); i++)
                {
                    await chatHub.Clients.Client(connectionIdsOfReceipant[i].ConnectionId).SendAsync("friendRequestCancelled", userName);
                }

            return Ok();
             
            


        }
        [HttpPost("response")]
        public async Task<IActionResult> Response(ResponseFriendRequestDto response)
        {
            int id = int.Parse(JwtHandler.ExtractPayload(Request)[JwtRegisteredClaimNames.NameId].ToString()!);//who send the response
            if (!response.IsAccepted)
            {
                int result = await unitOfWork.FriendRequestRepository.ExecuteDeleteAsync(x => x.RecipientId == id && x.Sender.UserName == response.UserName);
                return result > 0 ? Ok() : BadRequest();
            }
            var user2 = await unitOfWork.UserRepository.GetOneByAsync(x => x.UserName == response.UserName);


            if (user2 is null) return BadRequest();
            if (!await unitOfWork.FriendRequestRepository.ExistsAsync(x => x.SenderId == user2.Id && x.RecipientId == id))
                return Created();
            var user1 = await unitOfWork.UserRepository.GetByIdAsync(id);
            var newGroup = Guid.NewGuid().ToString()!;
            await unitOfWork.GroupRepository.AddAsync(new Group { Id = newGroup, Users = new List<User> { user1, user2 } });
            await unitOfWork.FriendRequestRepository.ExecuteDeleteAsync(x => x.RecipientId == id && x.Sender.UserName == response.UserName);
            Func<User[], Task> addToGroup = async (User[] users) =>
            {
                for (int p = 0; p < users.Length; p++)
                    if (users[p].UserConnections is not null && users[p].UserConnections!.Any())
                    {
                        var connections = users[p].UserConnections!.ToList();
                        for (int i = 0; i < connections.Count; i++)
                        {
                            await chatHub.Groups.AddToGroupAsync(connections[i].ConnectionId, newGroup);
                            if (p == users.Length - 1)
                            {
                                await chatHub.Clients.Client(connections[i].ConnectionId).SendAsync("requestAccepted", user1.UserName, user1.FirstName, user1.LastName,newGroup);
                            }
                        }
                    }
            };

            await Task.WhenAll(addToGroup([user1, user2]),
            unitOfWork.SaveChangesAsync());

            return Ok();


        }
        [HttpGet("people/{pageNum}")]
        public async Task<IActionResult> Search(string searchKey,int pageNum)
        {
            var id = int.Parse(JwtHandler.ExtractPayload(Request)[JwtRegisteredClaimNames.NameId].ToString()!);
            var user = await unitOfWork.UserRepository.GetByIdAsync(id);
            searchKey = searchKey.Trim();
            //last Modification (excepting the friends )
            var users =(await unitOfWork.UserRepository.GetWhere(x => (((x.FirstName + " " + x.LastName).Contains(searchKey)) || x.UserName.Contains(searchKey)) && x.EmailConfirmed == true && x.Id != id&&!x.Groups.Any(g=>g.Users.Any(u=>u.Id==user.Id)), pageNum))
                .Select(x => new SearchResult() {Id=x.Id,UserName= x.UserName,FirstName= x.FirstName,LastName= x.LastName,GotRequest = false }).ToList();
            unitOfWork.SetLazyLoading(true);
            if(user.SentRequests is not null)
            for (int i =0;i<users.Count();i++)
            {
                if (user.SentRequests.Any(x => x.RecipientId == users[i].Id))
                {
                        users[i].GotRequest = true;
                }
                if (user.RecievedRequests is not null && user.RecievedRequests.Any(x => x.SenderId == users[i].Id))
                    users[i].SentRequest = true;
            }
            
            return Ok(users);
        }

    }
}
