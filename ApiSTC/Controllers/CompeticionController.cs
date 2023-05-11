using ApiSTC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiSTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompeticionController : ControllerBase
    {
        private RepositoryCompeticion repo;
        public CompeticionController(RepositoryCompeticion repo)
        {
            this.repo = repo;
        }

    }
}
