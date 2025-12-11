using MBA.EducaOn.Vendas.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.EducaOn.Vendas.Data.Mappings;

public class PedidoMapping : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Codigo)
               .IsRequired();

        builder.Property(c => c.AlunoId)
               .IsRequired();

        builder.Property(c => c.VoucherId);
        builder.Property(c => c.VoucherUtilizado)
               .IsRequired();

        builder.Property(c => c.Desconto)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(c => c.ValorTotal)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(c => c.DataCadastro)
               .IsRequired();

        builder.Property(c => c.PedidoStatus)
               .IsRequired()
               .HasConversion(v => v.ToString(),
                              v => (PedidoStatus)Enum.Parse(typeof(PedidoStatus), v));

        // 1 : N => Pedido : PedidoItems
        builder.HasMany(c => c.PedidoItems)
               .WithOne(c => c.Pedido)
               .HasForeignKey(c => c.PedidoId);

    }
}
