using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BE.Common;
using BE.Eso;
using BE.Schedule;
using BE.Service;
using BL.Calendar;
using DAL.Calendar;
using DAL.Eso;
using DAL.Service;
using DAL.Sigesoft;
using NetPdf;
using SigesoftWebAPI.Controllers.Eso;
using static BE.Eso.RecipesCustom;

namespace BL.Eso
{
    public class EsoBl
    {
        public List<ComponentList> GetServiceComponentsForBuildMenu(string serviceId)
        {
            return new EsoDal().GetServiceComponentsForBuildMenu(serviceId);
        }

        public List<ComponentList> NewMedicalConsultation(string serviceId)
        {
            return new EsoDal().GetServiceComponentsForBuildMenu(serviceId);
        }

        public string GetServiceIdNewMedicalConsultation(string personId, int nodeId, int systemUserId)
        {
            var oScheduleCustom = new ScheduleCustom();
            oScheduleCustom.PersonId = personId;
            oScheduleCustom.TypeId = (int)Enumeratores.TypeSchedule.AgendadoIniciado;
            return  new ScheduleBl().ScheduleMedicalConsultation(oScheduleCustom, nodeId, systemUserId);
        }

        public string SaveMedicalConsultation(string serviceId, List<ServiceComponentFieldsList> oServicecomponentfields, string personId,
            string serviceComponentId, ServiceComponentDto oDataServiceComponent, int nodeId, int systemUserId)
        {
            if (oServicecomponentfields[0].Frontend != null && oServicecomponentfields[0].Frontend.ToUpper() == "FRONTEND")
                oServicecomponentfields = SaveAndGetServiceComponentId(serviceId, oServicecomponentfields, personId, nodeId, systemUserId);

            var servCompId = new EsoDal().SaveMedicalExam(oServicecomponentfields, personId, oServicecomponentfields[0].v_ServiceComponentId, nodeId,
                systemUserId);

            if (!string.IsNullOrEmpty(serviceComponentId))
                Dynamichandler(oServicecomponentfields, oServicecomponentfields[0].v_ServiceId, nodeId, systemUserId);

            oDataServiceComponent.v_ServiceComponentId = servCompId;
            var oServiceComponentDto = SaveInfoServiceComponent(oDataServiceComponent, nodeId, systemUserId);

            return servCompId + "|" + oServiceComponentDto.d_UpdateDate;
        }

        public List<ValorCampo> GetInfo(string serviceComponetId)
        {
            return new EsoDal().GetInfo(serviceComponetId);
        }

        public ServiceComponentBe GetInfoServiceComponent(string serviceComponentId)
        {
            return new EsoDal().GetInfoServiceComponent(serviceComponentId);
        }

        public List<TimeLine> TimeLineByServiceId(string serviceId)
        {
            var timeLineService = new List<TimeLine>();

            var serviceComponents = new ServiceComponentDal().ServiceComponentByServiceId(serviceId);

            #region Step1
            var culminateComponents = serviceComponents.FindAll(p => p.ServiceComponentStatusId == (int)Enumeratores.ServiceComponentStatus.Evaluado).ToList();
            if (culminateComponents.Count == 0)
            {
                // Primer Paso
                var oTimeLine = new TimeLine();
                oTimeLine.Step = Constants.SERVICE_STARTED;
                oTimeLine.Active = (int)Enumeratores.SiNo.Si;
                oTimeLine.StatusService = "Finalizar Atención";
                timeLineService.Add(oTimeLine);

                //Segundo Paso
                oTimeLine = new TimeLine();
                oTimeLine.Step = Constants.SERVICE_FINISHED;
                oTimeLine.Active = (int)Enumeratores.SiNo.No;
                timeLineService.Add(oTimeLine);

                //Tercer Paso
                oTimeLine = new TimeLine();
                oTimeLine.Step = Constants.SERVICE_PENDING_STATUS;
                oTimeLine.Active = (int)Enumeratores.SiNo.No;
                
                timeLineService.Add(oTimeLine);

                return timeLineService;
            }
            #endregion

            #region Step2
            var pendingExams = serviceComponents.FindAll(p => p.ServiceComponentStatusId != (int)Enumeratores.ServiceComponentStatus.Evaluado).ToList();
            if (pendingExams.Count > 0)
            {
                // Primer Paso
                var oTimeLine = new TimeLine();
                oTimeLine.Step = Constants.SERVICE_STARTED;
                oTimeLine.Active = (int)Enumeratores.SiNo.No;
                timeLineService.Add(oTimeLine);

                //Segundo Paso
                oTimeLine = new TimeLine();
                oTimeLine.Step = Constants.SERVICE_FINISHED;
                oTimeLine.Active = (int)Enumeratores.SiNo.Si;
                oTimeLine.StatusService = "Finalizar Atención";
                timeLineService.Add(oTimeLine);

                //Tercer Paso
                oTimeLine = new TimeLine();
                oTimeLine.Step = Constants.SERVICE_PENDING_STATUS;
                oTimeLine.Active = (int)Enumeratores.SiNo.No;
               
                timeLineService.Add(oTimeLine);

                return timeLineService;
            }
            #endregion

            #region Step3

            if (pendingExams.Count == 0)
            {
                var serviceCulmined = new ServiceDal().GetServiceStatus(serviceId);
                if (!(serviceCulmined == Enumeratores.ServiceStatus.Culminado))
                {
                    // Primer Paso
                    var oTimeLine = new TimeLine();
                    oTimeLine.Step = Constants.SERVICE_STARTED;
                    oTimeLine.Active = (int)Enumeratores.SiNo.No;
                    timeLineService.Add(oTimeLine);

                    //Segundo Paso
                    oTimeLine = new TimeLine();
                    oTimeLine.Step = Constants.SERVICE_FINISHED;
                    oTimeLine.Active = (int)Enumeratores.SiNo.No;
                    timeLineService.Add(oTimeLine);

                    //Tercer Paso
                    oTimeLine = new TimeLine();
                    oTimeLine.Step = Constants.SERVICE_PENDING_STATUS;
                    oTimeLine.Active = (int)Enumeratores.SiNo.Si;
                    oTimeLine.StatusService = serviceCulmined.ToString();
                    //oTimeLine.StatusService = serviceCulmined == Enumeratores.ServiceStatus.Iniciado ? "Click para Culminar Atx" : serviceCulmined.ToString();
                    timeLineService.Add(oTimeLine);

                    return timeLineService;

                }
                else
                {
                    // Primer Paso
                    var oTimeLine = new TimeLine();
                    oTimeLine.Step = Constants.SERVICE_STARTED;
                    oTimeLine.Active = (int)Enumeratores.SiNo.No;
                    timeLineService.Add(oTimeLine);

                    //Segundo Paso
                    oTimeLine = new TimeLine();
                    oTimeLine.Step = Constants.SERVICE_FINISHED;
                    oTimeLine.Active = (int)Enumeratores.SiNo.No;
                    timeLineService.Add(oTimeLine);

                    //Tercer Paso
                    oTimeLine = new TimeLine();
                    oTimeLine.Step = Constants.SERVICE_PENDING_STATUS;
                    oTimeLine.Active = (int)Enumeratores.SiNo.No;
                    oTimeLine.StatusService = "Atención Culminada";
                    timeLineService.Add(oTimeLine);

                    return timeLineService;

                }

            }
            #endregion

            return null;

        }

        public bool SaveServiceStatus(string serviceId, int statusServiceId,int systemUserId)
        {
            return  new EsoDal().SaveServiceStatus(serviceId, statusServiceId, systemUserId);
        }

        #region private

        private ServiceComponentDto SaveInfoServiceComponent(ServiceComponentDto oDataServiceComponent, int nodeId, int systemUserId)
        {
             return new ServiceComponentDal().UpdateServiceComponentId(oDataServiceComponent, nodeId, systemUserId);
        }

        private void Dynamichandler(List<ServiceComponentFieldsList> oServicecomponentfields, string oServiceId, int nodeId, int systemUserId)
        {
            var serCompBD = new ServiceComponentDal().GetServiceComponentDtos(oServiceId);

            var serCompFront = oServicecomponentfields.GroupBy(g => g.v_ComponentId).Select(s => s.FirstOrDefault());

            foreach (var item in serCompFront)
            {
                var result = serCompBD.Find(p => p.v_ComponentId == item.v_ComponentId);
                if (result == null)
                {
                    if (item.i_IsDeleted == (int)Enumeratores.SiNo.No)
                    {
                        new SchedulePersonDal().AddServiceComponentTemp(oServiceId, item.v_ComponentId, nodeId, systemUserId);
                    }
                    else if (item.i_IsDeleted == (int)Enumeratores.SiNo.Si)
                    {
                        //Eliminar componente
                    }
                }
            }

        }

        private List<ServiceComponentFieldsList> SaveAndGetServiceComponentId(string serviceId, List<ServiceComponentFieldsList> oServicecomponentfields, string personId,
            int nodeId, int systemUserId)
        {
            var servicecomponenteId = new SchedulePersonDal().SaveAndGetServiceComponentId(serviceId, personId, oServicecomponentfields, nodeId,
                systemUserId);

            foreach (var item in oServicecomponentfields)
            {
                item.v_ServiceComponentId = servicecomponenteId;
            }

            return oServicecomponentfields;
        }

        #endregion


        string _ruta = HttpContext.Current.Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["directorioESO"]);
        private List<string> _filesNameToMerge = new List<string>();
        private MergeExPDF _mergeExPDF = new MergeExPDF();
        public string BuildRecipe(BoardPrintRecipes data)
        {
            //System.Diagnostics.Process proc = new System.Diagnostics.Process();
            var MedicalCenter = new SigesoftDal().GetInfoMedicalCenter();
            var pathFile = string.Format("{0}.pdf", Path.Combine(_ruta, data.ServiceId + "-" + data.PersonId + "-receta"));

            RecipesMedical.CreateRecipe(data, MedicalCenter, pathFile);

            _filesNameToMerge.Add($"{Path.Combine(_ruta, data.ServiceId + "-" + data.PersonId + "-receta")}.pdf");

            var reportsPdf = _filesNameToMerge.ToList();

            _mergeExPDF.FilesName = reportsPdf;
            _mergeExPDF.DestinationFile = _ruta + "/" + data.ServiceId + "-receta.pdf";
            _mergeExPDF.Execute();
            //_mergeExPDF.RunFile();

            return data.ServiceId + "-receta";
        }


    }
}
