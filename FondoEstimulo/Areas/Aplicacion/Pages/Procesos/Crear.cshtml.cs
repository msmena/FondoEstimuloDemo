using FondoEstimulo.Models;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Procesos
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
        public Proceso Proceso { get; set; }

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

            string usuario = _userManager.GetUserId(User);

            try
            {
                Proceso.Fecha = DateTime.Today;
                _context.Procesos.Add(Proceso);
                await _context.SaveChangesAsync();

                Evento evento = new();
                evento.Accion = Parametro.EventoAccion.Creacion;
                evento.Origen = Parametro.EventoOrigen.Proceso;
                evento.UsuarioID = usuario;
                evento.RegistroID = Proceso.ProcesoID.ToString();
                evento.Serializar(Proceso);
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();

                IList<Models.Agentes.Comun.Anticipo> anticiposComun = new List<Models.Agentes.Comun.Anticipo>();
                IList<Models.Agentes.Comun.Agente> agentesComun = _context.AgentesComun.ToList();
                Proceso.Registros = agentesComun.Count;

                Parametro parametro = _context.Parametros.FirstOrDefault(m => m.ParametroID == 1);

                foreach (Models.Agentes.Comun.Agente agente in agentesComun)
                {
                    if (agente.Tipo != Parametro.AgenteTipo.Retirado)
                        break;

                    Models.Agentes.Comun.Anticipo anticipo = new();

                    anticipo.Fecha = DateTime.Today;
                    anticipo.AgenteID = agente.AgenteID;
                    anticipo.Agente = agente;
                    anticipo.Periodo = Proceso.Periodo;
                    anticipo.ProcesoID = Proceso.ProcesoID;
                    anticipo.Tipo = agente.Tipo;

                    if (anticipo.Agente == null)
                        anticipo.Agente = _context.AgentesComun.FirstOrDefault(m => m.AgenteID == anticipo.Agente.AgenteID);

                    anticipo.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(m => m.EscalafonID == anticipo.Agente.EscalafonID);
                    anticipo.Agente.BonificacionTitulo = _context.BonificacionesTitulo.FirstOrDefault(n => n.BonificacionTituloID == anticipo.Agente.BonificacionTituloID);
                    anticipo.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(e => e.EscalafonID == anticipo.Agente.BonificacionTituloEscalafonID);

                    anticipo.CalcularTotal(parametro);
                    anticiposComun.Add(anticipo);
                }

                _context.AnticiposComunes.AddRange(anticiposComun);

                IList<Models.Agentes.BOCEP.Anticipo> anticiposBOCEP = new List<Models.Agentes.BOCEP.Anticipo>();
                IList<Models.Agentes.BOCEP.Agente> agentesBOCEP = _context.AgentesBOCEP.ToList();
                Proceso.Registros += agentesBOCEP.Count;

                foreach (Models.Agentes.BOCEP.Agente agente in agentesBOCEP)
                {
                    Models.Agentes.BOCEP.Anticipo anticipo = new();

                    anticipo.Fecha = DateTime.Today;
                    anticipo.AgenteID = agente.AgenteID;
                    anticipo.Agente = agente;
                    anticipo.Periodo = Proceso.Periodo;
                    anticipo.ProcesoID = Proceso.ProcesoID;

                    if (anticipo.Agente == null)
                        anticipo.Agente = _context.AgentesBOCEP.FirstOrDefault(m => m.AgenteID == anticipo.Agente.AgenteID);

                    anticipo.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(m => m.EscalafonID == anticipo.Agente.EscalafonID);
                    anticipo.Agente.BonificacionTitulo = _context.BonificacionesTitulo.FirstOrDefault(n => n.BonificacionTituloID == anticipo.Agente.BonificacionTituloID);
                    anticipo.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(e => e.EscalafonID == anticipo.Agente.BonificacionTituloEscalafonID);

                    anticipo.CalcularTotal(parametro);
                    anticiposBOCEP.Add(anticipo);
                }

                _context.AnticiposBOCEP.AddRange(anticiposBOCEP);

                IList<Models.Agentes.Funcionario.Anticipo> anticiposFuncionario = new List<Models.Agentes.Funcionario.Anticipo>();
                IList<Models.Agentes.Funcionario.Agente> agentesFuncionario = _context.AgentesFuncionarios.ToList();
                Proceso.Registros += agentesFuncionario.Count;

                foreach (Models.Agentes.Funcionario.Agente agente in agentesFuncionario)
                {
                    Models.Agentes.Funcionario.Anticipo anticipo = new();

                    anticipo.Fecha = DateTime.Today;
                    anticipo.AgenteID = agente.AgenteID;
                    anticipo.Agente = agente;
                    anticipo.Periodo = Proceso.Periodo;
                    anticipo.ProcesoID = Proceso.ProcesoID;

                    if (anticipo.Agente == null)
                        anticipo.Agente = _context.AgentesFuncionarios.FirstOrDefault(m => m.AgenteID == anticipo.Agente.AgenteID);

                    anticipo.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(m => m.EscalafonID == anticipo.Agente.EscalafonID);
                    anticipo.Agente.BonificacionTitulo = _context.BonificacionesTitulo.FirstOrDefault(n => n.BonificacionTituloID == anticipo.Agente.BonificacionTituloID);
                    anticipo.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(e => e.EscalafonID == anticipo.Agente.BonificacionTituloEscalafonID);

                    anticipo.CalcularTotal(parametro);
                    anticiposFuncionario.Add(anticipo);
                }

                _context.AnticiposFuncionarios.AddRange(anticiposFuncionario);

                foreach (Models.Agentes.Comun.Anticipo anticipo in anticiposComun)
                {
                    Evento eventoAgente = new();
                    eventoAgente.Accion = Parametro.EventoAccion.Creacion;
                    eventoAgente.Origen = Parametro.EventoOrigen.Anticipo;
                    eventoAgente.UsuarioID = usuario;
                    eventoAgente.RegistroID = anticipo.AnticipoID.ToString();
                    eventoAgente.Serializar(anticipo);
                    _context.Eventos.Add(eventoAgente);
                }

                foreach (Models.Agentes.BOCEP.Anticipo anticipo in anticiposBOCEP)
                {
                    Evento eventoAgente = new();
                    eventoAgente.Accion = Parametro.EventoAccion.Creacion;
                    eventoAgente.Origen = Parametro.EventoOrigen.Anticipo;
                    eventoAgente.UsuarioID = usuario;
                    eventoAgente.RegistroID = anticipo.AnticipoID.ToString();
                    eventoAgente.Serializar(anticipo);
                    _context.Eventos.Add(eventoAgente);
                }

                foreach (Models.Agentes.Funcionario.Anticipo anticipo in anticiposFuncionario)
                {
                    Evento eventoAgente = new();
                    eventoAgente.Accion = Parametro.EventoAccion.Creacion;
                    eventoAgente.Origen = Parametro.EventoOrigen.Anticipo;
                    eventoAgente.UsuarioID = usuario;
                    eventoAgente.RegistroID = anticipo.AnticipoID.ToString();
                    eventoAgente.Serializar(anticipo);
                    _context.Eventos.Add(eventoAgente);
                }

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