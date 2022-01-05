using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FondoEstimulo.Models
{
    public class Parametro
    {
        #region Public Enums

        /// <summary>
        /// Tipos de roles en el sistema.
        /// </summary>
        public enum Roles { Administrador, Registro };

        /// <summary>
        /// Tipos de acción en la tabla Eventos.
        /// </summary>
        public enum EventoAccion { Creacion, Modificacion, Eliminacion };

        /// <summary>
        /// Tipo de origen en la tabla Eventos.
        /// </summary>
        public enum EventoOrigen { Agente, Anticipo, Escalafon, Parametro, Proceso, Usuario };

        /// <summary>
        /// Tipos de agente.
        /// </summary>
        public enum AgenteTipo { Activo, Retirado, Adscripto, Jubilado }

        #endregion Public Enums

        #region Public Properties

        /// <summary>
        /// Apartado A del Escalafón General.
        /// </summary>
        public static string EscalafonApartadoA { get { return "A"; } }

        /// <summary>
        /// Apartado B del Escalafón General.
        /// </summary>
        public static string EscalafonApartadoB { get { return "B"; } }

        /// <summary>
        /// Apartado C del Escalafón General.
        /// </summary>
        public static string EscalafonApartadoC { get { return "C"; } }

        /// <summary>
        /// Apartado D del Escalafón General.
        /// </summary>
        public static string EscalafonApartadoD { get { return "D"; } }

        /// <summary>
        /// Apartado E del Escalafón General.
        /// </summary>
        public static string EscalafonApartadoE { get { return "E"; } }

        /// <summary>
        /// Apartado F del Escalafón General.
        /// </summary>
        public static string EscalafonApartadoF { get { return "F"; } }

        /// <summary>
        /// Apartado BOCEP del Escalafón General.
        /// </summary>
        public static string EscalafonApartadoBOCEP { get { return "BOCEP"; } }

        /// <summary>
        /// Apartado Gabinete del Escalafón General.
        /// </summary>
        public static string EscalafonApartadoGabinete { get { return "Gabinete"; } }

        /// <summary>
        /// Bonificación por titulo universitario, aplicando el 31%.
        /// </summary>
        public static string BTituloUniversitario31 { get { return "Universitario31"; } }

        /// <summary>
        /// Bonificación por titulo universitario, aplicando el 29%.
        /// </summary>
        public static string BTituloUniversitario29 { get { return "Universitario29"; } }

        /// <summary>
        /// Bonificación por titulo universitario, aplicando el 27%.
        /// </summary>
        public static string BTituloUniversitario27 { get { return "Universitario27"; } }

        /// <summary>
        /// Bonificación por titulo universitario, aplicando el 25%.
        /// </summary>
        public static string BTituloUniversitario25 { get { return "Universitario25"; } }

        /// <summary>
        /// Bonificación por titulo universitario, aplicando el 22%.
        /// </summary>
        public static string BTituloUniversitario22 { get { return "Universitario22"; } }

        /// <summary>
        /// Bonificación por titulo universitario, aplicando el 20,5%.
        /// </summary>
        public static string BTituloUniversitario2050 { get { return "Universitario2050"; } }

        /// <summary>
        /// Bonificación por titulo secundario.
        /// </summary>
        public static string BTituloSecundario { get { return "Secundario"; } }

        /// <summary>
        /// Bonificación por titulo: sin titulo vigente.
        /// </summary>
        public static string BTituloSinTitulo { get { return "Sin Titulo"; } }

        /// <summary>
        /// Identificador interno de los parámetros. Por el momento existe único registro.
        /// </summary>
        public int ParametroID { get; set; }

        /// <summary>
        /// Coeficiente del concepto Incompatibilidad. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Incompatibilidad")]
        [Required(ErrorMessage = "Campo Incompatibilidad es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(6, 4)")]
        public decimal Incompatibilidad { get; set; }

        /// <summary>
        /// Coeficiente del concepto Bonificación Ley 6655. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Bonificación Ley 6655")]
        [Required(ErrorMessage = "Campo Bonif. Ley 6655 es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(6, 4)")]
        public decimal Ley6655 { get; set; }

        /// <summary>
        /// Valor del concepto Riesgo Caja (Monto Base). Se visualiza como monto monetario.
        /// </summary>
        [Display(Name = "Riesgo Caja (Monto Base)")]
        [Required(ErrorMessage = "Campo Riesgo Caja es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal RiesgoCajaBase { get; set; }

        /// <summary>
        /// Valor monetario del concepto Sum Fija Rem.
        /// </summary>
        [Display(Name = "Suma fija Rem")]
        [Required(ErrorMessage = "Campo Suma fija Rem es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SumaFijaRem { get; set; }

        #endregion Public Properties
    }
}