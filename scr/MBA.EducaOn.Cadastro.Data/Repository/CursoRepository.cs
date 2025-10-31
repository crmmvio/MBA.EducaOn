using MBA.EducaOn.Core.Data;
using MBA.EducaOn.GestaoConteudo.Domain;
using MBA.EducaOn.GestaoConteudo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MBA.EducaOn.GestaoConteudo.Data.Repository;

public class CursoRepository : ICursoRepository
{
    private readonly ConteudoDbContext _context;
    private bool _disposed;

    public CursoRepository(ConteudoDbContext context)
    {
       _context = context;
    }

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    /// <inheritdoc />
    public async Task<Curso?> ObterPorIdAsync(Guid id)
    {
        return await _context.Cursos.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Curso>> ObterTodosAsync()
    {
        return await _context.Cursos.AsNoTracking().ToListAsync();
    }

    /// <inheritdoc />
    public void Adicionar(Curso curso)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Atualizar(Curso curso)
    {
        throw new NotImplementedException();
    }

    #region Dispose

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

}
