using ApiSTC.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using STC.Models;
using System.Data;

namespace ApiSTC.Repositories
{
    public class RepositoryCompeticion
    {
        private STCContext context;
        public RepositoryCompeticion(STCContext context)
        {
            this.context = context;
        }

        public List<Competicion> GetCompeticionesPais(int IdPais)
        {
            string sql = "SP_PROCEDURE_COMPETICION_PAIS @IDPAIS";
            SqlParameter pamid = new SqlParameter("@IDPAIS", IdPais);

            var consulta = this.context.Competiciones.FromSqlRaw(sql, pamid);
            List<Competicion> competiciones = consulta.AsEnumerable().ToList();
            return competiciones;
        }

        public List<VistaEquiposCompeticion> GetEquiposComp(int idComptemp)
        {
            throw new NotImplementedException();
        }

        public List<Partido> GetUltimosPartidosComp(int idComp, int cantidad)
        {
            string sql = "SP_ULTIMOS_PARTIDOS @IDCOMPETICION,@CANTIDAD";
            SqlParameter pamidcomp = new SqlParameter("@IDCOMPETICION", idComp);
            SqlParameter pamcantidad = new SqlParameter("@CANTIDAD", cantidad);
            var consulta = this.context.Partidos.FromSqlRaw(sql, pamidcomp, pamcantidad);
            List<Partido> partidos = consulta.AsEnumerable().ToList();
            return partidos;
        }

        public async Task<List<Competicion>> GetCompeticiones()
        {
            string sql = "SP_COMPETICIONES";
            var consulta = this.context.Competiciones.FromSqlRaw(sql);
            List<Competicion> compes = consulta.AsEnumerable().ToList();
            return compes;
        }

        public async Task<List<EquipoCompStats>> GetCompeticionStandings(int idComp, int season)
        {
            var consulta = (from datos in this.context.EquiposCompStats
                            where datos.IdCompeticion == idComp && datos.IdTemporada == season
                            select datos).OrderBy(x => x.Posicion);

            return await consulta.ToListAsync();
        }
        public Competicion BaseFindCompeticion(int idLiga)
        {

            var consulta = from datos in this.context.Competiciones
                           where datos.IdCompeticion == idLiga
                           select datos;
            return consulta.FirstOrDefault();
        }

        public async Task<ModelCompeticionPartidos> GetUltimosPartidosComp(int idcomp, int season, int cantidad, int posicion)
        {
            string sql = "SP_PROCEDURE_PARTIDOS_COMP @IDCOMPETICION ,@TEMPORADA, @POSICION , @CANTIDAD , @NUMREGISTROS OUT";
            SqlParameter pamidcomp = new SqlParameter("@IDCOMPETICION", idcomp);
            SqlParameter pamseason = new SqlParameter("@TEMPORADA", season);
            SqlParameter pamposicion = new SqlParameter("@POSICION", posicion);
            SqlParameter pamcantidad = new SqlParameter("@CANTIDAD", cantidad);
            SqlParameter pamregistros = new SqlParameter("@NUMREGISTROS", -1);
            pamregistros.Direction = ParameterDirection.Output;
            var consulta = this.context.Partidos.FromSqlRaw(sql, pamidcomp, pamseason, pamposicion, pamcantidad, pamregistros);
            List<Partido> partidos = await consulta.ToListAsync();


            int registros = (int)pamregistros.Value;

            ModelCompeticionPartidos modelo = new ModelCompeticionPartidos();
            modelo.partidos = partidos;
            Competicion compe = BaseFindCompeticion(idcomp);
            modelo.competicion = compe;
            modelo.registros = registros;
            return modelo;
        }
        public async Task<Partido> GetUltimoPartidoDisputado(int idcomp, int season)
        {
            var consulta = (from datos in this.context.Partidos
                            where datos.IdCompeticion == idcomp && datos.IdTemporada == season && datos.Estado == "Match Finished"
                            select datos).OrderByDescending(x => x.FechaInicio);
            Partido partido = await consulta.FirstOrDefaultAsync();
            return partido;
        }
        public async Task<List<Partido>> GetProximosPartidos(int idcomp, int season, int cantidad)
        {
            var consulta = (from datos in this.context.Partidos
                            where datos.IdCompeticion == idcomp && datos.IdTemporada == season && datos.Estado != "Match Finished" && datos.FechaInicio.Date > new DateTime()
                            select datos).OrderByDescending(x => x.FechaInicio);

            List<Partido> proximosPartidos = await consulta.ToListAsync();
            return proximosPartidos;
        }
        public async Task<ResumenPartidosCompeticion> GetResumenCompeticion(int idComp, int season)
        {
            ResumenPartidosCompeticion resumen = new ResumenPartidosCompeticion();
            var consulta = (from datos in this.context.Partidos
                            where datos.IdCompeticion == idComp && datos.IdTemporada == season && datos.EstadoFinal == "FT"
                            select datos).OrderByDescending(x => x.FechaInicio);

            Partido partido = await consulta.FirstOrDefaultAsync();
            resumen.UltimoPartido = partido;

            //var consulta= from datos in this.context.Partidos
            List<Partido> UltimosPartidos;



            return resumen;

        }
    }
}
