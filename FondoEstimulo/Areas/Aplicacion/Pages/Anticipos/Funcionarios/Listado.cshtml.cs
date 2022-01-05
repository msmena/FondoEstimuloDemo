using FondoEstimulo.Data;
using FondoEstimulo.Models.Agentes.Funcionario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Anticipos.Funcionarios
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

        public Paginado<Anticipo> Anticipos { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OrdenCampo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? IndicePagina { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FDNI { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FNombre { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FEscalafon { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FPeriodoDesde { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FPeriodoHasta { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task OnGetAsync(int? periodoDesdeAno, int? periodoDesdeMes, int? periodoHastaAno, int? periodoHastaMes)
        {
            var tamañoPagina = _configuration.GetValue("PageSize", 10);

            var anticiposQuery = _context.AnticiposFuncionarios.AsQueryable();
            anticiposQuery = anticiposQuery.Include(a => a.Agente).AsNoTracking();

            if (FPeriodoDesde.HasValue || (periodoDesdeAno.HasValue && periodoDesdeMes.HasValue))
            {
                if (!FPeriodoDesde.HasValue)
                    FPeriodoDesde = new DateTime(periodoDesdeAno.Value, periodoDesdeMes.Value, 1);

                anticiposQuery = anticiposQuery.Where(s => s.Periodo >= FPeriodoDesde);
            }

            if (FPeriodoHasta.HasValue || periodoHastaAno.HasValue && periodoHastaMes.HasValue)
            {
                if (!FPeriodoHasta.HasValue)
                    FPeriodoHasta = new DateTime(periodoHastaAno.Value, periodoHastaMes.Value, 1);

                anticiposQuery = anticiposQuery.Where(s => s.Periodo <= FPeriodoHasta);
            }

            if (FDNI.HasValue)
            {
                anticiposQuery = anticiposQuery.Where(s => s.Agente.DNI.ToString().Contains(FDNI.Value.ToString()));
            }

            if (!String.IsNullOrWhiteSpace(FNombre))
            {
                anticiposQuery = anticiposQuery.Where(s => s.Agente.NombreNormalizado.Contains(FNombre.ToUpper()));
            }

            if (!String.IsNullOrWhiteSpace(FEscalafon))
            {
                anticiposQuery = anticiposQuery.Where(s => s.Escalafon.ToUpper().Contains(FEscalafon.ToUpper()));
            }

            if (!String.IsNullOrWhiteSpace(OrdenCampo))
            {
                string ordenCampo = OrdenCampo;
                string ordenDireccion = "asc";

                if (OrdenCampo.Contains("desc"))
                {
                    ordenDireccion = "desc";
                    ordenCampo = ordenCampo[0..^5];
                }

                ordenCampo = ordenCampo.Replace("__", ".");
                anticiposQuery = anticiposQuery.OrderBy($"{ordenCampo} {ordenDireccion}");
            }
            else
            {
                anticiposQuery = anticiposQuery.OrderBy(s => s.Periodo).ThenBy(s => s.Agente.Nombre);
            }

            Anticipos = await Paginado<Anticipo>.CreateAsync(anticiposQuery, IndicePagina ?? 1, tamañoPagina);
        }

        #endregion Public Methods
    }
}