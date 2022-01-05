using FondoEstimulo.Models.Agentes.Funcionario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Anticipos.Funcionarios
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

        public Anticipo Anticipo { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Anticipo = await _context.AnticiposFuncionarios
                .Include(a => a.Agente).AsNoTracking().FirstOrDefaultAsync(m => m.AnticipoID == id);

            if (Anticipo == null)
            {
                return NotFound();
            }
            return Page();
        }

        #endregion Public Methods
    }
}