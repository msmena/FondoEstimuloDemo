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

namespace FondoEstimulo.Areas.Aplicacion.Pages.Agentes
{
    public class ImportarModel : PageModel
    {
        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ImportarModel(Data.FondoEstimuloContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Required]
        [BindProperty]
        [Display(Name = "Archivo")]
        public IFormFile FormFile { get; set; }

        public string ErrorMessage { get; set; }
        public string RegistrosModificadosMessage { get; set; }

        public IActionResult OnGet(bool? importError = false, int? cantidadRegistrosModificados = -1, int? cantidadTotal = 0)
        {
            if (importError.GetValueOrDefault())
                ErrorMessage = "Ha ocurrido un error al intentar importar el archivo.";

            if (cantidadRegistrosModificados > -1)
                RegistrosModificadosMessage = string.Format("{0} registros importados de un total de {1} registros leídos.", 
                    cantidadRegistrosModificados,
                    cantidadTotal);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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
                contenido = contenido.Replace(",", " ");
                contenido = contenido.Replace(";", ",");
                contenido = contenido.Replace("ñ", "n");
                contenido = contenido.Replace("ó", "o");
                System.IO.File.WriteAllText(archivoUbicacion, contenido);

                using var fs = new StreamReader(archivoUbicacion);
                List<Models.Agentes.Mapa.Agente> registros = new CsvHelper.CsvReader(fs,
                    System.Globalization.CultureInfo.InvariantCulture)
                    .GetRecords<Models.Agentes.Mapa.Agente>().ToList();
                int cantidadRegistros = registros.Count;
                IList<Models.Agentes.Agente> agentesModificados = new List<Models.Agentes.Agente>();
                using var transaccion = _context.Database.BeginTransaction();
                Log.Information("Se han detectado: {0} registros", cantidadRegistros);

                for (int i = 0; i < cantidadRegistros; i++)
                {
                    Models.Agentes.Mapa.Agente registro = registros.ElementAt(i);
                    if (registro.JurId != 20)
                        continue;

                    Models.Agentes.Agente agente, agenteOriginal;
                    agente = agenteOriginal = _context.Agentes.FirstOrDefault(a => a.DNI == registro.Documento);
                    if (agente == null)
                        continue;

                    // int originalEscalafonID = agente.EscalafonID;
                    // agente.AsignarCargo(registro.Funcion, _context);

                    if (agente.AgenteIDExterno != registro.PtaId)
                    {
                        Models.Evento evento = new();
                        evento.Accion = Models.Parametro.EventoAccion.Modificacion;
                        evento.Origen = Models.Parametro.EventoOrigen.Agente;
                        evento.UsuarioID = _userManager.GetUserId(User);
                        agentesModificados.Add(agente);
                        evento.RegistroID = agente.AgenteID.ToString();
                        evento.Serializar(agente);
                        _context.Eventos.Add(evento);

                        agente.AgenteIDExterno = registro.PtaId;
                    }
                }

                await _context.SaveChangesAsync();
                transaccion.Commit();
                Log.Information("Cantidad de registros modificados: {0}", agentesModificados.Count);
                return RedirectToAction("./Importar", new { cantidadRegistrosModificados = agentesModificados.Count, cantidadTotal = cantidadRegistros });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return RedirectToAction("./Importar", new { importError = true });
            }
        }
    }
}