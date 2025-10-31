using AutoMapper;
using MBA.EducaOn.GestaoAlunos.Application.ViewModels;
using MBA.EducaOn.GestaoAlunos.Domain;

namespace MBA.EducaOn.GestaoAlunos.Application.AutoMapper.Profiles;

public class ViewModelToDomainMappingProfile :Profile
{
    public ViewModelToDomainMappingProfile()
    {
        CreateMap<AlunoViewModel, Aluno>();
    }
}
