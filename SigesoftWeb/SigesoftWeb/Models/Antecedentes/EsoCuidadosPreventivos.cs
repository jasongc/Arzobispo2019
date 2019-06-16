﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SigesoftWeb.Models.Antecedentes
{
    public class EsoCuidadosPreventivosFechas
    {
        public DateTime FechaServicio { get; set; }
        public List<EsoCuidadosPreventivos> Listado { get; set; }
    }
    public class EsoCuidadosPreventivos
    {
        public string Nombre { get; set; }
        public int ParameterId { get; set; }
        public int GrupoId { get; set; }
        public bool Valor { get; set; }
        public List<EsoCuidadosPreventivos> Hijos { get; set; }
        public EsoCuidadosPreventivosComentarios DataComentario { get; set; }
    }
    public class EsoCuidadosPreventivosComentarios
    {
        public string Comentario { get; set; }
        public int GrupoId { get; set; }
        public int ParametroId { get; set; }
    }
}