using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FondoEstimulo.Models.Agentes.BOCEP
{
    [Table("AgentesBOCEP")]
    public class Agente : Agentes.Agente
    {
        #region Public Properties

        /// <summary>
        /// Coeficiente del concepto dedicación. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Dedicación")]
        [Required(ErrorMessage = "Campo Dedicación es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal Dedicacion { get; set; }

        /// <summary>
        /// Coeficiente del concepto Asignación complementaria. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Asignación personal por adecuación escalafonaria")]
        [Required(ErrorMessage = "Asignación complementaria es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal AsignacionComplementaria { get; set; }

        /// <summary>
        /// Valor del concepto Suma fija. Se visualiza como monto monetario.
        /// </summary>
        [Display(Name = "Suma fija")]
        [Required(ErrorMessage = "Campo Suma fija es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SumaFijo { get; set; }

        #endregion Public Properties
    }
}