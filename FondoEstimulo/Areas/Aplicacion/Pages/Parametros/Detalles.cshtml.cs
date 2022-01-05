using FondoEstimulo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Parametros
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

        public Parametro Parametro { get; set; }
        public List<BonificacionTitulo> Bonificaciones { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Parametro = await _context.Parametros.AsNoTracking().
                FirstOrDefaultAsync(m => m.ParametroID == id);
            Bonificaciones = await _context.BonificacionesTitulo.AsNoTracking().
                ToListAsync();

            if (Parametro == null)
            {
                return NotFound();
            }
            return Page();
        }

        #endregion Public Methods
    }
}