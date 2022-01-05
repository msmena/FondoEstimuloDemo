using FondoEstimulo.Data;
using FondoEstimulo.Models;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Usuarios
{
    [Authorize(Roles = "Administrador")]
    public class DeleteModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<DeleteModel>();
        private readonly FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public DeleteModel(FondoEstimuloContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        public DetailModel Usuario { get; set; }
        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(string id, bool? saveChangesError = false)
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

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Ha ocurrido un error al intentar eliminar el registro.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
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

            try
            {
                if (userIsAdmin)
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

                var resultAction = await _userManager.DeleteAsync(user);

                if (resultAction.Succeeded)
                {
                    Evento evento = new();
                    evento.Accion = Parametro.EventoAccion.Eliminacion;
                    evento.Origen = Parametro.EventoOrigen.Usuario;
                    evento.UsuarioID = _userManager.GetUserId(User);
                    evento.RegistroID = id.ToString();
                    evento.Serializar(user);
                    _context.Eventos.Add(evento);

                    Log.Information("Usuario Administrador elimino a usuario.");
                }
                foreach (var error in resultAction.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex, ErrorMessage);
                return RedirectToAction("./Eliminar", new { id, saveChangesError = true });
            }

            return RedirectToPage("./Listado");
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