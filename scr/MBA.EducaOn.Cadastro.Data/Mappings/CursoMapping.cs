using MBA.EducaOn.GestaoConteudo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.EducaOn.GestaoConteudo.Data.Mappings;

public class CursoMapping : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("Cursos");
        builder.HasKey(a => a.Id);

        builder.Property(c => c.Nome)
               .HasMaxLength(Curso.NomeMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(c=> c.Descricao)
               .HasMaxLength(Curso.DescricaoMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(c => c.Valor)
               .IsRequired();

        builder.Property(c => c.CargaHoraria)
               .IsRequired();

        builder.Property(c => c.PublicoAlvo)
               .HasMaxLength(Curso.PublicoAlvoMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(c => c.Objetivo)
               .HasMaxLength(Curso.ObjetivoMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(c => c.Requisitos)
               .HasMaxLength(Curso.RequisitosMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(c => c.DataCadastro)
               .IsRequired();

        builder.OwnsOne(c => c.ConteudoProgramatico, cp =>
        {
            cp.Property(c => c.ConteudoDescricao)
              .HasColumnName("ConteudoDescricao")
              .HasMaxLength(ConteudoProgramatico.DescricaoMaxLength)
              .IsUnicode(false);

            cp.Property(c => c.Revisao)
              .HasColumnName("Revisao");

            cp.Property(c => c.DataRevisao);
        });

        builder.Property(c => c.Ativo)
               .IsRequired();

        builder.HasMany(c=> c.Aulas)
               .WithOne()
               .HasForeignKey(a => a.CursoId);

    }
}
