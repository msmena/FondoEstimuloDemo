using CsvHelper.Configuration.Attributes;

namespace FondoEstimulo.Models.Agentes.Mapa
{
    public class Agente
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

        [Name("JurId")]
        public int JurId { get; set; }

        [Name("Juridiccion Descripcion")]
        public string JurDescripcion { get; set; }

        [Name("Escalafon")]
        public int Escalafon { get; set; }

        [Name("Cargo")]
        public int Cargo { get; set; }

        [Name("Funcion")]
        public int Funcion { get; set; }

        [Name("Cargo denominacion")]
        public string CargoDenominacion { get; set; }

        [Name("Sit Revista")]
        public int SitRevista { get; set; }

        [Name("Pta Tipo")]
        public int PtaTipo { get; set; }

        [Name("Pefijo")]
        public int Prefijo { get; set; }

        [Name("Documento")]
        public int Documento { get; set; }

        [Name("Digito")]
        public int Digito { get; set; }

        [Name("Apellido y Nombre")]
        public string Nombre { get; set; }

        [Name("Oficina")]
        public int Oficina { get; set; }

        [Name("Anexo")]
        public int Anexo { get; set; }

        [Name("Oficina Denominacion")]
        public string OficinaDenominacion { get; set; }

        [Name("Dias Tra.")]
        public int DiasTranscurridos { get; set; }

        [Name("Programa")]
        public int Programa { get; set; }

        [Name("Subprograma")]
        public int SubPrograma { get; set; }

        [Name("Obra")]
        public int Obra { get; set; }
    }
}