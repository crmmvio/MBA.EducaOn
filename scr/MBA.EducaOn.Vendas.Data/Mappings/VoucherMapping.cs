using MBA.EducaOn.Vendas.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.EducaOn.Vendas.Data.Mappings;

public class VoucherMapping : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.ToTable("Vouchers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Codigo)
               .HasMaxLength(Voucher.CodigoMaxLength)
               .IsUnicode(false)
               .IsRequired();

        // 1 : N => Voucher : Pedidos
        builder.HasMany(c => c.Pedidos)
               .WithOne(c => c.Voucher)
               .HasForeignKey(c => c.VoucherId);

    }
}
