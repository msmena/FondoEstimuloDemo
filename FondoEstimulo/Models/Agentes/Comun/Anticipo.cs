using FondoEstimulo.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FondoEstimulo.Models.Agentes.Comun
{
    [Table("AnticiposAgentesComunes")]
    public class Anticipo : Agentes.Anticipo
    {
        #region Public Properties

        /// <summary>
        /// Identificación del agente al que pertenece el anticipo.
        /// </summary>
        [Display(Name = "Agente")]
        public int AgenteID { get; set; }

        /// <summary>
        /// Tipo de agente vigente.
        /// </summary>
        public Parametro.AgenteTipo Tipo { get; set; }

        /// <summary>
        /// Valor monetario del Complemento básico al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Complemento básico")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal ComplementoBasico { get; set; }

        /// <summary>
        /// Valor monetario del Suplemento remunerativo al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Suplemento remunerativo")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SuplementoRemunerativo { get; set; }

        /// <summary>
        /// Fecha de retiro. Cuando el agente es retirado: se calcula el concepto antigüedad hasta esta fecha.
        /// </summary>
        [Display(Name = "Fecha de Retiro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime? RetiroFecha { get; set; }

        /// <summary>
        /// Coeficiente del concepto Asignación por Reparación Histórica al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Coeficiente de Asignación por Reparación Histórica")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal ReparacionHistoricaCoeficiente { get; set; }

        /// <summary>
        /// Valor monetario del concepto Asignación por Reparación Histórica.
        /// </summary>
        [Display(Name = "Asignación por Reparación Histórica")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal ReparacionHistorica { get; set; }

        /// <summary>
        /// Coeficiente del concepto Dedicación, al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Coeficiente de Dedicación")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal DedicacionCoeficiente { get; set; }

        /// <summary>
        /// Valor monetario del concepto Dedicación.
        /// </summary>
        [Display(Name = "Dedicación")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Dedicacion { get; set; }

        /// <summary>
        /// Valor monetario del Complemento básico del escalafón al que se le aplica la bonificación por titulo, al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Título escalafón complemento básico")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal TituloEscalafonComplementoBasico { get; set; }

        /// <summary>
        /// Coeficiente del concepto Bonificación Ley 6655.
        /// </summary>
        [Display(Name = "Coeficiente de Bonificación Ley 6655")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal Ley6655Coeficiente { get; set; }

        /// <summary>
        /// Valor monetario del concepto Bonificación 6655.
        /// </summary>
        [Display(Name = "Bonificación Ley 6655")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Ley6655 { get; set; }

        /// <summary>
        /// Valor monetario del concepto Fondo fijo.
        /// </summary>
        [Display(Name = "Fondo fijo")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal FondoFijo { get; set; }

        /// <summary>
        /// Coeficiente del concepto Riesgo de caja.
        /// </summary>
        [Display(Name = "Coeficiente de Riesgo de Caja")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal RiesgoCajaCoeficiente { get; set; }

        /// <summary>
        /// Valor monetario del concepto Riesgo de caja.
        /// </summary>
        [Display(Name = "Riesgo de caja")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal RiesgoCaja { get; set; }

        /// <summary>
        /// Valor monetario del concepto Asignación personal por adecuación escalafonaria.
        /// </summary>
        [Display(Name = "Asignación personal por adecuación escalafonaria")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal AdecuacionEscalafonaria { get; set; }

        /// <summary>
        /// Valor monetario del concepto Subrogancia.
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Subrogancia { get; set; }

        /// <summary>
        /// Coeficiente del concepto Sup. Rep. no Bonificable.
        /// </summary>
        [Display(Name = "Coeficiente de Sup.Rep. No Bonif.")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal SRNoBonificableCoeficiente { get; set; }

        /// <summary>
        /// Valor monetario del concepto Fondo Real.
        /// Se calcula: FondoEstimulo * SRNoBonificableCoeficiente.
        /// </summary>
        [Display(Name = "Fondo Real")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal FondoReal { get; set; }

        /// <summary>
        /// Agente del cual se hace la liquidación.
        /// </summary>
        public Agente Agente { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Proceso que calcula el total del anticipo cuando se realiza por proceso automático.
        /// </summary>
        /// <param name="parametro">Objeto con los parámetros para cálculos secundarios.</param>
        public void CalcularTotal(Parametro parametro)
        {
            if (Agente == null || Agente.Escalafon == null || Agente.BonificacionTitulo == null || parametro == null)
                return;

            if (Agente.Retiro.HasValue)
                RetiroFecha = Agente.Retiro.Value;

            Escalafon = Agente.Escalafon.EscalafonApartadoGrupoIdentificador;
            Tipo = Agente.Tipo;
            SueldoBasico = Agente.Escalafon.SueldoBasico;
            ComplementoBasico = Agente.Escalafon.ComplementoBasico;
            SuplementoRemunerativo = Agente.Escalafon.SuplementoRemunerativo;
            SubtotalBasico = SueldoBasico + ComplementoBasico + SuplementoRemunerativo;

            DateTime periodo = Periodo;
            // Por defecto tomo la fecha del periodo fiscal para calcular los años de antigüedad.
            // Pero si el agente está retirado y tiene fecha de retiro: tomo esa fecha para hacer el cálculo.
            // El agente no cumple más años de antigüedad al estar retirado.
            if (Agente.Tipo == Parametro.AgenteTipo.Retirado && Agente.Retiro.HasValue)
                periodo = Agente.Retiro.Value;

            if (periodo > Agente.InicioActividades)
            {
                AñosAntiguedad = (int)Math.Floor(periodo.Subtract(Agente.InicioActividades).TotalDays / 365);

                // Comprobación para años bisiestos
                if (Agente.InicioActividades > periodo.AddYears(-AñosAntiguedad))
                    AñosAntiguedad--;
            }

            ReparacionHistoricaCoeficiente = Agente.ReparacionHistorica;
            ReparacionHistorica = SueldoBasico * ReparacionHistoricaCoeficiente;
            Antiguedad = ((SueldoBasico + ComplementoBasico + ReparacionHistorica) * 2 / 100) * AñosAntiguedad;

            if (Agente.Incompatibilidad)
            {
                IncompatibilidadCoeficiente = parametro.Incompatibilidad;
                Incompatibilidad = SubtotalBasico * IncompatibilidadCoeficiente;
            }

            if (Agente.Dedicacion > 0)
            {
                DedicacionCoeficiente = Agente.Dedicacion;
                Dedicacion = SueldoBasico * DedicacionCoeficiente;
            }

            TituloTipo = Agente.BonificacionTitulo.Descripcion;
            TituloTipoCoeficiente = Agente.BonificacionTitulo.Valor;

            if (Agente.BonificacionTituloEscalafon != null)
            {
                TituloEscalafon = Agente.BonificacionTituloEscalafon.EscalafonApartadoGrupoIdentificador;
                TituloEscalafonSueldoBasico = Agente.BonificacionTituloEscalafon.SueldoBasico;
                TituloEscalafonComplementoBasico = Agente.BonificacionTituloEscalafon.ComplementoBasico;

                Titulo = (TituloEscalafonSueldoBasico + TituloEscalafonComplementoBasico) * TituloTipoCoeficiente;
            }

            if (Agente.Ley6655)
            {
                Ley6655Coeficiente = parametro.Ley6655;
                Ley6655 = SueldoBasico * Ley6655Coeficiente;
            }

            FondoFijo = Agente.FondoFijo;

            if (Agente.RiesgoCaja > 0)
            {
                RiesgoCajaCoeficiente = Agente.RiesgoCaja;
                RiesgoCaja = parametro.RiesgoCajaBase * RiesgoCajaCoeficiente;
            }

            AdecuacionEscalafonaria = Agente.AdecuacionEscalafonaria;
            Subrogancia = Agente.Subrogancia;
            Subtotal = SubtotalBasico + Antiguedad + Incompatibilidad + Dedicacion + ReparacionHistorica + Titulo;
            Subtotal = Subtotal + Ley6655 + FondoEstimulo + RiesgoCaja + AdecuacionEscalafonaria + Subrogancia + FondoFijo;
            SAC = Subtotal / 12;
            Total = Subtotal + SAC;

            FondoEstimuloCoeficiente = Agente.FondoEstimulo;
            FondoEstimulo = Total * FondoEstimuloCoeficiente;

            if (Agente.Tipo == Parametro.AgenteTipo.Retirado)
            {
                SRNoBonificableCoeficiente = Agente.SRNoBonificable;
                FondoReal = FondoEstimulo * SRNoBonificableCoeficiente;
            }
            else
            {
                FondoReal = FondoEstimulo;
            }
        }

        /// <summary>
        /// Proceso que calcula el total del anticipo cuando se realiza por importación de liquidación.
        /// </summary>
        /// <param name="parametro">Objeto con los parámetros para cálculos secundarios.</param>
        public void CalcularTotalImportacion(Parametro parametro)
        {
            if (Agente == null || Agente.Escalafon == null || parametro == null)
                return;

            if (Agente.Retiro.HasValue)
                RetiroFecha = Agente.Retiro.Value;

            Escalafon = Agente.Escalafon.EscalafonApartadoGrupoIdentificador;
            Tipo = Agente.Tipo;
            SubtotalBasico = SueldoBasico + ComplementoBasico + SuplementoRemunerativo;

            DateTime periodo = Periodo;
            // Por defecto tomo la fecha del periodo fiscal para calcular los años de antigüedad.
            // Pero si el agente está retirado y tiene fecha de retiro: tomo esa fecha para hacer el cálculo.
            // El agente no cumple más años de antigüedad al estar retirado.
            if (Agente.Tipo == Parametro.AgenteTipo.Retirado && Agente.Retiro.HasValue)
                periodo = Agente.Retiro.Value;

            if (periodo > Agente.InicioActividades)
            {
                AñosAntiguedad = (int)Math.Floor(periodo.Subtract(Agente.InicioActividades).TotalDays / 365);

                // Comprobación para años bisiestos
                if (Agente.InicioActividades > periodo.AddYears(-AñosAntiguedad))
                    AñosAntiguedad--;
            }

            Antiguedad = ((SueldoBasico + ComplementoBasico + ReparacionHistorica) * 2 / 100) * AñosAntiguedad;

            // Cálculo adicional de la diferencia de subrogancia dentro del campo de antiguedad.
            if (Subrogancia > 0)
                Antiguedad += ((Subrogancia) * 2 / 100) * AñosAntiguedad;

            if (SueldoBasico > 0)
            {
                if (ReparacionHistorica > 0)
                    ReparacionHistoricaCoeficiente = ReparacionHistorica / SueldoBasico;

                if (Dedicacion > 0)
                    DedicacionCoeficiente = Dedicacion / SueldoBasico;

                if (Agente.Ley6655)
                    Ley6655Coeficiente = Ley6655 / SueldoBasico;
            }

            if (Incompatibilidad > 0 && SubtotalBasico > 0)
                IncompatibilidadCoeficiente = Incompatibilidad / SubtotalBasico;

            Subtotal = SubtotalBasico + Antiguedad + Incompatibilidad + Dedicacion + ReparacionHistorica + Titulo;
            Subtotal = Subtotal + Ley6655 + FondoEstimulo + RiesgoCaja + AdecuacionEscalafonaria + Subrogancia + FondoFijo;

            if (Subtotal > 0)
                SAC = Subtotal / 12;

            Total = Subtotal + SAC;

            FondoEstimuloCoeficiente = Agente.FondoEstimulo;
            FondoEstimulo = Total * FondoEstimuloCoeficiente;
            FondoReal = FondoEstimulo;

        }

        /// <summary>
        /// Asignación del importe de concepto dependiendo del código del mismo.
        /// </summary>
        /// <param name="concepto">Código de concepto.</param>
        /// <param name="reajuste">Tipo de reajuste.</param>
        /// <param name="importe">Importe en valor monetario.</param>
        public void AsignarConcepto(int concepto, int reajuste, decimal importe)
        {
            // Tipo de reajuste 2 corresponde a descuento. Por el momento no se toma en cuenta.
            if (reajuste == 2)
                return;

            switch (concepto)
            {
                case 1: this.SueldoBasico = importe; break;
                case 7: this.ComplementoBasico = importe; break;
                case 16: this.ReparacionHistorica = importe; break;
                case 100: this.Subrogancia = importe; break;
                case 120: this.Dedicacion = importe; break;
                case 154: this.SuplementoRemunerativo += importe; break;
                case 205: this.AdecuacionEscalafonaria += importe; break;
                case 210: this.Incompatibilidad = importe; break;
                case 212: this.Titulo = importe; break;
                case 213: this.Titulo = importe; break;
                case 223: this.RiesgoCaja = importe; break;
                case 233: this.Ley6655 = importe; break;
                case 302: this.FondoFijo = importe; break;
            }
        }

        #endregion Public Methods
    }
}