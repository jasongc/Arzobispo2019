using BL.PlanIntegral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SigesoftWebAPI.Controllers
{
    public class PlanIntegralController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetPlanIntegralAndFiltered(string personId)
        {
            var result = new PlanIntegralBL().GetPlanIntegralAndFiltered(personId);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetProblemaPagedAndFiltered(string personId)
        {
            var result = new PlanIntegralBL().GetProblemaPagedAndFiltered(personId);
            return Ok(result);
        }
    }
}
