namespace MBA.EducaOn.Core.Data;

public interface IUnitOfWork
{
    Task<bool> Commit();
}
