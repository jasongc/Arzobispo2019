using BE.Antecedentes;
using BL.Common;
using DAL;
using DAL.Antecedentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BE.Common.Enumeratores;

namespace BL.Antecedentes
{
    public class EsoAntecedentesBL
    {
        public BoardEsoAntecedentes ObtenerEsoAntecedentesPorGrupoId(string PersonId)
        {
            BoardEsoAntecedentes _BoardEsoAntecedentes = new BoardEsoAntecedentes();
            DatabaseContext ctx = new DatabaseContext();
            DateTime BirthDatePacient = ctx.Person.Where(x => x.v_PersonId == PersonId).FirstOrDefault().d_Birthdate.Value;
            int Edad = new PacientBL().GetEdad(BirthDatePacient);
            int GrupoEtario = ObtenerIdGrupoEtarioDePaciente(Edad);
            int GrupoBase = 282; //Antecedentes
            int Grupo = int.Parse(GrupoBase.ToString() + GrupoEtario.ToString());

            var Actual = new EsoAntecedentesDal().ObtenerEsoAntecedentesPorGrupoId(Grupo, GrupoEtario, PersonId);

            int GrupoEtarioAnterior = 0;

            switch (GrupoEtario)
            {
                case 1:
                    {
                        GrupoEtarioAnterior = 2;
                        break;
                    }
                case 2:
                    {
                        GrupoEtarioAnterior = 4;
                        break;
                    }
                case 3:
                    {
                        GrupoEtarioAnterior = 1;
                        break;
                    }
                case 4:
                    {
                        GrupoEtarioAnterior = 4;
                        break;
                    }
                default:
                    {
                        GrupoEtarioAnterior = 0;
                        break;
                    }
            }

            var Anterior = new EsoAntecedentesDal().ObtenerEsoAntecedentesPorGrupoId(Grupo, GrupoEtarioAnterior, PersonId);

            _BoardEsoAntecedentes.AntecedenteActual = Actual;
            _BoardEsoAntecedentes.AntecedenteAnterior = Anterior;
            return _BoardEsoAntecedentes;
        }

        public List<EsoCuidadosPreventivosFechas> ObtenerFechasCuidadosPreventivos(string PersonId)
        {
            try
            {
                DatabaseContext ctx = new DatabaseContext();
                var objPacient = ctx.Person.Where(x => x.v_PersonId == PersonId).FirstOrDefault();
                int Edad = new PacientBL().GetEdad(objPacient.d_Birthdate.Value);
                int _GrupoEtario = ObtenerIdGrupoEtarioDePaciente(Edad);
                int GrupoBase = 0;
                switch (_GrupoEtario)
                {
                    case (int)GrupoEtario.Ninio:
                        {
                            GrupoBase = 292;
                            break;
                        }
                    case (int)GrupoEtario.Adolecente:
                        {
                            GrupoBase = 285;
                            break;
                        }
                    case (int)GrupoEtario.Adulto:
                        {
                            if (objPacient.i_SexTypeId == 1)
                            {
                                GrupoBase = 284;
                                break;
                            }
                            else
                            {
                                GrupoBase = 283;
                                break;
                            }
                        }
                    case (int)GrupoEtario.AdultoMayor:
                        {
                            GrupoBase = 286;
                            break;
                        }
                    default:
                        {
                            GrupoBase = 0;
                            break;
                        }
                }
                var data = new EsoAntecedentesDal().ObtenerFechasCuidadosPreventivos(PersonId);
                var Comentarios = new EsoAntecedentesDal().ObtenerComentariosCuidadosPreventivos(PersonId);
                foreach (var F in data)
                {
                    F.Listado = new EsoAntecedentesDal().ObtenerListadoCuidadosPreventivos(GrupoBase, PersonId, F.FechaServicio);
                    foreach (var obj in F.Listado)
                    {
                        var find = Comentarios.Find(x => x.GrupoId == obj.GrupoId && x.ParametroId == obj.ParameterId);
                        if (find != null)
                        {
                            obj.DataComentario = find;
                        }
                    }

                }



                
                return data;
            }
            catch (Exception e)
            {
                return new List<EsoCuidadosPreventivosFechas>();
            }
        }

        public int ObtenerIdGrupoEtarioDePaciente(int _edad)
        {
            try
            {
                if (_edad <= 12)
                {
                    return 4;
                }
                else if (13 <= _edad && _edad <= 17)
                {
                    return 2;
                }
                else if (18 <= _edad && _edad <= 64)
                {
                    return 1;
                }
                else
                {
                    return 3;
                }
            }
            catch (Exception e)
            {
                return 0;
            }

        }
    }
}
