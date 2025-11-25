using MBA.EducaOn.Core.Data;
using MBA.EducaOn.GestaoConteudo.Domain;
using MBA.EducaOn.GestaoConteudo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
    public async Task<Curso> ObterPorIdAsync(Guid id)
    {
        return await _context.Cursos
                             .Include(c => c.Aulas)
                             .FirstOrDefaultAsync(e=> e.Id == id);
    }

    /// <inheritdoc />
    public async Task<Curso> ObterPorAulaIdAsync(Guid id)
    {
        var result = await _context.Cursos
                                   .Include(c => c.Aulas)
                                   .FirstOrDefaultAsync(c => c.Aulas.Any(a => a.Id == id));

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Curso>> ObterTodosAsync()
    {
        return await _context.Cursos
                             .Include(e=>e.Aulas)
                             .AsNoTracking()
                             .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> ExistAsync(Guid id)
    {
        var curso = await _context.Cursos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        return curso != null;
    }

    /// <inheritdoc />
    public async Task<bool> ExistAsync(string nome)
    {
        var curso = await _context.Cursos.AsNoTracking().FirstOrDefaultAsync(e => e.Nome == nome);

        return curso != null;
    }

    /// <inheritdoc />
    public void Adicionar(Curso curso)
    {
        _context.Cursos.Add(curso);
    }

    /// <inheritdoc />
    public void Atualizar(Curso curso)
    {
        _context.Cursos.Update(curso);
    }

    /// <inheritdoc />
    public async Task Deletar(Guid id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        _context.Cursos.Remove(curso);
    }

    ///// <inheritdoc />
    //public async Task<Curso> DeletarAula(Guid id)
    //{

    //}

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
