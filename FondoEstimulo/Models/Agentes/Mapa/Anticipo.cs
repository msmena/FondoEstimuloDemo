using CsvHelper.Configuration.Attributes;

namespace FondoEstimulo.Models.Agentes.Mapa
{
    public class Anticipo
    {
        [Name("Ano")]
        public int Año { get; set; }

        [Name("Mes")]
        public int Mes { get; set; }
        
        [Name("Tipo Liq")]
        public int LiquidacionTipo { get; set; }

        [Name("Nro Liq")]
        public int LiquidacionNro { get; set; }

        [Name("PtaId ")]
        public int PtaId { get; set; }

        [Name("Concepto")]
        public int Concepto { get; set; }

        [Name("Denominacion")]
        public string Denominacion { get; set; }

        [Name("Reajuste")]
        public int Reajuste { get; set; }

        [Name("Unidades")]
        public int Unidades { get; set; }

        [Name("Importe")]
        public string Importe { get; set; }

        [Name("Vencimiento")]
        public string Vencimiento { get; set; }
    }
}
