using MBA.EducaOn.Core.Data;
using MBA.EducaOn.GestaoAlunos.Domain;
using MBA.EducaOn.GestaoAlunos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MBA.EducaOn.GestaoAlunos.Data.Repository;

public class AlunoRepository : IAlunoRepository
{
    private readonly AlunoDbContext _context;
    private bool disposedValue;

    public AlunoRepository(AlunoDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    /// <inheritdoc />
    public async Task<Aluno> ObterPorIdAsync(Guid id)
    {
        return await _context.Alunos.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Aluno>> ObterTodosAsync()
    {
        return await _context.Alunos.AsNoTracking().ToListAsync();
    }

    /// <inheritdoc />
    public void Adicionar(Aluno Aluno)
    {
        _context.Alunos.Add(Aluno);
    }

    /// <inheritdoc />
    public void Atualizar(Aluno Aluno)
    {
        _context.Alunos.Update(Aluno);
    }

    #region Dispose

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion

}
