using MBA.EducaOn.Vendas.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.EducaOn.Vendas.Data.Mappings;

public class PedidoItemMapping : IEntityTypeConfiguration<PedidoItem>
{
    public void Configure(EntityTypeBuilder<PedidoItem> builder)
    {
        builder.ToTable("PedidoItems");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.PedidoId)
               .IsRequired();
        
        builder.Property(c => c.CursoId)
               .IsRequired();

        builder.Property(c => c.CursoNome)
               .HasMaxLength(PedidoItem.NomeCursoMaxLength)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(c => c.ValorUnitario)
               .IsRequired();

        // 1 : N => Pedido : Pagamento
        builder.HasOne(c => c.Pedido)
               .WithMany(c => c.PedidoItems);

    }
}
