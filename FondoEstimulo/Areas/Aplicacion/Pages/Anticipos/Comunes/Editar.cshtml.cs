using FondoEstimulo.Models.Agentes.Comun;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Anticipos.Comunes
{
    public class EditModel : AgenteNombrePageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Private Methods

        private bool AnticipoExists(int id)
        {
            return _context.AnticiposComunes.Any(e => e.AnticipoID == id);
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

            Anticipo = await _context.AnticiposComunes
                .Include(a => a.Agente).FirstOrDefaultAsync(m => m.AnticipoID == id);

            if (Anticipo == null)
            {
                return NotFound();
            }

            PopulateAgentesComunesDropDownList(_context);

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

            Anticipo anticipoOriginal = await _context.AnticiposComunes.AsNoTracking()
                .Include(a => a.Agente).FirstOrDefaultAsync(m => m.AnticipoID == Anticipo.AnticipoID);
            Models.Evento evento = new();
            evento.Accion = Models.Parametro.EventoAccion.Modificacion;
            evento.Origen = Models.Parametro.EventoOrigen.Anticipo;
            evento.UsuarioID = _userManager.GetUserId(User);

            _context.Attach(Anticipo).State = EntityState.Modified;

            try
            {
                evento.RegistroID = Anticipo.AnticipoID.ToString();
                evento.Serializar(Anticipo);
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex, ErrorMessage);

                if (!AnticipoExists(Anticipo.AnticipoID))
                {
                    return NotFound();
                }
                else
                {
                    return RedirectToAction("./Editar", new { saveChangesError = true });
                }
            }

            return RedirectToPage("./Listado");
        }

        #endregion Public Methods
    }
}