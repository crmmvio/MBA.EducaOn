using AutoMapper;
using MBA.EducaOn.GestaoAlunos.Application.ViewModels;
using MBA.EducaOn.GestaoAlunos.Domain.Interfaces.Repositories;

namespace MBA.EducaOn.GestaoAlunos.Application.Services;

public class AlunoService : IAlunoService
{
    private readonly IMapper _mapper;
    private readonly IAlunoRepository  _alunoRepository;
    private bool disposedValue;

    public AlunoService(IMapper mapper, IAlunoRepository alunoRepository)
    {
        _mapper = mapper;
        _alunoRepository = alunoRepository;
    }
        
    /// <inheritdoc />
    public async Task<AlunoViewModel> ObterPorId(Guid id)
    {
        var aluno = await _alunoRepository.ObterPorIdAsync(id);
        var alunoViewModel = _mapper.Map<AlunoViewModel>(aluno);

        return alunoViewModel;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AlunoViewModel>> ObterTodos()
    {
        var listAluno = await _alunoRepository.ObterTodosAsync();
        var listAlunoViewModel = _mapper.Map<IEnumerable<AlunoViewModel>>(listAluno);

        return listAlunoViewModel;
    }

    /// <inheritdoc />
    public async Task Adicionar(AlunoViewModel cursoViewModel)
    {
        var aluno = _mapper.Map<Domain.Aluno>(cursoViewModel);
        _alunoRepository.Adicionar(aluno);

        await _alunoRepository.UnitOfWork.Commit();
    }

    /// <inheritdoc />
    public async Task<AlunoViewModel> Atualizar(AlunoViewModel cursoViewModel)
    {
        var aluno = _mapper.Map<Domain.Aluno>(cursoViewModel);
        _alunoRepository.Atualizar(aluno);

        await _alunoRepository.UnitOfWork.Commit();

        return cursoViewModel;
    }

    #region Dispose

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _alunoRepository.Dispose();
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
