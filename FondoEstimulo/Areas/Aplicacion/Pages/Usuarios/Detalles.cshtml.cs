using FondoEstimulo.Models;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Usuarios
{
    [Authorize(Roles = "Administrador")]
    public class DetailsModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<DetailsModel>();
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public DetailsModel(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        public DetailModel Usuario { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
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

            Usuario = new DetailModel
            {
                UserID = user.Id,
                User = user.UserName,
                Email = user.Email,
                Rol = userIsAdmin ? Parametro.Roles.Administrador : Parametro.Roles.Registro,
            };

            return Page();
        }

        #endregion Public Methods

        #region Public Classes

        public class DetailModel
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
        }

        #endregion Public Classes
    }
}