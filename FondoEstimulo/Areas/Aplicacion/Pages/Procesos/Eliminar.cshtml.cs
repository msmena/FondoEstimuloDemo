using FondoEstimulo.Models;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Procesos
{
    public class DeleteModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public DeleteModel(Data.FondoEstimuloContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        [BindProperty]
        public Proceso Proceso { get; set; }

        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
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

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Ha ocurrido un error al intentar crear el registro.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Proceso = await _context.Procesos.AsNoTracking().FirstOrDefaultAsync(m => m.ProcesoID == id);

            if (Proceso != null)
            {
                try
                {
                    using var transaccion = _context.Database.BeginTransaction();
                    var anticiposCQuery = _context.AnticiposComunes.AsQueryable();
                    anticiposCQuery = anticiposCQuery.Where(s => s.ProcesoID == Proceso.ProcesoID);
                    IList<Models.Agentes.Comun.Anticipo> anticiposComun = await anticiposCQuery.ToListAsync();
                    _context.AnticiposComunes.RemoveRange(anticiposComun);
                    Log.Information("Cantidad de anticipos comunes eliminados: {cantidad}", anticiposComun.Count.ToString());

                    foreach (Models.Agentes.Comun.Anticipo anticipo in anticiposComun)
                    {
                        Evento eventoAnticipo = new();
                        eventoAnticipo.Accion = Parametro.EventoAccion.Eliminacion;
                        eventoAnticipo.Origen = Parametro.EventoOrigen.Anticipo;
                        eventoAnticipo.UsuarioID = _userManager.GetUserId(User);
                        eventoAnticipo.RegistroID = anticipo.AnticipoID.ToString();
                        eventoAnticipo.Serializar(anticipo);
                        _context.Eventos.Add(eventoAnticipo);
                    }

                    var anticiposBQuery = _context.AnticiposBOCEP.AsQueryable();
                    anticiposBQuery = anticiposBQuery.Where(s => s.ProcesoID == Proceso.ProcesoID);
                    IList<Models.Agentes.BOCEP.Anticipo> anticiposBOCEP = await anticiposBQuery.ToListAsync();
                    _context.AnticiposBOCEP.RemoveRange(anticiposBOCEP);
                    Log.Information("Cantidad de anticipos BOCEP eliminados: {cantidad}", anticiposBOCEP.Count.ToString());

                    foreach (Models.Agentes.BOCEP.Anticipo anticipo in anticiposBOCEP)
                    {
                        Evento eventoAnticipo = new();
                        eventoAnticipo.Accion = Parametro.EventoAccion.Eliminacion;
                        eventoAnticipo.Origen = Parametro.EventoOrigen.Anticipo;
                        eventoAnticipo.UsuarioID = _userManager.GetUserId(User);
                        eventoAnticipo.RegistroID = anticipo.AnticipoID.ToString();
                        eventoAnticipo.Serializar(anticipo);
                        _context.Eventos.Add(eventoAnticipo);
                    }

                    var anticiposFQuery = _context.AnticiposFuncionarios.AsQueryable();
                    anticiposFQuery = anticiposFQuery.Where(s => s.ProcesoID == Proceso.ProcesoID);
                    IList<Models.Agentes.Funcionario.Anticipo> anticiposFuncionario = await anticiposFQuery.ToListAsync();
                    _context.AnticiposFuncionarios.RemoveRange(anticiposFuncionario);
                    Log.Information("Cantidad de anticipos funcionarios eliminados: {cantidad}", anticiposFuncionario.Count.ToString());

                    foreach (Models.Agentes.Funcionario.Anticipo anticipo in anticiposFuncionario)
                    {
                        Evento eventoAnticipo = new();
                        eventoAnticipo.Accion = Parametro.EventoAccion.Eliminacion;
                        eventoAnticipo.Origen = Parametro.EventoOrigen.Anticipo;
                        eventoAnticipo.UsuarioID = _userManager.GetUserId(User);
                        eventoAnticipo.RegistroID = anticipo.AnticipoID.ToString();
                        eventoAnticipo.Serializar(anticipo);
                        _context.Eventos.Add(eventoAnticipo);
                    }

                    _context.Procesos.Remove(Proceso);

                    Evento eventoProceso = new();
                    eventoProceso.Accion = Parametro.EventoAccion.Eliminacion;
                    eventoProceso.Origen = Parametro.EventoOrigen.Proceso;
                    eventoProceso.UsuarioID = _userManager.GetUserId(User);
                    eventoProceso.RegistroID = Proceso.ProcesoID.ToString();
                    eventoProceso.Serializar(Proceso);
                    _context.Eventos.Add(eventoProceso);

                    await _context.SaveChangesAsync();
                    transaccion.Commit();
                }
                catch (DbUpdateException ex)
                {
                    Log.Error(ex, ErrorMessage);
                    return RedirectToAction("./Eliminar", new { id, saveChangesError = true });
                }
            }

            return RedirectToPage("./Listado");
        }

        #endregion Public Methods
    }
}