using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.API.Hubs;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SignalR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {

        private readonly IHubContext<myHub> _hubContext; // I Hub context my hubi kalıtıyor. buradan istediğim gibi erişebiliyorum.

        public NotificationController(IHubContext<myHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("{teamCount}")]
        public async Task<IActionResult> SetTeamCount(int teamCount)
        {
            myHub.teamCount = teamCount;

            await _hubContext.Clients.All.SendAsync("Notify", $"Arkadaşlar takım  {teamCount} kişi olacaktır.");

            return Ok();
        }


    }
}
