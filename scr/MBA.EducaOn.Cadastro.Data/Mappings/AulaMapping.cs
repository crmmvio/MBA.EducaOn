using MBA.EducaOn.GestaoConteudo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.EducaOn.GestaoConteudo.Data.Mappings;

public class AulaMapping : IEntityTypeConfiguration<Aula>
{
    public void Configure(EntityTypeBuilder<Aula> builder)
    {
        builder.ToTable("Aulas");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Codigo)
               .HasMaxLength(Aula.CodigoMaxLength)
               .IsUnicode(false)
               .IsRequired();
        
        builder.Property(a => a.Titulo)
               .HasMaxLength(Aula.TituloMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(a => a.Descricao)
               .HasMaxLength(Aula.DescricaoMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(a => a.Ordem)
               .IsRequired();

        builder.Property(a => a.DataCadastro)
               .IsRequired();

        

    }
}
