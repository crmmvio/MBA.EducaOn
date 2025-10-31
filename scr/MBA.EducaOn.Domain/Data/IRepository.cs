using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.Core.Data;

public interface IRepository<T> : IDisposable where T : IAggregateRoot
{
    /// <summary>
    /// Obtém a unidade de trabalho atual para gerenciar transações no banco de dados.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}
