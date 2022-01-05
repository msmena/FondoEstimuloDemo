using FondoEstimulo.Models;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Parametros
{
    public class EditModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Private Methods

        private bool ParametroExists(long id)
        {
            return _context.Parametros.Any(e => e.ParametroID == id);
        }

        #endregion Private Methods

        #region Public Constructors

        public EditModel(Data.FondoEstimuloContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        [BindProperty]
        public Parametro Parametro { get; set; }

        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Parametro = await _context.Parametros.FirstOrDefaultAsync(m => m.ParametroID == id);

            if (Parametro == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Ha ocurrido un error al intentar editar el registro.";
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Parametro parametroOriginal = await _context.Parametros.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ParametroID == Parametro.ParametroID);
            Evento evento = new();
            evento.Accion = Parametro.EventoAccion.Modificacion;
            evento.Origen = Parametro.EventoOrigen.Parametro;
            evento.UsuarioID = _userManager.GetUserId(User);

            _context.Attach(Parametro).State = EntityState.Modified;

            try
            {
                evento.RegistroID = Parametro.ParametroID.ToString();
                evento.Serializar(Parametro);
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex, ErrorMessage);

                if (!ParametroExists(Parametro.ParametroID))
                {
                    return NotFound();
                }
                else
                {
                    return RedirectToAction("./Editar", new { saveChangesError = true });
                }
            }

            return RedirectToPage("./Detalles", new { id = 1 });
        }

        #endregion Public Methods
    }
}