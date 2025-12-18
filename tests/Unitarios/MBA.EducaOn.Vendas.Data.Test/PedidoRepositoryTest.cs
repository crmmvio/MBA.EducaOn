using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Vendas.Data.Repository;
using MBA.EducaOn.Vendas.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Reflection;

namespace MBA.EducaOn.Vendas.Data.Test
{
    public class PedidoRepositoryTest
    {
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;

        public PedidoRepositoryTest()
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

        private void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
            else
            {
                var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                field?.SetValue(obj, value);
            }
        }

        private Voucher CriarVoucher(string codigo, decimal? valorDesconto, decimal? percentual, int quantidade,
            TipoDescontoVoucher tipoDesconto, DateTime dataValidade, bool ativo, bool utilizado)
        {
            var voucher = Activator.CreateInstance(typeof(Voucher), true) as Voucher;

            SetPrivateProperty(voucher, nameof(Voucher.Codigo), codigo);
            SetPrivateProperty(voucher, nameof(Voucher.ValorDesconto), valorDesconto);
            SetPrivateProperty(voucher, nameof(Voucher.Percentual), percentual);
            SetPrivateProperty(voucher, nameof(Voucher.Quantidade), quantidade);
            SetPrivateProperty(voucher, nameof(Voucher.TipoDescontoVoucher), tipoDesconto);
            SetPrivateProperty(voucher, nameof(Voucher.DataValidade), dataValidade);
            SetPrivateProperty(voucher, nameof(Voucher.DataCriacao), DateTime.Now);
            SetPrivateProperty(voucher, nameof(Voucher.Ativo), ativo);
            SetPrivateProperty(voucher, nameof(Voucher.Utilizado), utilizado);

            return voucher;
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarPedido_QuandoExistir()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var pedidoId = Guid.Empty;

            await using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                context.Pedidos.Add(pedido);
                await context.SaveChangesAsync();
                pedidoId = pedido.Id;
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var result = await repository.ObterPorId(pedidoId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(pedidoId, result.Id);
                Assert.Equal(alunoId, result.AlunoId);
            }
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNull_QuandoNaoExistir()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var pedidoId = Guid.NewGuid();

            await using var context = CreateContext(dbName);
            var repository = new PedidoRepository(context);

            // Act
            var result = await repository.ObterPorId(pedidoId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterProximoCodigo_DeveRetornar1_QuandoNaoHouverPedidos()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var repository = new PedidoRepository(context);

            // Act
            var result = await repository.ObterProximoCodigo();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ObterProximoCodigo_DeveRetornarProximoNumero_QuandoHouverPedidos()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();

            await using (var context = CreateContext(dbName))
            {
                var pedido1 = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 5);
                var pedido2 = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 10);
                context.Pedidos.AddRange(pedido1, pedido2);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var result = await repository.ObterProximoCodigo();

                // Assert
                Assert.Equal(11, result);
            }
        }

        [Fact]
        public async Task ObterListaPorAlunoId_DeveRetornarTodosOsPedidosDoAluno()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var outroAlunoId = Guid.NewGuid();

            await using (var context = CreateContext(dbName))
            {
                var pedido1 = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                var pedido2 = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 2);
                var pedido3 = Pedido.PedidoFactory.NovoPedidoRascunho(outroAlunoId, 3);
                context.Pedidos.AddRange(pedido1, pedido2, pedido3);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var result = await repository.ObterListaPorAlunoId(alunoId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.All(result, p => Assert.Equal(alunoId, p.AlunoId));
            }
        }

        [Fact]
        public async Task ObterPedidoRascunhoPorAlunoId_DeveRetornarPedidoRascunho_QuandoExistir()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();

            await using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                var pedidoItem = new PedidoItem(Guid.NewGuid(), "Curso", 100m);
                pedido.AdicionarItem(pedidoItem);
                context.Pedidos.Add(pedido);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var result = await repository.ObterPedidoRascunhoPorAlunoId(alunoId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(alunoId, result.AlunoId);
                Assert.Equal(PedidoStatus.Rascunho, result.PedidoStatus);
                Assert.NotEmpty(result.PedidoItems);
            }
        }

        [Fact]
        public async Task ObterPedidoRascunhoPorAlunoId_DeveCarregarVoucher_QuandoExistir()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();

            await using (var context = CreateContext(dbName))
            {
                var voucher = CriarVoucher("VOUCHER10", 10m, null, 1, TipoDescontoVoucher.Valor, DateTime.Now.AddDays(30), true, false);
                context.Vouchers.Add(voucher);
                await context.SaveChangesAsync();

                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                pedido.AdicionarItem(new PedidoItem(Guid.NewGuid(), "Curso", 100m));
                pedido.AplicarVoucher(voucher);
                context.Pedidos.Add(pedido);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var result = await repository.ObterPedidoRascunhoPorAlunoId(alunoId);

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Voucher);
                Assert.Equal("VOUCHER10", result.Voucher.Codigo);
            }
        }

        [Fact]
        public async Task ObterPedidoRascunhoPorAlunoId_DeveRetornarNull_QuandoNaoExistirRascunho()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();

            await using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                pedido.IniciarPedido(); // Muda o status para não-rascunho
                context.Pedidos.Add(pedido);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var result = await repository.ObterPedidoRascunhoPorAlunoId(alunoId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task ObterItemPorId_DeveRetornarItem_QuandoExistir()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var cursoId = Guid.NewGuid();
            var itemId = Guid.Empty;

            await using (var context = CreateContext(dbName))
            {
                var pedidoItem = new PedidoItem(cursoId, "Curso Teste", 150m);
                context.PedidoItems.Add(pedidoItem);
                await context.SaveChangesAsync();
                itemId = pedidoItem.Id;
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var result = await repository.ObterItemPorId(itemId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(itemId, result.Id);
                Assert.Equal(cursoId, result.CursoId);
            }
        }

        [Fact]
        public async Task ObterItemPorPedido_DeveRetornarItem_QuandoExistir()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var pedidoId = Guid.Empty;

            await using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                var pedidoItem = new PedidoItem(cursoId, "Curso", 100m);
                pedido.AdicionarItem(pedidoItem);
                context.Pedidos.Add(pedido);
                await context.SaveChangesAsync();
                pedidoId = pedido.Id;
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var result = await repository.ObterItemPorPedido(pedidoId, cursoId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(pedidoId, result.PedidoId);
                Assert.Equal(cursoId, result.CursoId);
            }
        }

        [Fact]
        public async Task ObterVoucherPorCodigo_DeveRetornarVoucher_QuandoExistir()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var codigo = "VOUCHER20";

            await using (var context = CreateContext(dbName))
            {
                var voucher = CriarVoucher(codigo, 20m, null, 5, TipoDescontoVoucher.Valor, DateTime.Now.AddDays(30), true, false);
                context.Vouchers.Add(voucher);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var result = await repository.ObterVoucherPorCodigo(codigo);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(codigo, result.Codigo);
                Assert.Equal(20m, result.ValorDesconto);
            }
        }

        [Fact]
        public async Task Adicionar_DeveAdicionarPedido()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var pedidoId = Guid.Empty;

            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                pedidoId = pedido.Id;

                // Act
                repository.Adicionar(pedido);
                await repository.UnitOfWork.Commit();
            }

            // Assert
            await using (var context = CreateContext(dbName))
            {
                var pedidoSalvo = await context.Pedidos.FindAsync(pedidoId);
                Assert.NotNull(pedidoSalvo);
                Assert.Equal(alunoId, pedidoSalvo.AlunoId);
            }
        }

        [Fact]
        public async Task AdicionarItem_DeveAdicionarItemAoPedido()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var cursoId = Guid.NewGuid();
            var itemId = Guid.Empty;

            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var pedidoItem = new PedidoItem(cursoId, "Curso", 100m);
                itemId = pedidoItem.Id;

                // Act
                repository.AdicionarItem(pedidoItem);
                await repository.UnitOfWork.Commit();
            }

            // Assert
            await using (var context = CreateContext(dbName))
            {
                var itemSalvo = await context.PedidoItems.FindAsync(itemId);
                Assert.NotNull(itemSalvo);
                Assert.Equal(cursoId, itemSalvo.CursoId);
            }
        }

        [Fact]
        public async Task Atualizar_DeveAtualizarPedido()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var pedidoId = Guid.Empty;

            await using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                context.Pedidos.Add(pedido);
                await context.SaveChangesAsync();
                pedidoId = pedido.Id;
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var pedido = await context.Pedidos.FindAsync(pedidoId);
                pedido.IniciarPedido();
                repository.Atualizar(pedido);
                await repository.UnitOfWork.Commit();
            }

            // Assert
            await using (var context = CreateContext(dbName))
            {
                var pedidoAtualizado = await context.Pedidos.FindAsync(pedidoId);
                Assert.Equal(PedidoStatus.Iniciado, pedidoAtualizado.PedidoStatus);
            }
        }

        [Fact]
        public async Task RemoverItem_DeveRemoverItemDoPedido()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var itemId = Guid.Empty;

            await using (var context = CreateContext(dbName))
            {
                var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
                var pedidoItem = new PedidoItem(cursoId, "Curso", 100m);
                pedido.AdicionarItem(pedidoItem);
                context.Pedidos.Add(pedido);
                await context.SaveChangesAsync();
                itemId = pedidoItem.Id;
            }

            // Act
            await using (var context = CreateContext(dbName))
            {
                var repository = new PedidoRepository(context);
                var item = await context.PedidoItems.FindAsync(itemId);
                repository.RemoverItem(item);
                await repository.UnitOfWork.Commit();
            }

            // Assert
            await using (var context = CreateContext(dbName))
            {
                var itemRemovido = await context.PedidoItems.FindAsync(itemId);
                Assert.Null(itemRemovido);
            }
        }

        [Fact]
        public void Dispose_NaoDeveLancarExcecao()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repository = new PedidoRepository(context);

            // Act & Assert
            repository.Dispose();
        }
    }
}
