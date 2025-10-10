using MBA.EducaOn.GestaoAlunos.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.EducaOn.GestaoAlunos.Data.Mappings;

public class CertificadoMapping : IEntityTypeConfiguration<Certificado>
{
    public void Configure(EntityTypeBuilder<Certificado> builder)
    {
        builder.ToTable("Certificados");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.CursoId)
               .IsRequired();

        builder.Property(a => a.DataEmissao)
               .IsRequired();
        
        builder.Property(a => a.Codigo)
               .HasMaxLength(Certificado.CodigoMaxLength)
               .IsUnicode(false)
               .IsRequired();
    }
}
