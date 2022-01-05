using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Escalafon
{
    public class CreateModel : PageModel
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
        public Models.Escalafon Escalafon { get; set; }

        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public IActionResult OnGet(bool? saveChangesError = false)
        {
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
            evento.Origen = Models.Parametro.EventoOrigen.Escalafon;
            evento.UsuarioID = _userManager.GetUserId(User);

            try
            {
                _context.EscalafonGeneral.Add(Escalafon);
                await _context.SaveChangesAsync();
                evento.RegistroID = Escalafon.EscalafonID.ToString();
                evento.Serializar(Escalafon);
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