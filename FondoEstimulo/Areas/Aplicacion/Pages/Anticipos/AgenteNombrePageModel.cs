using FondoEstimulo.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Anticipos
{
    public class AgenteNombrePageModel : PageModel
    {
        #region Public Properties

        public SelectList AgenteNameSL { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void PopulateAgentesComunesDropDownList(FondoEstimuloContext _context,
            object selectedAgente = null)
        {
            var agenteQuery = from d in _context.AgentesComun
                              orderby d.Nombre
                              select d;

            AgenteNameSL = new SelectList(agenteQuery.AsNoTracking(),
                        "AgenteID", "Nombre", selectedAgente);
        }

        public void PopulateAgentesBOCEPDropDownList(FondoEstimuloContext _context,
            object selectedAgente = null)
        {
            var agenteQuery = from d in _context.AgentesBOCEP
                              orderby d.Nombre
                              select d;

            AgenteNameSL = new SelectList(agenteQuery.AsNoTracking(),
                        "AgenteID", "Nombre", selectedAgente);
        }

        public void PopulateAgentesFuncionariosDropDownList(FondoEstimuloContext _context,
            object selectedAgente = null)
        {
            var agenteQuery = from d in _context.AgentesFuncionarios
                              orderby d.Nombre
                              select d;

            AgenteNameSL = new SelectList(agenteQuery.AsNoTracking(),
                        "AgenteID", "Nombre", selectedAgente);
        }

        #endregion Public Methods
    }
}