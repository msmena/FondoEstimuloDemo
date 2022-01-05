using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Escalafon
{
    public class DetailsModel : PageModel
    {
        #region Private Fields

        private readonly FondoEstimulo.Data.FondoEstimuloContext _context;

        #endregion Private Fields

        #region Public Constructors

        public DetailsModel(FondoEstimulo.Data.FondoEstimuloContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Properties

        public Models.Escalafon Escalafon { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Escalafon = await _context.EscalafonGeneral.AsNoTracking().FirstOrDefaultAsync(m => m.EscalafonID == id);

            if (Escalafon == null)
            {
                return NotFound();
            }
            return Page();
        }

        #endregion Public Methods
    }
}