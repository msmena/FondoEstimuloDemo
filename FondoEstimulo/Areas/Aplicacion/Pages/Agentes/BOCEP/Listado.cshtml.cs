using FondoEstimulo.Data;
using FondoEstimulo.Models.Agentes.BOCEP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Agentes.BOCEP
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

        public Paginado<Agente> Agentes { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OrdenCampo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? IndicePagina { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FDNI { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FNombre { get; set; }

        public SelectList Apartados { get; set; }
        public SelectList Grupos { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FApartado { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FGrupo { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task OnGetAsync()
        {
            var apartadoQuery = from m in _context.EscalafonGeneral
                                orderby m.Apartado
                                select m.Apartado;

            var grupoQuery = from m in _context.EscalafonGeneral
                             orderby m.Grupo descending
                             select m.Grupo;

            Apartados = new SelectList(await apartadoQuery.Distinct().AsNoTracking().ToListAsync());
            Grupos = new SelectList(await grupoQuery.Distinct().ToListAsync());

            var tamañoPagina = _configuration.GetValue("PageSize", 10);

            var agentesQuery = _context.AgentesBOCEP.AsQueryable();
            agentesQuery = agentesQuery.Include(a => a.Escalafon).AsNoTracking();

            if (FDNI.HasValue)
            {
                agentesQuery = agentesQuery.Where(s => s.DNI.ToString().Contains(FDNI.Value.ToString()));
            }

            if (!String.IsNullOrEmpty(FNombre))
            {
                agentesQuery = agentesQuery.Where(s => s.NombreNormalizado.Contains(FNombre.ToUpper()));
            }

            if (!String.IsNullOrEmpty(FApartado))
            {
                agentesQuery = agentesQuery.Where(s => s.Escalafon.Apartado.Equals(FApartado));
            }

            if (FGrupo.HasValue)
            {
                agentesQuery = agentesQuery.Where(s => s.Escalafon.Grupo.Equals(FGrupo.Value));
            }

            string ordenCampo = OrdenCampo;
            if (!String.IsNullOrWhiteSpace(OrdenCampo))
            {
                string ordenDireccion = "asc";

                if (OrdenCampo.Contains("desc"))
                {
                    ordenDireccion = "desc";
                    ordenCampo = OrdenCampo.Substring(0, OrdenCampo.Length - (ordenDireccion.Length + 1));
                }

                ordenCampo = ordenCampo.Replace("__", ".");

                if (ordenCampo.Equals("Escalafon.EscalafonIdentificador"))
                {
                    if (ordenDireccion.Equals("asc"))
                    {
                        agentesQuery = agentesQuery.OrderBy(s => s.Escalafon.Apartado).ThenByDescending(s => s.Escalafon.Grupo);
                    }
                    else
                    {
                        agentesQuery = agentesQuery.OrderByDescending(s => s.Escalafon.Apartado).ThenBy(s => s.Escalafon.Grupo);
                    }
                }
                else
                {
                    agentesQuery = agentesQuery.OrderBy($"{ordenCampo} {ordenDireccion}");
                }
            }
            else
            {
                agentesQuery = agentesQuery.OrderBy(s => s.Nombre);
            }

            Agentes = await Paginado<Agente>.CreateAsync(agentesQuery, IndicePagina ?? 1, tamañoPagina);
        }

        #endregion Public Methods
    }
}