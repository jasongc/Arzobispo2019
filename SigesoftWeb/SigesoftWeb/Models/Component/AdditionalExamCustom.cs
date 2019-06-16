﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SigesoftWeb.Models.Component
{
    public class AdditionalExamCustom
    {

        public string AdditionalExamId { get; set; }
        public string ServiceId { get; set; }
        public string PersonId { get; set; }
        public string ProtocolId { get; set; }
        public string ComponentId { get; set; }
        public string Commentary { get; set; }
        public int IsProcessed { get; set; }
        public int IsNewService { get; set; }

    }

    public class AdditionalExamUpdate
    {

        public string v_ComponentName { get; set; }
        public string v_PacientName { get; set; }
        public string v_ServiceId { get; set; }
        public string v_ComponentId { get; set; }
        public string v_AdditionalExamId { get; set; }
        public int i_IsProcessed { get; set; }
    }
    public class AdditionalExamCreate
    {
        public int IsNewService { get; set; }
        public string ComponentId { get; set; }
        public string PersonId { get; set; }
        public float Price { get; set; }
        public string ServiceId { get; set; }
        public int MedicoTratante { get; set; }

    }
}