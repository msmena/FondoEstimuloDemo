using FondoEstimulo.Models.Agentes.BOCEP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Agentes.BOCEP
{
    public class DetailsModel : PageModel
    {
        #region Private Fields

        private readonly Data.FondoEstimuloContext _context;

        #endregion Private Fields

        #region Public Constructors

        public DetailsModel(Data.FondoEstimuloContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Properties

        public Agente Agente { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id)
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
            return Page();
        }

        #endregion Public Methods
    }
}