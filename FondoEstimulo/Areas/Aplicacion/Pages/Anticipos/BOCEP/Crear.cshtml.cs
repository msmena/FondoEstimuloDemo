using FondoEstimulo.Models.Agentes.BOCEP;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Anticipos.BOCEP
{
    public class CreateModel : AgenteNombrePageModel
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
        public Anticipo Anticipo { get; set; }

        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public IActionResult OnGet(bool? saveChangesError = false)
        {
            PopulateAgentesBOCEPDropDownList(_context);
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
            evento.Origen = Models.Parametro.EventoOrigen.Anticipo;
            evento.UsuarioID = _userManager.GetUserId(User);

            if (Anticipo.Agente == null)
                Anticipo.Agente = _context.AgentesBOCEP.FirstOrDefault(m => m.AgenteID == Anticipo.Agente.AgenteID);

            Models.Parametro parametro = _context.Parametros.FirstOrDefault(m => m.ParametroID == 1);
            Anticipo.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(m => m.EscalafonID == Anticipo.Agente.EscalafonID);
            Anticipo.Agente.BonificacionTitulo = _context.BonificacionesTitulo.FirstOrDefault(n => n.BonificacionTituloID == Anticipo.Agente.BonificacionTituloID);
            Anticipo.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(e => e.EscalafonID == Anticipo.Agente.BonificacionTituloEscalafonID);

            try
            {
                Anticipo.Fecha = DateTime.Today;
                Anticipo.CalcularTotal(parametro);
                _context.AnticiposBOCEP.Add(Anticipo);
                await _context.SaveChangesAsync();
                evento.RegistroID = Anticipo.AnticipoID.ToString();
                evento.Serializar(Anticipo);
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