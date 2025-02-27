using WebConcessionariasVeiculos.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace WebConcessionariasVeiculos.Data
{
    public class DbConcessionariaContext : IdentityDbContext<Usuario>
    {
        public DbConcessionariaContext(DbContextOptions<DbConcessionariaContext> options) : base(options) { }

        public DbSet<Fabricante> Fabricantes { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Concessionaria> Concessionarias { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        //public DbSet<Usuario> Usuarios { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("AspNetUsers");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Fabricante>().HasIndex(f => f.Nome).IsUnique();
            modelBuilder.Entity<Concessionaria>().HasIndex(c => c.Nome).IsUnique();
            modelBuilder.Entity<Venda>().HasIndex(v => v.NumeroProtocolo).IsUnique();

            modelBuilder.Entity<Fabricante>().HasQueryFilter(f => f.Ativo);
            modelBuilder.Entity<Veiculo>().HasQueryFilter(v => v.Ativo);
            modelBuilder.Entity<Concessionaria>().HasQueryFilter(c => c.Ativo);
            modelBuilder.Entity<Venda>().HasQueryFilter(v => v.Ativo);

            // Corrigindo problema de precisão em campos decimais
            modelBuilder.Entity<Venda>()
                .Property(v => v.PrecoVenda)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Veiculo>()
                .Property(v => v.Preco)
                .HasColumnType("decimal(18,2)");

            // 🔴 Correção: Evitar erro de cascata na tabela 'Vendas'
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Veiculo)
                .WithMany()
                .HasForeignKey(v => v.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict); // ❌ Impede exclusão em cascata

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Fabricante)
                .WithMany()
                .HasForeignKey(v => v.FabricanteId)
                .OnDelete(DeleteBehavior.Restrict); // ❌ Impede exclusão em cascata

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Concessionaria)
                .WithMany()
                .HasForeignKey(v => v.ConcessionariaId)
                .OnDelete(DeleteBehavior.Restrict); // ❌ Impede exclusão em cascata
        }
    }
}
