using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.Core.Data;

public interface IRepository<T> : IDisposable where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
