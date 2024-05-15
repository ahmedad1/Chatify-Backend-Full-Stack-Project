using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryPattern.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Chatify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController(IUnitOfWork unitOfWork) : ControllerBase
    {
        [HttpGet("group/{group}/messages/page/{pageNumber}")]
        public async Task<IActionResult> Get(string group,int pageNumber) {
            int id = int.Parse(JwtHandler.ExtractPayload(Request)[JwtRegisteredClaimNames.NameId].ToString()!);
            var groupObj =await unitOfWork.GroupRepository.GetByIdAsync(group);
            if (groupObj is null || !groupObj.Users.Any(x => x.Id == id))
                return Forbid();
            var result = (await unitOfWork.MessageRepository.GetWhere(x => x.GroupId == group, pageNumber, ["Sender", "Receiver"],true,20)).Select(x => new {x.Id,Message=x.MessageText,UserName=x.Sender.UserName,x.GroupId});
          
            return Ok(result);
          
            
        }
        
        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            int id = int.Parse(JwtHandler.ExtractPayload(Request)[JwtRegisteredClaimNames.NameId].ToString()!);
            bool hasFriendRequests =await unitOfWork.FriendRequestRepository.ExistsAsync(x => x.RecipientId == id);
            var result =await  unitOfWork.GroupRepository.GetWhere(x => x.Users.Any(x => x.Id == id), null, ["Users"]);
            return Ok(new { HasFriendRequests = hasFriendRequests, groups =result.Select(x => new { x.Id, Users = x.Users.Where(x => x.Id != id).Select(u => new { u.UserName, u.FirstName, u.LastName }) }) });
        }
        
    }
}
