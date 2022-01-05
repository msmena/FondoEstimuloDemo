using FondoEstimulo.Models.Agentes.BOCEP;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Agentes.BOCEP
{
    public class CreateModel : AgentesBasePageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public CreateModel(Data.FondoEstimuloContext context, UserManager<Usuario> userManager)
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

        public IActionResult OnGet(bool? saveChangesError = false)
        {
            PopulateEscalafonDropDownList(_context);
            PopulateBonificacionTituloDropDownList(_context);

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Ha ocurrido un error al intentar crear el registro.";
            }

            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Models.Evento evento = new();
            evento.Accion = Models.Parametro.EventoAccion.Creacion;
            evento.Origen = Models.Parametro.EventoOrigen.Agente;
            evento.UsuarioID = _userManager.GetUserId(User);

            try
            {
                _context.AgentesBOCEP.Add(Agente);
                await _context.SaveChangesAsync();
                evento.RegistroID = Agente.AgenteID.ToString();
                evento.Serializar(Agente);
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex, ErrorMessage);
                return RedirectToAction("./Crear", new { saveChangesError = true });
            }
            return RedirectToPage("./Listado");
        }

        #endregion Public Methods
    }
}