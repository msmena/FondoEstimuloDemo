using FondoEstimulo.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FondoEstimulo.Models.Agentes.BOCEP
{
    [Table("AnticiposAgentesBOCEP")]
    public class Anticipo : Agentes.Anticipo
    {
        #region Public Properties

        /// <summary>
        /// Identificación del agente al que pertenece el anticipo.
        /// </summary>
        [Display(Name = "Agente")]
        public int AgenteID { get; set; }

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
        /// Coeficiente del concepto Asignación complementaria al momento de hacer el anticipo.
        /// </summary>
        [Display(Name = "Coeficiente de Asignación Complementaria")]
        [DisplayFormat(DataFormatString = "{0:P4}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal AsignacionComplementariaCoeficiente { get; set; }

        /// <summary>
        /// Valor monetario del concepto Asignación Complementaria.
        /// Se calcula: SueldoBasico * AsignacionComplementariaCoeficiente.
        /// </summary>
        [Display(Name = "Asignación Complementaria")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal AsignacionComplementaria { get; set; }

        /// <summary>
        /// Valor monetario del concepto Suma fijo.
        /// </summary>
        [Display(Name = "Suma fijo")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SumaFijo { get; set; }

        /// <summary>
        /// Valor monetario del concepto Sum fija remunerativa.
        /// </summary>
        [Display(Name = "Suma fija remunerativa")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SumaFijaRemunerativa { get; set; }

        /// <summary>
        /// Valor monetario del concepto Suplemento Remunerativo.
        /// </summary>
        [Display(Name = "Suplemento remunerativo")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal SuplementoRemunerativo { get; set; }

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
            SubtotalBasico = SueldoBasico;
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

                Titulo = TituloEscalafonSueldoBasico * TituloTipoCoeficiente;
            }

            AsignacionComplementariaCoeficiente = Agente.AsignacionComplementaria;
            AsignacionComplementaria = SueldoBasico * AsignacionComplementariaCoeficiente;
            SuplementoRemunerativo = SueldoBasico / 2;
            SumaFijo = Agente.SumaFijo;
            SumaFijaRemunerativa = parametro.SumaFijaRem;

            Subtotal = SubtotalBasico + Antiguedad + Incompatibilidad + Dedicacion + SumaFijaRemunerativa + Titulo;
            Subtotal = Subtotal + FondoEstimulo + SumaFijo + AsignacionComplementaria + SuplementoRemunerativo;
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
            SueldoBasico = Agente.Escalafon.SueldoBasico;
            SubtotalBasico = SueldoBasico;
            AñosAntiguedad = (int)Math.Floor(Periodo.Subtract(Agente.InicioActividades).TotalDays / 365);

            // Comprobación para años bisiestos
            if (Agente.InicioActividades > Periodo.AddYears(-AñosAntiguedad))
                AñosAntiguedad--;

            Antiguedad = (SubtotalBasico * 2 / 100) * AñosAntiguedad;

            if (Incompatibilidad > 0 && SubtotalBasico > 0)
                IncompatibilidadCoeficiente = Incompatibilidad / SubtotalBasico;

            if (Dedicacion > 0 && SueldoBasico > 0)
                DedicacionCoeficiente = Dedicacion / SueldoBasico;

            TituloTipo = Agente.BonificacionTitulo.Descripcion;
            TituloTipoCoeficiente = Agente.BonificacionTitulo.Valor;

            if (Agente.BonificacionTituloEscalafon != null)
            {
                TituloEscalafon = Agente.BonificacionTituloEscalafon.EscalafonApartadoGrupoIdentificador;
                TituloEscalafonSueldoBasico = Agente.BonificacionTituloEscalafon.SueldoBasico;

                Titulo = TituloEscalafonSueldoBasico * TituloTipoCoeficiente;
            }

            AsignacionComplementariaCoeficiente = Agente.AsignacionComplementaria;
            AsignacionComplementaria = SueldoBasico * AsignacionComplementariaCoeficiente;
            SuplementoRemunerativo = SueldoBasico / 2;
            SumaFijo = Agente.SumaFijo;
            //SumaFijaRemunerativa = parametro.SumaFijaRem;

            Subtotal = SubtotalBasico + Antiguedad + Incompatibilidad + Dedicacion + SumaFijaRemunerativa + Titulo;
            Subtotal = Subtotal + FondoEstimulo + SumaFijo + AsignacionComplementaria + SuplementoRemunerativo;
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
        public void AsignarConcepto(int concepto, int reajuste, decimal importe)
        {
            // Tipo de reajuste 2 corresponde a descuento. Por el momento no se toma en cuenta.
            if (reajuste == 2)
                return;

            switch (concepto)
            {
                case 1: this.SueldoBasico = importe; break;
                case 154: this.SuplementoRemunerativo += importe; break;
                case 120: this.Dedicacion = importe; break;
                case 210: this.Incompatibilidad = importe; break;
                case 212: this.Titulo = importe; break;
                case 213: this.Titulo = importe; break;
            }
        }

        #endregion Public Methods
    }
}