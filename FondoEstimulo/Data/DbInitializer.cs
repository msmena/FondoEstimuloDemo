using FondoEstimulo.Models;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo.Data
{
    public static class DbInitializer
    {
        #region Private Methods

        private static async Task EnsureUserAdmin(IServiceProvider serviceProvider)
        {
            Serilog.ILogger Log = Serilog.Log.ForContext<Program>();
            var userManager = serviceProvider.GetService<UserManager<Usuario>>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                var exception = new Exception("No existe servicio RoleManager");
                Log.Fatal(exception.Message);
                throw exception;
            }

            const string adminUserEmail = "administrador@dominio.com";

            var user = await userManager.FindByNameAsync(FondoEstimuloContext.AdminRol);
            if (user == null)
            {
                user = new Usuario
                {
                    UserName = "Admin",
                    Email = adminUserEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user);
                Log.Information("Usuario Admininistrador creado.");
            }

            if (user == null)
            {
                var exception = new Exception("Error al intentar crear el usuario administrador");
                Log.Fatal(exception.Message);
                throw exception;
            }

            await roleManager.CreateAsync(new IdentityRole(FondoEstimuloContext.AdminRol));
            Log.Information("Rol de Admininistrador creado.");

            await userManager.AddToRoleAsync(user, FondoEstimuloContext.AdminRol);
            Log.Information("Asignacion de rol de Administrador al usuario Administrador.");

            // Por el momento no se utiliza rol de registro. Es admin o no.
            // await roleManager.CreateAsync(new IdentityRole(FondoEstimuloContext.UsuarioRol));
        }

        #endregion Private Methods

        #region Public Methods

        public static async Task Initialize(IServiceProvider serviceProvider, FondoEstimuloContext context)
        {
            Serilog.ILogger Log = Serilog.Log.ForContext<Program>();

            context.Database.EnsureCreated();

            if (context.EscalafonGeneral.Any())
            {
                return;   // DB has been seeded
            }

            Log.Information("Base de datos creada.");
            await EnsureUserAdmin(serviceProvider);

            var parametro = new Parametro { Incompatibilidad = 0.40M, Ley6655 = 0.25M, RiesgoCajaBase = 8346M, SumaFijaRem = 2000M };

            context.Parametros.Add(parametro);
            context.SaveChanges();
            Log.Information("Registros de parametros guardados.");

            var titulos = new BonificacionTitulo[]
            {
                new BonificacionTitulo{ Descripcion=Parametro.BTituloUniversitario31, Valor=0.31M },
                new BonificacionTitulo{ Descripcion=Parametro.BTituloUniversitario29, Valor=0.29M },
                new BonificacionTitulo{ Descripcion=Parametro.BTituloUniversitario27, Valor=0.27M },
                new BonificacionTitulo{ Descripcion=Parametro.BTituloUniversitario25, Valor=0.25M },
                new BonificacionTitulo{ Descripcion=Parametro.BTituloUniversitario22, Valor=0.22M },
                new BonificacionTitulo{ Descripcion=Parametro.BTituloUniversitario2050, Valor=0.2050M },
                new BonificacionTitulo{ Descripcion=Parametro.BTituloSecundario, Valor=0.175M},
                new BonificacionTitulo{ Descripcion=Parametro.BTituloSinTitulo, Valor=0M}
            };

            context.BonificacionesTitulo.AddRange(titulos);
            context.SaveChanges();
            Log.Information("Registros de titulos guardados");

            var escalafones = new Escalafon[]
            {
                new Escalafon{Apartado=Parametro.EscalafonApartadoA,Grupo=02,SueldoBasico=3500,ComplementoBasico=1500,SuplementoRemunerativo=2500},
                new Escalafon{Apartado=Parametro.EscalafonApartadoA,Grupo=01,SueldoBasico=3500,ComplementoBasico=1500,SuplementoRemunerativo=2500},
                new Escalafon{Apartado=Parametro.EscalafonApartadoB,Grupo=01,SueldoBasico=4500,ComplementoBasico=2000,SuplementoRemunerativo=3000},
                new Escalafon{Apartado=Parametro.EscalafonApartadoC,Grupo=01,SueldoBasico=5000,ComplementoBasico=2500,SuplementoRemunerativo=4000},
                new Escalafon{Apartado=Parametro.EscalafonApartadoC,Grupo=02,SueldoBasico=5500,ComplementoBasico=3000,SuplementoRemunerativo=4000},
                new Escalafon{Apartado=Parametro.EscalafonApartadoC,Grupo=03,SueldoBasico=6500,ComplementoBasico=3500,SuplementoRemunerativo=4000},
                new Escalafon{Apartado=Parametro.EscalafonApartadoGabinete,Grupo=02,SueldoBasico=10000,ComplementoBasico=4000,SuplementoRemunerativo=6000}
            };

            context.EscalafonGeneral.AddRange(escalafones);
            context.SaveChanges();
            Log.Information("Registros de Escalafones guardados.");

            var agentesComunes = new Models.Agentes.Comun.Agente[]
            {
                new Models.Agentes.Comun.Agente { DNI=255120,
                    Nombre="Agente comun 1",
                    Tipo=Parametro.AgenteTipo.Activo,
                    EscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoC & i.Grupo == 11).EscalafonID,
                    InicioActividades=DateTime.Parse("1995-08-01"),
                    Incompatibilidad=true,
                    Dedicacion=0.25M,
                    ReparacionHistorica=0.05M,
                    BonificacionTituloID=titulos.Single(t => t.Descripcion == Parametro.BTituloUniversitario27).BonificacionTituloID,
                    BonificacionTituloEscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoC & i.Grupo == 11).EscalafonID,
                    Ley6655=false,
                    FondoFijo=1700,
                    Subrogancia=13156.88M,
                    FondoEstimulo=1.30M },
                new Models.Agentes.Comun.Agente { DNI=255121,
                    Nombre="Agente comun 2",
                    Tipo=Parametro.AgenteTipo.Activo,
                    EscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoD & i.Grupo == 04).EscalafonID,
                    InicioActividades=DateTime.Parse("2019-01-01"),
                    Incompatibilidad=true,
                    BonificacionTituloID=titulos.Single(t => t.Descripcion == Parametro.BTituloSinTitulo).BonificacionTituloID,
                    Ley6655=true,
                    FondoFijo=1800,
                    FondoEstimulo=1.30M },
                new Models.Agentes.Comun.Agente { DNI=255122,
                    Nombre="Agente comun 3",
                    Tipo=Parametro.AgenteTipo.Activo,
                    EscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoC & i.Grupo == 09).EscalafonID,
                    InicioActividades=DateTime.Parse("1987-09-01"),
                    Incompatibilidad=true,
                    Dedicacion=0.15M,
                    ReparacionHistorica=0.10M,
                    BonificacionTituloID=titulos.Single(t => t.Descripcion == Parametro.BTituloUniversitario22).BonificacionTituloID,
                    BonificacionTituloEscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoC & i.Grupo == 09).EscalafonID,
                    Ley6655=false,
                    FondoFijo=1700,
                    FondoEstimulo=1.30M }
            };

            context.AgentesComun.AddRange(agentesComunes);
            context.SaveChanges();
            Log.Information("Registros de Agentes comunes guardados.");

            var agentesBOCEP = new Models.Agentes.BOCEP.Agente[]
            {
                new Models.Agentes.BOCEP.Agente{ DNI=255123,
                    Nombre="Agente BOCEP 1",
                    EscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoBOCEP & i.Grupo == 01).EscalafonID,
                    InicioActividades=DateTime.Parse("2016-09-01"),
                    FondoEstimulo=1.30M,
                    Dedicacion=0.15M,
                    BonificacionTituloID=titulos.Single(t => t.Descripcion == Parametro.BTituloSinTitulo).BonificacionTituloID,
                    SumaFijo=1900,
                    Incompatibilidad=true,
                    AsignacionComplementaria=0.35M },
                new Models.Agentes.BOCEP.Agente{ DNI=255124,
                    Nombre="Agente BOCEP 2",
                    EscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoBOCEP & i.Grupo == 01).EscalafonID,
                    InicioActividades=DateTime.Parse("2000-07-01"),
                    FondoEstimulo=1.30M,
                    Dedicacion=0.15M,
                    BonificacionTituloID=titulos.Single(t => t.Descripcion == Parametro.BTituloSecundario).BonificacionTituloID,
                    BonificacionTituloEscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoBOCEP & i.Grupo == 01).EscalafonID,
                    SumaFijo=1900,
                    Incompatibilidad=true,
                    AsignacionComplementaria=0.35M },
                new Models.Agentes.BOCEP.Agente{ DNI=255125,
                    Nombre="Agente BOCEP 3",
                    EscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoBOCEP & i.Grupo == 01).EscalafonID,
                    InicioActividades=DateTime.Parse("1996-06-01"),
                    FondoEstimulo=1.30M,
                    Dedicacion=0.15M,
                    BonificacionTituloID=titulos.Single(t => t.Descripcion == Parametro.BTituloSecundario).BonificacionTituloID,
                    BonificacionTituloEscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoBOCEP & i.Grupo == 01).EscalafonID,
                    SumaFijo=1900,
                    Incompatibilidad=true,
                    AsignacionComplementaria=0.35M }
            };

            context.AgentesBOCEP.AddRange(agentesBOCEP);
            context.SaveChanges();
            Log.Information("Registros de Agentes BOCEP guardados.");

            var agentesFuncionarios = new Models.Agentes.Funcionario.Agente[]
            {
                new Models.Agentes.Funcionario.Agente{ DNI=255126,
                    Nombre="Agente funcionario 1",
                    EscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoGabinete & i.Grupo == 01).EscalafonID,
                    InicioActividades=DateTime.Parse("2021-03-01"),
                    FondoEstimulo=0.50M,
                    BonificacionTituloID=titulos.Single(t => t.Descripcion == Parametro.BTituloSinTitulo).BonificacionTituloID,
                    Incompatibilidad=false,
                    CompensancionJerarquica=24299M,
                    AdicionalRemunerativo=19154M,
                    SRNoBonificable=0.3M },
                new Models.Agentes.Funcionario.Agente{ DNI=255127,
                    Nombre="Agente funcionario 2",
                    EscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoGabinete & i.Grupo == 02).EscalafonID,
                    InicioActividades=DateTime.Parse("2005-12-01"),
                    FondoEstimulo=1.05M,
                    BonificacionTituloID=titulos.Single(t => t.Descripcion == Parametro.BTituloUniversitario27).BonificacionTituloID,
                    BonificacionTituloEscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoGabinete & i.Grupo == 02).EscalafonID,
                    Incompatibilidad=true,
                    CompensancionJerarquica=21260M,
                    AdicionalRemunerativo=16763M,
                    SRNoBonificable=0.3M },
                new Models.Agentes.Funcionario.Agente{ DNI=255128,
                    Nombre="Agente funcionario 3",
                    EscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoGabinete & i.Grupo == 01).EscalafonID,
                    InicioActividades=DateTime.Parse("2019-12-01"),
                    FondoEstimulo=1.05M,
                    BonificacionTituloID=titulos.Single(t => t.Descripcion == Parametro.BTituloUniversitario25).BonificacionTituloID,
                    BonificacionTituloEscalafonID=escalafones.Single(i => i.Apartado == Parametro.EscalafonApartadoGabinete & i.Grupo == 01).EscalafonID,
                    Incompatibilidad=true,
                    CompensancionJerarquica=24299M,
                    AdicionalRemunerativo=19154M,
                    SRNoBonificable=0.3M }
            };

            context.AgentesFuncionarios.AddRange(agentesFuncionarios);
            context.SaveChanges();
            Log.Information("Registros de Agentes Funcionarios guardados.");
        }

        #endregion Public Methods
    }
}