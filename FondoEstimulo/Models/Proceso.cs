using System;
using System.ComponentModel.DataAnnotations;

namespace FondoEstimulo.Models
{
    public class Proceso
    {
        #region Public Properties

        /// <summary>
        /// Identificador interno del proceso mensual.
        /// </summary>
        public int ProcesoID { get; set; }

        /// <summary>
        /// Fecha de creación del proceso.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Período mensual del proceso.
        /// </summary>
        [Display(Name = "Período")]
        [Required(ErrorMessage = "Campo Periodo es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime Periodo { get; set; }

        /// <summary>
        /// Cantidad de anticipos creados en el proceso.
        /// </summary>
        public int Registros { get; set; }

        #endregion Public Properties
    }
}