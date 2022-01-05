using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FondoEstimulo.Pages
{
    [AllowAnonymous]
    public class ReestablecerContrasenaModel : PageModel
    {
        #region Private Fields

        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _environment;

        #endregion Private Fields

        #region Public Constructors

        public ReestablecerContrasenaModel(UserManager<Usuario> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _environment = env;
        }

        #endregion Public Constructors

        #region Public Properties

        [BindProperty]
        public InputModel Input { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./Index");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page("/EstablecerContrasena", pageHandler: null, values: new { area = "", code }, protocol: Request.Scheme);

                Email email = new();
                string cuerpoEmail = string.Format(email.CuerpoEstablecerContrasena, user.UserName, Input.Email, $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>enlace</a>");
                await email.Notificar(Input.Email, "ATP - Modulo Fondo de Estimulo - Especificación contraseña", cuerpoEmail, _environment.IsProduction());

                return RedirectToPage("./Index", new { CustomMessage = "Se ha enviado correo electrónico para reestablecer la contraseña." });
            }

            return Page();
        }

        #endregion Public Methods

        #region Public Classes

        public class InputModel
        {
            #region Public Properties

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            #endregion Public Properties
        }

        #endregion Public Classes
    }
}