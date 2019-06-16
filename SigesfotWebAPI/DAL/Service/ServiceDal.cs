using BE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using BE.Service;
using System.Transactions;
using DAL.Hospitalizacion;
using BE.Message;
using static BE.Common.Enumeratores;
using BE.Pacient;
using BE.RoleNodeComponentProfile;
using BE.Sigesoft;
using BE.Piso;
using BE.Categoria;
using BE.Component;

namespace DAL.Service
{
    public class ServiceDal
    {
        private static DatabaseContext ctx = new DatabaseContext();

        public string AddService(ServiceDto oServiceDto, int nodeId, int systemUserId)
        {
            var serviceId = new Common.Utils().GetPrimaryKey(nodeId, 23, "SR");
            
            oServiceDto.v_ServiceId = serviceId;
            oServiceDto.i_IsDeleted = (int) Enumeratores.SiNo.No;
            oServiceDto.d_InsertDate = DateTime.UtcNow;
            oServiceDto.i_InsertUserId = systemUserId;

            ctx.Service.Add(oServiceDto);
            ctx.SaveChanges();
            
            return oServiceDto.v_ServiceId;
        }

        public string DarDeBaja(string personId)
        {
            var services = (from a in ctx.Service where a.v_PersonId == personId && a.i_IsDeleted == 0 select a).ToList();
            
            foreach (var service in services)
            {
                service.i_StatusVigilanciaId = 2;
            }

            ctx.SaveChanges();
            return personId;
        }

        public Enumeratores.ServiceStatus GetServiceStatus(string serviceId)
        {
            using (var contex = new DatabaseContext())
            {
                var serviceStatusId = (from a in contex.Service where a.v_ServiceId == serviceId select a).FirstOrDefault().i_ServiceStatusId;

                if (serviceStatusId == (int)Enumeratores.ServiceStatus.Culminado)
                {
                    return Enumeratores.ServiceStatus.Culminado;
                }
                else if (serviceStatusId == (int)Enumeratores.ServiceStatus.Incompleto)
                {
                    return Enumeratores.ServiceStatus.Incompleto;
                }
                else if (serviceStatusId == (int)Enumeratores.ServiceStatus.Iniciado || serviceStatusId == (int)Enumeratores.ServiceStatus.PorIniciar)
                {
                    return Enumeratores.ServiceStatus.Iniciado;
                }
                else 
                {
                    return Enumeratores.ServiceStatus.Cancelado;
                }
            }
        }

        public string CreateService(ServiceCustom data, int nodeId, int userId)
        {
            try
            {
                var newId = new Common.Utils().GetPrimaryKey(nodeId, 23, "SR");
                ServiceDto oServiceDto = new ServiceDto();
                oServiceDto.v_ServiceId = newId;
                oServiceDto.v_ProtocolId = data.ProtocolId;
                oServiceDto.v_PersonId = data.PersonId;
                oServiceDto.i_MasterServiceId = data.MasterServiceId;
                oServiceDto.i_ServiceStatusId = data.ServiceStatusId;
                oServiceDto.i_AptitudeStatusId = data.AptitudeStatusId;
                oServiceDto.d_ServiceDate = DateTime.Now;
                oServiceDto.d_GlobalExpirationDate = null;
                oServiceDto.d_ObsExpirationDate = null;
                oServiceDto.v_OrganizationId = data.OrganizationId;
                oServiceDto.i_FlagAgentId = data.FlagAgentId;
                oServiceDto.v_Motive = data.Motive;
                oServiceDto.i_IsFac = 1;
                oServiceDto.i_StatusLiquidation = 1;
                oServiceDto.i_IsFacMedico = 0;
                oServiceDto.i_IsDeleted = 0;
                oServiceDto.v_centrocosto = data.CentroCosto;
                oServiceDto.i_MedicoPagado = 0;
                oServiceDto.i_InsertUserId = userId;
                oServiceDto.d_InsertDate = DateTime.Now;

                ctx.Service.Add(oServiceDto);
                ctx.SaveChanges();
                return newId;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ServiceDto GetServiceByServiceId(string ServiceId)
        {
            return ctx.Service.Where(x => x.v_ServiceId == ServiceId).FirstOrDefault();
        }

        public bool UpdateServiceForProtocolo(ServiceCustom data, int userId)
        {
            try
            {
                var objService = ctx.Service.Where(x => x.v_ServiceId == data.ServiceId).FirstOrDefault();
                objService.v_ProtocolId = data.ProtocolId;
                objService.v_centrocosto = data.CentroCosto;
                objService.i_MasterServiceId = data.MasterServiceId;

                objService.d_UpdateDate = DateTime.Now;
                objService.i_UpdateUserId = userId;

                return ctx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RegistrarCarta(MultiDataModel data)
        {
            try
            {
                var objservice = ctx.Service.Where(x => x.v_ServiceId == data.String1).FirstOrDefault();
                objservice.v_NroCartaSolicitud = data.String2;
                objservice.d_UpdateDate = DateTime.Now;
                objservice.i_UpdateUserId = data.Int1;

                return ctx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public MessageCustom FusionarServicios(List<string> ServicesId, int userId, int nodeId)
        {
            MessageCustom _MessageCustom = new MessageCustom();
            try
            {
                List<HospitalizacionServiceBE> ListHospServices = new List<HospitalizacionServiceBE>();
                List<HospitalizacionServiceBE> ListHospServicesDistintos = new List<HospitalizacionServiceBE>();
                List<string> ServicesNoEncontrados = new List<string>();
                using (var ts = new TransactionScope())
                {
                    #region FindListHospServices
                    foreach (var serviceId in ServicesId)
                    {
                        var query = (from hser in ctx.HospitalizacionService
                                     where hser.v_ServiceId == serviceId && hser.i_IsDeleted == 0
                                     select hser).FirstOrDefault();

                        if (query != null)
                        {
                            ListHospServices.Add(query);
                        }
                        else
                        {
                            ServicesNoEncontrados.Add(serviceId);
                        }

                    }
                    #endregion

                    string HospitalizacionId = "";
                    var objHospitlizacion = ListHospServices.FindAll(x => x.v_HopitalizacionId != null).FirstOrDefault();
                    if (objHospitlizacion != null)
                    {
                        HospitalizacionId = objHospitlizacion.v_HopitalizacionId;
                    }
                    if (HospitalizacionId != "" && HospitalizacionId != null)
                    {
                        //Actualizo la HospitalizacionService con los mismos HospitalizacionId
                        foreach (var HospService in ListHospServices)
                        {
                            if (HospitalizacionId != HospService.v_HopitalizacionId)
                            {
                                var query = (from hser in ctx.HospitalizacionService
                                             where hser.v_HospitalizacionServiceId == HospService.v_HospitalizacionServiceId
                                             select hser).FirstOrDefault();
                                query.v_HopitalizacionId = HospitalizacionId;
                                query.d_UpdateDate = DateTime.Now;
                                query.i_UpdateUserId = userId;
                                ctx.SaveChanges();
                            }

                        }
                    }
                    else
                    {
                        if (ListHospServices.Count > 0)
                        {
                            HospitalizacionId = new HospitalizacionDal().AddHospitalizacion(ListHospServices[0].v_ServiceId, nodeId, userId);
                            foreach (var HospService in ListHospServices)
                            {
                                var query = (from hser in ctx.HospitalizacionService
                                             where hser.v_HospitalizacionServiceId == HospService.v_HospitalizacionServiceId
                                             select hser).FirstOrDefault();
                                query.v_HopitalizacionId = HospitalizacionId;
                                query.d_UpdateDate = DateTime.Now;
                                query.i_UpdateUserId = userId;
                                ctx.SaveChanges();
                            }
                        }

                    }

                    if (ServicesNoEncontrados.Count > 0)
                    {
                        //Agrego una nueva HospitalizacionService
                        if (HospitalizacionId != "" && HospitalizacionId != null)
                        {
                            foreach (var _serviceId in ServicesNoEncontrados)
                            {
                                bool result = new HospitalizacionDal().AddHospitalizacionService(HospitalizacionId, _serviceId, nodeId, userId);
                                if (!result) throw new Exception("Sucedió un error al generar las nuevas hospitalizaciones services");
                            }
                        }
                        else //Agrego una nueva Hospitalizacion
                        {
                            string _HospitalizacionId = new HospitalizacionDal().AddHospitalizacion(ServicesNoEncontrados[0], nodeId, userId);
                            foreach (var serviceId in ServicesNoEncontrados)
                            {
                                if (_HospitalizacionId != null)
                                {
                                    //Agrego la hospitalizacionService
                                    bool result = new HospitalizacionDal().AddHospitalizacionService(_HospitalizacionId, serviceId, nodeId, userId);
                                    if (!result) throw new Exception("Sucedió un error al generar las nuevas hospitalizaciones services");
                                }
                                else
                                {
                                    throw new Exception("Sucedió un error al generar las nuevas hospitalizaciones");
                                }
                            }
                        }
                    }
                    ts.Complete();                  
                }
                
                _MessageCustom.Error = false;
                _MessageCustom.Status = (int)StatusHttp.Ok;
                _MessageCustom.Message = "Los servicios se fusionaron correctamente";
                return _MessageCustom;
            }
            catch (Exception ex)
            {
                _MessageCustom.Error = true;
                _MessageCustom.Status = (int)StatusHttp.BadRequest;
                _MessageCustom.Message = ex.Message;
                return _MessageCustom;
            }
        }

        public List<SaldoPaciente> GetListSaldosPaciente(string serviceId)
        {
            try
            {
                var list = (from ser in ctx.Service
                            join src in ctx.ServiceComponent on ser.v_ServiceId equals src.v_ServiceId
                            join com in ctx.Component on src.v_ComponentId equals com.v_ComponentId
                            where ser.v_ServiceId == serviceId && src.d_SaldoPaciente > 0
                            select new SaldoPaciente
                            {
                                v_Name = com.v_Name,
                                d_SaldoPaciente = src.d_SaldoPaciente,
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<RoleNodeComponentProfileCustom> GetRoleNodeComponentProfileByRoleNodeId(int pintNodeId, int pintRoleId)
        {
            //mon.IsActive = true;

            try
            {
                DatabaseContext ctx = new DatabaseContext();

                var query = (from a in ctx.RoleNodeComponentProfile
                             where (a.i_NodeId == pintNodeId) &&
                                   (a.i_RoleId == pintRoleId) &&
                                   (a.i_IsDeleted == (int)SiNo.No)
                             select new RoleNodeComponentProfileCustom
                             {
                                 v_ComponentId = a.v_ComponentId,
                                 v_RoleNodeComponentId = a.v_RoleNodeComponentId,
                                 i_Read = a.i_Read,
                             }).ToList();

                return query;
            }
            catch (Exception ex)
            {
                throw;

            }

        }

        public List<ServiceComponentList> GetServiceComponentsCulminados(string pstrServiceId)
        {
            //mon.IsActive = true;
            try
            {

                var query = (from A in ctx.ServiceComponent
                             join B in ctx.SystemParameter on new { a = A.i_ServiceComponentStatusId.Value, b = 127 }
                                    equals new { a = B.i_ParameterId, b = B.i_GroupId }
                             join C in ctx.Component on A.v_ComponentId equals C.v_ComponentId
                             where (A.v_ServiceId == pstrServiceId) &&
                                   (A.i_IsDeleted == 0) &&
                                   (A.i_IsRequiredId == (int?)SiNo.Si) &&
                                   (A.i_ServiceComponentStatusId != (int)ServiceComponentStatus.Evaluado)

                             select new ServiceComponentList
                             {
                                 v_ComponentId = A.v_ComponentId,
                                 v_ComponentName = C.v_Name,
                                 i_ServiceComponentStatusId = A.i_ServiceComponentStatusId.Value,
                                 v_ServiceComponentStatusName = B.v_Value1,
                                 i_CategoryId = C.i_CategoryId
                             }).ToList();

                return query;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ServiceComponentList> GetServiceComponents(string pstrServiceId)
        {


            int isDeleted = (int)SiNo.No;
            int isRequired = (int)SiNo.Si;

            try
            {

                var query = (from A in ctx.ServiceComponent
                             join B in ctx.SystemParameter on new { a = A.i_ServiceComponentStatusId.Value, b = 127 }
                                      equals new { a = B.i_ParameterId, b = B.i_GroupId }
                             join C in ctx.Component on A.v_ComponentId equals C.v_ComponentId
                             join D in ctx.SystemParameter on new { a = A.i_QueueStatusId.Value, b = 128 }
                                      equals new { a = D.i_ParameterId, b = D.i_GroupId }
                             join E in ctx.Service on A.v_ServiceId equals E.v_ServiceId
                             join F in ctx.SystemParameter on new { a = C.i_CategoryId, b = 116 }
                                      equals new { a = F.i_ParameterId, b = F.i_GroupId } into F_join
                             from F in F_join.DefaultIfEmpty()

                             where A.v_ServiceId == pstrServiceId &&
                                   A.i_IsDeleted == isDeleted &&
                                   A.i_IsRequiredId == isRequired

                             select new ServiceComponentList
                             {
                                 v_ComponentId = A.v_ComponentId,
                                 v_ComponentName = C.v_Name,
                                 i_ServiceComponentStatusId = A.i_ServiceComponentStatusId.Value,
                                 v_ServiceComponentStatusName = B.v_Value1,
                                 d_StartDate = A.d_StartDate.Value,
                                 d_EndDate = A.d_EndDate.Value,
                                 i_QueueStatusId = A.i_QueueStatusId.Value,
                                 v_QueueStatusName = D.v_Value1,
                                 ServiceStatusId = E.i_ServiceStatusId.Value,
                                 v_Motive = E.v_Motive,
                                 i_CategoryId = C.i_CategoryId,
                                 v_CategoryName = C.i_CategoryId == -1 ? C.v_Name : F.v_Value1,
                                 v_ServiceId = E.v_ServiceId,
                                 v_ServiceComponentId = A.v_ServiceComponentId,
                             });

                var objData = query.AsEnumerable()
                             .Where(s => s.i_CategoryId != -1)
                             .GroupBy(x => x.i_CategoryId)
                             .Select(group => group.First());

                List<ServiceComponentList> obj = objData.ToList();

                obj.AddRange(query.Where(p => p.i_CategoryId == -1));

                return obj;
            }
            catch (Exception ex)
            { 
                return null;
            }
        }

        public bool PermitirLlamar(string pstrServiceId, int pintPiso)
        {
            //mon.IsActive = true;


            try
            {
                bool Respuesta = true;

                var query = (from s in ctx.ServiceComponent
                             join c in ctx.Component on s.v_ComponentId equals c.v_ComponentId
                             join P in ctx.SystemParameter on new { a = 116, b = c.i_CategoryId }
                                   equals new { a = P.i_GroupId, b = P.i_ParameterId } //into P_join
                                                                                       //from P in P_join.DefaultIfEmpty()

                             join P1 in ctx.SystemParameter on new { a = 127, b = s.i_ServiceComponentStatusId.Value }
                            equals new { a = P1.i_GroupId, b = P1.i_ParameterId } //into P1_join
                                                                                  //from P1 in P1_join.DefaultIfEmpty()

                             where s.v_ServiceId == pstrServiceId
                             select new PisoCustom
                             {
                                 v_Categoria = P.v_Value1,
                                 ValorPiso = P.v_Value2,
                                 i_CategoriaId = c.i_CategoryId,
                                 i_EstadoComponente = s.i_ServiceComponentStatusId.Value,
                                 v_EstadoComponente = P1.v_Value1
                             });


                var objData = query.AsEnumerable()
                           .GroupBy(x => x.i_CategoriaId)
                           .Select(group => group.First())
                           .OrderBy(o => o.ValorPiso);


                foreach (var item in objData)
                {
                    if (int.Parse(item.ValorPiso.ToString()) < pintPiso && item.i_EstadoComponente != (int)ServiceComponentStatus.Evaluado)
                    {
                        Respuesta = false;
                    }
                }

                return Respuesta;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<ServiceComponentList> GetServiceComponentByCategoryId(int pstrCategoryId, string pstrServiceId)
        {
            //mon.IsActive = true;

            try
            {
                var objEntity = (from a in ctx.ServiceComponent
                                 join b in ctx.Component on a.v_ComponentId equals b.v_ComponentId
                                 where b.i_CategoryId == pstrCategoryId && a.v_ServiceId == pstrServiceId && a.i_IsRequiredId == (int)SiNo.Si
                                 select new ServiceComponentList
                                 {
                                     v_ServiceComponentId = a.v_ServiceComponentId,
                                     v_ServiceId = a.v_ServiceId,
                                     v_ComponentId = a.v_ComponentId,
                                     v_ComponentName = b.v_Name,
                                     i_ServiceComponentStatusId = a.i_ServiceComponentStatusId
                                 }).ToList();

                List<ServiceComponentList> objDataList = objEntity.ToList();
                return objDataList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool UpdateAdditionalExam(List<ServiceComponentList> pobjDtoEntity, string serviceId, int? isRequiredId, int userId)
        {
            //mon.IsActive = true;

            try
            {

                var serviceComponentId = pobjDtoEntity.Select(p => p.v_ServiceComponentId).ToArray();

                // Obtener la entidad fuente
                var objEntitySource = (from sc in ctx.ServiceComponent
                                       where sc.v_ServiceId == serviceId && serviceComponentId.Contains(sc.v_ServiceComponentId)
                                       select sc).ToList();


                foreach (var item in objEntitySource)
                {
                    item.d_UpdateDate = DateTime.Now;
                    item.i_UpdateUserId = userId;
                    item.i_IsRequiredId = isRequiredId;
                }

                // Guardar los cambios
                

                return ctx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Categoria> GetAllComponents(int? filterType, string name)
        {

            int isDeleted = (int)SiNo.No;
            string codigoSegus = "";
            string nameCategory = "";
            string nameComponent = "";
            string nameSubCategory = "";
            string componentId = "";
            if (filterType == (int)TipoBusqueda.CodigoSegus)
            {
                codigoSegus = name;

            }
            else if (filterType == (int)TipoBusqueda.NombreCategoria)
            {
                nameCategory = name;
            }
            else if (filterType == (int)TipoBusqueda.NombreComponent)
            {
                nameComponent = name;
            }
            else if (filterType == (int)TipoBusqueda.NombreSubCategoria)
            {
                nameSubCategory = name;
            }
            else if (filterType == (int)TipoBusqueda.ComponentId)
            {
                componentId = name;
            }


            try
            {
                System.Linq.IQueryable<Categoria> query;
                if (name == "")
                {
                    query = (from C in ctx.Component
                             join F in ctx.SystemParameter on new { a = C.i_CategoryId, b = 116 }
                                 equals new { a = F.i_ParameterId, b = F.i_GroupId } into F_join
                             from F in F_join.DefaultIfEmpty()

                             where C.i_IsDeleted == 0
                             select new Categoria
                             {
                                 v_ComponentId = C.v_ComponentId,
                                 v_ComponentName = C.v_Name,

                                 v_CodigoSegus = C.v_CodigoSegus,
                                 i_CategoryId = C.i_CategoryId,
                                 v_CategoryName = C.i_CategoryId == -1 ? C.v_Name : F.v_Value1,

                             });

                }
                else if (filterType == (int)TipoBusqueda.ComponentId)
                {
                    query = (from C in ctx.Component
                             join F in ctx.SystemParameter on new { a = C.i_CategoryId, b = 116 }
                                 equals new { a = F.i_ParameterId, b = F.i_GroupId } into F_join

                             from F in F_join.DefaultIfEmpty()

                             where C.i_IsDeleted == 0 && C.v_ComponentId == componentId
                             select new Categoria
                             {
                                 v_ComponentId = C.v_ComponentId,
                                 v_ComponentName = C.v_Name,
                                 v_CodigoSegus = C.v_CodigoSegus,
                                 i_CategoryId = C.i_CategoryId,
                                 v_CategoryName = C.i_CategoryId == -1 ? C.v_Name : F.v_Value1,

                             });
                    var query2 = query
                        .GroupBy(x => new { x.v_CodigoSegus, x.v_ComponentName, x.v_ComponentId, x.i_CategoryId, x.v_CategoryName })
                        .Select(g => new { g.Key.v_CodigoSegus, g.Key.v_ComponentName, g.Key.v_ComponentId, g.Key.i_CategoryId, g.Key.v_CategoryName });

                }
                else
                {
                    query = (from C in ctx.Component
                             join F in ctx.SystemParameter on new { a = C.i_CategoryId, b = 116 }
                                 equals new { a = F.i_ParameterId, b = F.i_GroupId } into F_join
                             from F in F_join.DefaultIfEmpty()

                             join G in ctx.SystemParameter on new { a = F.i_ParameterId, b = 116 }
                                 equals new { a = G.i_ParentParameterId.Value, b = G.i_GroupId } into G_join
                             from G in G_join.DefaultIfEmpty()

                             where C.i_IsDeleted == 0 && (G.v_Value1.Contains(nameSubCategory) && C.v_Name.Contains(nameComponent) && F.v_Value1.Contains(nameCategory) && C.v_CodigoSegus.Contains(codigoSegus))
                             select new Categoria
                             {
                                 v_ComponentId = C.v_ComponentId,
                                 v_ComponentName = C.v_Name,
                                 v_CodigoSegus = C.v_CodigoSegus,
                                 i_CategoryId = C.i_CategoryId,
                                 v_CategoryName = C.i_CategoryId == -1 ? C.v_Name : F.v_Value1,

                             });
                    var query2 = query
                        .GroupBy(x => new
                        { x.v_CodigoSegus, x.v_ComponentName, x.v_ComponentId, x.i_CategoryId, x.v_CategoryName })
                        .Select(g => new { g.Key.v_CodigoSegus, g.Key.v_ComponentName, g.Key.v_ComponentId, g.Key.i_CategoryId, g.Key.v_CategoryName });

                }


                var objData = query.AsEnumerable()
                    .Where(s => s.i_CategoryId != -1)
                    .GroupBy(x => x.i_CategoryId)
                    .Select(group => group.First());

                List<Categoria> obj = objData.ToList();

                Categoria objCategoriaList;
                List<Categoria> Lista = new List<Categoria>();

                //int CategoriaId_Old = 0;
                for (int i = 0; i < obj.Count(); i++)
                {
                    objCategoriaList = new Categoria();

                    objCategoriaList.i_CategoryId = obj[i].i_CategoryId.Value;
                    objCategoriaList.v_CategoryName = obj[i].v_CategoryName;

                    var x = query.ToList().FindAll(p => p.i_CategoryId == obj[i].i_CategoryId.Value);

                    x.Sort((z, y) => z.v_ComponentName.CompareTo(y.v_ComponentName));
                    ComponentDetailList objComponentDetailList;
                    List<ComponentDetailList> ListaComponentes = new List<ComponentDetailList>();
                    foreach (var item in x)
                    {
                        objComponentDetailList = new ComponentDetailList();
                        objComponentDetailList.v_ComponentId = item.v_ComponentId;
                        objComponentDetailList.v_ComponentName = item.v_ComponentName;
                        //objComponentDetailList.v_ServiceComponentId = item.v_ServiceComponentId;
                        var list = ListaComponentes.Find(z => z.v_ComponentId == item.v_ComponentId);
                        if (list == null)
                        {
                            ListaComponentes.Add(objComponentDetailList);
                        }

                    }

                    objCategoriaList.Componentes = ListaComponentes;

                    Lista.Add(objCategoriaList);

                }
                return Lista;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool AddAdditionalExam(List<AdditionalExamCustom> listAdditionalExam, int userId, int nodeId)
        {
            try
            {
                foreach (var exam in listAdditionalExam)
                {
                    var addtionalExamId = new Common.Utils().GetPrimaryKey(nodeId, 49, "AE");

                    AdditionalExamBE objAdditionalExam = new AdditionalExamBE();
                    objAdditionalExam.v_AdditionalExamId = addtionalExamId;
                    objAdditionalExam.v_ServiceId = exam.ServiceId;
                    objAdditionalExam.v_PersonId = exam.PersonId;
                    objAdditionalExam.v_ProtocolId = exam.ProtocolId;
                    objAdditionalExam.v_Commentary = exam.Commentary;
                    objAdditionalExam.v_ComponentId = exam.ComponentId;
                    objAdditionalExam.i_IsNewService = exam.IsNewService;
                    objAdditionalExam.i_IsProcessed = exam.IsProcessed;
                    objAdditionalExam.i_IsDeleted = (int)SiNo.No;
                    objAdditionalExam.d_InsertDate = DateTime.Now;
                    objAdditionalExam.i_InsertUserId = userId;

                    ctx.AdditionalExam.Add(objAdditionalExam);
                }

                return ctx.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public bool LiberarPaciente(List<string> list)
        {
            try
            {
                foreach (var servicecomponentId in list)
                {
                    // Obtener la entidad fuente

                    var objEntitySource = ctx.ServiceComponent.SingleOrDefault(p => p.v_ServiceComponentId == servicecomponentId);
                    objEntitySource.i_QueueStatusId = (int)QueueStatusId.Libre;
                    objEntitySource.i_Iscalling = (int)SiNo.No;
                    objEntitySource.i_Iscalling_1 = (int)SiNo.No;
                    objEntitySource.d_EndDate = DateTime.Now;

                }

                // Guardar los cambios
                ctx.SaveChanges();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateServiceComponentOfficeLlamando(List<string> servicescomponent, string oficina)
        {
            //mon.IsActive = true;

            try
            {
                foreach (var id in servicescomponent)
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in ctx.ServiceComponent
                                           where a.v_ServiceComponentId == id
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.v_NameOfice = oficina;
                    objEntitySource.i_QueueStatusId = (int)QueueStatusId.llamando;
                    ctx.SaveChanges();
                }
                // Guardar los cambios
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public UsuarioGrabo DevolverDatosUsuarioFirma(int systemuserId)
        {
            var objEntity = (from me in ctx.SystemUser

                             join pme in ctx.Professional on me.v_PersonId equals pme.v_PersonId into pme_join
                             from pme in pme_join.DefaultIfEmpty()

                             join B in ctx.Person on pme.v_PersonId equals B.v_PersonId

                             where me.i_SystemUserId == systemuserId
                             select new UsuarioGrabo
                             {
                                 Firma = pme.b_SignatureImage,
                                 Nombre = B.v_FirstLastName + " " + B.v_SecondLastName + " " + B.v_FirstName,
                                 CMP = pme.v_ProfessionalCode
                             }).FirstOrDefault();

            return objEntity;

        }

        public bool LiberarPacientelaboratorio(List<string> list, int i_ServiceComponentStatusId_Antiguo)
        {
            try
            {
                int status = i_ServiceComponentStatusId_Antiguo == 1 ? (int)ServiceComponentStatus.PorAprobacion : i_ServiceComponentStatusId_Antiguo;
                foreach (var servicecomponentId in list)
                {
                    // Obtener la entidad fuente

                    var objEntitySource = ctx.ServiceComponent.SingleOrDefault(p => p.v_ServiceComponentId == servicecomponentId);
                    objEntitySource.i_QueueStatusId = (int)QueueStatusId.Libre;
                    objEntitySource.i_Iscalling = (int)SiNo.No;
                    objEntitySource.i_Iscalling_1 = (int)SiNo.No;
                    objEntitySource.d_EndDate = DateTime.Now;
                    objEntitySource.i_ServiceComponentStatusId = status;

                }
                // Guardar los cambios
                ctx.SaveChanges();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public OrganizationDto GetInfoMedicalCenter()
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                OrganizationDto objDtoEntity = null;
                var objEntity = (from o in dbContext.Organization
                                 where o.v_OrganizationId == Constants.OWNER_ORGNIZATION_ID
                                 select new OrganizationDto
                                 {
                                     v_OrganizationId = o.v_OrganizationId,
                                     b_Image = o.b_Image,
                                     v_Name = o.v_Name,
                                     v_Address = o.v_Address,

                                 }).SingleOrDefault();

                var other = (from o in dbContext.Location
                             where o.v_OrganizationId == Constants.OWNER_ORGNIZATION_ID
                             select new OrganizationDto
                             {
                                 v_OrganizationId = o.v_OrganizationId,
                                 v_Name = o.v_Name,
                             }).SingleOrDefault();
                objEntity.v_SectorName = other == null ? "" : other.v_Name;

                if (objEntity != null)
                    objDtoEntity = objEntity;

                return objDtoEntity;
            }
        }

        public List<AdditionalExamCustom> GetAdditionalExamByServiceId_all(string serviceId, int userId)
        {
            DatabaseContext dbcontext = new DatabaseContext();

            var list = (from ade in dbcontext.AdditionalExam
                        where ade.v_ServiceId == serviceId && ade.i_IsDeleted == 0 && ade.i_InsertUserId == userId
                        select new AdditionalExamCustom
                        {
                            ComponentId = ade.v_ComponentId,
                            ServiceId = ade.v_ServiceId,
                            IsProcessed = ade.i_IsProcessed.Value,
                            IsNewService = ade.i_IsNewService.Value
                        }).ToList();

            return list;
        }


        public bool UpdateServiceForCalendar(DateTime InicioCircuito, int userId, string serviceId)
        {
            //mon.IsActive = true;

            try
            {
                DatabaseContext dbContext = new DatabaseContext();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.Service
                                       where a.v_ServiceId == serviceId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.i_ServiceStatusId = (int)ServiceStatus.Iniciado;
                objEntitySource.d_UpdateDate = DateTime.Now;
                objEntitySource.i_UpdateUserId = userId;

                // Guardar los cambios
                return dbContext.SaveChanges() > 0;

                
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
