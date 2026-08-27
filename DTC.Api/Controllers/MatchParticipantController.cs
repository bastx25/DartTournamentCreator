using DTC.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/matchparticipants")]
    [ApiController]

    public class MatchParticipantController : ControllerBase
    {
        private readonly IMatchParticipantRepository _matchPartRepo;

        public MatchParticipantController(IMatchParticipantRepository matchParticipantRepository)
        {
            _matchPartRepo = matchParticipantRepository;
        }
    }
}
