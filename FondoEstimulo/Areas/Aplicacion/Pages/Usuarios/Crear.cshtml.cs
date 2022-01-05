using FondoEstimulo.Data;
using FondoEstimulo.Models;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Usuarios
{
    [Authorize(Roles = "Administrador")]
    public class RegisterModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<RegisterModel>();
        private readonly FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _environment;

        #endregion Private Fields

        #region Public Constructors

        public RegisterModel(FondoEstimuloContext context, UserManager<Usuario> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _environment = env;
        }

        #endregion Public Constructors

        #region Public Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void OnGet(bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Ha ocurrido un error al intentar crear el registro.";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new Usuario { UserName = Input.User, Email = Input.Email };
                    var result = await _userManager.CreateAsync(user);

                    if (result.Succeeded)
                    {
                        Evento evento = new();
                        evento.Accion = Parametro.EventoAccion.Creacion;
                        evento.Origen = Parametro.EventoOrigen.Usuario;
                        evento.UsuarioID = _userManager.GetUserId(User);
                        evento.RegistroID = await _userManager.GetUserIdAsync(user);
                        evento.Serializar(user);
                        _context.Eventos.Add(evento);

                        Log.Information("Usuario Administrador creo nueva cuenta.");

                        if (Input.Rol == Parametro.Roles.Administrador)
                        {
                            var resultRol = await _userManager.AddToRoleAsync(user, FondoEstimuloContext.AdminRol);

                            if (resultRol.Succeeded)
                            {
                                Log.Information("Usuario Administrador asigno rol administrador a nueva cuenta.");
                            }
                            foreach (var error in resultRol.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }

                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page("/EstablecerContrasena", pageHandler: null, values: new { area = "", code }, protocol: Request.Scheme);
                        Email email = new();
                        string cuerpoEmail = string.Format(email.CuerpoEstablecerContrasena, Input.User, Input.Email, $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>enlace</a>");
                        await email.Notificar(Input.Email, "ATP - Modulo Fondo de Estimulo - Especificación contraseña", cuerpoEmail, _environment.IsProduction());
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ErrorMessage);
                    return RedirectToAction("./Crear", new { saveChangesError = true });
                }
            }

            return RedirectToPage("./Listado");
        }

        #endregion Public Methods

        #region Public Classes

        public class InputModel
        {
            #region Public Properties

            [Required]
            [Display(Name = "Usuario")]
            public string User { get; set; }

            [Required]
            [Display(Name = "Rol")]
            public Parametro.Roles Rol { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            #endregion Public Properties

            //[Required]
            //[StringLength(100, ErrorMessage = "El {0} debe contener al menos {2} y un máximo de {1} caracteresde longitud.", MinimumLength = 6)]
            //[DataType(DataType.Password)]
            //[Display(Name = "Password")]
            //public string Password { get; set; }

            //[DataType(DataType.Password)]
            //[Display(Name = "Confirm password")]
            //[Compare("Password", ErrorMessage = "El password y la confirmación no coinciden.")]
            //public string ConfirmPassword { get; set; }
        }

        #endregion Public Classes
    }
}