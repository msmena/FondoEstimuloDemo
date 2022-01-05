using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace FondoEstimulo.Pages
{
    [AllowAnonymous]
    public class EstablecerContrasenaModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<EstablecerContrasenaModel>();
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public EstablecerContrasenaModel(SignInManager<Usuario> signInManager,
            UserManager<Usuario> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        public string ErrorMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void OnGet(string code = null, bool? saveChangesError = false)
        {
            if (_signInManager.IsSignedIn(User))
            {
                Log.Information("Se ha intentado establecer contraseña con un usuario ya identificado.");
                Response.Redirect("./Index");
            }

            if (code == null)
            {
                Log.Information("Se ha intentado establecer contraseña sin información del usuario.");
                Response.Redirect("./Index");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Ha ocurrido un error al intentar crear el registro.";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                Log.Information("Se ha intentado establecer contraseña de un usuario que no existe.");
                return RedirectToPage("./Index");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Contrasena);
            if (result.Succeeded)
            {
                Log.Information("Se ha establecido la contraseña de forma satisfactoria.");
                return RedirectToPage("./Index", new { CustomMessage = "Se ha establecido correctamente la contraseña." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        #endregion Public Methods

        #region Public Classes

        public class InputModel
        {
            #region Public Properties

            public string Code { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Contrasena", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
            public string ConfirmarContrasena { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "El {0} debe contener al menos {2} y un máximo de {1} caracteres de longitud.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Contrasena { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            #endregion Public Properties
        }

        #endregion Public Classes
    }
}