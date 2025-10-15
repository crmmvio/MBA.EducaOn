using MBA.EducaOn.GestaoAlunos.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.EducaOn.GestaoAlunos.Data.Mappings;

public class MatriculaMapping : IEntityTypeConfiguration<Matricula>
{
    public void Configure(EntityTypeBuilder<Matricula> builder)
    {
        builder.ToTable("Matriculas");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.CursoId)
               .HasColumnName("CursoId")
               .IsRequired();

        builder.Property(a => a.DataMatricula)
               .IsRequired();

        builder.Property(a => a.DataValidade)
               .IsRequired();

        builder.Property(a => a.Ativo)
               .IsRequired();
    }
}
