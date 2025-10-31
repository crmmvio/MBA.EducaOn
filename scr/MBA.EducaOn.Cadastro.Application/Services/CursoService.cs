using AutoMapper;
using MBA.EducaOn.GestaoConteudo.Application.ViewModels;
using MBA.EducaOn.GestaoConteudo.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MBA.EducaOn.GestaoConteudo.Application.Services;

public class CursoService : ICursoService
{
    private readonly IMapper _mapper;
    private readonly ICursoRepository _cursoRepository;
    private bool disposedValue;

    public CursoService(IMapper mapper, ICursoRepository cursoRepository)
    {
        _mapper = mapper;
        _cursoRepository = cursoRepository;
    }

    /// <inheritdoc />
    public async Task<CursoViewModel> ObterPorId(Guid id)
    {
        var curso = await _cursoRepository.ObterPorIdAsync(id);
        var cursoViewModel = _mapper.Map<CursoViewModel>(curso);

        return cursoViewModel;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CursoViewModel>> ObterTodos()
    {
        var listCurso = await _cursoRepository.ObterTodosAsync();
        var listCursoViewModel = _mapper.Map<IEnumerable<CursoViewModel>>(listCurso);

        return listCursoViewModel;
    }

    /// <inheritdoc />
    public Task Adicionar(CursoViewModel cursoViewModel)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<CursoViewModel> Atualizar(CursoViewModel cursoViewModel)
    {
        throw new NotImplementedException();
    }

    #region Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _cursoRepository?.Dispose();
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
