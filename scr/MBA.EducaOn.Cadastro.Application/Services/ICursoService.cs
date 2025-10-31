using MBA.EducaOn.GestaoConteudo.Application.ViewModels;

namespace MBA.EducaOn.GestaoConteudo.Application.Services;

public interface ICursoService : IDisposable
{
    Task<CursoViewModel> ObterPorId(Guid id);
    Task<IEnumerable<CursoViewModel>> ObterTodos();
    Task Adicionar(CursoViewModel cursoViewModel);
    Task<CursoViewModel> Atualizar(CursoViewModel cursoViewModel);
}
