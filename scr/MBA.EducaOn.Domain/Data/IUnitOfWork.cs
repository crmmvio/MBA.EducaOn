namespace MBA.EducaOn.Core.Data;

public interface IUnitOfWork
{
    /// <summary>
    /// Realiza o commit da transação atual de forma assíncrona.
    /// </summary>
    /// <returns>Uma tarefa que representa a operação assíncrona de commit. O resultado da tarefa contém <see langword="true"/> se o commit foi bem-sucedido; caso contrário, <see langword="false"/>.</returns>
    Task<bool> Commit();
}
