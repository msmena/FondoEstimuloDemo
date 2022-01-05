using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Areas.Aplicacion.Pages.Procesos
{
    public class CompararModel : PageModel
    {
        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<IndexModel>();
        private readonly Data.FondoEstimuloContext _context;
        private readonly UserManager<Usuario> _userManager;

        public CompararModel(Data.FondoEstimuloContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Required]
        [Display(Name = "Archivo")]
        [BindProperty]
        public IFormFile FormFile { get; set; }

        [BindProperty]
        [Required]
        public DateTime Periodo { get; set; }

        [BindProperty]
        [Required]
        public int Pestaña { get; set; } = 1;

        public string ErrorMessage { get; set; }
        public List<string> ResultadoMessage { get; set; }

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

            string[] extensionesPermitidas = { ".xls", ".xlsx" };
            var ext = Path.GetExtension(FormFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !extensionesPermitidas.Contains(ext))
            {
                ErrorMessage = "Extensión de archivo no permitido.";
                return Page();
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                var archivoUbicacion = Path.GetTempFileName();
                using (var streamReader = System.IO.File.Create(archivoUbicacion))
                {
                    await FormFile.CopyToAsync(streamReader);
                }

                FileInfo archivo = new(archivoUbicacion);
                using var paquete = new ExcelPackage(archivo);
                await paquete.LoadAsync(archivo);
                Pestaña--;
                var hoja = paquete.Workbook.Worksheets[Pestaña];
                int fila = 1;
                ResultadoMessage = new List<string>();
                bool esAnticipoComun = false;

                while (string.IsNullOrWhiteSpace(hoja.Cells[fila, 1].Value?.ToString()) == false)
                {
                    _ = int.TryParse(hoja.Cells[fila, 1].Value?.ToString(), out int dni);

                    if (dni == 0)
                    {
                        fila++;
                        continue;
                    }

                    Models.Agentes.Agente agente = _context.Agentes.FirstOrDefault(a => a.DNI == dni);

                    if (agente == null)
                    {
                        ResultadoMessage.Add($"Agente no encontrado. DNI: {dni:N0}; Nombre: {hoja.Cells[fila, 3].Value?.ToString()}.");
                        fila++;
                        continue;
                    }

                    Models.Agentes.Anticipo anticipo = _context.AnticiposComunes.FirstOrDefault(a => a.AgenteID == agente.AgenteID && a.Periodo == Periodo);

                    if (anticipo == null)
                    {
                        anticipo = _context.AnticiposBOCEP.FirstOrDefault(a => a.AgenteID == agente.AgenteID && a.Periodo == Periodo);

                        if (anticipo == null)
                        {
                            anticipo = _context.AnticiposFuncionarios.FirstOrDefault(a => a.AgenteID == agente.AgenteID && a.Periodo == Periodo);

                            if (anticipo == null)
                            {
                                ResultadoMessage.Add($"DNI: {agente.DNI:N0}; Nombre: {agente.Nombre}: Anticipo no encontrado.");
                                fila++;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        esAnticipoComun = true;
                    }

                    _ = decimal.TryParse(hoja.Cells[fila, 2].Value?.ToString(), out decimal fondoImportado);
                    fondoImportado = decimal.Round(fondoImportado, 2);
                    if ((anticipo.FondoEstimulo - fondoImportado) > 1)
                    {
                        if (esAnticipoComun)
                        {
                            Models.Agentes.Comun.Anticipo anticipoComun = (Models.Agentes.Comun.Anticipo)anticipo;

                            _ = decimal.TryParse(hoja.Cells[fila, 17].Value?.ToString(), out decimal fondoFijoImportado);
                            if (anticipoComun.FondoFijo != fondoFijoImportado)
                                ResultadoMessage.Add($"DNI: {agente.DNI:N0}; Nombre: {agente.Nombre}: Diferencia de importe fondo fijo. Calculado: {anticipoComun.FondoFijo:C}; Planilla: {fondoFijoImportado:C}.");

                            _ = decimal.TryParse(hoja.Cells[fila, 16].Value?.ToString(), out decimal ley6655Importado);
                            ley6655Importado = decimal.Round(ley6655Importado, 2);
                            if (anticipoComun.Ley6655 != ley6655Importado)
                                ResultadoMessage.Add($"DNI: {agente.DNI:N0}; Nombre: {agente.Nombre}: Diferencia de importe Ley 6655. Calculado: {anticipoComun.Ley6655:C}; Planilla: {ley6655Importado:C}.");

                            _ = decimal.TryParse(hoja.Cells[fila, 19].Value?.ToString(), out decimal adecuacionEscalafonariaImportado);
                            adecuacionEscalafonariaImportado = decimal.Round(adecuacionEscalafonariaImportado, 2);
                            if (anticipoComun.AdecuacionEscalafonaria != adecuacionEscalafonariaImportado)
                                ResultadoMessage.Add($"DNI: {agente.DNI:N0}; Nombre: {agente.Nombre}: Diferencia de importe fondo fijo. Calculado: {anticipoComun.AdecuacionEscalafonaria:C}; Planilla: {adecuacionEscalafonariaImportado:C}.");

                            _ = decimal.TryParse(hoja.Cells[fila, 12].Value?.ToString(), out decimal dedicacionImportado);
                            dedicacionImportado = decimal.Round(dedicacionImportado, 2);
                            if (anticipoComun.Dedicacion != dedicacionImportado)
                            ResultadoMessage.Add($"DNI: {agente.DNI:N0}; Nombre: {agente.Nombre}: Diferencia importe dedicacion. Calculado: {anticipoComun.Dedicacion:C}; Planilla: {dedicacionImportado:C}.");
                        }

                        _ = int.TryParse(hoja.Cells[fila, 10].Value?.ToString(), out int antiguedadImportado);
                        if (anticipo.AñosAntiguedad != antiguedadImportado)
                            ResultadoMessage.Add($"DNI: {agente.DNI:N0}; Nombre: {agente.Nombre}: Diferencia años de antiguedad. Calculado: {anticipo.AñosAntiguedad}; Planilla: {antiguedadImportado}.");

                        _ = decimal.TryParse(hoja.Cells[fila, 11].Value?.ToString(), out decimal incompatibilidadImportado);
                        incompatibilidadImportado = decimal.Round(incompatibilidadImportado, 2);
                        if (anticipo.Incompatibilidad != incompatibilidadImportado)
                            ResultadoMessage.Add($"DNI: {agente.DNI:N0}; Nombre: {agente.Nombre}: Diferencia importe incompatibilidad. Calculado: {anticipo.Incompatibilidad:C}; Planilla: {incompatibilidadImportado:C}.");

                        _ = decimal.TryParse(hoja.Cells[fila, 15].Value?.ToString(), out decimal tituloImportado);
                        tituloImportado = decimal.Round(tituloImportado, 2);
                        if ((anticipo.Titulo - tituloImportado) > 1)
                            ResultadoMessage.Add($"DNI: {agente.DNI:N0}; Nombre: {agente.Nombre}: Diferencia importe titulo. Calculado: {anticipo.Titulo:C}; Planilla: {tituloImportado:C}.");

                        ResultadoMessage.Add($"DNI: {agente.DNI:N0}; Nombre: {agente.Nombre}: Diferencia de importe fondo estimulo. Calculado: {anticipo.FondoEstimulo:C}; Planilla: {fondoImportado:C}.");
                    }

                    fila++;
                }

                return Page();
                // return RedirectToAction("./Comparar", new { resultado = resultadoProceso.ToString() });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return RedirectToAction("./Comparar", new { importError = true });
            }
        }
    }
}