using FondoEstimulo.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Agentes
{
    public class AgentesBasePageModel : PageModel
    {
        #region Public Properties

        public SelectList EscalafonNameSL { get; set; }

        public SelectList BonificacionTituloSL { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void PopulateEscalafonDropDownList(FondoEstimuloContext _context,
            object selectedEscalafon = null)
        {
            var escalafonQuery = from d in _context.EscalafonGeneral
                                 orderby d.Apartado, d.Grupo descending
                                 select d;

            EscalafonNameSL = new SelectList(escalafonQuery.AsNoTracking(),
                        "EscalafonID", "EscalafonApartadoGrupoIdentificador", selectedEscalafon);
        }

        public void PopulateBonificacionTituloDropDownList(FondoEstimuloContext _context,
            object selectedTitulo = null)
        {
            var tituloQuery = from d in _context.BonificacionesTitulo
                              orderby d.Valor
                              select d;

            BonificacionTituloSL = new SelectList(tituloQuery.AsNoTracking(),
                "BonificacionTituloID", "Descripcion", selectedTitulo);
        }

        #endregion Public Methods
    }
}