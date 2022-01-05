using FondoEstimulo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Procesos
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

        public Proceso Proceso { get; set; }
        public IList<Models.Agentes.Comun.Agente> AgentesSinAnticipo { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Proceso = await _context.Procesos.AsNoTracking().FirstOrDefaultAsync(m => m.ProcesoID == id);

            if (Proceso == null)
            {
                return NotFound();
            }

            AgentesSinAnticipo = await _context.AgentesComun.AsNoTracking().Where(s => !_context.AnticiposComunes.AsNoTracking().Any(p => p.AgenteID == s.AgenteID && p.Periodo == Proceso.Periodo)).OrderBy(s => s.Nombre).ToListAsync();
            
            return Page();
        }

        #endregion Public Methods
    }
}