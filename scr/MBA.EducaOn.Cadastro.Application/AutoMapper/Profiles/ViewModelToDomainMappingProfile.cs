using AutoMapper;
using MBA.EducaOn.GestaoConteudo.Application.ViewModels;
using MBA.EducaOn.GestaoConteudo.Domain;

namespace MBA.EducaOn.GestaoConteudo.Application.AutoMapper.Profiles;

public class ViewModelToDomainMappingProfile :Profile
{
    public ViewModelToDomainMappingProfile()
    {
        CreateMap<CursoViewModel, Curso>()
            .ConstructUsing(c =>
                new Curso(c.Nome, 
                          c.Descricao, 
                          c.Valor, 
                          c.CargaHoraria, 
                          c.PublicoAlvo, 
                          c.Objetivo,
                          c.Requisitos, 
                          new ConteudoProgramatico(c.ConteudoDescricao, c.Revisao, c.DataRevisao)
                )
            )
            .IgnoreAllPropertiesWithAnInaccessibleSetter();

        CreateMap<AulaViewModel, Aula>()
            .ConstructUsing( c=> 
                new Aula(c.CursoId, c.Codigo, c.Titulo, c.Descricao, c.Ordem)
            )
            .IgnoreAllPropertiesWithAnInaccessibleSetter();
    }
}
