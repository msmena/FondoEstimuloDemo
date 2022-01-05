using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Escalafon
{
    public class EditModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Private Methods

        private bool EscalafonExists(int id)
        {
            return _context.EscalafonGeneral.Any(e => e.EscalafonID == id);
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

            Models.Escalafon escalafonOriginal = await _context.EscalafonGeneral.AsNoTracking().FirstOrDefaultAsync(m => m.EscalafonID == Escalafon.EscalafonID);
            Models.Evento evento = new();
            evento.Accion = Models.Parametro.EventoAccion.Modificacion;
            evento.Origen = Models.Parametro.EventoOrigen.Escalafon;
            evento.UsuarioID = _userManager.GetUserId(User);

            _context.Attach(Escalafon).State = EntityState.Modified;

            try
            {
                evento.RegistroID = Escalafon.EscalafonID.ToString();
                evento.Serializar(Escalafon);
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex, ErrorMessage);

                if (!EscalafonExists(Escalafon.EscalafonID))
                {
                    return NotFound();
                }
                else
                {
                    return RedirectToAction("./Editar", new { Escalafon.EscalafonID, saveChangesError = true });
                }
            }

            return RedirectToPage("./Listado");
        }

        #endregion Public Methods
    }
}