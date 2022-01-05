using FondoEstimulo.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FondoEstimulo.Models.Agentes
{
    [Table("Anticipos")]
    public class Anticipo
    {
        #region Public Properties

        /// <summary>
        /// Identificación interna de del anticipo.
        /// </summary>
        public int AnticipoID { get; set; }

        /// <summary>
        /// Fecha de generación del anticipo.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Identificador del proceso mensual al que pertenece, si aplica.
        /// </summary>
        public int? ProcesoID { get; set; }

        /// <summary>
        /// Mes del período fiscal.
        /// </summary>
        [Display(Name = "Período")]
        [Required(ErrorMessage = "Campo Periodo es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime Periodo { get; set; }

        /// <summary>
        /// Apartado y grupo del escalafón al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Escalafón")]
        [MaxLength(50)]
        public string Escalafon { get; set; }

        /// <summary>
        /// Valor monetario del Sueldo básico al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Sueldo Básico")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SueldoBasico { get; set; }

        /// <summary>
        /// Valor monetario del concepto Subtotal básico.
        /// </summary>
        [Display(Name = "Subtotal básico")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SubtotalBasico { get; set; }

        /// <summary>
        /// Diferencia de años entre periodo fiscal e inicio de actividades, o fecha de retiro, en caso de que aplique.
        /// </summary>
        [Display(Name = "Antigüedad en años")]
        public int AñosAntiguedad { get; set; }

        /// <summary>
        /// Valor monetario del concepto Antigüedad.
        /// Se calcula: ((SueldoBasico + ComplementoBasico + ReparacionHistorica) * 2 / 100) * AñosAntiguedad;
        /// </summary>
        [Display(Name = "Antigüedad")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Antiguedad { get; set; }

        /// <summary>
        /// Coeficiente del concepto Incompatibilidad al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Coeficiente de Incompatibilidad")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal IncompatibilidadCoeficiente { get; set; }

        /// <summary>
        /// Valor monetario del concepto Incompatibilidad.
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Incompatibilidad { get; set; }

        /// <summary>
        /// Tipo de título.
        /// </summary>
        [Display(Name = "Tipo de título")]
        [MaxLength(50)]
        public string TituloTipo { get; set; }

        /// <summary>
        /// Coeficiente del concepto Título, al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Coeficiente del tipo de título")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal TituloTipoCoeficiente { get; set; }

        /// <summary>
        /// Apartado y grupo del escalafón al que se aplica la bonificación por titulo al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Escalafón del título")]
        [MaxLength(50)]
        public string TituloEscalafon { get; set; }

        /// <summary>
        /// Valor monetario del Sueldo básico del escalafón al que se aplica la bonificación por titulo, al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Título escalafón sueldo básico")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal TituloEscalafonSueldoBasico { get; set; }

        /// <summary>
        /// Valor monetario del concepto Título.
        /// </summary>
        [Display(Name = "Título")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Titulo { get; set; }

        /// <summary>
        /// Valor monetario de la suma de los conceptos.
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Valor monetario del concepto SAC.
        /// Se calcula: Subtotal / 12.
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SAC { get; set; }

        /// <summary>
        /// Valor monetario de la suma de todos los conceptos + SAC.
        /// Se calcula: Subtotal + SAC.
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Total { get; set; }

        /// <summary>
        /// Coeficiente del concepto Fondo de Estimulo.
        /// </summary>
        [Display(Name = "Coeficiente de Fondo Estimulo")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal FondoEstimuloCoeficiente { get; set; }

        /// <summary>
        /// Valor monetario del concepto Fondo de Estimulo.
        /// Se calcula: Total * FondoEstimuloCoeficiente.
        /// </summary>
        [Display(Name = "Fondo Estimulo")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal FondoEstimulo { get; set; }

        /// <summary>
        /// Proceso mensual del cual se generó, si aplica.
        /// </summary>
        public Proceso Proceso { get; set; }

        #endregion Public Properties
    }
}