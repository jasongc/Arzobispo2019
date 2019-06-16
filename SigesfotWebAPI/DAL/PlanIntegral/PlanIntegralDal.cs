using BE.PlanIntegral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.PlanIntegral
{
    public class PlanIntegralDal
    {
        public List<PlanIntegralList> GetPlanIntegralAndFiltered(string pstrPersonId)
        {
            //mon.IsActive = true;

            try
            {
                DatabaseContext dbContext = new DatabaseContext();
                var query = from A in dbContext.PlanIntegral
                            join B in dbContext.SystemParameter on new { a = A.i_TipoId.Value, b = 281 }  // CATEGORIA DEL EXAMEN
                                                      equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                            from B in B_join.DefaultIfEmpty()
                            where A.i_IsDeleted == 0 && A.v_PersonId == pstrPersonId

                            select new PlanIntegralList
                            {
                                v_PlanIntegral = A.v_PlanIntegral,
                                v_PersonId = A.v_PersonId,
                                i_TipoId = A.i_TipoId.Value,
                                v_Descripcion = A.v_Descripcion,
                                d_Fecha = A.d_Fecha.Value,
                                v_Lugar = A.v_Lugar,
                                v_Tipo = B.v_Value1
                            };
                List<PlanIntegralList> objData = query.ToList();
                return objData;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ProblemaList> GetProblemaPagedAndFiltered(string pstrPersonId)
        {
            //mon.IsActive = true;

            try
            {
                DatabaseContext dbContext = new DatabaseContext();
                var query = from A in dbContext.Problema
                            join B in dbContext.SystemParameter on new { a = A.i_EsControlado.Value, b = 111 }  // CATEGORIA DEL EXAMEN
                                                        equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                            from B in B_join.DefaultIfEmpty()
                            where A.i_IsDeleted == 0 && A.v_PersonId == pstrPersonId

                            select new ProblemaList
                            {
                                v_ProblemaId = A.v_ProblemaId,
                                i_Tipo = A.i_Tipo,
                                v_PersonId = A.v_PersonId,
                                d_Fecha = A.d_Fecha.Value,
                                v_Descripcion = A.v_Descripcion,
                                i_EsControlado = A.i_EsControlado,
                                v_Observacion = A.v_Observacion,
                                v_EsControlado = B.v_Value1
                            };

                List<ProblemaList> objData = query.ToList();

                return objData;

            }
            catch (Exception ex)
            {

                return null;
            }
        }


    }
}
