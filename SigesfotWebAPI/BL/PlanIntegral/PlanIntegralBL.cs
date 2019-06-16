using BE.PlanIntegral;
using DAL.PlanIntegral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.PlanIntegral
{
    public class PlanIntegralBL
    {
        public List<PlanIntegralList> GetPlanIntegralAndFiltered(string pstrPersonId)
        {
            return new PlanIntegralDal().GetPlanIntegralAndFiltered(pstrPersonId);
        }

        public List<ProblemaList> GetProblemaPagedAndFiltered(string pstrPersonId)
        {
            return new PlanIntegralDal().GetProblemaPagedAndFiltered(pstrPersonId);
        }
    }
}
