using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Core.Data;
using MBA.EducaOn.Core.Messages.CommonMessages.Notifications;
using MBA.EducaOn.Vendas.Application.Commands;
using MBA.EducaOn.Vendas.Application.Commands.Handlers;
using MBA.EducaOn.Vendas.Application.Queries.ViewModels;
using MBA.EducaOn.Vendas.Domain;
using MBA.EducaOn.Vendas.Domain.Interfaces;
using Moq;
using System.Reflection;

namespace MBA.EducaOn.Vendas.Application.Test
{
    public class PedidoCommandHandlerTest
    {
        private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PedidoCommandHandler _handler;

        public PedidoCommandHandlerTest()
        {
            _pedidoRepositoryMock = new Mock<IPedidoRepository>();
            _mediatorHandlerMock = new Mock<IMediatorHandler>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _pedidoRepositoryMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

            _handler = new PedidoCommandHandler(_pedidoRepositoryMock.Object, _mediatorHandlerMock.Object);
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

        [Fact]
        public async Task Handle_AdicionarItemPedidoCommand_DeveCriarNovoPedidoRascunho_QuandoPedidoNaoExistir()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var command = new AdicionarItemPedidoCommand(alunoId, cursoId, "Curso Teste", 100m);

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync((Pedido)null);
            _pedidoRepositoryMock.Setup(r => r.ObterProximoCodigo()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _pedidoRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Pedido>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_AdicionarItemPedidoCommand_DeveAdicionarItemAoPedidoExistente_QuandoPedidoExistir()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var command = new AdicionarItemPedidoCommand(alunoId, cursoId, "Curso Teste", 100m);

            var pedidoExistente = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedidoExistente);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _pedidoRepositoryMock.Verify(r => r.AdicionarItem(It.IsAny<PedidoItem>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_AdicionarItemPedidoCommand_DeveRetornarFalse_QuandoItemJaExisteNoPedido()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var command = new AdicionarItemPedidoCommand(alunoId, cursoId, "Curso Teste", 100m);

            var pedidoExistente = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
            var itemExistente = new PedidoItem(cursoId, "Curso Teste", 100m);
            pedidoExistente.AdicionarItem(itemExistente);

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedidoExistente);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _pedidoRepositoryMock.Verify(r => r.AdicionarItem(It.IsAny<PedidoItem>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RemoverItemPedidoCommand_DeveRemoverItemDoPedido_QuandoPedidoEItemExistirem()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var command = new RemoverItemPedidoCommand(alunoId, cursoId);

            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
            var pedidoItem = new PedidoItem(cursoId, "Curso", 100m);
            pedido.AdicionarItem(pedidoItem);

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedido);
            _pedidoRepositoryMock.Setup(r => r.ObterItemPorPedido(pedido.Id, cursoId)).ReturnsAsync(pedidoItem);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _pedidoRepositoryMock.Verify(r => r.RemoverItem(pedidoItem), Times.Once);
            _pedidoRepositoryMock.Verify(r => r.Atualizar(pedido), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_RemoverItemPedidoCommand_DevePublicarNotificacao_QuandoPedidoNaoExistir()
        {
            // Arrange
            var command = new RemoverItemPedidoCommand(Guid.NewGuid(), Guid.NewGuid());
            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(command.AlunoId)).ReturnsAsync((Pedido)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mediatorHandlerMock.Verify(m => m.PublicarNotificacao(It.Is<DomainNotification>(n => n.Value == "Pedido não encontrado!")), Times.Once);
        }

        [Fact]
        public async Task Handle_AplicarVoucherPedidoCommand_DeveAplicarVoucher_QuandoVoucherValido()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var codigoVoucher = "VOUCHER10";
            var command = new AplicarVoucherPedidoCommand(alunoId, codigoVoucher);

            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
            pedido.AdicionarItem(new PedidoItem(Guid.NewGuid(), "Curso", 100m));

            var voucher = CriarVoucher(
                codigo: codigoVoucher,
                valorDesconto: 10m,
                percentual: null,
                quantidade: 1,
                tipoDesconto: TipoDescontoVoucher.Valor,
                dataValidade: DateTime.Now.AddDays(30),
                ativo: true,
                utilizado: false
            );

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedido);
            _pedidoRepositoryMock.Setup(r => r.ObterVoucherPorCodigo(codigoVoucher)).ReturnsAsync(voucher);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _pedidoRepositoryMock.Verify(r => r.Atualizar(pedido), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_AplicarVoucherPedidoCommand_DevePublicarNotificacao_QuandoVoucherNaoExistir()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var command = new AplicarVoucherPedidoCommand(alunoId, "INVALIDO");

            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedido);
            _pedidoRepositoryMock.Setup(r => r.ObterVoucherPorCodigo(It.IsAny<string>())).ReturnsAsync((Voucher)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mediatorHandlerMock.Verify(m => m.PublicarNotificacao(It.Is<DomainNotification>(n => n.Value == "Voucher não encontrado!")), Times.Once);
        }

        [Fact]
        public async Task Handle_IniciarPedidoCommand_DeveIniciarPedido_QuandoPedidoExistir()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var pedidoId = Guid.NewGuid();
            
            var carrinhoViewModel = new CarrinhoViewModel
            {
                PedidoId = pedidoId,
                AlunoId = alunoId,
                ValorTotal = 100m,
                Pagamento = new CarrinhoPagamentoViewModel
                {
                    NomeCartao = "Nome Teste",
                    NumeroCartao = "4111111111111111",
                    ExpiracaoCartao = "12/25",
                    CvvCartao = "123"
                }
            };

            var command = new IniciarPedidoCommand(carrinhoViewModel);

            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(alunoId, 1);
            pedido.AdicionarItem(new PedidoItem(Guid.NewGuid(), "Curso", 100m));

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorAlunoId(alunoId)).ReturnsAsync(pedido);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(PedidoStatus.Iniciado, pedido.PedidoStatus);
            _pedidoRepositoryMock.Verify(r => r.Atualizar(pedido), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_FinalizarPedidoCommand_DeveFinalizarPedido_QuandoPedidoExistir()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var command = new FinalizarPedidoCommand(pedidoId, Guid.NewGuid());

            var pedido = new Pedido(pedidoId, 100m);
            pedido.IniciarPedido();

            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync(pedido);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(PedidoStatus.Pago, pedido.PedidoStatus);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_CancelarProcessamentoPedidoCommand_DeveTornarPedidoRascunho_QuandoPedidoExistir()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var command = new CancelarProcessamentoPedidoCommand(pedidoId, Guid.NewGuid());

            var pedido = new Pedido(pedidoId, 100m);
            pedido.IniciarPedido();

            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync(pedido);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(PedidoStatus.Rascunho, pedido.PedidoStatus);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_CancelarProcessamentoPedidoNotificarAlunoCommand_DeveTornarPedidoRascunhoEPublicarEvento()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var pedidoId = Guid.NewGuid();
            var command = new CancelarProcessamentoPedidoNotificarAlunoCommand(pedidoId, alunoId);

            var pedido = new Pedido(pedidoId, 100m);
            pedido.IniciarPedido();

            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync(pedido);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(PedidoStatus.Rascunho, pedido.PedidoStatus);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
    }
}