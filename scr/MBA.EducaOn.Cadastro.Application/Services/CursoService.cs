using AutoMapper;
using MBA.EducaOn.GestaoConteudo.Application.ViewModels;
using MBA.EducaOn.GestaoConteudo.Domain;
using MBA.EducaOn.GestaoConteudo.Domain.Interfaces.Repositories;

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
    public async Task<bool> ExistAsync(Guid id) => await _cursoRepository.ExistAsync(id);

    /// <inheritdoc />
    public async Task<bool> ExistAsync(string nome) => await _cursoRepository.ExistAsync(nome);

    /// <inheritdoc />
    public async Task<CursoViewModel> Adicionar(CursoViewModel cursoViewModel)
    {
        var curso = _mapper.Map<Curso>(cursoViewModel);
        _cursoRepository.Adicionar(curso);
        await _cursoRepository.UnitOfWork.Commit();

        return _mapper.Map<CursoViewModel>(curso);
    }

    /// <inheritdoc />
    public async Task<CursoViewModel> Atualizar(CursoViewModel cursoViewModel)
    {
        var curso = _mapper.Map<Curso>(cursoViewModel);
        _cursoRepository.Atualizar(curso);

        await _cursoRepository.UnitOfWork.Commit();

        return cursoViewModel;
    }

    /// <inheritdoc />
    public async Task Deletar(Guid id)
    {
        await _cursoRepository.Deletar(id);
        await _cursoRepository.UnitOfWork.Commit();
    }

    /// <inheritdoc />
    public async Task<CursoViewModel> AdicionarAula(AulaViewModel aulaViewModel)
    {
        var curso = await _cursoRepository.ObterPorIdAsync(aulaViewModel.CursoId);
        var aula = _mapper.Map<Aula>(aulaViewModel);

        curso.AdicionarAula(aula);

        await _cursoRepository.UnitOfWork.Commit();
        return _mapper.Map<CursoViewModel>(curso);
    }

    public async Task<CursoViewModel> DeletarAulaAsync(Guid id)
    {
        var curso = await _cursoRepository.ObterPorIdAsync(id);
        var aula = curso.Aulas.First(e=> e.Id == id);

        curso.RemoverAula(aula);
        
        await _cursoRepository.UnitOfWork.Commit();
        return _mapper.Map<CursoViewModel>(curso);
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
