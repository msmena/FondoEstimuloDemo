using FondoEstimulo.Data;
using FondoEstimulo.Models.Agentes.Comun;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages
{
    public class IndexModel : PageModel
    {
        #region Private Fields

        private readonly FondoEstimuloContext _context;

        #endregion Private Fields

        #region Public Constructors

        public IndexModel(FondoEstimuloContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Properties

        public IList<Agente> Agentes1 { get; set; }
        public IList<Agente> Agentes2 { get; set; }
        public string TituloListado1 { get; set; }
        public string TituloListado2 { get; set; }
        public DateTime Fecha1 { get; set; }
        public DateTime Fecha2 { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task OnGetAsync()
        {
            Fecha1 = new(DateTime.Today.Year, DateTime.Today.Month, 1);
            Fecha2 = Fecha1.AddMonths(1);

            const string titulo = "Agentes que cumplen antigüedad en el mes {0} del año {1}";
            TituloListado1 = string.Format(titulo, Fecha1.Month, Fecha1.Year);
            TituloListado2 = string.Format(titulo, Fecha2.Month, Fecha2.Year);

            var agentes = _context.AgentesComun
                .AsQueryable()
                .Where(s => s.Tipo == Models.Parametro.AgenteTipo.Activo || s.Tipo == Models.Parametro.AgenteTipo.Adscripto)
                .Include(s => s.Escalafon)
                .AsNoTracking()
                .OrderBy(s => s.Nombre);

            Agentes1 = await agentes.ToListAsync();
            Agentes2 = Agentes1;
            Agentes1 = Agentes1.Where(s => s.InicioActividades.Month == Fecha1.Month).ToList();
            Agentes2 = Agentes2.Where(s => s.InicioActividades.Month == Fecha2.Month).ToList();
        }

        #endregion Public Methods
    }
}