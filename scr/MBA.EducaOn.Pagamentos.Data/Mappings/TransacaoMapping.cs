using MBA.EducaOn.Pagamentos.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.EducaOn.Pagamentos.Data.Mappings;

public class TransacaoMapping : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.HasKey(c => c.Id);

        builder.ToTable("Transacoes");
    }
}
