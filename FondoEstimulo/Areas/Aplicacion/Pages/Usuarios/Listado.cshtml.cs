using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Usuarios
{
    [Authorize(Roles = "Administrador")]
    public class IndexModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly IConfiguration _configuration;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public IndexModel(UserManager<Usuario> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        #endregion Public Constructors

        #region Public Properties

        public Paginado<Usuario> Usuarios { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OrdenCampo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? IndicePagina { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FNombre { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FEmail { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task OnGetAsync()
        {
            var tamañoPagina = _configuration.GetValue("PageSize", 10);
            var usuariosQuery = _userManager.Users.AsQueryable()
                .Include(a => a.UserRoles).
                AsNoTracking();

            if (!String.IsNullOrEmpty(FNombre))
                usuariosQuery = usuariosQuery.Where(s => s.NormalizedUserName.Contains(FNombre.ToUpper()));

            if (!String.IsNullOrEmpty(FEmail))
                usuariosQuery = usuariosQuery.Where(s => s.NormalizedEmail.Contains(FEmail.ToUpper()));

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
                usuariosQuery = usuariosQuery.OrderBy($"{ordenCampo} {ordenDireccion}");
            }
            else
            {
                usuariosQuery = usuariosQuery.OrderBy(s => s.NormalizedUserName);
            }

            Usuarios = await Paginado<Usuario>.CreateAsync(usuariosQuery, IndicePagina ?? 1, tamañoPagina);
            Log.Information("Datos de usuarios obtenidos. Cantidad: {cantidadRegistros}; Indice pagina: {indicePagina}; " +
                "Tamaño pagina: {tamanoPagina}", Usuarios.Count.ToString(), IndicePagina ?? 1, tamañoPagina);
        }

        #endregion Public Methods
    }
}