using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Cuenta
{
    public class CambiarContrasenaModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<CambiarContrasenaModel>();
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public CambiarContrasenaModel(UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public string StatusMessage { get; set; }

        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(bool? saveChangesError = false)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se ha podido obtener informacion del usuario ID '{_userManager.GetUserId(User)}'.");
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Ha ocurrido un error al intentar crear el registro.";
            }

            Input = new();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                Log.Information("Se ha intentado establecer contrase�a de un usuario que no existe.");
                return RedirectToPage("/Index");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.ContrasenaActual, Input.Contrasena);
            if (changePasswordResult.Succeeded)
            {
                Log.Information("Se ha establecido la contrase�a de forma satisfactoria.");
                return RedirectToPage(new { CustomMessage = "Se ha establecido correctamente la contrase�a." });
            }

            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }

        #endregion Public Methods

        #region Public Classes

        public class InputModel
        {
            #region Public Properties

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Contrase�a actual:")]
            public string ContrasenaActual { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "El {0} debe contener al menos {2} y un m�ximo de {1} caracteres de longitud.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Contrase�a:")]
            public string Contrasena { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contrase�a:")]
            [Compare("Contrasena", ErrorMessage = "La contrase�a y la confirmaci�n no coinciden.")]
            public string ConfirmarContrasena { get; set; }

            #endregion Public Properties
        }

        #endregion Public Classes
    }
}