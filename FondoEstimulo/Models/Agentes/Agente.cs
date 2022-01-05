using FondoEstimulo.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FondoEstimulo.Models.Agentes
{
    [Table("Agentes")]
    public class Agente
    {
        #region Private Fields

        private string nombre;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Identificación interna de agente.
        /// </summary>
        public int AgenteID { get; set; }

        /// <summary>
        /// Identificación interna de agente en el sistema PON.
        /// </summary>
        [Display(Name = "ID Externo")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public int AgenteIDExterno { get; set; }

        /// <summary>
        /// Bonificacón por titulo.
        /// </summary>
        [Display(Name = "Titulo")]
        public BonificacionTitulo BonificacionTitulo { get; set; }

        /// <summary>
        /// Escala en el que se aplica la bonificación del título.
        /// </summary>
        [Display(Name = "Escalafón del Titulo")]
        public Escalafon BonificacionTituloEscalafon { get; set; }

        /// <summary>
        /// Escala del escalafón general en el que se aplica la bonificación de título.
        /// </summary>
        [Display(Name = "Escalafón del titulo")]
        public int? BonificacionTituloEscalafonID { get; set; }

        /// <summary>
        /// Tipo de bonificación por título.
        /// </summary>
        [Display(Name = "Titulo")]
        [Required(ErrorMessage = "Campo Titulo es obligatorio.")]
        public int BonificacionTituloID { get; set; }

        /// <summary>
        /// Documento nacional de identidad. Identificador único por agente.
        /// </summary>
        [Display(Name = "D.N.I.")]
        [Required(ErrorMessage = "Campo DNI es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Valor debe ser mayor a {1}.")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public int DNI { get; set; }

        /// <summary>
        /// Escala dentro del escalafón.
        /// </summary>
        public Escalafon Escalafon { get; set; }

        /// <summary>
        /// Escala dentro del escalafón.
        /// </summary>
        [Display(Name = "Escalafón")]
        public int EscalafonID { get; set; }

        /// <summary>
        /// Coeficiente del concepto Fondo Estimulo Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Fondo estimulo")]
        [Required(ErrorMessage = "Campo Fondo estimulo es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal FondoEstimulo { get; set; }

        /// <summary>
        /// Valor que indica si aplica el concepto Incompatibilidad.
        /// </summary>
        public bool Incompatibilidad { get; set; }

        /// <summary>
        /// Fecha de inicio de actividades en la entidad.
        /// </summary>
        [Display(Name = "Inicio de Actividades")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [Required(ErrorMessage = "Campo Inicio de Antividades es obligatorio.")]
        [DataType(DataType.Date)]
        public DateTime InicioActividades { get; set; }

        /// <summary>
        /// Nombre completo.
        /// </summary>
        [Required(ErrorMessage = "Campo Nombre es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z'ñ''Ñ''á''Á''é''í''Í''ó''Ó''ú''Ú'\s]*$", ErrorMessage = "No están permitidos caracteres especiales ni numéricos.")]
        //[MaxLength(150)] -- Deshabilitado largo de texto en base PostgreSQL
        public string Nombre
        {
            get
            {
                return nombre;
            }
            set
            {
                nombre = value;
                NombreNormalizado = value.ToUpper();
            }
        }

        /// <summary>
        /// Mismo valor que el campo Nombre pero en mayúsculas. Útil para realizar búsquedas rápidas.
        /// </summary>
        //[MaxLength(150)] -- Deshabilitado largo de texto en base PostgreSQL
        public string NombreNormalizado { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void AsignarCargo(int funcion, FondoEstimuloContext context)
        {
            switch (funcion)
            {
                case 104: break; // Administrador general ATP
                case 105: break; // Subadministrador general ATP
                case 838: break; // Personal de Gabinete;
                case 839: break; // Personal transitorio - Categoria Administrativo y Tecnico
                case 842: break; // Jornalizado categoria oficial
                case 844: break; // Jornalizado categoria ayudante
                case 1004: break; // Administrativo Transitorio E
                case 1013:
                    Escalafon = context.EscalafonGeneral // Director 2
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoA && e.Grupo == 02); break;
                case 1014:
                    Escalafon = context.EscalafonGeneral // Director 1
          .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoA && e.Grupo == 01); break;
                case 1015:
                    Escalafon = context.EscalafonGeneral // Jefe departamento
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoB && e.Grupo == 01); break;
                case 1016:
                    Escalafon = context.EscalafonGeneral // Profesional 7
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 07); break;
                case 1017:
                    Escalafon = context.EscalafonGeneral // Profesional 6
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 06); break;
                case 1018:
                    Escalafon = context.EscalafonGeneral // Profesional 5
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 05); break;
                case 1019:
                    Escalafon = context.EscalafonGeneral // Profesional 4
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 04); break;
                case 1020:
                    Escalafon = context.EscalafonGeneral // Profesional 3
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 03); break;
                case 1021:
                    Escalafon = context.EscalafonGeneral // Profesional 2
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 02); break;
                case 1022:
                    Escalafon = context.EscalafonGeneral // Profesional 1
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 01); break;
                case 1023:
                    Escalafon = context.EscalafonGeneral // Administrativo 7
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 07); break;
                case 1024:
                    Escalafon = context.EscalafonGeneral // Administrativo 6
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 06); break;
                case 1025:
                    Escalafon = context.EscalafonGeneral // Administrativo 5
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 05); break;
                case 1026:
                    Escalafon = context.EscalafonGeneral // Administrativo 4
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 04); break;
                case 1027:
                    Escalafon = context.EscalafonGeneral // Administrativo 3
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 03); break;
                case 1028:
                    Escalafon = context.EscalafonGeneral // Administrativo 2
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 02); break;
                case 1029:
                    Escalafon = context.EscalafonGeneral // Administrativo 1
          .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 01); break;
                case 1030: break; // Obrero Maestranza 5
                case 1031: break; // Obrero Maestranza 4
                case 1032: break; // Obrero Maestranza 3
                case 1033: break; // Obrero Maestranza 2
                case 1034: break; // Obrero Maestranza 1
                case 1035: break; // Servicios 5
                case 1036: break; // Servicios 4
                case 1037: break; // Servicios 3
                case 1038: break; // Servicios 2
                case 1039: break; // Servicios 1
                case 1040: break; // Director general
                case 1041: break; // Director
                case 1042:
                    Escalafon = context.EscalafonGeneral // Profesional 08
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 08); break;
                case 1043:
                    Escalafon = context.EscalafonGeneral // Profesional 09
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 09); break;
                case 1044:
                    Escalafon = context.EscalafonGeneral // Profesional 10
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 10); break;
                case 1045:
                    Escalafon = context.EscalafonGeneral // Administrativo 8
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 08); break;
                case 1046:
                    Escalafon = context.EscalafonGeneral // Administrativo 9
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 09); break;
                case 1047:
                    Escalafon = context.EscalafonGeneral // Administrativo 10
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 10); break;
                case 1048: break; // Obrero - Maestranza 6
                case 1049: break; // Obrero - Maestranza 7
                case 1050: break; // Obrero - Maestranza 8
                case 1051: break; // Servicios 6
                case 1052: break; // Servicios 7
                case 1053: break; // Servicios 8
                case 1054: break; // Servicios 9
                case 1055: break; // Obrero - Maestranza 9
                case 1056:
                    Escalafon = context.EscalafonGeneral // Administrativo 11
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoD && e.Grupo == 11); break;
                case 1057:
                    Escalafon = context.EscalafonGeneral // Profesional 11
                 .FirstOrDefault(e => e.Apartado == Parametro.EscalafonApartadoC && e.Grupo == 11); break;
            }

            if (Escalafon != null)
                EscalafonID = Escalafon.EscalafonID;
        }

        #endregion Public Methods
    }
}