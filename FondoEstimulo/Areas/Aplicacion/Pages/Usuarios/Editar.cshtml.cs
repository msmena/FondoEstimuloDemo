using FondoEstimulo.Data;
using FondoEstimulo.Models;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Usuarios
{
    [Authorize(Roles = "Administrador")]
    public class EditModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<EditModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public EditModel(Data.FondoEstimuloContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(string id, bool? saveChangesError = false)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                Log.Error("Se intenta editar un ID vacio", id);
                return NotFound();
            }

            Usuario user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                Log.Error("Usuario no encontrado en la base de datos, ID: {usuarioID}", id);
                return NotFound();
            }

            bool userIsAdmin = await _userManager.IsInRoleAsync(user, "Administrador");

            Input = new InputModel
            {
                UserID = user.Id,
                User = user.UserName,
                Email = user.Email,
                Rol = userIsAdmin ? Parametro.Roles.Administrador : Parametro.Roles.Registro,
            };

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Ha ocurrido un error al intentar crear el registro.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuario user = await _userManager.FindByIdAsync(Input.UserID);
                    Usuario usuarioOriginal = user;
                    bool userIsAdmin = await _userManager.IsInRoleAsync(user, "Administrador");

                    user.UserName = Input.User;
                    user.Email = Input.Email;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        Evento evento = new();
                        evento.Accion = Parametro.EventoAccion.Modificacion;
                        evento.Origen = Parametro.EventoOrigen.Usuario;
                        evento.UsuarioID = _userManager.GetUserId(User);
                        evento.RegistroID = Input.UserID;
                        evento.Serializar(user);
                        _context.Eventos.Add(evento);

                        Log.Information("Usuario Administrador actualizo usuario.");

                        if (Input.Rol == Parametro.Roles.Administrador && !userIsAdmin)
                        {
                            var resultRolChange = await _userManager.AddToRoleAsync(user, FondoEstimuloContext.AdminRol);

                            if (resultRolChange.Succeeded)
                            {
                                Log.Information("Usuario Administrador asigno rol Administrador a usuario.");
                            }
                            foreach (var error in resultRolChange.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                        else if (Input.Rol != Parametro.Roles.Administrador && userIsAdmin)
                        {
                            var resultRolChange = await _userManager.RemoveFromRoleAsync(user, FondoEstimuloContext.AdminRol);

                            if (resultRolChange.Succeeded)
                            {
                                Log.Information("Usuario Administrador removio rol Administrador a usuario.");
                            }
                            foreach (var error in resultRolChange.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }

                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        //var callbackUrl = Url.Page(
                        //    "/Account/ConfirmEmail",
                        //    pageHandler: null,
                        //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        //    protocol: Request.Scheme);

                        //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        //{
                        //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        //}
                        //else
                        //{
                        //    await _signInManager.SignInAsync(user, isPersistent: false);
                        //    return LocalRedirect(returnUrl);
                        //}
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ErrorMessage);
                    return RedirectToAction("./Editar", new { saveChangesError = true });
                }
            }

            return RedirectToPage("./Listado");
        }

        #endregion Public Methods

        #region Public Classes

        public class InputModel
        {
            #region Public Properties

            public string UserID { get; set; }

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