using BL.Antecedentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SigesoftWebAPI.Controllers
{
    public class AntecedentesController : ApiController
    {
        [HttpGet]
        public IHttpActionResult ObtenerEsoAntecedentesPorGrupoId(string PersonId)
        {

            var result = new EsoAntecedentesBL().ObtenerEsoAntecedentesPorGrupoId(PersonId);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult ObtenerFechasCuidadosPreventivos(string PersonId)
        {

            var result = new EsoAntecedentesBL().ObtenerFechasCuidadosPreventivos(PersonId);
            return Ok(result);
        }
    }
}
