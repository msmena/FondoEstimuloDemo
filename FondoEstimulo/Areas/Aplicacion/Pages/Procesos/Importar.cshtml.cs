using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Procesos
{
    public class ImportarModel : PageModel
    {
        #region Private Fields

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public ImportarModel(Data.FondoEstimuloContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        [Required]
        [BindProperty]
        [Display(Name = "Archivo")]
        public IFormFile FormFile { get; set; }

        public string ErrorMessage { get; set; }
        public string RegistrosAgregadosMessage { get; set; }
        public List<string> AgentesNoEncontradosMessage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public IActionResult OnGet(bool? importError = false)
        {
            if (importError.GetValueOrDefault())
                ErrorMessage = "Ha ocurrido un error al intentar importar el archivo.";

            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (FormFile == null || FormFile.Length <= 0)
                return Page();

            string[] extensionesPermitidas = { ".csv" };
            var ext = Path.GetExtension(FormFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !extensionesPermitidas.Contains(ext))
            {
                ErrorMessage = "Extensión de archivo no permitido.";
                return Page();
            }

            try
            {
                var archivoUbicacion = Path.GetTempFileName();
                using (var streamReader = System.IO.File.Create(archivoUbicacion))
                {
                    await FormFile.CopyToAsync(streamReader);
                }

                string contenido = System.IO.File.ReadAllText(archivoUbicacion, System.Text.Encoding.Latin1);
                contenido = contenido.Replace(";", ",");
                contenido = contenido.Replace("ñ", "n");
                contenido = contenido.Replace("ó", "o");
                System.IO.File.WriteAllText(archivoUbicacion, contenido);

                using var fs = new StreamReader(archivoUbicacion);
                using var lector = new CsvHelper.CsvReader(fs, System.Globalization.CultureInfo.InvariantCulture);
                var registros = lector.GetRecords<Models.Agentes.Mapa.Anticipo>().ToList();
                int cantidadRegistros = registros.Count;
                int cantidadRegistrosProcesados = 0;

                Log.Information("Se han detectado: {0} registros", cantidadRegistros);

                decimal importe = 0;
                DateTime periodoFiscal = DateTime.Today;

                IList<Models.Agentes.Comun.Anticipo> anticiposComun = new List<Models.Agentes.Comun.Anticipo>();
                IList<Models.Agentes.BOCEP.Anticipo> anticiposBOCEP = new List<Models.Agentes.BOCEP.Anticipo>();
                IList<Models.Agentes.Funcionario.Anticipo> anticiposFuncionario = new List<Models.Agentes.Funcionario.Anticipo>();
                List<string> agentesNoEncontrados = new();

                Models.Parametro parametro = _context.Parametros.FirstOrDefault(m => m.ParametroID == 1);

                for (int i = 0; i < cantidadRegistros; i++)
                {
                    Models.Agentes.Mapa.Anticipo registro = registros.ElementAt(i);
                    periodoFiscal = new DateTime(registro.Año, registro.Mes, 1);
                    _ = decimal.TryParse(registro.Importe, out importe);

                    if (importe > 0)
                        importe /= 100;

                    Models.Agentes.Comun.Agente agenteComun = _context.AgentesComun
                        .FirstOrDefault(a => a.AgenteIDExterno == registro.PtaId);

                    if (agenteComun != null && agenteComun.Tipo != Models.Parametro.AgenteTipo.Jubilado)
                    {
                        Models.Agentes.Comun.Anticipo anticipoComun = anticiposComun
                            .FirstOrDefault(a => a.AgenteID == agenteComun.AgenteID);

                        if (anticipoComun == null)
                        {
                            anticipoComun = new();
                            anticipoComun.Fecha = DateTime.Today;
                            anticipoComun.AgenteID = agenteComun.AgenteID;
                            anticipoComun.Periodo = periodoFiscal;
                            anticipoComun.Agente = _context.AgentesComun.FirstOrDefault(m => m.AgenteID == agenteComun.AgenteID);
                            anticipoComun.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(m => m.EscalafonID == anticipoComun.Agente.EscalafonID);
                            anticiposComun.Add(anticipoComun);
                        }

                        anticipoComun.AsignarConcepto(registro.Concepto, registro.Reajuste, importe);
                        cantidadRegistrosProcesados++;
                        // Log.Information("Agente: {0}; Concepto: {1}; Importe: {2}", agenteComun.Nombre, registro.Concepto, importe);
                    }
                    else
                    {
                        Models.Agentes.BOCEP.Agente agenteBocep = _context.AgentesBOCEP
                            .FirstOrDefault(a => a.AgenteIDExterno == registro.PtaId);

                        if (agenteBocep != null)
                        {
                            Models.Agentes.BOCEP.Anticipo anticipoBOCEP = anticiposBOCEP
                                .FirstOrDefault(a => a.AgenteID == agenteBocep.AgenteID);

                            if (anticipoBOCEP == null)
                            {
                                anticipoBOCEP = new();
                                anticipoBOCEP.Fecha = DateTime.Today;
                                anticipoBOCEP.AgenteID = agenteBocep.AgenteID;
                                anticipoBOCEP.Periodo = periodoFiscal;
                                anticipoBOCEP.Agente = _context.AgentesBOCEP.FirstOrDefault(m => m.AgenteID == agenteBocep.AgenteID);
                                anticipoBOCEP.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(m => m.EscalafonID == anticipoBOCEP.Agente.EscalafonID);
                                anticipoBOCEP.Agente.BonificacionTitulo = _context.BonificacionesTitulo.FirstOrDefault(m => m.BonificacionTituloID == anticipoBOCEP.Agente.BonificacionTituloID);
                                anticipoBOCEP.Agente.BonificacionTituloEscalafon = _context.EscalafonGeneral.FirstOrDefault(m => m.EscalafonID == anticipoBOCEP.Agente.BonificacionTituloEscalafonID);
                                anticiposBOCEP.Add(anticipoBOCEP);
                            }

                            anticipoBOCEP.AsignarConcepto(registro.Concepto, registro.Reajuste, importe);
                            cantidadRegistrosProcesados++;
                        }
                        else
                        {
                            Models.Agentes.Funcionario.Agente agenteFuncionario = _context.AgentesFuncionarios
                                .FirstOrDefault(a => a.AgenteIDExterno == registro.PtaId);

                            if (agenteFuncionario != null)
                            {
                                Models.Agentes.Funcionario.Anticipo anticipoFuncionario = anticiposFuncionario
                                    .FirstOrDefault(a => a.AgenteID == agenteFuncionario.AgenteID);

                                if (anticipoFuncionario == null)
                                {
                                    anticipoFuncionario = new();
                                    anticipoFuncionario.Fecha = DateTime.Today;
                                    anticipoFuncionario.AgenteID = agenteFuncionario.AgenteID;
                                    anticipoFuncionario.Periodo = periodoFiscal;
                                    anticipoFuncionario.Agente = _context.AgentesFuncionarios.FirstOrDefault(m => m.AgenteID == agenteFuncionario.AgenteID);
                                    anticipoFuncionario.Agente.Escalafon = _context.EscalafonGeneral.FirstOrDefault(m => m.EscalafonID == anticipoFuncionario.Agente.EscalafonID);
                                    anticiposFuncionario.Add(anticipoFuncionario);
                                }

                                anticipoFuncionario.AsignarConcepto(registro.Concepto, registro.Reajuste, importe);
                                cantidadRegistrosProcesados++;
                            }
                            else
                            {
                                if (!agentesNoEncontrados.Contains(registro.PtaId.ToString()))
                                {
                                    agentesNoEncontrados.Add(registro.PtaId.ToString());
                                    Log.Information("Agente no encontrado. Identificacion: {0}", registro.PtaId);
                                }
                            }
                        }
                    }
                }

                Log.Information("Se ha terminado la importación de registros");

                Models.Proceso procesoMensual = new();
                procesoMensual.Fecha = DateTime.Today;
                procesoMensual.Periodo = periodoFiscal;
                procesoMensual.Registros = anticiposComun.Count + anticiposBOCEP.Count + anticiposFuncionario.Count;
                _context.Procesos.Add(procesoMensual);
                await _context.SaveChangesAsync();

                Models.Evento evento = new();
                evento.Accion = Models.Parametro.EventoAccion.Creacion;
                evento.Origen = Models.Parametro.EventoOrigen.Proceso;
                evento.UsuarioID = _userManager.GetUserId(User);
                evento.RegistroID = procesoMensual.ProcesoID.ToString();
                evento.Serializar(procesoMensual);
                _context.Eventos.Add(evento);

                using var transaccion = _context.Database.BeginTransaction();

                foreach (Models.Agentes.Comun.Anticipo anticipo in anticiposComun)
                {
                    anticipo.ProcesoID = procesoMensual.ProcesoID;
                    anticipo.CalcularTotalImportacion(parametro);
                    _context.AnticiposComunes.Add(anticipo);
                }

                foreach (Models.Agentes.BOCEP.Anticipo anticipo in anticiposBOCEP)
                {
                    anticipo.ProcesoID = procesoMensual.ProcesoID;
                    anticipo.CalcularTotalImportacion(parametro);
                    _context.AnticiposBOCEP.Add(anticipo);
                }

                foreach (Models.Agentes.Funcionario.Anticipo anticipo in anticiposFuncionario)
                {
                    anticipo.ProcesoID = procesoMensual.ProcesoID;
                    anticipo.CalcularTotalImportacion(parametro);
                    _context.AnticiposFuncionarios.Add(anticipo);
                }

                await _context.SaveChangesAsync();
                foreach (Models.Agentes.Comun.Anticipo anticipo in anticiposComun)
                {
                    Models.Evento eventoComun = new();
                    eventoComun.Accion = Models.Parametro.EventoAccion.Creacion;
                    eventoComun.Origen = Models.Parametro.EventoOrigen.Anticipo;
                    eventoComun.UsuarioID = _userManager.GetUserId(User);
                    eventoComun.RegistroID = anticipo.AnticipoID.ToString();
                    eventoComun.Serializar(anticipo);
                    _context.Eventos.Add(eventoComun);
                }

                foreach (Models.Agentes.BOCEP.Anticipo anticipo in anticiposBOCEP)
                {
                    Models.Evento eventoBOCEP = new();
                    eventoBOCEP.Accion = Models.Parametro.EventoAccion.Creacion;
                    eventoBOCEP.Origen = Models.Parametro.EventoOrigen.Anticipo;
                    eventoBOCEP.UsuarioID = _userManager.GetUserId(User);
                    eventoBOCEP.RegistroID = anticipo.AnticipoID.ToString();
                    eventoBOCEP.Serializar(anticipo);
                    _context.Eventos.Add(eventoBOCEP);
                }

                foreach (Models.Agentes.Funcionario.Anticipo anticipo in anticiposFuncionario)
                {
                    Models.Evento eventoFuncionario = new();
                    eventoFuncionario.Accion = Models.Parametro.EventoAccion.Creacion;
                    eventoFuncionario.Origen = Models.Parametro.EventoOrigen.Anticipo;
                    eventoFuncionario.UsuarioID = _userManager.GetUserId(User);
                    eventoFuncionario.RegistroID = anticipo.AnticipoID.ToString();
                    eventoFuncionario.Serializar(anticipo);
                    _context.Eventos.Add(eventoFuncionario);
                }

                await _context.SaveChangesAsync();
                transaccion.Commit();
                Log.Information("Se han importado {0} registros anticipos comunes.", anticiposComun.Count);
                Log.Information("Se han importado {0} registros anticipos BOCEP.", anticiposBOCEP.Count);
                Log.Information("Se han importado {0} registros anticipos funcionarios.", anticiposFuncionario.Count);

                RegistrosAgregadosMessage = string.Format($"{cantidadRegistrosProcesados} registros importados de un total de {cantidadRegistros} leídos.");
                AgentesNoEncontradosMessage = agentesNoEncontrados;

                return Page();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return RedirectToAction("./Importar", new { importError = true });
            }
        }

        #endregion Public Methods
    }
}