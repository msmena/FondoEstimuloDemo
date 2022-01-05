using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FondoEstimulo.Models.Agentes.Comun
{
    [Table("AgentesComunes")]
    public class Agente : Agentes.Agente
    {
        #region Public Properties

        /// <summary>
        /// Tipo de agente vigente.
        /// </summary>
        [Required(ErrorMessage = "Campo Tipo es obligatorio.")]
        public Parametro.AgenteTipo Tipo { get; set; }

        /// <summary>
        /// Coeficiente del concepto dedicación. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Dedicación")]
        [Required(ErrorMessage = "Campo Dedicación es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal Dedicacion { get; set; }

        /// <summary>
        /// Coeficiente del concepto Riesgo de Caja, se aplica sobre monto base en parámetro. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Riesgo de Caja")]
        [Required(ErrorMessage = "Campo Riesgo de Caja es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal RiesgoCaja { get; set; }

        /// <summary>
        /// Coeficiente del concepto Asignación por Reparación Histórica. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Asignación por Reparación Histórica")]
        [Required(ErrorMessage = "Campo Asignación por Reparación Histórica es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal ReparacionHistorica { get; set; }

        /// <summary>
        /// Valor del concepto Asignación personal por adecuación escalafonaria. Se visualiza como monto monetario.
        /// </summary>
        [Display(Name = "Asignación personal por adecuación escalafonaria")]
        [Required(ErrorMessage = "Asignación personal por adecuación escalafonaria es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal AdecuacionEscalafonaria { get; set; }

        /// <summary>
        /// Valor del campo Subrogancia. Se visualiza como monto monetario.
        /// </summary>
        [Required(ErrorMessage = "Campo Subrogancia es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Subrogancia { get; set; }

        /// <summary>
        /// Valor del concepto Fondo fija. Se visualiza como monto monetario.
        /// </summary>
        [Display(Name = "Fondo fija")]
        [Required(ErrorMessage = "Campo Fondo fija es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal FondoFijo { get; set; }

        /// <summary>
        /// Valor que indica si aplica el concepto Bonificación Ley 6655.
        /// </summary>
        [Display(Name = "Bonificación Ley 6655")]
        public bool Ley6655 { get; set; }

        /// <summary>
        /// Coeficiente del concepto Sup.Rep. No Bonif. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Sup.Rep. No Bonif.")]
        [Required(ErrorMessage = "Campo S.R. No Bonif. es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal SRNoBonificable { get; set; }

        /// <summary>
        /// Fecha de retiro. Se calcula antigüedad hasta esta fecha, cuando el agente es retirado.
        /// </summary>
        [Display(Name = "Fecha de Retiro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime? Retiro { get; set; }

        /// <summary>
        /// Fecha de jubilación. Periodo a partir del cual no se calculan liquidaciones.
        /// </summary>
        [Display(Name = "Fecha de Jubilación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime? Jubilacion { get; set; }

        #endregion Public Properties
    }
}