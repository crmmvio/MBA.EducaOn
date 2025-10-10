using MBA.EducaOn.Core.Data;
using MBA.EducaOn.Core.Messages;
using MBA.EducaOn.GestaoAlunos.Domain;
using Microsoft.EntityFrameworkCore;

namespace MBA.EducaOn.GestaoAlunos.Data;

public class AlunoContext : DbContext, IUnitOfWork
{

    public AlunoContext(DbContextOptions<AlunoContext> options)
    : base(options) { }

    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Certificado> Certificados { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
        //    property.SetColumnType("varchar(100)");

        modelBuilder.Ignore<Event>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AlunoContext).Assembly);
    }

    public async Task<bool> Commit()
    {
        foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("DataCadastro").CurrentValue = DateTime.Now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property("DataCadastro").IsModified = false;
            }
        }

        return await base.SaveChangesAsync() > 0;
    }
}
