using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FondoEstimulo.Models
{
    public class BonificacionTitulo
    {
        #region Public Properties

        /// <summary>
        /// Identificación interna del tipo de bonificación por título.
        /// </summary>
        public int BonificacionTituloID { get; set; }

        /// <summary>
        /// Descripción textual del tipo de bonificación por título.
        /// </summary>
        //[MaxLength(150)] -- Deshabilitado largo de texto en base PostgreSQL
        public string Descripcion { get; set; }

        /// <summary>
        /// Coeficiente del tipo de bonificación por título. Se visualiza como porcentaje.
        /// </summary>
        [Required(ErrorMessage = "Campo Valor es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(6, 4)")]
        public decimal Valor { get; set; }

        #endregion Public Properties
    }
}