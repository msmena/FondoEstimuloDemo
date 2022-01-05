using FondoEstimulo.Data;
using FondoEstimulo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Procesos
{
    public class IndexModel : PageModel
    {
        #region Private Fields

        private readonly FondoEstimuloContext _context;
        private readonly IConfiguration _configuration;

        #endregion Private Fields

        #region Public Constructors

        public IndexModel(FondoEstimuloContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #endregion Public Constructors

        #region Public Properties

        public Paginado<Proceso> Procesos { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OrdenCampo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? IndicePagina { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FPeriodoDesde { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FPeriodoHasta { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task OnGetAsync(int? periodoDesdeAno, int? periodoDesdeMes, int? periodoHastaAno, int? periodoHastaMes)
        {
            var tamañoPagina = _configuration.GetValue("PageSize", 10);

            var procesosQuery = _context.Procesos.AsQueryable().AsNoTracking();

            if (FPeriodoDesde.HasValue || (periodoDesdeAno.HasValue && periodoDesdeMes.HasValue))
            {
                if (!FPeriodoDesde.HasValue)
                    FPeriodoDesde = new DateTime(periodoDesdeAno.Value, periodoDesdeMes.Value, 1);

                procesosQuery = procesosQuery.Where(e => e.Periodo >= FPeriodoDesde);
            }

            if (FPeriodoHasta.HasValue || periodoHastaAno.HasValue && periodoHastaMes.HasValue)
            {
                if (!FPeriodoHasta.HasValue)
                    FPeriodoHasta = new DateTime(periodoHastaAno.Value, periodoHastaMes.Value, 1);

                procesosQuery = procesosQuery.Where(e => e.Periodo <= FPeriodoHasta);
            }

            string ordenCampo = OrdenCampo;
            if (!String.IsNullOrWhiteSpace(ordenCampo))
            {
                string ordenDireccion = "asc";

                if (ordenCampo.Contains("desc"))
                {
                    ordenDireccion = "desc";
                    ordenCampo = ordenCampo.Substring(0, ordenCampo.Length - 5);
                }

                ordenCampo = ordenCampo.Replace("__", ".");
                procesosQuery = procesosQuery.OrderBy($"{ordenCampo} {ordenDireccion}");
            }
            else
            {
                procesosQuery = procesosQuery.OrderBy(s => s.Fecha);
            }

            Procesos = await Paginado<Proceso>.CreateAsync(procesosQuery, IndicePagina ?? 1, tamañoPagina);
        }

        #endregion Public Methods
    }
}