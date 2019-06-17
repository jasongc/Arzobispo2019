using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using BE.Common;
using BE.Eso;
using BE.Plan;
using BE.Service;
using BE.Warehouse;
using BL.Eso;
using BL.ProductWarehouse;
using Newtonsoft.Json;
using static BE.Eso.RecipesCustom;

namespace SigesoftWebAPI.Controllers.Eso
{
    public class EsoController : ApiController
    {
        private readonly EsoBl _oEsoBl = new EsoBl();
        private readonly InputOutputBl _InputOutputBl = new InputOutputBl();
        [HttpGet]
        public IHttpActionResult GetServiceComponentsForBuildMenu(string serviceId)
        {
            var result = _oEsoBl.GetServiceComponentsForBuildMenu(serviceId);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetServiceIdNewMedicalConsultation(string personId, int nodeId, int systemUserId)
        {
            var result = _oEsoBl.GetServiceIdNewMedicalConsultation(personId, nodeId, systemUserId);
            return Ok(result);
        }

        //[HttpGet]
        //public IHttpActionResult NewMedicalConsultation(string personId, int nodeId, int systemUserId)
        //{
        //    var result = _oEsoBl.NewMedicalConsultation(personId, nodeId, systemUserId);
        //    return Ok(result);
        //}

        [HttpGet]
        public IHttpActionResult GetDataNewMedicalConsultation(string serviceId)
        {
            var result = _oEsoBl.NewMedicalConsultation(serviceId);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult SaveMedicalConsultation(MultiDataModel model)
        {
            //if (model.Int3 != (int)Enumeratores.SaveEso.Allowed)
            //    return BadRequest("Acción no Permitida");

            if (string.IsNullOrWhiteSpace(model.String1))
                return BadRequest("Información Inválida");

            var data = JsonConvert.DeserializeObject<List<ServiceComponentFieldsList>>(model.String1);
            var dataServiceComponent = JsonConvert.DeserializeObject<ServiceComponentDto>(model.String2);
            
            //var scope = new TransactionScope(
            //    TransactionScopeOption.RequiresNew,
            //    new TransactionOptions()
            //    {
            //        IsolationLevel = IsolationLevel.RepeatableRead
            //    });

            //using (scope)
            //{
                var result = _oEsoBl.SaveMedicalConsultation(data[0].v_ServiceId, data, model.String2, data[0].v_ServiceComponentId, dataServiceComponent, model.Int1, model.Int2);

                //scope.Complete();
                return Ok(result);
            //}
        }

        [HttpGet]
        public IHttpActionResult GetInfo(string serviceComponentId)
        {
            var result = _oEsoBl.GetInfo(serviceComponentId);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetInfoServiceComponent(string serviceComponentId)
        {
            var result = _oEsoBl.GetInfoServiceComponent(serviceComponentId);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult TimeLineByServiceId(string serviceId)
        {
            var result = _oEsoBl.TimeLineByServiceId(serviceId);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult SaveServiceStatus(MultiDataModel model)
        {
            if (string.IsNullOrWhiteSpace(model.String1))
                return BadRequest("Información Inválida");

            var data = JsonConvert.DeserializeObject<ServiceStatusBE>(model.String1);

            var result = _oEsoBl.SaveServiceStatus(data.ServiceId,data.StatusServiceId, model.Int1);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GeneratePrintRecipes(MultiDataModel model)
        {

            var data = JsonConvert.DeserializeObject<BoardPrintRecipes>(model.String1);
            data.NodeId = model.Int1;
            data.InsertUserId = model.Int2;

            _InputOutputBl.GenerateMovementOutput(data);

            var filename = _oEsoBl.BuildRecipe(data);

            return Ok(filename);
        }
    }
}
