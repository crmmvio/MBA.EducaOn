using AutoMapper;
using MBA.EducaOn.GestaoAlunos.Application.AutoMapper.Profiles;
using Microsoft.Extensions.Logging.Abstractions;

namespace MBA.EducaOn.GestaoAlunos.Application.AutoMapper;

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
