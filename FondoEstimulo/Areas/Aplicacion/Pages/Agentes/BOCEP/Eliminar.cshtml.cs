using FondoEstimulo.Models.Agentes.BOCEP;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Agentes.BOCEP
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

        public Agente Agente { get; set; }
        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Agente = await _context.AgentesBOCEP
                .Include(a => a.Escalafon)
                .Include(b => b.BonificacionTitulo)
                .Include(c => c.BonificacionTituloEscalafon)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.AgenteID == id);

            if (Agente == null)
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

            Agente = await _context.AgentesBOCEP.AsNoTracking().FirstOrDefaultAsync(m => m.AgenteID == id);
            Models.Evento evento = new();
            evento.Accion = Models.Parametro.EventoAccion.Eliminacion;
            evento.Origen = Models.Parametro.EventoOrigen.Agente;
            evento.UsuarioID = _userManager.GetUserId(User);

            if (Agente != null)
            {
                try
                {
                    _context.AgentesBOCEP.Remove(Agente);
                    evento.RegistroID = Agente.AgenteID.ToString();
                    evento.Serializar(Agente);
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