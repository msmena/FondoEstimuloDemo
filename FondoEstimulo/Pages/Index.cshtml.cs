using FondoEstimulo.Data;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FondoEstimulo.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly FondoEstimuloContext _context;
        private readonly IWebHostEnvironment _environment;

        #endregion Private Fields

        #region Private Methods

        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);

        [DllImport("Ws2_32.dll", CharSet = CharSet.Unicode)]
        private static extern Int32 inet_addr(string ip);

        private static string GetClientMAC(string strClientIP)
        {
            string mac_dest = "";
            try
            {
                Int32 ldest = inet_addr(strClientIP);
                Int32 lhost = inet_addr("");
                Int64 macinfo = new();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                string mac_src = macinfo.ToString("X");

                while (mac_src.Length < 12)
                {
                    mac_src = mac_src.Insert(0, "0");
                }

                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i % 2))
                    {
                        if (i == 10)
                        {
                            mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                        else
                        {
                            mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception("L?i " + err.Message);
            }
            return mac_dest;
        }

        #endregion Private Methods

        #region Public Constructors

        public IndexModel(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, FondoEstimuloContext context, IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _environment = env;
        }

        #endregion Public Constructors

        #region Public Properties

        public string CustomMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(string returnUrl = null, string customMessage = null)
        {
            CustomMessage = customMessage;

            if (_signInManager.IsSignedIn(User))
            {
                try
                {
                    string nombreUsuario = _userManager.GetUserName(User);
                    Usuario usuario = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.Equals(nombreUsuario));

                    if (usuario is null)
                    {
                        ModelState.AddModelError(string.Empty, "Usuario no habilitado para el ingresado al sistema.");
                        return Page();
                    }

                    InicioSesion inicioSesion = new()
                    {
                        UserName = nombreUsuario,
                        Fecha = DateTime.Now
                    };

                    if (Request.HttpContext.Connection.RemoteIpAddress != null)
                    {
                        inicioSesion.IP = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        inicioSesion.MAC = GetClientMAC(inicioSesion.IP);
                    }
                    _context.IniciosSesion.Add(inicioSesion);
                    await _context.SaveChangesAsync();
                    Log.Information("Inicio de sesion guardado");
                }
                catch (DbUpdateException ex)
                {
                    Log.Error(ex, ex.Message);
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return Page();
                }

                Log.Information("Usuario identificado automaticamente");
                Response.Redirect("./Aplicacion/Index");
            }

            Usuario userAdmin = await _userManager.FindByNameAsync("MatiasM");

            if (userAdmin == null)
            {
                Log.Error("Usuario administrador no encontrado en la base de datos");
                return NotFound();
            }

            if (string.IsNullOrEmpty(userAdmin.PasswordHash))
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(userAdmin);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page("/EstablecerContrasena", pageHandler: null, values: new { area = "", code }, protocol: Request.Scheme);

                Email email = new();
                string cuerpoEmail = string.Format(email.CuerpoEstablecerContrasena, userAdmin.UserName, userAdmin.Email, $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>enlace</a>");
                await email.Notificar(userAdmin.Email, "ATP - Modulo Fondo de Estimulo - Especificación contraseña", cuerpoEmail, _environment.IsProduction());
                CustomMessage = "Se ha enviado email para especificar contraseña de administrador";
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Usuario, Input.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    Log.Information("Usuario identificado manualmente");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Identificación de usuario errónea");
                    Log.Information("Usuario no identificado.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        #endregion Public Methods

        #region Public Classes

        public class InputModel
        {
            #region Public Properties

            [Required(ErrorMessage = "Campo contraseña es obligatorio.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Campo Usuario es obligatorio.")]
            public string Usuario { get; set; }

            #endregion Public Properties
        }

        #endregion Public Classes
    }
}