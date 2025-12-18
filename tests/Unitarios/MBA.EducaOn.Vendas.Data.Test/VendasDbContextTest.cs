using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Vendas.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MBA.EducaOn.Vendas.Data.Test
{
    public class VendasDbContextTest
    {
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;

        public VendasDbContextTest()
        {
            _mediatorHandlerMock = new Mock<IMediatorHandler>();
        }

        private VendasDbContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<VendasDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            return new VendasDbContext(options, _mediatorHandlerMock.Object);
        }

        [Fact]
        public void VendasDbContext_DeveSerInstanciadoComSucesso()
        {
            // Arrange & Act
            using var context = CreateContext(Guid.NewGuid().ToString());

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Pedidos);
            Assert.NotNull(context.PedidoItems);
            Assert.NotNull(context.Vouchers);
        }

        [Fact]
        public async Task Commit_DeveDefinirDataCadastro_QuandoEntidadeForAdicionada()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                context.Pedidos.Add(pedido);

                // Act
                var result = await context.Commit();

                // Assert
                Assert.True(result);
            }

            using (var context = CreateContext(dbName))
            {
                var pedido = await context.Pedidos.FirstOrDefaultAsync();
                Assert.NotNull(pedido);
                Assert.NotEqual(DateTime.MinValue, pedido.DataCadastro);
                Assert.True(pedido.DataCadastro <= DateTime.Now);
            }
        }

        [Fact]
        public async Task Commit_NaoDeveAlterarDataCadastro_QuandoEntidadeForModificada()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            DateTime dataCadastroOriginal;

            using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                context.Pedidos.Add(pedido);
                await context.Commit();
                dataCadastroOriginal = pedido.DataCadastro;
            }

            await Task.Delay(100); // Pequeno delay para garantir diferença de tempo

            using (var context = CreateContext(dbName))
            {
                var pedido = await context.Pedidos.FirstOrDefaultAsync();
                pedido.IniciarPedido();
                context.Pedidos.Update(pedido);

                // Act
                await context.Commit();

                // Assert
                Assert.Equal(dataCadastroOriginal, pedido.DataCadastro);
            }
        }

        [Fact]
        public async Task Commit_DeveRetornarTrue_QuandoSalvarComSucesso()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);

            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid(), 1);
            context.Pedidos.Add(pedido);

            // Act
            var result = await context.Commit();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Commit_DeveRetornarFalse_QuandoNaoHouverMudancas()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);

            // Act
            var result = await context.Commit();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Pedidos_DeveAdicionarERecuperarPedido()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var pedidoId = Guid.Empty;

            using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 100);
                context.Pedidos.Add(pedido);
                await context.Commit();
                pedidoId = pedido.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var pedidoRecuperado = await context.Pedidos.FindAsync(pedidoId);

                // Assert
                Assert.NotNull(pedidoRecuperado);
                Assert.Equal(alunoId, pedidoRecuperado.AlunoId);
                Assert.Equal(100, pedidoRecuperado.Codigo);
            }
        }

        [Fact]
        public async Task PedidoItems_DeveAdicionarERecuperarItem()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var cursoId = Guid.NewGuid();
            var pedidoItemId = Guid.Empty;

            using (var context = CreateContext(dbName))
            {
                var pedidoItem = new PedidoItem(cursoId, "Curso Teste", 150m);
                context.PedidoItems.Add(pedidoItem);
                await context.Commit();
                pedidoItemId = pedidoItem.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var itemRecuperado = await context.PedidoItems.FindAsync(pedidoItemId);

                // Assert
                Assert.NotNull(itemRecuperado);
                Assert.Equal(cursoId, itemRecuperado.CursoId);
                Assert.Equal("Curso Teste", itemRecuperado.CursoNome);
                Assert.Equal(150m, itemRecuperado.ValorUnitario);
            }
        }

        [Fact]
        public async Task Vouchers_DeveAdicionarERecuperarVoucher()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            Guid voucherId;

            using (var context = CreateContext(dbName))
            {
                var voucher = Activator.CreateInstance(typeof(Voucher), true) as Voucher;
                SetPrivateProperty(voucher, nameof(Voucher.Codigo), "VOUCHER10");
                SetPrivateProperty(voucher, nameof(Voucher.ValorDesconto), 10m);
                SetPrivateProperty(voucher, nameof(Voucher.TipoDescontoVoucher), TipoDescontoVoucher.Valor);
                SetPrivateProperty(voucher, nameof(Voucher.Quantidade), 10);
                SetPrivateProperty(voucher, nameof(Voucher.DataValidade), DateTime.Now.AddDays(30));
                SetPrivateProperty(voucher, nameof(Voucher.Ativo), true);
                SetPrivateProperty(voucher, nameof(Voucher.Utilizado), false);

                context.Vouchers.Add(voucher);
                await context.Commit();
                voucherId = voucher.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var voucherRecuperado = await context.Vouchers.FindAsync(voucherId);

                // Assert
                Assert.NotNull(voucherRecuperado);
                Assert.Equal("VOUCHER10", voucherRecuperado.Codigo);
                Assert.Equal(10m, voucherRecuperado.ValorDesconto);
            }
        }

        [Fact]
        public async Task OnModelCreating_DeveConfigurarDeleteBehaviorComoClientSetNull()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);

            // Act
            var foreignKeys = context.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys());

            // Assert
            Assert.All(foreignKeys, fk => Assert.Equal(DeleteBehavior.ClientSetNull, fk.DeleteBehavior));
        }

        [Fact]
        public async Task Commit_DevePersistirMultiplasEntidades()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var pedido1 = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                var pedido2 = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 2);
                var pedido3 = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 3);

                context.Pedidos.AddRange(pedido1, pedido2, pedido3);

                // Act
                var result = await context.Commit();

                // Assert
                Assert.True(result);
            }

            using (var context = CreateContext(dbName))
            {
                var pedidos = await context.Pedidos.ToListAsync();
                Assert.Equal(3, pedidos.Count);
            }
        }

        [Fact]
        public async Task Commit_DeveManterIntegridadeEntreePedidoEItens()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                var pedidoItem = new PedidoItem(cursoId, "Curso", 100m);
                pedido.AdicionarItem(pedidoItem);

                context.Pedidos.Add(pedido);
                await context.Commit();
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var pedidoRecuperado = await context.Pedidos
                    .Include(p => p.PedidoItems)
                    .FirstOrDefaultAsync();

                // Assert
                Assert.NotNull(pedidoRecuperado);
                Assert.Single(pedidoRecuperado.PedidoItems);
                Assert.Equal(cursoId, pedidoRecuperado.PedidoItems.First().CursoId);
            }
        }

        private void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var property = obj.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
            else
            {
                var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                field?.SetValue(obj, value);
            }
        }
    }
}