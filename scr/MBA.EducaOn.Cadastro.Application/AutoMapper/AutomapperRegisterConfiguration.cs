using AutoMapper;
using MBA.EducaOn.GestaoConteudo.Application.AutoMapper.Profiles;
using Microsoft.Extensions.Logging.Abstractions;

namespace MBA.EducaOn.GestaoConteudo.Application.AutoMapper;

public static class AutomapperRegisterConfiguration
{
    public static MapperConfiguration RegisterMappings()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DomainToVieModelMappingProfile());
            cfg.AddProfile(new ViewModelToDomainMappingProfile());
        }, NullLoggerFactory.Instance);

        return config;
    }
}
