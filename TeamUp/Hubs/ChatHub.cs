using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamUp.Areas.Identity.Data;
using TeamUp.Data;

namespace TeamUp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IServiceProvider _serviceProvider;

        public ChatHub(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SendMessage(string username, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", username, message);
        }

        public async Task JoinGroup(int id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
        }

        public async Task SendGroupMessage(int groupId, string userId, string message)
        {
            var sendDate = DateTime.Now;
            var userName = "";
            byte[] profilePicture = null;

            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                if(message != "")
                {
                    var newMessage = new TeamUpTeamChat
                    {
                        SendDate = DateTime.Now,
                        UserId = userId,
                        TeamId = groupId,
                        Message = message
                    };
                    newMessage.SendDate = newMessage.SendDate.AddTicks(-(newMessage.SendDate.Ticks % TimeSpan.TicksPerSecond));
                    sendDate = newMessage.SendDate;
                    userName = context.Users.Where(x => x.Id == userId).FirstOrDefault().FirstName;
                    profilePicture = context.Users.Where(x => x.Id == userId).FirstOrDefault().ProfilePicture;

                    context.TeamChat.Add(newMessage);
                    await context.SaveChangesAsync();
                }
            }
            await Clients.Group(groupId.ToString()).SendAsync("GroupMessage", userId, userName, message, sendDate, profilePicture);
        }
    }
}
