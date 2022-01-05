using System;
using System.ComponentModel.DataAnnotations;

namespace FondoEstimulo.Models.Aplicacion
{
    public class InicioSesion
    {
        #region Public Properties

        /// <summary>
        /// Identificador interno del inicio de sesión.
        /// </summary>
        public int InicioSesionID { get; set; }

        /// <summary>
        /// Nombre de usuario.
        /// </summary>
        //[MaxLength(150)] -- Deshabilitado largo de texto en base PostgreSQL
        public string UserName { get; set; }

        /// <summary>
        /// Fecha en que el que se realizó el inicio.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Direccón IP del usuario.
        /// </summary>
        //[MaxLength(20)] -- Deshabilitado largo de texto en base PostgreSQL
        public string IP { get; set; }

        /// <summary>
        /// Dirección MAP del usuario.
        /// </summary>
        public string MAC { get; set; }

        #endregion Public Properties
    }
}