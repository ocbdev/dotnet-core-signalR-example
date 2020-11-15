using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.API.Models;

namespace SignalR.API.Hubs
{
    public class myHub : Hub
    {
        public myHub(AddDbContext context)
        {
            _context = context;
        }

        public static List<string> Names { get; set; } = new List<string>();
        private static int ClientCount { get; set; } = 0;
        private readonly AddDbContext _context;
        public static int teamCount { get; set; } = 7;




        public async Task SendName(string name)
        {

            if (Names.Count >= teamCount)
                await Clients.Caller.SendAsync("Error", "Takımdaki oyuncu sayısı maximuma ulaştı.");
            else
            {
                Names.Add(name);
                await Clients.All.SendAsync("ReceiveName", name);
            }

        }

        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }

        public async override Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetNamesByGroup()
        {
            var teams = _context.Teams.Include(x => x.Users).Select(x => new
            {
                teamId = x.Id,
                Users = x.Users.ToList()
            });

            await Clients.All.SendAsync("ReceiveNamesByGroup",teams); 
        }   

        public async Task AddToGroup(string teamName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }

        public async Task SendNameByGroup(string Name, string teamName)
        {
            var team = _context.Teams.Where(x => x.name == teamName).FirstOrDefault();
            if (team != null)
                team.Users.Add(new User { name = Name });
            else
            {
                var newTeam = new Team { name = teamName };
                newTeam.Users.Add(new User { name = Name });
                _context.Teams.Add(newTeam);

            }

            await _context.SaveChangesAsync();

            await Clients.Group(teamName).SendAsync("ReceiveMessageByGroup", Name, team.Id);
    
        }

        public async Task RemoveToGroup(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }
    }
}
