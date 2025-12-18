using MBA.EducaOn.Vendas.Application.Queries;
using MBA.EducaOn.Vendas.Domain;
using MBA.EducaOn.Vendas.Domain.Interfaces;
using Moq;
using System.Reflection;

namespace MBA.EducaOn.Vendas.Application.Test
{
    public class PedidoQueriesTest
    {
        private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
        private readonly PedidoQueries _queries;

        public PedidoQueriesTest()
        {
            _pedidoRepositoryMock = new Mock<IPedidoRepository>();
            _queries = new PedidoQueries(_pedidoRepositoryMock.Object);
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

        private Voucher CriarVoucher(string codigo)
        {
            var voucher = Activator.CreateInstance(typeof(Voucher), true) as Voucher;
            SetPrivateProperty(voucher, nameof(Voucher.Codigo), codigo);
            return voucher;
        }

        [Fact]
        public async Task ObterCarrinhoAluno_DeveRetornarCarrinhoViewModel_QuandoPedidoExistir()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var pedidoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();

            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
            SetPrivateProperty(pedido, "Id", pedidoId);
            
            var pedidoItem = new PedidoItem(cursoId, "Curso Teste", 100m);
            pedido.AdicionarItem(pedidoItem);

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedido);

            // Act
            var result = await _queries.ObterCarrinhoAluno(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(alunoId, result.AlunoId);
            Assert.Equal(pedidoId, result.PedidoId);
            Assert.Equal(100m, result.ValorTotal);
            Assert.Single(result.Items);
            Assert.Equal("Curso Teste", result.Items.First().CursoNome);
        }

        [Fact]
        public async Task ObterCarrinhoAluno_DeveRetornarNull_QuandoPedidoNaoExistir()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync((Pedido)null);

            // Act
            var result = await _queries.ObterCarrinhoAluno(alunoId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterCarrinhoAluno_DeveIncluirVoucherCodigo_QuandoPedidoTiverVoucher()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var pedidoId = Guid.NewGuid();
            var voucherId = Guid.NewGuid();

            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
            SetPrivateProperty(pedido, "Id", pedidoId);
            SetPrivateProperty(pedido, "VoucherId", voucherId);

            var voucher = CriarVoucher("VOUCHER10");
            SetPrivateProperty(pedido, "Voucher", voucher);

            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Curso", 100m);
            pedido.AdicionarItem(pedidoItem);

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedido);

            // Act
            var result = await _queries.ObterCarrinhoAluno(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("VOUCHER10", result.VoucherCodigo);
        }

        [Fact]
        public async Task ObterCarrinhoAluno_DeveCalcularSubTotalCorretamente()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);

            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Curso 1", 100m);
            var pedidoItem2 = new PedidoItem(Guid.NewGuid(), "Curso 2", 50m);
            pedido.AdicionarItem(pedidoItem1);
            pedido.AdicionarItem(pedidoItem2);

            SetPrivateProperty(pedido, "Desconto", 20m);

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedido);

            // Act
            var result = await _queries.ObterCarrinhoAluno(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(20m, result.ValorDesconto);
            Assert.Equal(150m, result.ValorTotal);
            Assert.Equal(170m, result.SubTotal); // ValorTotal + Desconto
        }

        [Fact]
        public async Task ObterCarrinhoAluno_DeveRetornarTodosOsItens()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);

            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Curso 1", 100m);
            var pedidoItem2 = new PedidoItem(Guid.NewGuid(), "Curso 2", 150m);
            var pedidoItem3 = new PedidoItem(Guid.NewGuid(), "Curso 3", 200m);
            
            pedido.AdicionarItem(pedidoItem1);
            pedido.AdicionarItem(pedidoItem2);
            pedido.AdicionarItem(pedidoItem3);

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedido);

            // Act
            var result = await _queries.ObterCarrinhoAluno(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
            Assert.Contains(result.Items, i => i.CursoNome == "Curso 1" && i.ValorUnitario == 100m);
            Assert.Contains(result.Items, i => i.CursoNome == "Curso 2" && i.ValorUnitario == 150m);
            Assert.Contains(result.Items, i => i.CursoNome == "Curso 3" && i.ValorUnitario == 200m);
        }

        [Fact]
        public async Task ObterPedidosAluno_DeveRetornarApenasPedidosPagosECancelados()
        {
            // Arrange
            var alunoId = Guid.NewGuid();

            var pedidoPago = new Pedido(alunoId, false, 0m, 100m);
            SetPrivateProperty(pedidoPago, "Id", Guid.NewGuid());
            SetPrivateProperty(pedidoPago, "Codigo", 1);
            SetPrivateProperty(pedidoPago, "DataCadastro", DateTime.Now);
            pedidoPago.FinalizarPedido();

            var pedidoCancelado = new Pedido(alunoId, false, 0m, 200m);
            SetPrivateProperty(pedidoCancelado, "Id", Guid.NewGuid());
            SetPrivateProperty(pedidoCancelado, "Codigo", 2);
            SetPrivateProperty(pedidoCancelado, "DataCadastro", DateTime.Now);
            pedidoCancelado.CancelarPedido();

            var pedidoRascunho = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 3);

            var pedidos = new List<Pedido> { pedidoPago, pedidoCancelado, pedidoRascunho };

            _pedidoRepositoryMock.Setup(r => r.ObterListaPorAlunoId(alunoId)).ReturnsAsync(pedidos);

            // Act
            var result = await _queries.ObterPedidosAluno(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Codigo == 1);
            Assert.Contains(result, p => p.Codigo == 2);
            Assert.DoesNotContain(result, p => p.Codigo == 3);
        }

        [Fact]
        public async Task ObterPedidosAluno_DeveRetornarNull_QuandoNaoHouverPedidosPagosOuCancelados()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var pedidoRascunho = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
            var pedidos = new List<Pedido> { pedidoRascunho };

            _pedidoRepositoryMock.Setup(r => r.ObterListaPorAlunoId(alunoId)).ReturnsAsync(pedidos);

            // Act
            var result = await _queries.ObterPedidosAluno(alunoId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterPedidosAluno_DeveRetornarNull_QuandoNaoHouverPedidos()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.ObterListaPorAlunoId(alunoId)).ReturnsAsync(new List<Pedido>());

            // Act
            var result = await _queries.ObterPedidosAluno(alunoId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterPedidosAluno_DeveOrdenarPorCodigoDescendente()
        {
            // Arrange
            var alunoId = Guid.NewGuid();

            var pedido1 = new Pedido(alunoId, false, 0m, 100m);
            SetPrivateProperty(pedido1, "Id", Guid.NewGuid());
            SetPrivateProperty(pedido1, "Codigo", 1);
            SetPrivateProperty(pedido1, "DataCadastro", DateTime.Now.AddDays(-2));
            pedido1.FinalizarPedido();

            var pedido2 = new Pedido(alunoId, false, 0m, 200m);
            SetPrivateProperty(pedido2, "Id", Guid.NewGuid());
            SetPrivateProperty(pedido2, "Codigo", 3);
            SetPrivateProperty(pedido2, "DataCadastro", DateTime.Now.AddDays(-1));
            pedido2.FinalizarPedido();

            var pedido3 = new Pedido(alunoId, false, 0m, 150m);
            SetPrivateProperty(pedido3, "Id", Guid.NewGuid());
            SetPrivateProperty(pedido3, "Codigo", 2);
            SetPrivateProperty(pedido3, "DataCadastro", DateTime.Now);
            pedido3.FinalizarPedido();

            var pedidos = new List<Pedido> { pedido1, pedido2, pedido3 };

            _pedidoRepositoryMock.Setup(r => r.ObterListaPorAlunoId(alunoId)).ReturnsAsync(pedidos);

            // Act
            var result = await _queries.ObterPedidosAluno(alunoId);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(3, resultList.ElementAt(0).Codigo);
            Assert.Equal(2, resultList.ElementAt(1).Codigo);
            Assert.Equal(1, resultList.ElementAt(2).Codigo);
        }

        [Fact]
        public async Task ObterPedidosAluno_DeveMappearPropriedadesCorretamente()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var pedidoId = Guid.NewGuid();
            var dataCadastro = DateTime.Now;

            var pedido = new Pedido(alunoId, false, 0m, 250m);
            SetPrivateProperty(pedido, "Id", pedidoId);
            SetPrivateProperty(pedido, "Codigo", 100);
            SetPrivateProperty(pedido, "DataCadastro", dataCadastro);
            pedido.FinalizarPedido();

            var pedidos = new List<Pedido> { pedido };

            _pedidoRepositoryMock.Setup(r => r.ObterListaPorAlunoId(alunoId)).ReturnsAsync(pedidos);

            // Act
            var result = await _queries.ObterPedidosAluno(alunoId);

            // Assert
            Assert.NotNull(result);
            var pedidoView = result.First();
            Assert.Equal(pedidoId, pedidoView.Id);
            Assert.Equal(100, pedidoView.Codigo);
            Assert.Equal(250m, pedidoView.ValorTotal);
            Assert.Equal(dataCadastro, pedidoView.DataCadastro);
            Assert.Equal((int)PedidoStatus.Pago, pedidoView.PedidoStatus);
        }
    }
}
