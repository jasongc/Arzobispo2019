using BE.Antecedentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BE.Common.Enumeratores;

namespace DAL.Antecedentes
{
    public class EsoAntecedentesDal
    {

        public List<EsoAntecedentesPadre> ObtenerEsoAntecedentesPorGrupoId(int GrupoId, int GrupoEtario, string PersonaId)
        {
            try
            {
                DatabaseContext dbContext = new DatabaseContext();
                int isNotDeleted = (int)SiNo.No;

                var data = (from a in dbContext.SystemParameter
                            where a.i_IsDeleted == isNotDeleted &&
                            a.i_GroupId == GrupoId
                            select new EsoAntecedentesPadre
                            {
                                GrupoId = a.i_GroupId,
                                ParametroId = a.i_ParameterId,
                                Nombre = a.v_Value1
                            }).ToList();


                foreach (var P in data)
                {
                    int grupoHijo = int.Parse(P.GrupoId.ToString() + P.ParametroId.ToString());
                    P.Hijos = (from a in dbContext.SystemParameter
                               join b in dbContext.AntecedentesAsistencial on new { a = a.i_ParameterId, b = GrupoEtario, c = PersonaId, d = P.ParametroId } equals new { a = b.i_ParametroId, b = b.i_GrupoEtario, c = b.v_personId, d = b.i_GrupoData } into temp
                               from b in temp.DefaultIfEmpty()
                               where a.i_IsDeleted == isNotDeleted &&
                               a.i_GroupId == grupoHijo
                               select new EsoAntecedentesHijo
                               {
                                   Nombre = a.v_Value1,
                                   GrupoId = a.i_GroupId,
                                   ParametroId = a.i_ParameterId,
                                   SI = b == null ? false : b.i_Valor.HasValue ? b.i_Valor.Value == (int)SiNo.Si : false,
                                   NO = b == null ? false : b.i_Valor.HasValue ? b.i_Valor.Value == (int)SiNo.No : false
                               }).ToList();
                }

                return data;
            }
            catch (Exception e)
            {
                return new List<EsoAntecedentesPadre>();
            }
        }

        public List<EsoCuidadosPreventivosFechas> ObtenerFechasCuidadosPreventivos(string PersonId)
        {
            try
            {
                DatabaseContext dbContext = new DatabaseContext();
                int isNotDeleted = (int)SiNo.No;

                var data = (from a in dbContext.Service
                            where a.i_IsDeleted == isNotDeleted &&
                            a.v_PersonId == PersonId &&
                            a.d_ServiceDate != null
                            select new EsoCuidadosPreventivosFechas()
                            {
                                FechaServicio = a.d_ServiceDate.Value
                            }).ToList();

                return data;
            }
            catch (Exception e)
            {
                return new List<EsoCuidadosPreventivosFechas>();
            }
        }

        public List<EsoCuidadosPreventivos> ObtenerListadoCuidadosPreventivos(int GrupoPadre, string PersonId, DateTime FechaServicio)
        {
            try
            {
                DatabaseContext dbContext = new DatabaseContext();
                int isNotDeleted = (int)SiNo.No;

                var data = (from a in dbContext.SystemParameter
                            join b in dbContext.CuidadoPreventivo on new { a = PersonId, b = FechaServicio, c = a.i_GroupId, d = a.i_ParameterId, e = (int)isNotDeleted } equals new { a = b.v_PersonId, b = b.d_ServiceDate.Value, c = b.i_GrupoId, d = b.i_ParametroId, e = b.i_IsDeleted.Value } into temp
                            from b in temp.DefaultIfEmpty()
                            where a.i_IsDeleted == isNotDeleted &&
                            a.i_GroupId == GrupoPadre
                            select new EsoCuidadosPreventivos
                            {
                                ParameterId = a.i_ParameterId,
                                Nombre = a.v_Value1,
                                GrupoId = a.i_GroupId,
                                Valor = b == null ? false : b.i_Valor == (int)SiNo.Si ? true : false
                            }).ToList();

                if (data.Count == 0)
                    return null;

                foreach (var D in data)
                {
                    int nuevoGrupo = int.Parse(GrupoPadre.ToString() + D.ParameterId.ToString());
                    D.Hijos = ObtenerListadoCuidadosPreventivos(nuevoGrupo, PersonId, FechaServicio);
                }


                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<EsoCuidadosPreventivosComentarios> ObtenerComentariosCuidadosPreventivos(string PersonaId)
        {
            try
            {
                DatabaseContext dbContext = new DatabaseContext();
                int isNotDeleted = (int)SiNo.No;

                var data = (from a in dbContext.CuidadoPreventivoComentario
                            where a.v_PersonId == PersonaId &&
                            a.i_IsDeleted == isNotDeleted
                            select new EsoCuidadosPreventivosComentarios()
                            {
                                GrupoId = a.i_GrupoId,
                                ParametroId = a.i_ParametroId,
                                Comentario = a.v_Comentario
                            }).ToList();

                return data;
            }
            catch (Exception e)
            {
                return new List<EsoCuidadosPreventivosComentarios>();
            }
        }

    }

}
