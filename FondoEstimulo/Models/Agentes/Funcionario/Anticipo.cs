using FondoEstimulo.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FondoEstimulo.Models.Agentes.Funcionario
{
    [Table("AnticiposAgentesFuncionarios")]
    public class Anticipo : Agentes.Anticipo
    {
        #region Public Properties

        /// <summary>
        /// Identificación del agente al que pertenece el anticipo.
        /// </summary>
        [Display(Name = "Agente")]
        public int AgenteID { get; set; }

        /// <summary>
        /// Coeficiente del concepto Sup. Rep. No Bonificable. Se visualiza como porcentaje.
        /// </summary>
        [Display(Name = "Sup. Rep. No Bonificable coeficiente")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal SRNoBonificableCoeficiente { get; set; }

        /// <summary>
        /// Valor monetario del concepto Sup. Rep. No Bonificable.
        /// Se calcula: SubtotalBasico * SRNoBonificableCoeficiente;
        /// </summary>
        [Display(Name = "Sup. Rep. No Bonificable")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SRNoBonificable { get; set; }

        /// <summary>
        /// Valor monetario del concepto Compensanción Jerárquica.
        /// </summary>
        [Display(Name = "Compensación Jerárquica")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal CompensacionJerarquica { get; set; }

        /// <summary>
        /// Valor monetario del concepto Adicional Remunerativo.
        /// </summary>
        [Display(Name = "Adicional Remunerativo")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal AdicionalRemunerativo { get; set; }

        /// <summary>
        /// Valor monetario del Complemento básico del escalafón al que se le aplica la bonificación por titulo, al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Título escalafón complemento básico")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal TituloEscalafonComplementoBasico { get; set; }

        /// <summary>
        /// Valor monetario del concepto Sum Fija Rem.
        /// </summary>
        [Display(Name = "Suma fija Rem")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SumaFijaRem { get; set; }

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

            Escalafon = Agente.Escalafon.EscalafonApartadoGrupoIdentificador;
            SueldoBasico = Agente.Escalafon.SueldoBasico;
            CompensacionJerarquica = Agente.CompensancionJerarquica;
            AdicionalRemunerativo = Agente.AdicionalRemunerativo;
            SubtotalBasico = SueldoBasico + CompensacionJerarquica + AdicionalRemunerativo;
            AñosAntiguedad = (int)Math.Floor(Periodo.Subtract(Agente.InicioActividades).TotalDays / 365);

            // Comprobación para años bisiestos
            if (Agente.InicioActividades > Periodo.AddYears(-AñosAntiguedad))
                AñosAntiguedad--;

            Antiguedad = (SubtotalBasico * 2 / 100) * AñosAntiguedad;

            if (Agente.Incompatibilidad)
            {
                IncompatibilidadCoeficiente = parametro.Incompatibilidad;
                Incompatibilidad = SubtotalBasico * IncompatibilidadCoeficiente;
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

            SumaFijaRem = parametro.SumaFijaRem;
            SRNoBonificableCoeficiente = Agente.SRNoBonificable;
            SRNoBonificable = SubtotalBasico * SRNoBonificableCoeficiente;

            Subtotal = SubtotalBasico + Antiguedad + Incompatibilidad + SumaFijaRem + Titulo + FondoEstimulo + SRNoBonificable;
            SAC = Subtotal / 12;
            Total = Subtotal + SAC;

            FondoEstimuloCoeficiente = Agente.FondoEstimulo;
            FondoEstimulo = Total * FondoEstimuloCoeficiente;
        }

        /// <summary>
        /// Proceso que calcula el total del anticipo cuando se realiza por importación de liquidación.
        /// </summary>
        /// <param name="parametro">Objeto con los parámetros para cálculos secundarios.</param>
        public void CalcularTotalImportacion(Parametro parametro)
        {
            if (Agente == null || Agente.Escalafon == null || parametro == null)
                return;

            Escalafon = Agente.Escalafon.EscalafonApartadoGrupoIdentificador;
            SubtotalBasico = SueldoBasico + CompensacionJerarquica + AdicionalRemunerativo;
            AñosAntiguedad = (int)Math.Floor(Periodo.Subtract(Agente.InicioActividades).TotalDays / 365);

            // Comprobación para años bisiestos
            if (Agente.InicioActividades > Periodo.AddYears(-AñosAntiguedad))
                AñosAntiguedad--;

            Antiguedad = (SubtotalBasico * 2 / 100) * AñosAntiguedad;

            if (SubtotalBasico > 0)
            {
                if (Incompatibilidad > 0)
                    IncompatibilidadCoeficiente = Incompatibilidad / SubtotalBasico;

                if (SRNoBonificable > 0)
                    SRNoBonificableCoeficiente = SRNoBonificable / SubtotalBasico;
            }

            Subtotal = SubtotalBasico + Antiguedad + Incompatibilidad + SumaFijaRem + Titulo + FondoEstimulo + SRNoBonificable;
            SAC = Subtotal / 12;
            Total = Subtotal + SAC;

            FondoEstimuloCoeficiente = Agente.FondoEstimulo;
            FondoEstimulo = Total * FondoEstimuloCoeficiente;
        }

        /// <summary>
        /// Asignación del importe de concepto dependiendo del código del mismo.
        /// </summary>
        /// <param name="concepto">Código de concepto.</param>
        /// <param name="reajuste">Tipo de reajuste.</param>
        /// <param name="importe">Importe en valor monetario.</param>
        public void AsignarConcepto(int concepto,  int reajuste, decimal importe)
        {
            // Tipo de reajuste 2 corresponde a descuento. Por el momento no se toma en cuenta.
            if (reajuste == 2)
                return;

            switch (concepto)
            {
                case 1: this.SueldoBasico = importe; break;
                case 24: this.CompensacionJerarquica = importe; break;
                case 45: this.SRNoBonificable = importe; break;
                case 194: this.AdicionalRemunerativo = importe; break;
                case 210: this.Incompatibilidad = importe; break;
                case 212: this.Titulo = importe; break;
                case 213: this.Titulo = importe; break;
                // case 235: this.SumaFijaRem = importe; break;
            }
        }

        #endregion Public Methods
    }
}