﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Service
{
    [Table("service")]
    public class ServiceDto
    {
        [Key]
        public string v_ServiceId { get; set; }

        public string v_ProtocolId { get; set; }
        public string v_PersonId { get; set; }
        public int? i_MasterServiceId { get; set; }
        public int? i_ServiceStatusId { get; set; }
        public string v_Motive { get; set; }
        public int? i_AptitudeStatusId { get; set; }
        public DateTime? d_ServiceDate { get; set; }
        public DateTime? d_GlobalExpirationDate { get; set; }
        public DateTime? d_ObsExpirationDate { get; set; }
        public int? i_FlagAgentId { get; set; }
        public string v_OrganizationId { get; set; }
        public string v_LocationId { get; set; }
        public string v_MainSymptom { get; set; }
        public int? i_TimeOfDisease { get; set; }
        public int? i_TimeOfDiseaseTypeId { get; set; }
        public string v_Story { get; set; }
        public int? i_DreamId { get; set; }
        public int? i_UrineId { get; set; }
        public int? i_DepositionId { get; set; }
        public int? i_AppetiteId { get; set; }
        public int? i_ThirstId { get; set; }
        public DateTime? d_Fur { get; set; }
        public string v_CatemenialRegime { get; set; }
        public int? i_MacId { get; set; }
        public int? i_IsNewControl { get; set; }
        public int? i_HasMedicalBreakId { get; set; }
        public DateTime? d_MedicalBreakStartDate { get; set; }
        public DateTime? d_MedicalBreakEndDate { get; set; }
        public string v_GeneralRecomendations { get; set; }
        public int? i_DestinationMedicationId { get; set; }
        public int? i_TransportMedicationId { get; set; }
        public DateTime? d_StartDateRestriction { get; set; }
        public DateTime? d_EndDateRestriction { get; set; }
        public int? i_HasRestrictionId { get; set; }
        public int? i_HasSymptomId { get; set; }

        public DateTime? d_UpdateDate { get; set; }

        public DateTime? d_NextAppointment { get; set; }

        public int? i_IsDeleted { get; set; }
        public int? i_InsertUserId { get; set; }
        public DateTime? d_InsertDate { get; set; }
        public int? i_UpdateUserId { get; set; }

        public int? i_SendToTracking { get; set; }
        public int? i_InsertUserMedicalAnalystId { get; set; }
        public int? i_UpdateUserMedicalAnalystId { get; set; }
        public DateTime? d_InsertDateMedicalAnalyst { get; set; }
        public DateTime? d_UpdateDateMedicalAnalyst { get; set; }
        public int? i_InsertUserOccupationalMedicalId { get; set; }
        public int? i_UpdateUserOccupationalMedicaltId { get; set; }
        public DateTime? d_InsertDateOccupationalMedical { get; set; }
        public DateTime? d_UpdateDateOccupationalMedical { get; set; }
        public int? i_HazinterconsultationId { get; set; }
        public string v_Gestapara { get; set; }
        public string v_Menarquia { get; set; }
        public DateTime? d_PAP { get; set; }
        public DateTime? d_Mamografia { get; set; }
        public string v_CiruGine { get; set; }
        public string v_Findings { get; set; }
        public int? i_StatusLiquidation { get; set; }
        public int? i_ServiceTypeOfInsurance { get; set; }
        public int? i_ModalityOfInsurance { get; set; }
        public int? i_IsFac { get; set; }
        public int? i_InicioEnf { get; set; }
        public int? i_CursoEnf { get; set; }
        public int? i_Evolucion { get; set; }
        public string v_ExaAuxResult { get; set; }
        public string v_ObsStatusService { get; set; }
        public DateTime? d_FechaEntrega { get; set; }
        public string v_AreaId { get; set; }
        public string v_FechaUltimoPAP { get; set; }
        public string v_ResultadosPAP { get; set; }
        public string v_FechaUltimaMamo { get; set; }
        public string v_ResultadoMamo { get; set; }
        public float? r_Costo { get; set; }
        public int? i_EnvioCertificado { get; set; }
        public int? i_EnvioHistoria { get; set; }
        public string v_IdVentaCliente { get; set; }
        public string v_IdVentaAseguradora { get; set; }
        public string v_InicioVidaSexaul { get; set; }
        public string v_NroParejasActuales { get; set; }
        public string v_NroAbortos { get; set; }
        public string v_PrecisarCausas { get; set; }
        public int? i_MedicoTratanteId { get; set; }
        public int? i_IsFacMedico { get; set; }
        public string v_centrocosto { get; set; }
        public string v_NroLiquidacion { get; set; }
        public int? i_MedicoPagado { get; set; }
        //public int? i_StatusControl { get; set; }
        public int? i_IsControl { get; set; }
        public int? i_IsRevisedHistoryId { get; set; }
        public int? i_StatusVigilanciaId { get; set; }
        public string v_NroCartaSolicitud { get; set; }
        public List<DiagnosticRepositoryDto> diagnosticrepository { get; set; }
    }
}
