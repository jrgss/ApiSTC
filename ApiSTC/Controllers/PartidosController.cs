using ApiSTC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STC.Models;

namespace ApiSTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidosController : ControllerBase
    {
        private RepositoryPartidos repo;
        public PartidosController(RepositoryPartidos repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [Route("[action]/{idLocal}/{idVisitante}/{idCompeticion}/{idTemporada}")]

        public async Task<ActionResult<Partido>> BaseFindPartido(int idLocal, int idVisitante, int idCompeticion, int idTemporada)
        {
            Partido partido=this.repo.BaseFindPartido(idLocal,idVisitante,idCompeticion,idTemporada);
            return partido;
            
        }
        [HttpGet]
        [Route("[action]/{idComp}/{dia}")]
        public async Task<ActionResult<List<Partido>>> GetPartidosDiaComp(int idComp,string fecha)
        {
            DateTime dia = DateTime.Parse(fecha);
            List<Partido> partidos = await this.repo.GetPartidosDiaComp(dia, idComp);
        return partidos;
        }
        [HttpGet]
        [Route("[action]/{idComp}")]
        public async Task<ActionResult<ModelPartidoCompleto>> BaseFindModeloPartidoYEventos(int idComp)
        {
            ModelPartidoCompleto model = await this.repo.BaseFindModeloPartidoYEventos(idComp);
            return model;
        }
        [HttpGet]
        [Route("[action]/{idVenue}")]
        public async Task<ActionResult<VenueB>> BaseFindVenue(int idVenue)
        {
            VenueB venue = await this.repo.BaseFindVenue(idVenue);
            return venue;
        }
    }
}
