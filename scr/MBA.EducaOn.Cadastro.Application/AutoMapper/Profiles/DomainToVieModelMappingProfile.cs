using AutoMapper;
using MBA.EducaOn.GestaoConteudo.Application.ViewModels;
using MBA.EducaOn.GestaoConteudo.Domain;

namespace MBA.EducaOn.GestaoConteudo.Application.AutoMapper.Profiles
{
    public class DomainToVieModelMappingProfile : Profile
    {
        public DomainToVieModelMappingProfile()
        {
            CreateMap<Curso, CursoViewModel>();
        }
    }
}
