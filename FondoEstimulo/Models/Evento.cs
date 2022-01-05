using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FondoEstimulo.Models
{
    public class Evento
    {
        #region Public Properties

        /// <summary>
        /// Identificador único del registro.
        /// </summary>
        public int EventoID { get; set; }

        /// <summary>
        /// Fecha y hora del evento.
        /// </summary>
        public DateTime Fecha { get; set; } = DateTime.Now;

        /// <summary>
        /// Usuario responsable del evento.
        /// </summary>
        //[MaxLength(450)] -- Deshabilitado largo de texto en base PostgreSQL
        public string UsuarioID { get; set; }

        /// <summary>
        /// Tipo de evento.
        /// </summary>
        public Parametro.EventoAccion Accion { get; set; }

        /// <summary>
        /// Tipo de origen.
        /// </summary>
        public Parametro.EventoOrigen Origen { get; set; }

        /// <summary>
        /// Identificador del registro al que se refiere el evento.
        /// </summary>
        //[MaxLength(450)] -- Deshabilitado largo de texto en base PostgreSQL
        public string RegistroID { get; set; }

        /// <summary>
        /// Texto JSon que contiene los datos del registro antes del evento.
        /// </summary>
        //[MaxLength(4000)] -- Deshabilitado largo de texto en base PostgreSQL
        public string Detalle { get; set; }

        /// <summary>
        /// Usuario responsable del evento.
        /// </summary>
        public Aplicacion.Usuario Usuario { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void Serializar(object objeto)
        {
            if (Accion != Parametro.EventoAccion.Creacion)
                Detalle = JsonConvert.SerializeObject(objeto);
        }

        #endregion Public Methods
    }
}