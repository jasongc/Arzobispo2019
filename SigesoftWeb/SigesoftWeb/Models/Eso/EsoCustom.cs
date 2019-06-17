using SigesoftWeb.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SigesoftWeb.Models.Eso
{
    public class ComponentList
    {
        public string v_ComponentId { get; set; }
        public string v_Name { get; set; }
        public string v_ServiceComponentId { get; set; }
        //public string v_IsGroupName { get; set; }
        public int? i_CategoryId { get; set; }
        public string v_CategoryName { get; set; }

        //public int i_DiagnosableId { get; set; }
        //public string v_DiagnosableName { get; set; }
        public int? i_ComponentTypeId { get; set; }
        //public string v_ComponentTypeName { get; set; }
        public float? r_BasePrice { get; set; }
        public int? i_UIIsVisibleId { get; set; }

        public int i_IsDeleted { get; set; }
        public string v_CreationUser { get; set; }
        public string v_UpdateUser { get; set; }
        public DateTime? d_CreationDate { get; set; }
        public DateTime? d_UpdateDate { get; set; }
        public string[] componentsId { get; set; }

        // jerarquia

        public List<ComponentFieldsList> Fields { get; set; }

        public int? i_Index { get; set; }

        public int? i_GroupedComponentId { get; set; }
        public string v_GroupedComponentName { get; set; }
        public int i_IsGroupedComponent { get; set; }
        public List<ComponentList> GroupedComponentsName { get; set; }
        public string v_ComponentCopyId { get; set; }
        public int? i_ServiceComponentStatusId { get; set; }

    }

    public class ComponentFieldsList
    {
        public string v_ComponentFieldId { get; set; }
        public string v_ComponentId { get; set; }
        public string v_Group { get; set; }
        public string v_TextLabel { get; set; }
        public int i_LabelWidth { get; set; }
        public string v_DefaultText { get; set; }
        public int i_ControlId { get; set; }
        public int i_GroupId { get; set; }
        public int i_ItemId { get; set; }
        public int i_ControlWidth { get; set; }
        public int i_HeightControl { get; set; }
        public int i_MaxLenght { get; set; }
        public int i_IsRequired { get; set; }
        public string v_IsRequired { get; set; }
        public int i_IsCalculate { get; set; }
        public int i_Order { get; set; }
        public int i_MeasurementUnitId { get; set; }
        public Single r_ValidateValue1 { get; set; }
        public Single r_ValidateValue2 { get; set; }
        public int i_Column { get; set; }

        public int? i_HasAutomaticDxId { get; set; }
        public string v_HasAutomaticDxComponentFieldsId { get; set; }

        public string v_MeasurementUnitName { get; set; }

        /// <summary>
        /// Indica si el campo es de fuente para algun calculo
        /// </summary>
        public int i_IsSourceFieldToCalculate { get; set; }
        /// <summary>
        /// Campo1 participante del calculo 
        /// </summary>
        public string v_SourceFieldToCalculateId1 { get; set; }
        /// <summary>
        /// Campo2 participante del calculo
        /// </summary>
        public string v_SourceFieldToCalculateId2 { get; set; }
        /// <summary>
        /// Campo donde se muestra el resultado del calculo
        /// </summary>
        public string v_TargetFieldOfCalculateId { get; set; }
        public string v_Formula { get; set; }
        public string v_FormulaChild { get; set; }

        public string v_SourceFieldToCalculateJoin { get; set; }

        public int i_IsDeleted { get; set; }
        public string v_CreationUser { get; set; }
        public string v_UpdateUser { get; set; }
        public DateTime? d_CreationDate { get; set; }
        public DateTime? d_UpdateDate { get; set; }

        public List<ComponentFieldValues> Values { get; set; }
        public List<KeyValueDTO> ComboValues { get; set; }

        public List<TargetFieldOfCalculate> TargetFieldOfCalculateId { get; set; }
        public List<Formulate> Formula { get; set; }

        public int? i_RecordStatus { get; set; }
        public int? i_RecordType { get; set; }

        public string v_ComponentName { get; set; }

        public int? i_NroDecimales { get; set; }
        public int? i_ReadOnly { get; set; }
        public int? i_Enabled { get; set; }
    }

    public class ComponentFieldValues
    {
        public string v_ComponentFieldValuesId { get; set; }
        public string v_ComponentFieldsId { get; set; }
        public string v_AnalyzingValue1 { get; set; }
        public string v_AnalyzingValue2 { get; set; }
        public int i_OperatorId { get; set; }
        public string v_Recommendation { get; set; }
        public int i_Cie10Id { get; set; }
        public string v_Restriction { get; set; }
        public string v_LegalStandard { get; set; }

        public int? i_IsAnormal { get; set; }
        public int? i_ValidationMonths { get; set; }
        public string v_ComponentId { get; set; }

        //public int i_IsDeleted { get; set; }
        //public string v_CreationUser { get; set; }
        //public string v_UpdateUser { get; set; }
        //public DateTime? d_CreationDate { get; set; }
        //public DateTime? d_UpdateDate { get; set; }

        public string v_DiseasesId { get; set; }
        public string v_DiseasesName { get; set; }  // diagnostico
        public string v_CIE10 { get; set; }
        public List<RecomendationList> Recomendations { get; set; }
        public List<RestrictionList> Restrictions { get; set; }
        public int? i_GenderId { get; set; }

    }

    public class TargetFieldOfCalculate
    {
        public string v_TargetFieldOfCalculateId { get; set; }
    }

    public class Formulate
    {
        public string v_Formula { get; set; }
        public string v_TargetFieldOfCalculateId { get; set; }
    }

    public class RecomendationList
    {
        public string v_RecommendationId { get; set; }
        public string v_ServiceId { get; set; }
        public string v_DiagnosticRepositoryId { get; set; }
        public string v_ComponentId { get; set; }
        public string v_MasterRecommendationId { get; set; }

        public string v_MasterRecommendationRestrictionId { get; set; }

        public int? i_RecordStatus { get; set; }
        public int? i_RecordType { get; set; }

        public int? i_IsDeleted { get; set; }
        public string v_CreationUser { get; set; }
        public string v_UpdateUser { get; set; }
        public DateTime? d_CreationDate { get; set; }
        public DateTime? d_UpdateDate { get; set; }

        //

        public string v_ComponentFieldValuesRecommendationId { get; set; }
        public string v_ComponentFieldValuesId { get; set; }

        //

        public string v_RecommendationName { get; set; }

        public int i_Item { get; set; }

        public string v_DiseasesId { get; set; }

    }

    public class RestrictionList
    {
        public string v_RestrictionId { get; set; }
        public string v_RestrictionByDiagnosticId { get; set; }
        public string v_DiagnosticRepositoryId { get; set; }
        public string v_ServiceId { get; set; }
        public string v_ComponentId { get; set; }
        public string v_MasterRestrictionId { get; set; }

        public int? i_RecordStatus { get; set; }
        public int? i_RecordType { get; set; }

        public int? i_ItemId { get; set; }

        public int? i_IsDeleted { get; set; }
        public string v_CreationUser { get; set; }
        public string v_UpdateUser { get; set; }
        public DateTime? d_CreationDate { get; set; }
        public DateTime? d_UpdateDate { get; set; }

        public DateTime? d_StartDateRestriction { get; set; }
        public DateTime? d_EndDateRestriction { get; set; }

        //
        public string v_ComponentFieldValuesRestrictionId { get; set; }
        public string v_ComponentFieldValuesId { get; set; }
        //

        public string v_RestrictionName { get; set; }
        public int i_Item { get; set; }

        public string v_DiseasesId { get; set; }

    }


    public class ServiceComponentFieldsList
    {
        public string v_ServiceComponentFieldsId { get; set; }
        public string v_ComponentFieldsId { get; set; }
        public string Frontend { get; set; }
        public string v_ServiceComponentId { get; set; }
        public int? i_IsDeleted { get; set; }
        public int? i_InsertUserId { get; set; }
        public DateTime? d_InsertDate { get; set; }
        public int? i_UpdateUserId { get; set; }
        public DateTime? d_UpdateDate { get; set; }
        public int? i_HasAutomaticDxId { get; set; }
        public string v_PersonId { get; set; }
        public List<ServiceComponentFieldValuesList> ServiceComponentFieldValues { get; set; }

        // Datos de valores de campos tabla [ServiceComponentFieldValues]
        public string v_ServiceComponentFieldValuesId { get; set; }
        public string v_ComponentFieldValuesId { get; set; }
        public string v_Value1 { get; set; }
        public string v_ComponentFielName { get; set; }  // v_TextLabel

        // Conclusiones + Diagnosticos
        public string v_ConclusionAndDiagnostic { get; set; }

        public string v_Value1Name { get; set; }
        public int i_GroupId { get; set; }

        public string v_MeasurementUnitName { get; set; }

        public string v_ComponentId { get; set; }

        public string v_ServiceId { get; set; }
        public string v_Paciente { get; set; }
        public DateTime? d_ServiceDate { get; set; }
        public string v_ComponentFieldId { get; set; }
    }

    public class ServiceComponentFieldValuesList
    {
        public string v_ServiceComponentFieldValuesId { get; set; }
        public string v_ComponentFieldValuesId { get; set; }
        public string v_ServiceComponentFieldsId { get; set; }
        public string v_Value1 { get; set; }
        public string v_Value2 { get; set; }
        public int? i_Index { get; set; }
        public int? i_Value1 { get; set; }
        public int? i_IsDeleted { get; set; }
        public int? i_InsertUserId { get; set; }
        public DateTime? d_InsertDate { get; set; }
        public int? i_UpdateUserId { get; set; }
        public DateTime? d_UpdateDate { get; set; }

        public string v_ComponentFieldId { get; set; }
        public string v_ServiceComponentId { get; set; }
        public string v_ComponentFielName { get; set; }


        public string v_Value1Name { get; set; }

        public int i_GroupId { get; set; }
        public int? i_CategoryId { get; set; }

        public string v_UnidadMedida { get; set; }
        public string v_ComponentId { get; set; }

        public byte[] fotoTipo { get; set; }

        public string v_ServicioId { get; set; }
        public DateTime? d_ServiceDate { get; set; }

    }
}
