using ApiSTC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STC.Models;

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
        [HttpGet]
        [Route("[action]/{idComp}")]
        public async Task<ActionResult<Competicion>> GetCompeticion(int idComp)
        {
            Competicion competicion = this.repo.BaseFindCompeticion(idComp);
            return competicion;
        }
        [HttpGet]
        [Route("[action]/{idComp}/{season}")]
        public async Task<ActionResult<List<EquipoCompStats>>> GetCompeticionStandings(int idComp, int season)
        {
            List<EquipoCompStats> equiposstats = await this.repo.GetCompeticionStandings(idComp, season);
            return equiposstats;
        }
        [HttpGet]
        [Route("[action]/{idComp}/{season}/{cantidad}/{posicion}")]
        public async Task<ActionResult<ModelCompeticionPartidos>> GetUltimosPartidosComp(int idComp, int season,int cantidad, int posicion)
        {
            ModelCompeticionPartidos modelo= await this.repo.GetUltimosPartidosComp(idComp, season, cantidad,posicion);
            return modelo;
        }
        [HttpGet]
        [Route("[action]/{idComp}/{season}")]
        public async Task<ActionResult<Partido>> GetUltimoPartidoDisputado(int idComp, int season)
        {
            Partido ultimoPartido = await this.repo.GetUltimoPartidoDisputado(idComp, season);
            return ultimoPartido;
        }
        [HttpGet]
        [Route("[action]/{idComp}/{season}/{cantidad}")]
        public async Task<ActionResult<List<Partido>>> GetProximosPartidos(int idComp,int season,int cantidad)
        {
            List<Partido> ProximosPartidos = await this.repo.GetProximosPartidos(idComp, season, cantidad);
            return ProximosPartidos;
        }

    }
}
