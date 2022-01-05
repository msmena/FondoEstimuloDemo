using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FondoEstimulo.Models.Agentes.Funcionario
{
    [Table("AgentesFuncionarios")]
    public class Agente : Agentes.Agente
    {
        #region Public Properties

        /// <summary>
        /// Coeficiente del concepto Sup.Rep. No Bonif. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Sup. Rep. No Bonif.")]
        [Required(ErrorMessage = "Campo Sup. Rep. No Bonif. es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal SRNoBonificable { get; set; }

        /// <summary>
        /// Valor monetario del concepto Compensación Jerárquica.
        /// </summary>
        [Display(Name = "Compensación Jerárquica")]
        [Required(ErrorMessage = "Campo Compensanción Jerárquica es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal CompensancionJerarquica { get; set; }

        /// <summary>
        /// Valor monetario del concepto Adicional Remunerativo.
        /// </summary>
        [Display(Name = "Adicional Remunerativo")]
        [Required(ErrorMessage = "Campo Adicional Remunerativo es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal AdicionalRemunerativo { get; set; }

        #endregion Public Properties
    }
}