using ApiSTC.Data;
using Microsoft.EntityFrameworkCore;
using STC.Models;

namespace ApiSTC.Repositories
{
    public class RepositoryPartidos
    {
        private STCContext context;
        public RepositoryPartidos(STCContext context)
        {
            this.context = context;

        }




        public async Task<List<Partido>> GetPartidosDiaComp(DateTime dia, int idcomp)
        {
            var consulta = from datos in this.context.Partidos
                           where datos.FechaInicio.Date == dia.Date && datos.IdCompeticion == idcomp
                           select datos;

            return await consulta.ToListAsync();
        }
        public Partido BaseFindPartido(int idLocal, int idVisitante, int idCompeticion, int idTemporada)
        {
            var consulta = from datos in this.context.Partidos
                           where datos.IdLocal == idLocal && datos.IdVisitante == idVisitante && datos.IdCompeticion == idCompeticion && datos.IdTemporada == idTemporada
                           select datos;

            return consulta.FirstOrDefault();
        }
        public Competicion BaseFindCompeticion(int idLiga)
        {

            var consulta = from datos in this.context.Competiciones
                           where datos.IdCompeticion == idLiga
                           select datos;
            return consulta.FirstOrDefault();
        }
        public async Task<Equipo> BaseFindEquipo(int id)
        {
            var consulta = from datos in this.context.Equipos
                           where datos.IdEquipo == id
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<VenueB> BaseFindVenue(int id)
        {
            var consulta = from datos in this.context.Venues
                           where datos.IdVenue == id
                           select datos;

            return await consulta.FirstOrDefaultAsync();
            //return await this.context.Venues.FirstOrDefaultAsync(x => x.IdVenue == id);


        }

        public async Task<ModelPartidoCompleto> BaseFindModeloPartidoYEventos(int idPartido)
        {
            var consulta = from datos in this.context.Partidos
                           where datos.IdPartido == idPartido
                           select datos;
            Partido partido = await consulta.FirstOrDefaultAsync();
            ModelPartidoCompleto model = new ModelPartidoCompleto();
            model.partido = partido;
            List<Evento> eventos = await GetEventsFixture(partido.IdPartido);
            model.eventos = eventos;

            ModelStatsPartido modelstats = new ModelStatsPartido();
            List<StatsPartido> statsLocal = await GetStatsEquipoPartido(partido.IdPartido, partido.IdLocal);
            List<StatsPartido> statsVisitante = await GetStatsEquipoPartido(partido.IdPartido, partido.IdVisitante);

            modelstats.statsLocal = statsLocal;
            modelstats.statsVisitante = statsVisitante;
            model.EstadisticasPartido = modelstats;


            List<ModelLineUp> listaModelsLineups = await GetLineupsPartido(partido.IdPartido, partido.IdLocal, partido.IdVisitante);

            model.LineUps = listaModelsLineups;





            return model;

        }
        public async Task<List<Evento>> GetEventsFixture(int idFixture)
        {
            var consulta = (from datos in this.context.Eventos
                            where datos.IdPartido == idFixture
                            select datos).OrderBy(x => x.Elapsed).ThenBy(x => x.Extra);
            return await consulta.ToListAsync();
        }
        public async Task<List<StatsPartido>> GetStatsEquipoPartido(int idFixture, int idEquipo)
        {
            var consulta = from datos in this.context.StatsPartidos
                           where datos.IdPartido == idFixture && datos.IdEquipo == idEquipo
                           select datos;
            return await consulta.ToListAsync();
        }


        public async Task<List<ModelLineUp>> GetLineupsPartido(int idpartido, int idlocal, int idvisitante)
        {
            List<ModelLineUp> modelo = new List<ModelLineUp>();
            ModelLineUp modelLocal = new ModelLineUp();
            ModelLineUp modelVisitante = new ModelLineUp();

            var consultaLineupB = from datos in this.context.Lineups
                                  where datos.IdPartido == idpartido && datos.IdEquipo == idlocal
                                  select datos;
            LineupB lineupLocal = await consultaLineupB.FirstOrDefaultAsync();
            var consultaJugadoresLocal = from datos in this.context.LineupPlayers
                                         where datos.IdPartido == idpartido && datos.IdEquipo == idlocal
                                         select datos;
            List<LineupPlayer> jugadoresLocal = await consultaJugadoresLocal.ToListAsync();
            List<LineupPlayer> titularesLocal = new List<LineupPlayer>();
            List<LineupPlayer> suplentesLocal = new List<LineupPlayer>();

            foreach (LineupPlayer jugador in jugadoresLocal)
            {
                if (jugador.Grid.Equals(""))
                {
                    suplentesLocal.Add(jugador);
                }
                else
                {
                    titularesLocal.Add(jugador);
                }
            }

            modelLocal.Lineup = lineupLocal;
            modelLocal.Titulares = titularesLocal;
            modelLocal.Suplentes = suplentesLocal;

            //MODELO DEL VISITANTE
            var consultaLineupBVisitante = from datos in this.context.Lineups
                                           where datos.IdPartido == idpartido && datos.IdEquipo == idvisitante
                                           select datos;
            LineupB lineupVisitante = await consultaLineupBVisitante.FirstOrDefaultAsync();

            var consultaJugadoresVisitante = from datos in this.context.LineupPlayers
                                             where datos.IdPartido == idpartido && datos.IdEquipo == idvisitante
                                             select datos;
            List<LineupPlayer> jugadoresVisitante = await consultaJugadoresVisitante.ToListAsync();
            List<LineupPlayer> titularesVisitante = new List<LineupPlayer>();
            List<LineupPlayer> suplentesVisitante = new List<LineupPlayer>();

            foreach (LineupPlayer jugador in jugadoresVisitante)
            {
                if (jugador.Grid.Equals(""))
                {
                    suplentesVisitante.Add(jugador);
                }
                else
                {
                    titularesVisitante.Add(jugador);
                }
            }
            modelVisitante.Lineup = lineupVisitante;
            modelVisitante.Titulares = titularesVisitante;
            modelVisitante.Suplentes = suplentesVisitante;
            modelo.Add(modelLocal);
            modelo.Add(modelVisitante);

            return modelo;

        }
    }
}
