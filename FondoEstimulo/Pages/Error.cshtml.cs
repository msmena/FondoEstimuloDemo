using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace FondoEstimulo.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<ErrorModel>();
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IWebHostEnvironment _environment;

        #endregion Private Fields

        #region Public Constructors

        public ErrorModel(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _environment = env;
        }

        #endregion Public Constructors

        #region Public Properties

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        #endregion Public Properties

        #region Public Methods

        public async void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            Log.Error("Id de Error: {id}", RequestId);

            string usuario = "Anónimo";
            string url = usuario;

            if (_signInManager.IsSignedIn(User))
                usuario = _userManager.GetUserName(User);

            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            if (statusCodeReExecuteFeature != null)
            {
                url = statusCodeReExecuteFeature.OriginalPathBase
                    + statusCodeReExecuteFeature.OriginalPath
                    + statusCodeReExecuteFeature.OriginalQueryString;
            }

            Email email = new();
            string cuerpoEmail = string.Format(email.CuerpoError, usuario, RequestId, url);
            await email.Notificar("atp.gmmena@chaco.gov.ar", "ATP - Modulo Fondo de Estimulo - Error", cuerpoEmail, _environment.IsProduction(), true);

            Log.Information("Se ha enviado email de información del error");
        }

        #endregion Public Methods
    }
}