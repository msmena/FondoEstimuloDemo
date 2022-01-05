using FondoEstimulo.Models.Agentes.BOCEP;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Anticipos.BOCEP
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
        public Anticipo Anticipo { get; set; }

        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Anticipo = await _context.AnticiposBOCEP
                .Include(a => a.Agente).AsNoTracking().FirstOrDefaultAsync(m => m.AnticipoID == id);

            if (Anticipo == null)
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

            Anticipo = await _context.AnticiposBOCEP.AsNoTracking().FirstOrDefaultAsync(m => m.AnticipoID == id);
            Models.Evento evento = new();
            evento.Accion = Models.Parametro.EventoAccion.Eliminacion;
            evento.Origen = Models.Parametro.EventoOrigen.Anticipo;
            evento.UsuarioID = _userManager.GetUserId(User);

            if (Anticipo != null)
            {
                try
                {
                    _context.AnticiposBOCEP.Remove(Anticipo);
                    evento.RegistroID = Anticipo.AnticipoID.ToString();
                    evento.Serializar(Anticipo);
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