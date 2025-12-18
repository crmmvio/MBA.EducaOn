using AutoMapper;
using MBA.EducaOn.GestaoConteudo.Application.ViewModels;
using MBA.EducaOn.GestaoConteudo.Domain;

namespace MBA.EducaOn.GestaoConteudo.Application.AutoMapper.Profiles
{
    public class DomainToVieModelMappingProfile : Profile
    {
        public DomainToVieModelMappingProfile()
        {
            CreateMap<Curso, CursoViewModel>()
                .ForMember(dest => dest.ConteudoDescricao, opt => opt.MapFrom(src => src.ConteudoProgramatico.ConteudoDescricao))
                .ForMember(dest => dest.Revisao, opt => opt.MapFrom(src => src.ConteudoProgramatico.Revisao))
                .ForMember(dest => dest.DataRevisao, opt => opt.MapFrom(src => src.ConteudoProgramatico.DataRevisao));
            CreateMap<Aula, AulaViewModel>();
        }
    }
}
