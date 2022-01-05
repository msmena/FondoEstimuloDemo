using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Escalafon
{
    public class DeleteModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public DeleteModel(Data.FondoEstimuloContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        [BindProperty]
        public Models.Escalafon Escalafon { get; set; }

        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Escalafon = await _context.EscalafonGeneral.FirstOrDefaultAsync(m => m.EscalafonID == id);

            if (Escalafon == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Ha ocurrido un error al intentar eliminar el registro.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Escalafon = await _context.EscalafonGeneral.AsNoTracking().FirstOrDefaultAsync(m => m.EscalafonID == id);
            Models.Evento evento = new();
            evento.Accion = Models.Parametro.EventoAccion.Eliminacion;
            evento.Origen = Models.Parametro.EventoOrigen.Escalafon;
            evento.UsuarioID = _userManager.GetUserId(User);

            if (Escalafon != null)
            {
                try
                {
                    _context.EscalafonGeneral.Remove(Escalafon);
                    evento.RegistroID = Escalafon.EscalafonID.ToString();
                    evento.Serializar(Escalafon);
                    _context.Eventos.Add(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Log.Error(ex, ErrorMessage);
                    return RedirectToAction("./Eliminar", new { id, saveChangesError = true });
                }
            }

            return RedirectToPage("./Listado");
        }

        #endregion Public Methods
    }
}