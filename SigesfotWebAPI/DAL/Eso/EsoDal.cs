using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using BE.Common;
using  BE.Eso;
using BE.Service;
using DAL.Common;

namespace DAL.Eso
{
    public class EsoDal
    {
        private static readonly DatabaseContext _Ctx = new DatabaseContext();

        public List<ComponentList> GetServiceComponentsForBuildMenu(string serviceId)
        {
            var isDeleted = 0;
            var recomId = (int)Enumeratores.Typifying.Recomendaciones;
            var restricId = (int)Enumeratores.Typifying.Restricciones;
            var groupMeasurementUnitId = 105;

            #region Recomendation

            var valueFieldsRecome = (from s in _Ctx.Service
                join sc in _Ctx.ServiceComponent on s.v_ServiceId equals sc.v_ServiceId
                join c in _Ctx.Component on sc.v_ComponentId equals c.v_ComponentId
                join cfs in _Ctx.ComponentFields on c.v_ComponentId equals cfs.v_ComponentId
                join cfsv in _Ctx.ComponentFieldValues on cfs.v_ComponentFieldId equals cfsv.v_ComponentFieldId
                join rec in _Ctx.ComponentFieldValuesRecommendation on cfsv.v_ComponentFieldValuesId equals rec.v_ComponentFieldValuesId
                join mrec in _Ctx.MasterRecommendationRestricction on rec.v_MasterRecommendationRestricctionId equals mrec.v_MasterRecommendationRestricctionId

                where (rec.i_IsDeleted == isDeleted) &&
                      (mrec.i_TypifyingId == recomId) &&
                      (s.v_ServiceId == serviceId) &&
                      (sc.i_IsDeleted == isDeleted)

                select new RecomendationList
                {
                    v_ComponentFieldValuesRecommendationId = rec.v_ComponentFieldValuesRecommendationId,
                    v_ComponentFieldValuesId = rec.v_ComponentFieldValuesId,
                    v_MasterRecommendationId = rec.v_MasterRecommendationRestricctionId,
                    v_RecommendationName = mrec.v_Name,
                    v_ComponentId = sc.v_ComponentId,
                    i_RecordStatus = (int)Enumeratores.RecordStatus.Grabado,
                    i_RecordType = (int)Enumeratores.RecordType.NoTemporal,

                }).ToList();

            #endregion

            #region Restriction

            var valueFieldsRestri = (from s in _Ctx.Service
                join sc in _Ctx.ServiceComponent on s.v_ServiceId equals sc.v_ServiceId
                join c in _Ctx.Component on sc.v_ComponentId equals c.v_ComponentId
                join cfs in _Ctx.ComponentFields on c.v_ComponentId equals cfs.v_ComponentId
                join cfsv in _Ctx.ComponentFieldValues on cfs.v_ComponentFieldId equals cfsv.v_ComponentFieldId
                join res in _Ctx.ComponentFieldValuesRestriction on cfsv.v_ComponentFieldValuesId equals res.v_ComponentFieldValuesId
                join mres in _Ctx.MasterRecommendationRestricction on res.v_MasterRecommendationRestricctionId equals mres.v_MasterRecommendationRestricctionId

                where (res.i_IsDeleted == isDeleted) &&
                      (mres.i_TypifyingId == restricId) &&
                      (s.v_ServiceId == serviceId) &&
                      (sc.i_IsDeleted == isDeleted)

                select new RestrictionList
                {
                    v_ComponentFieldValuesRestrictionId = res.v_ComponentFieldValuesRestrictionId,
                    v_ComponentFieldValuesId = res.v_ComponentFieldValuesId,
                    v_MasterRestrictionId = res.v_MasterRecommendationRestricctionId,
                    v_RestrictionName = mres.v_Name,
                    v_ComponentId = sc.v_ComponentId,
                    i_RecordStatus = (int)Enumeratores.RecordStatus.Grabado,
                    i_RecordType = (int)Enumeratores.RecordType.NoTemporal
                }).ToList();
            #endregion

            #region Values

            var valueFields = (from s in _Ctx.Service
                join sc in _Ctx.ServiceComponent on s.v_ServiceId equals sc.v_ServiceId
                join c in _Ctx.Component on sc.v_ComponentId equals c.v_ComponentId
                join cfs in _Ctx.ComponentFields on c.v_ComponentId equals cfs.v_ComponentId
                join cfsv in _Ctx.ComponentFieldValues on cfs.v_ComponentFieldId equals cfsv.v_ComponentFieldId
                join dise in _Ctx.Diseases on cfsv.v_DiseasesId equals dise.v_DiseasesId
                where (cfsv.i_IsDeleted == isDeleted) &&
                      (s.v_ServiceId == serviceId) &&
                      (sc.i_IsDeleted == isDeleted) &&
                      (sc.i_IsDeleted == isDeleted)

                select new ComponentFieldValues
                {
                    v_ComponentFieldValuesId = cfsv.v_ComponentFieldValuesId,
                    v_ComponentFieldsId = cfsv.v_ComponentFieldId,
                    v_AnalyzingValue1 = cfsv.v_AnalyzingValue1,
                    v_AnalyzingValue2 = cfsv.v_AnalyzingValue2,
                    i_OperatorId = cfsv.i_OperatorId.Value,
                    v_LegalStandard = cfsv.v_LegalStandard,
                    i_IsAnormal = cfsv.i_IsAnormal,
                    i_ValidationMonths = cfsv.i_ValidationMonths,
                    v_DiseasesName = cfsv.Diseases.v_Name,
                    v_DiseasesId = cfsv.v_DiseasesId,
                    v_ComponentId = sc.v_ComponentId,
                    i_GenderId = cfsv.i_GenderId,
                    v_CIE10 = dise.v_CIE10Id

                }).ToList();

            valueFields.ForEach(a =>
            {
                a.Recomendations = valueFieldsRecome.FindAll(p => p.v_ComponentFieldValuesId == a.v_ComponentFieldValuesId);
                a.Restrictions = valueFieldsRestri.FindAll(p => p.v_ComponentFieldValuesId == a.v_ComponentFieldValuesId);
            });

            #endregion

            #region Fields


            var comFields = (from s in _Ctx.Service
                              join sc in _Ctx.ServiceComponent on s.v_ServiceId equals sc.v_ServiceId
                              join c in _Ctx.Component on sc.v_ComponentId equals c.v_ComponentId
                              join cfs in _Ctx.ComponentFields on c.v_ComponentId equals cfs.v_ComponentId
                              join cf in _Ctx.ComponentField on cfs.v_ComponentFieldId equals cf.v_ComponentFieldId

                              let hazAutoDx = (from jjj in _Ctx.DiagnosticRepository
                                               where (jjj.v_ComponentFieldId == cfs.v_ComponentFieldId) &&
                                                   (jjj.v_ServiceId == serviceId) &&
                                                   (jjj.i_IsDeleted == isDeleted)
                                               select new
                                               {
                                                   i_HasAutomaticDxId = jjj.v_ComponentFieldId != null ? (int?)Enumeratores.SiNo.Si : (int?)Enumeratores.SiNo.No
                                               })

                              join dh in _Ctx.DataHierarchy on new { a = groupMeasurementUnitId, b = cf.i_MeasurementUnitId.Value } 
                                                  equals new { a = dh.i_GroupId, b = dh.i_ItemId } into dhJoin
                              from dh in dhJoin.DefaultIfEmpty()
                              
                             where (cfs.i_IsDeleted == isDeleted) &&
                                    (cf.i_IsDeleted == isDeleted) &&
                                    (s.v_ServiceId == serviceId) &&
                                    (sc.i_IsDeleted == isDeleted) &&
                                    (sc.i_IsRequiredId == (int?)Enumeratores.SiNo.Si)

                              select new ComponentFieldsList
                              {
                                  v_ComponentFieldId = cf.v_ComponentFieldId,
                                  v_TextLabel = cf.v_TextLabel,
                                  v_ComponentId = cfs.v_ComponentId,
                                  i_LabelWidth = cf.i_LabelWidth.Value,
                                  v_DefaultText = cf.v_DefaultText,
                                  i_ControlId = cf.i_ControlId.Value,
                                  i_GroupId = cf.i_GroupId.Value,
                                  i_ItemId = cf.i_ItemId.Value,
                                  i_ControlWidth = cf.i_WidthControl.Value,
                                  i_HeightControl = cf.i_HeightControl.Value,
                                  i_MaxLenght = cf.i_MaxLenght.Value,
                                  i_IsRequired = cf.i_IsRequired.Value,
                                  i_Column = cf.i_Column.Value,
                                  v_MeasurementUnitName = dh.v_Value1,
                                  i_IsCalculate = cf.i_IsCalculate.Value,
                                  i_Order = cf.i_Order.Value,
                                  i_MeasurementUnitId = cf.i_MeasurementUnitId.Value,
                                  r_ValidateValue1 = cf.r_ValidateValue1.Value,
                                  r_ValidateValue2 = cf.r_ValidateValue2.Value,
                                  v_Group = cfs.v_Group,
                                  v_Formula = cf.v_Formula,
                                  i_NroDecimales = cf.i_NroDecimales.Value,
                                  i_ReadOnly = cf.i_ReadOnly.Value,
                                  i_Enabled = cf.i_Enabled.Value,
                                  i_HasAutomaticDxId = hazAutoDx.FirstOrDefault().i_HasAutomaticDxId,

                              }).ToList();

            valueFields.Sort((x, y) => string.Compare(x.v_ComponentFieldsId, y.v_ComponentFieldsId, StringComparison.Ordinal));
            comFields.Sort((x, y) => string.Compare(x.v_ComponentFieldId, y.v_ComponentFieldId, StringComparison.Ordinal));
            comFields.Sort((x, y) => string.Compare(x.v_Group, y.v_Group, StringComparison.Ordinal));
            comFields.ForEach(a => a.Values = valueFields.FindAll(p => p.v_ComponentFieldsId == a.v_ComponentFieldId));

            var oSystemparameter = (from a in _Ctx.SystemParameter select a).ToList();

            comFields.ForEach(a => a.ComboValues = a.i_GroupId == 0
                ? null
                : (from x in oSystemparameter
                    where x.i_GroupId == a.i_GroupId
                    select new KeyValueDTO
                    {
                        Id = x.i_ParameterId.ToString(),
                        Value = x.v_Value1
                    }).ToList());


            Formulate formu = null;
            TargetFieldOfCalculate targetFieldOfCalculate = null;

            foreach (var item in comFields)
            {
                List<Formulate> formuList = new List<Formulate>();
                List<TargetFieldOfCalculate> targetFieldOfCalculateList = new List<TargetFieldOfCalculate>();

                var find = comFields.FindAll(p => p.v_Formula != null && p.v_Formula.Contains(item.v_ComponentFieldId));

                if (find.Count != 0)
                {
                    item.i_IsSourceFieldToCalculate = (int)Enumeratores.SiNo.Si;

                    foreach (var f in find)
                    {
                        formu = new Formulate();
                        formu.v_Formula = f.v_Formula;
                        formu.v_TargetFieldOfCalculateId = f.v_ComponentFieldId;
                        formuList.Add(formu);

                        targetFieldOfCalculate = new TargetFieldOfCalculate();
                        targetFieldOfCalculate.v_TargetFieldOfCalculateId = f.v_ComponentFieldId;
                        targetFieldOfCalculateList.Add(targetFieldOfCalculate);
                    }

                    item.Formula = formuList;
                    item.TargetFieldOfCalculateId = targetFieldOfCalculateList;
                }

            }

            // obligatorio para que los controles se dibujen en orden adecuado
            comFields.Sort((x, y) => x.i_Order.CompareTo(y.i_Order));

            #endregion

            #region Components

            List<ComponentList> components = (from A in _Ctx.ServiceComponent
                                              join bbb in _Ctx.Component on A.v_ComponentId equals bbb.v_ComponentId

                                              join fff in _Ctx.SystemParameter on new { a = bbb.i_CategoryId, b = 116 } 
                                                                                           equals new { a = fff.i_ParameterId, b = fff.i_GroupId } into j5Join
                                              from fff in j5Join.DefaultIfEmpty()

                                              where (A.v_ServiceId == serviceId) &&
                                                    (bbb.i_ComponentTypeId == (int?)Enumeratores.ComponentType.Examen) &&
                                                    (A.i_IsDeleted == 0) &&
                                                    (A.i_IsRequiredId == (int?)Enumeratores.SiNo.Si)
                                              select new ComponentList
                                              {
                                                  v_ComponentId = bbb.v_ComponentId,
                                                  v_Name = bbb.v_Name,
                                                  i_UIIsVisibleId = bbb.i_UIIsVisibleId,
                                                  i_ComponentTypeId = bbb.i_ComponentTypeId,
                                                  v_ServiceComponentId = A.v_ServiceComponentId,
                                                  d_CreationDate = A.d_InsertDate,
                                                  d_UpdateDate = A.d_UpdateDate,
                                                  i_IsDeleted = A.i_IsDeleted.Value,
                                                  i_CategoryId = bbb.i_CategoryId,
                                                  v_CategoryName = fff.v_Value1,
                                                  i_GroupedComponentId = bbb.i_CategoryId,
                                                  v_GroupedComponentName = fff.v_Value1,
                                                  v_ComponentCopyId = bbb.v_ComponentId,
                                                  i_ServiceComponentStatusId = A.i_ServiceComponentStatusId.Value,
                                                  i_Index = bbb.i_UIIndex
                                              }).ToList();
            
            components.Sort((x, y) => string.Compare(x.v_ComponentId, y.v_ComponentId, StringComparison.Ordinal));
            components.ForEach(a => a.Fields = comFields.FindAll(p => p.v_ComponentId == a.v_ComponentId));

            components.OrderBy(o1 => o1.v_ServiceComponentId).ThenBy(o2 => o2.i_Index).ToList();

            #endregion

            #region Agrupar componentes individuales en un solo examen component ejem: Laboratorio , Psicologia

            var categories = components.FindAll(p => p.i_CategoryId != -1)
                            .GroupBy(g => g.i_CategoryId)
                            .Select(s => s.First())
                            .OrderByDescending(o => o.v_CategoryName).ToList();

            components.Sort((x, y) => x.i_CategoryId.Value.CompareTo(y.i_CategoryId.Value));

            for (int i = 0; i < categories.Count; i++)
            {
                var categoryId = categories[i].i_CategoryId;

                var componentsByCategory = components.FindAll(p => p.i_CategoryId == categoryId)
                                          .OrderBy(o => o.i_Index).ToList();

                var join = string.Join("|", componentsByCategory.Select(p => p.v_ComponentId));
                categories[i].v_ComponentId = join;

                var groupedComponentsName = new List<ComponentList>();

                for (int j = 0; j < componentsByCategory.Count; j++)
                {
                    var groupedComponentName = new ComponentList();
                    groupedComponentName.v_ComponentId = componentsByCategory[j].v_ComponentCopyId;
                    groupedComponentName.v_GroupedComponentName = componentsByCategory[j].v_Name;
                    groupedComponentName.i_Index = componentsByCategory[j].i_Index;
                    groupedComponentsName.Add(groupedComponentName);
                }

                categories[i].GroupedComponentsName = groupedComponentsName;
            }

            var componentsToImport = components.FindAll(p => p.i_CategoryId != -1)
                                    .OrderBy(o => o.i_CategoryId).ToList();
         
            for (int i = 0; i < categories.Count; i++)
            {
                var categoryId = categories[i].i_CategoryId;

                var fields = componentsToImport.FindAll(p => p.i_CategoryId == categoryId)
                            .SelectMany(p => p.Fields).ToList();

                categories[i].i_IsGroupedComponent = (int)Enumeratores.SiNo.Si;
                categories[i].v_Name = categories[i].v_CategoryName;
                categories[i].Fields = new List<ComponentFieldsList>();
                categories[i].Fields.AddRange(fields);
            }

            for (int i = 0; i < componentsToImport.Count; i++)
            {
                components.Remove(componentsToImport[i]);
            }

            components.AddRange(categories);

            #endregion

            return components;
        }

        //public string SaveMedicalExam(List<ServiceComponentFieldsList> oServicecomponentfields, string personId, string serviceComponentId, int nodeId, int systemUserId)
        //{

        //    try
        //    {
        //        using (var ts = new TransactionScope())
        //        {
        //            using (var oCtx = new DatabaseContext())
        //            {
        //                var servicecomponentFielId = "";
        //                var serviceComponentfields = (from a in oCtx.ServiceComponentFields
        //                                              where a.v_ServiceComponentId == serviceComponentId
        //                                              select a).ToList();

        //                if (serviceComponentfields.Count == 0)
        //                {

        //                    var firstScFielId = new Common.Utils().GetFirstPrimaryKey(nodeId, 35, oServicecomponentfields.Count());

        //                    foreach (var cf in oServicecomponentfields)
        //                    {

        //                        var oServiceComponentFieldsDto = new ServiceComponentFieldsDto();

        //                        oServiceComponentFieldsDto.v_ComponentFieldId = cf.v_ComponentFieldId;
        //                        oServiceComponentFieldsDto.v_ServiceComponentId = cf.v_ServiceComponentId;
        //                        oServiceComponentFieldsDto.d_InsertDate = DateTime.Now;
        //                        oServiceComponentFieldsDto.i_InsertUserId = systemUserId;
        //                        oServiceComponentFieldsDto.i_IsDeleted = 0;
        //                        servicecomponentFielId = new Common.Utils().FormarPk(nodeId, 35, "CF", firstScFielId);

        //                        oServiceComponentFieldsDto.v_ServiceComponentFieldsId = servicecomponentFielId;

        //                        oCtx.ServiceComponentFields.Add(oServiceComponentFieldsDto);
        //                        oCtx.SaveChanges();

        //                        var firstScfValueId = new Common.Utils().GetFirstPrimaryKey(nodeId, 36, cf.ServiceComponentFieldValues.Count());

        //                        foreach (var fv in cf.ServiceComponentFieldValues)
        //                        {
        //                            var oServiceComponentFieldValuesDto = new ServiceComponentFieldValuesDto();

        //                            oServiceComponentFieldValuesDto.v_ComponentFieldValuesId = fv.v_ComponentFieldValuesId;
        //                            oServiceComponentFieldValuesDto.v_Value1 = fv.v_Value1;
        //                            oServiceComponentFieldValuesDto.d_InsertDate = DateTime.Now;
        //                            oServiceComponentFieldValuesDto.i_InsertUserId = systemUserId;
        //                            oServiceComponentFieldValuesDto.i_IsDeleted = 0;

        //                            var serviceComponentFieldValuesId = new Common.Utils().FormarPk(nodeId, 36, "CV", firstScfValueId);
        //                            oServiceComponentFieldValuesDto.v_ServiceComponentFieldsId = servicecomponentFielId;
        //                            oServiceComponentFieldValuesDto.v_ServiceComponentFieldValuesId = serviceComponentFieldValuesId;

        //                            oCtx.ServiceComponentFieldValues.Add(oServiceComponentFieldValuesDto);
        //                            oCtx.SaveChanges();

        //                            //Next
        //                            firstScfValueId++;
        //                        }

        //                        //Next
        //                        firstScFielId++;
        //                    }
        //                    //oCtx.SaveChanges();
        //                }
        //                else
        //                {
        //                    serviceComponentfields.Sort((x, y) => String.Compare(x.v_ComponentFieldId, y.v_ComponentFieldId, StringComparison.Ordinal));

        //                    foreach (var cf in oServicecomponentfields)
        //                    {
        //                        var q = serviceComponentfields.Find(p => p.v_ComponentFieldId == cf.v_ComponentFieldId);

        //                        q.d_UpdateDate = DateTime.Now;
        //                        q.i_UpdateUserId = systemUserId;

        //                        foreach (var fv in cf.ServiceComponentFieldValues)
        //                        {
        //                            var q1 = (from a in oCtx.ServiceComponentFieldValues
        //                                      where a.v_ServiceComponentFieldsId == q.v_ServiceComponentFieldsId
        //                                      select a).FirstOrDefault();

        //                            if (q1 == null)
        //                            {
        //                                var oServiceComponentFieldValuesDto = new ServiceComponentFieldValuesDto();

        //                                oServiceComponentFieldValuesDto.v_ComponentFieldValuesId =
        //                                    fv.v_ComponentFieldValuesId;
        //                                oServiceComponentFieldValuesDto.v_Value1 = fv.v_Value1;
        //                                oServiceComponentFieldValuesDto.d_InsertDate = DateTime.Now;
        //                                oServiceComponentFieldValuesDto.i_InsertUserId = systemUserId;
        //                                oServiceComponentFieldValuesDto.i_IsDeleted = 0;
        //                                var newId = new Common.Utils().GetPrimaryKey(nodeId, 36, "CV");

        //                                oServiceComponentFieldValuesDto.v_ServiceComponentFieldValuesId = newId;
        //                                oServiceComponentFieldValuesDto.v_ServiceComponentFieldsId =
        //                                    q.v_ServiceComponentFieldsId;

        //                                oCtx.ServiceComponentFieldValues.Add(oServiceComponentFieldValuesDto);
        //                            }
        //                            else
        //                            {
        //                                q1.v_Value1 = fv.v_Value1;
        //                                q1.d_UpdateDate = DateTime.Now;
        //                                q1.i_UpdateUserId = systemUserId;
        //                            }
        //                            oCtx.SaveChanges();
        //                        }
        //                    }

        //                }
        //            }

        //            ts.Complete();
        //            return serviceComponentId;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        var error = Common.Utils.ExceptionFormatter(ex);
        //        new LogDal().SaveLog(nodeId.ToString(), "ESO WEB", systemUserId.ToString(), Enumeratores.LogEventType.Create, "ESO WEB =" + personId, "ESO WEBBB=" + serviceComponentId, Enumeratores.Success.Failed, error);
        //        return "";
        //    }


        //}

        public string SaveMedicalExam(List<ServiceComponentFieldsList> oServicecomponentfields, string personId, string serviceComponentId, int nodeId, int systemUserId)
        {

            try
            {

                using (var ts = new TransactionScope())
                {
                    using (var oCtx = new DatabaseContext())
                    {
                        var servicecomponentFielId = "";
                        var serviceComponentfields = (from a in oCtx.ServiceComponentFields
                                                      where a.v_ServiceComponentId == serviceComponentId
                                                      select a).ToList();

                        serviceComponentfields.Sort((x, y) => String.Compare(x.v_ComponentFieldId, y.v_ComponentFieldId, StringComparison.Ordinal));

                        foreach (var cf in oServicecomponentfields)
                        {
                            var q = serviceComponentfields.Find(p => p.v_ComponentFieldId == cf.v_ComponentFieldId);

                            if (q == null) // ADD
                            {
                                var oServiceComponentFieldsDto = new ServiceComponentFieldsDto();

                                oServiceComponentFieldsDto.v_ComponentFieldId = cf.v_ComponentFieldId;
                                oServiceComponentFieldsDto.v_ServiceComponentId = cf.v_ServiceComponentId;
                                oServiceComponentFieldsDto.d_InsertDate = DateTime.Now;
                                oServiceComponentFieldsDto.i_InsertUserId = systemUserId;
                                oServiceComponentFieldsDto.i_IsDeleted = 0;
                                servicecomponentFielId = new Common.Utils().GetPrimaryKey(nodeId, 35, "CF");

                                oServiceComponentFieldsDto.v_ServiceComponentFieldsId = servicecomponentFielId;

                                oCtx.ServiceComponentFields.Add(oServiceComponentFieldsDto);

                                oCtx.SaveChanges();

                                foreach (var fv in cf.ServiceComponentFieldValues)
                                {
                                    var oServiceComponentFieldValuesDto = new ServiceComponentFieldValuesDto();

                                    oServiceComponentFieldValuesDto.v_ComponentFieldValuesId = fv.v_ComponentFieldValuesId;
                                    oServiceComponentFieldValuesDto.v_Value1 = fv.v_Value1;
                                    oServiceComponentFieldValuesDto.d_InsertDate = DateTime.Now;
                                    oServiceComponentFieldValuesDto.i_InsertUserId = systemUserId;
                                    oServiceComponentFieldValuesDto.i_IsDeleted = 0;

                                    var serviceComponentFieldValuesId = new Common.Utils().GetPrimaryKey(nodeId, 36, "CV");
                                    oServiceComponentFieldValuesDto.v_ServiceComponentFieldsId = servicecomponentFielId;
                                    oServiceComponentFieldValuesDto.v_ServiceComponentFieldValuesId = serviceComponentFieldValuesId;

                                    oCtx.ServiceComponentFieldValues.Add(oServiceComponentFieldValuesDto);

                                    oCtx.SaveChanges();
                                }
                            }
                            else
                            {

                                q.d_UpdateDate = DateTime.Now;
                                q.i_UpdateUserId = systemUserId;

                                foreach (var fv in cf.ServiceComponentFieldValues)
                                {
                                    var q1 = (from a in oCtx.ServiceComponentFieldValues
                                              where a.v_ServiceComponentFieldsId == q.v_ServiceComponentFieldsId
                                              select a).FirstOrDefault();

                                    if (q1 == null)
                                    {
                                        var oServiceComponentFieldValuesDto = new ServiceComponentFieldValuesDto();

                                        oServiceComponentFieldValuesDto.v_ComponentFieldValuesId = fv.v_ComponentFieldValuesId;
                                        oServiceComponentFieldValuesDto.v_Value1 = fv.v_Value1;
                                        oServiceComponentFieldValuesDto.d_InsertDate = DateTime.Now;
                                        oServiceComponentFieldValuesDto.i_InsertUserId = systemUserId;
                                        oServiceComponentFieldValuesDto.i_IsDeleted = 0;
                                        var newId = new Common.Utils().GetPrimaryKey(nodeId, 36, "CV");

                                        oServiceComponentFieldValuesDto.v_ServiceComponentFieldValuesId = newId;
                                        oServiceComponentFieldValuesDto.v_ServiceComponentFieldsId = q.v_ServiceComponentFieldsId;

                                        oCtx.ServiceComponentFieldValues.Add(oServiceComponentFieldValuesDto);
                                    }
                                    else
                                    {
                                        q1.v_Value1 = fv.v_Value1;
                                        q1.d_UpdateDate = DateTime.Now;
                                        q1.i_UpdateUserId = systemUserId;
                                    }
                                }
                                oCtx.SaveChanges();

                            }
                        }

                        //oCtx.SaveChanges();

                    }

                    ts.Complete();
                    return serviceComponentId;
                }
            }
            catch (Exception ex)
            {
                var error = Common.Utils.ExceptionFormatter(ex);
                new LogDal().SaveLog(nodeId.ToString(), "ESO WEB", systemUserId.ToString(), Enumeratores.LogEventType.Create, "ESO WEB =" + personId, "ESO WEBBB=" + serviceComponentId, Enumeratores.Success.Failed, error);
                return "";
            }
        }

        public List<ValorCampo> GetInfo(string serviceComponetId)
        {
            using (DatabaseContext ctx =  new DatabaseContext())
            {
                var query = (from a in ctx.ServiceComponentFields
                    join b in ctx.ServiceComponentFieldValues on a.v_ServiceComponentFieldsId equals b.v_ServiceComponentFieldsId
                    where a.i_IsDeleted == (int)Enumeratores.SiNo.No && a.v_ServiceComponentId == serviceComponetId
                    select new ValorCampo
                    {
                        v_ComponentFieldId = a.v_ComponentFieldId,
                        v_Value = b.v_Value1
                    }).ToList();

                return query;
            }
        }

        public ServiceComponentBe GetInfoServiceComponent(string serviceComponentId)
        {
            var query = (from A in _Ctx.ServiceComponent
                        join J1 in _Ctx.SystemUser on new { i_InsertUserId = A.i_InsertUserId }
                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1join
                        from J1 in J1join.DefaultIfEmpty()

                         join J2 in _Ctx.SystemUser on new { i_UpdateUserId = A.i_ApprovedUpdateUserId}
                             equals new { i_UpdateUserId = J2.i_SystemUserId } into J2join
                         from J2 in J2join.DefaultIfEmpty()

                         where A.v_ServiceComponentId == serviceComponentId
                            select new 
                            {
                                ServiceComponentId = A.v_ServiceComponentId,
                                ComponentId = A.v_ComponentId,
                                ServiceComponentStatusId = A.i_ServiceComponentStatusId.Value,
                                Comment = A.v_Comment,
                                ExternalInternalId = A.i_ExternalInternalId.Value,
                                StartDate = A.d_StartDate,
                                EndDate = A.d_EndDate,
                                QueueStatusId = A.i_QueueStatusId.Value,
                                CreationUser = J1.v_UserName,
                                UpdateUser = J2.v_UserName,
                                CreationDate = A.d_InsertDate,
                                UpdateDate = A.d_UpdateDate,
                                IsDeleted = A.i_IsDeleted.Value,
                                IsApprovedId = A.i_IsApprovedId

                            }).ToList();

            var result = (from A in query
                          select new ServiceComponentBe
                          {
                              ServiceComponentId = A.ServiceComponentId,
                              ComponentId = A.ComponentId,
                              ServiceComponentStatusId = A.ServiceComponentStatusId,
                              Comment = A.Comment,
                              ExternalInternalId = A.ExternalInternalId,
                              StartDate = A.StartDate,
                              EndDate = A.EndDate,
                              QueueStatusId = A.QueueStatusId,
                              CreationUser = A.CreationUser,
                              UpdateUser = A.UpdateUser,
                              CreationDate = A.CreationDate.ToString(),
                              UpdateDate = A.UpdateDate.ToString(),
                              IsDeleted = A.IsDeleted,
                              IsApprovedId = A.IsApprovedId

                          }).FirstOrDefault();
    
            return result;
        }

        public bool SaveServiceStatus(string serviceId, int statusServiceId, int systemUserId)
        {
            using (var contex = new DatabaseContext())
            {
                var objEntity = (from a in contex.Service where a.v_ServiceId == serviceId select a).FirstOrDefault();
                if (objEntity == null) return false;

                objEntity.i_ServiceStatusId = statusServiceId;
                objEntity.d_UpdateDate = DateTime.Now;
                objEntity.i_UpdateUserId = systemUserId;
                contex.SaveChanges();
                return true;
            }
            
        }


    }

}
                    