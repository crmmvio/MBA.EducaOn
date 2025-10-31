using MBA.EducaOn.GestaoConteudo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.EducaOn.GestaoConteudo.Data.Mappings;

public class MaterialMapping : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materiais");
        builder.HasKey(a => a.Id)
               .HasName("PK_MATERIAL");

        builder.Property(a => a.Nome)
               .HasMaxLength(Material.NomeMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(a => a.Descricao)
               .HasMaxLength(Material.DescricaoMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(a => a.UrlArquivo)
               .HasMaxLength(Material.UrlArquivoMaxLength)
               .IsUnicode(false);

        builder.Property(a => a.UrlLinkSiteReferencia)
               .HasMaxLength(Material.UrlLinkSiteReferenciaMaxLength)
               .IsUnicode(false);

    }
}
