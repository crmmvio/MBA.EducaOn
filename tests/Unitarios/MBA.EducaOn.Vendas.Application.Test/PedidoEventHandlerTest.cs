using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Core.Messages;
using MBA.EducaOn.Core.Messages.CommonMessages.IntegrationEvents;
using MBA.EducaOn.Vendas.Application.Commands;
using MBA.EducaOn.Vendas.Application.Events;
using Moq;

namespace MBA.EducaOn.Vendas.Application.Test
{
    public class PedidoEventHandlerTest
    {
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
        private readonly PedidoEventHandler _handler;

        public PedidoEventHandlerTest()
        {
            _mediatorHandlerMock = new Mock<IMediatorHandler>();
            _handler = new PedidoEventHandler(_mediatorHandlerMock.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ComMediatorValido_DeveCriarInstancia()
        {
            // Arrange & Act
            var handler = new PedidoEventHandler(_mediatorHandlerMock.Object);

            // Assert
            Assert.NotNull(handler);
        }

        #endregion

        #region PedidoRascunhoIniciadoEvent Tests

        [Fact]
        public async Task Handle_PedidoRascunhoIniciadoEvent_DeveCompletarComSucesso()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var evento = new PedidoRascunhoIniciadoEvent(alunoId, pedidoId);
            var cancellationToken = CancellationToken.None;

            // Act
            var resultado = _handler.Handle(evento, cancellationToken);

            // Assert
            Assert.NotNull(resultado);
            await resultado;
            Assert.True(resultado.IsCompleted);
            _mediatorHandlerMock.Verify(m => m.EnviarComando(It.IsAny<Command>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PedidoRascunhoIniciadoEvent_NaoDeveEnviarComandos()
        {
            // Arrange
            var evento = new PedidoRascunhoIniciadoEvent(Guid.NewGuid(), Guid.NewGuid());

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(It.IsAny<Command>()), Times.Never);
        }

        #endregion

        #region PedidoItemAdicionadoEvent Tests

        [Fact]
        public async Task Handle_PedidoItemAdicionadoEvent_DeveCompletarComSucesso()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var evento = new PedidoItemAdicionadoEvent(alunoId, pedidoId, cursoId, "Curso Teste", 100m, 1);
            var cancellationToken = CancellationToken.None;

            // Act
            var resultado = _handler.Handle(evento, cancellationToken);

            // Assert
            Assert.NotNull(resultado);
            await resultado;
            Assert.True(resultado.IsCompleted);
            _mediatorHandlerMock.Verify(m => m.EnviarComando(It.IsAny<Command>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PedidoItemAdicionadoEvent_NaoDeveEnviarComandos()
        {
            // Arrange
            var evento = new PedidoItemAdicionadoEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Curso", 50m, 1);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(It.IsAny<Command>()), Times.Never);
        }

        #endregion

        #region PedidoEstoqueRejeitadoEvent Tests

        [Fact]
        public async Task Handle_PedidoEstoqueRejeitadoEvent_DeveEnviarComandoCancelarProcessamento()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var evento = new PedidoEstoqueRejeitadoEvent(pedidoId, clienteId);
            var cancellationToken = CancellationToken.None;

            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<CancelarProcessamentoPedidoCommand>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(evento, cancellationToken);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(
                It.Is<CancelarProcessamentoPedidoCommand>(cmd =>
                    cmd.PedidoId == pedidoId &&
                    cmd.AlunoId == clienteId
                )
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_PedidoEstoqueRejeitadoEvent_ComDadosValidos_DeveProcessarCorretamente()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var evento = new PedidoEstoqueRejeitadoEvent(pedidoId, clienteId);
            
            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<CancelarProcessamentoPedidoCommand>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(It.IsAny<CancelarProcessamentoPedidoCommand>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PedidoEstoqueRejeitadoEvent_DevePassarParametrosCorretos()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var evento = new PedidoEstoqueRejeitadoEvent(pedidoId, clienteId);
            CancelarProcessamentoPedidoCommand comandoCapturado = null;

            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<CancelarProcessamentoPedidoCommand>()))
                .Callback<Command>(cmd => comandoCapturado = cmd as CancelarProcessamentoPedidoCommand)
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            Assert.NotNull(comandoCapturado);
            Assert.Equal(pedidoId, comandoCapturado.PedidoId);
            Assert.Equal(clienteId, comandoCapturado.AlunoId);
        }

        #endregion

        #region PedidoPagamentoRealizadoEvent Tests

        [Fact]
        public async Task Handle_PedidoPagamentoRealizadoEvent_DeveEnviarComandoFinalizarPedido()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var pagamentoId = Guid.NewGuid();
            var transacaoId = Guid.NewGuid();
            var evento = new PedidoPagamentoRealizadoEvent(pedidoId, clienteId, pagamentoId, transacaoId, 100m);
            var cancellationToken = CancellationToken.None;

            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(evento, cancellationToken);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(
                It.Is<FinalizarPedidoCommand>(cmd =>
                    cmd.PedidoId == pedidoId &&
                    cmd.AlunoId == clienteId
                )
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_PedidoPagamentoRealizadoEvent_ComDadosValidos_DeveProcessarCorretamente()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var evento = new PedidoPagamentoRealizadoEvent(pedidoId, clienteId, Guid.NewGuid(), Guid.NewGuid(), 100m);
            
            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PedidoPagamentoRealizadoEvent_DevePassarParametrosCorretos()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var evento = new PedidoPagamentoRealizadoEvent(pedidoId, clienteId, Guid.NewGuid(), Guid.NewGuid(), 100m);
            FinalizarPedidoCommand comandoCapturado = null;

            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()))
                .Callback<Command>(cmd => comandoCapturado = cmd as FinalizarPedidoCommand)
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            Assert.NotNull(comandoCapturado);
            Assert.Equal(pedidoId, comandoCapturado.PedidoId);
            Assert.Equal(clienteId, comandoCapturado.AlunoId);
        }

        [Theory]
        [MemberData(nameof(ObterDadosPagamentoRealizado))]
        public async Task Handle_PedidoPagamentoRealizadoEvent_ComDiferentesDados_DeveProcessarCorretamente(
            Guid pedidoId, Guid clienteId)
        {
            // Arrange
            var evento = new PedidoPagamentoRealizadoEvent(pedidoId, clienteId, Guid.NewGuid(), Guid.NewGuid(), 100m);
            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(
                It.Is<FinalizarPedidoCommand>(cmd =>
                    cmd.PedidoId == pedidoId &&
                    cmd.AlunoId == clienteId
                )
            ), Times.Once);
        }

        public static IEnumerable<object[]> ObterDadosPagamentoRealizado()
        {
            yield return new object[] { Guid.NewGuid(), Guid.NewGuid() };
            yield return new object[] { Guid.NewGuid(), Guid.NewGuid() };
            yield return new object[] { Guid.NewGuid(), Guid.NewGuid() };
        }

        #endregion

        #region PedidoPagamentoRecusadoEvent Tests

        [Fact]
        public async Task Handle_PedidoPagamentoRecusadoEvent_DeveLancarNotImplementedException()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var evento = new PedidoPagamentoRecusadoEvent(pedidoId, clienteId, Guid.NewGuid(), Guid.NewGuid(), 100m);
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(() =>
                _handler.Handle(evento, cancellationToken)
            );
        }

        [Fact]
        public async Task Handle_PedidoPagamentoRecusadoEvent_NaoDeveEnviarComandos()
        {
            // Arrange
            var evento = new PedidoPagamentoRecusadoEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m);

            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(() =>
                _handler.Handle(evento, CancellationToken.None)
            );

            _mediatorHandlerMock.Verify(m => m.EnviarComando(It.IsAny<Command>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PedidoPagamentoRecusadoEvent_ComDadosValidos_DeveLancarExcecao()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var evento = new PedidoPagamentoRecusadoEvent(pedidoId, clienteId, Guid.NewGuid(), Guid.NewGuid(), 100m);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotImplementedException>(() =>
                _handler.Handle(evento, CancellationToken.None)
            );

            Assert.NotNull(exception);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Handle_MultiplosEventos_DeveProcessarTodosCorretamente()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();

            var eventoRascunho = new PedidoRascunhoIniciadoEvent(clienteId, pedidoId);
            var eventoItemAdicionado = new PedidoItemAdicionadoEvent(clienteId, pedidoId, cursoId, "Curso Teste", 100m, 1);
            var eventoPagamentoRealizado = new PedidoPagamentoRealizadoEvent(pedidoId, clienteId, Guid.NewGuid(), Guid.NewGuid(), 100m);

            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(eventoRascunho, CancellationToken.None);
            await _handler.Handle(eventoItemAdicionado, CancellationToken.None);
            await _handler.Handle(eventoPagamentoRealizado, CancellationToken.None);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()), Times.Once);
        }

        [Fact]
        public async Task Handle_FluxoPagamentoCompleto_DeveEnviarComandosCorretos()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            
            var eventoPagamentoRealizado = new PedidoPagamentoRealizadoEvent(pedidoId, clienteId, Guid.NewGuid(), Guid.NewGuid(), 100m);
            
            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(eventoPagamentoRealizado, CancellationToken.None);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(
                It.Is<FinalizarPedidoCommand>(cmd => 
                    cmd.PedidoId == pedidoId && 
                    cmd.AlunoId == clienteId
                )
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_FluxoEstoqueRejeitado_DeveEnviarComandoCancelamento()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            
            var eventoEstoqueRejeitado = new PedidoEstoqueRejeitadoEvent(pedidoId, clienteId);
            
            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<CancelarProcessamentoPedidoCommand>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(eventoEstoqueRejeitado, CancellationToken.None);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(
                It.Is<CancelarProcessamentoPedidoCommand>(cmd => 
                    cmd.PedidoId == pedidoId && 
                    cmd.AlunoId == clienteId
                )
            ), Times.Once);
        }

        #endregion

        #region Cancellation Token Tests

        [Fact]
        public async Task Handle_ComCancellationTokenCancelado_DeveProcessarMesmoAssim()
        {
            // Arrange
            var evento = new PedidoRascunhoIniciadoEvent(Guid.NewGuid(), Guid.NewGuid());
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var resultado = _handler.Handle(evento, cts.Token);

            // Assert
            await resultado;
            Assert.True(resultado.IsCompleted);
        }

        [Fact]
        public async Task Handle_PedidoPagamentoRealizadoEvent_ComCancellationToken_DeveProcessar()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var evento = new PedidoPagamentoRealizadoEvent(pedidoId, clienteId, Guid.NewGuid(), Guid.NewGuid(), 100m);
            var cts = new CancellationTokenSource();

            _mediatorHandlerMock
                .Setup(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(evento, cts.Token);

            // Assert
            _mediatorHandlerMock.Verify(m => m.EnviarComando(It.IsAny<FinalizarPedidoCommand>()), Times.Once);
        }

        #endregion
    }
}
