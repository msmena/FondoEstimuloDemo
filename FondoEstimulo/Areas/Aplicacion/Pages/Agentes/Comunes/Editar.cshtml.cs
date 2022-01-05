using FondoEstimulo.Models.Agentes.Comun;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Agentes.Comunes
{
    public class EditModel : AgentesBasePageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Private Methods

        private bool AgenteExists(int id)
        {
            return _context.AgentesComun.Any(e => e.AgenteID == id);
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

            PopulateEscalafonDropDownList(_context);
            PopulateBonificacionTituloDropDownList(_context);

            Agente = await _context.AgentesComun.FirstOrDefaultAsync(m => m.AgenteID == id);

            if (Agente == null)
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

            if (Agente.Tipo == Models.Parametro.AgenteTipo.Retirado && !Agente.Retiro.HasValue)
                Agente.Retiro = System.DateTime.Today;

            if (Agente.Tipo == Models.Parametro.AgenteTipo.Jubilado && !Agente.Jubilacion.HasValue)
                Agente.Jubilacion = System.DateTime.Today;

            Agente agenteOriginal = await _context.AgentesComun.AsNoTracking().FirstOrDefaultAsync(m => m.AgenteID == Agente.AgenteID);
            Models.Evento evento = new();
            evento.Accion = Models.Parametro.EventoAccion.Modificacion;
            evento.Origen = Models.Parametro.EventoOrigen.Agente;
            evento.UsuarioID = _userManager.GetUserId(User);

            _context.Attach(Agente).State = EntityState.Modified;

            try
            {
                evento.RegistroID = Agente.AgenteID.ToString();
                evento.Serializar(Agente);
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex, ErrorMessage);

                if (!AgenteExists(Agente.AgenteID))
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