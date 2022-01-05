using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FondoEstimulo.Models
{
    [Table("EscalafonGeneral")]
    public class Escalafon
    {
        #region Public Properties

        /// <summary>
        /// Identificador interno del registro de escalafón.
        /// </summary>
        public int EscalafonID { get; set; }

        /// <summary>
        /// Identificador alfabético de la escala.
        /// </summary>
        [Required(ErrorMessage = "Campo Apartado es obligatorio.")]
        //[MaxLength(50)] -- Deshabilitado largo de texto en base PostgreSQL
        public string Apartado { get; set; }

        /// <summary>
        /// Identificador numérico de la escala.
        /// </summary>
        [Required(ErrorMessage = "Campo Grupo es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Valor debe ser mayor a {1}.")]
        public int Grupo { get; set; }

        /// <summary>
        /// Valor del campo Sueldo Básico. Se visualiza como monto monetario.
        /// </summary>
        [Display(Name = "Sueldo Básico")]
        [Required(ErrorMessage = "Campo Sueldo Básico es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SueldoBasico { get; set; }

        /// <summary>
        /// Valor del campo Complemento Básico. Se visualiza como monto monetario.
        /// </summary>
        [Display(Name = "Complemento básico")]
        [Required(ErrorMessage = "Campo Complemento básico es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal ComplementoBasico { get; set; }

        /// <summary>
        /// Valor del campo Suplemento Remunerativo. Se visualiza como monto monetario.
        /// </summary>
        [Display(Name = "Suplemento remunerativo")]
        [Required(ErrorMessage = "Campo Suplemento remunerativo es obligatorio.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SuplementoRemunerativo { get; set; }

        /// <summary>
        /// Texto alfanumérico que identifica la escala.
        /// </summary>
        [Display(Name = "Escalafón")]
        public string EscalafonApartadoGrupoIdentificador
        {
            get
            {
                return string.Concat(Apartado, Grupo.ToString());
            }
        }

        #endregion Public Properties
    }
}