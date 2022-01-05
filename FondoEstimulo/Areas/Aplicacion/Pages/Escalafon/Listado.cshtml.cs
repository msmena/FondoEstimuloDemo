using FondoEstimulo.Data;
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

namespace FondoEstimulo.Areas.Aplicacion.Pages.Escalafon
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

        public Paginado<Models.Escalafon> Escalafones { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OrdenCampo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? IndicePagina { get; set; }

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

            var grupoQuery = from x in _context.EscalafonGeneral
                             orderby x.Grupo descending
                             select x.Grupo;

            Apartados = new SelectList(await apartadoQuery.Distinct().AsNoTracking().ToListAsync());
            Grupos = new SelectList(await grupoQuery.Distinct().ToListAsync());

            var tamañoPagina = _configuration.GetValue("PageSize", 10);

            var escalafonQuery = _context.EscalafonGeneral.AsQueryable().AsNoTracking();

            if (!String.IsNullOrEmpty(FApartado))
            {
                escalafonQuery = escalafonQuery.Where(s => s.Apartado.Equals(FApartado));
            }

            if (FGrupo.HasValue)
            {
                escalafonQuery = escalafonQuery.Where(s => s.Grupo.Equals(FGrupo.Value));
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
                escalafonQuery = escalafonQuery.OrderBy($"{ordenCampo} {ordenDireccion}");
            }
            else
            {
                escalafonQuery = escalafonQuery.OrderBy(s => s.Apartado).ThenByDescending(s => s.Grupo);
            }

            Escalafones = await Paginado<Models.Escalafon>.CreateAsync(
                escalafonQuery, IndicePagina ?? 1, tamañoPagina);
        }

        #endregion Public Methods
    }
}