using FondoEstimulo.Models;
using FondoEstimulo.Models.Aplicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FondoEstimulo.Data
{
    public class FondoEstimuloContext : IdentityDbContext<Usuario, IdentityRole, string>
    {
        #region Private Fields

        private const string _adminRol = "Administrador";
        private const string _registroRol = "Registro";

        #endregion Private Fields

        #region Protected Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(b =>
            {
                b.ToTable("Usuarios");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UsuariosNotificacion");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UsuariosInicioSesion");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UsuariosTokens");
            });

            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RolesNotificacion");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UsuariosRoles");
            });

            modelBuilder.Entity<Usuario>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<Models.Agentes.Comun.Agente>(a =>
            {
                a.HasOne(e => e.Escalafon)
                    .WithMany()
                    .HasForeignKey(eg => eg.EscalafonID)
                    .IsRequired();

                a.HasOne(d => d.BonificacionTitulo)
                    .WithMany()
                    .HasForeignKey(bf => bf.BonificacionTituloID)
                    .IsRequired();

                a.HasOne(f => f.BonificacionTituloEscalafon)
                    .WithMany()
                    .HasForeignKey(bte => bte.BonificacionTituloEscalafonID);
            });

            // EF creates Unique Index for nullable fields
            modelBuilder.Entity<Models.Agentes.Comun.Agente>()
                .HasIndex(a => a.BonificacionTituloEscalafonID)
                .IsUnique(false);

            modelBuilder.Entity<Models.Agentes.BOCEP.Agente>(a =>
            {
                a.HasOne(e => e.Escalafon)
                    .WithMany()
                    .HasForeignKey(eg => eg.EscalafonID)
                    .IsRequired();

                a.HasOne(d => d.BonificacionTitulo)
                    .WithMany()
                    .HasForeignKey(bf => bf.BonificacionTituloID)
                    .IsRequired();

                a.HasOne(f => f.BonificacionTituloEscalafon)
                    .WithMany()
                    .HasForeignKey(bte => bte.BonificacionTituloEscalafonID);
            });

            modelBuilder.Entity<Models.Agentes.BOCEP.Agente>()
                .HasIndex(a => a.BonificacionTituloEscalafonID)
                .IsUnique(false);

            modelBuilder.Entity<Models.Agentes.Funcionario.Agente>(a =>
            {
                a.HasOne(e => e.Escalafon)
                    .WithMany()
                    .HasForeignKey(eg => eg.EscalafonID)
                    .IsRequired();

                a.HasOne(d => d.BonificacionTitulo)
                    .WithMany()
                    .HasForeignKey(bf => bf.BonificacionTituloID)
                    .IsRequired();

                a.HasOne(f => f.BonificacionTituloEscalafon)
                    .WithMany()
                    .HasForeignKey(bte => bte.BonificacionTituloEscalafonID);
            });

            modelBuilder.Entity<Models.Agentes.Funcionario.Agente>()
                .HasIndex(a => a.BonificacionTituloEscalafonID)
                .IsUnique(false);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        #endregion Protected Methods

        #region Public Constructors

        public FondoEstimuloContext(DbContextOptions<FondoEstimuloContext> options)
                    : base(options)
        { }

        #endregion Public Constructors

        #region Public Properties

        public static string AdminRol => _adminRol;
        public static string RegistroRol => _registroRol;
        public DbSet<Escalafon> EscalafonGeneral { get; set; }
        public DbSet<Models.Agentes.Agente> Agentes { get; set; }
        public DbSet<Models.Agentes.Comun.Agente> AgentesComun { get; set; }
        public DbSet<Models.Agentes.BOCEP.Agente> AgentesBOCEP { get; set; }
        public DbSet<Models.Agentes.Funcionario.Agente> AgentesFuncionarios { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<Models.Agentes.Anticipo> Anticipos { get; set; }
        public DbSet<Models.Agentes.Comun.Anticipo> AnticiposComunes { get; set; }
        public DbSet<Models.Agentes.BOCEP.Anticipo> AnticiposBOCEP { get; set; }
        public DbSet<Models.Agentes.Funcionario.Anticipo> AnticiposFuncionarios { get; set; }
        public DbSet<Proceso> Procesos { get; set; }
        public DbSet<InicioSesion> IniciosSesion { get; set; }
        public DbSet<BonificacionTitulo> BonificacionesTitulo { get; set; }
        public DbSet<Evento> Eventos { get; set; }

        #endregion Public Properties
    }
}