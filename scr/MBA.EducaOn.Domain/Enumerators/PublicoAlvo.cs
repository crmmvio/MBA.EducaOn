using System.ComponentModel;

namespace MBA.EducaOn.Core.Enumerators;

/// <summary>
/// Enumerator para Publico Alvo
/// </summary>
public enum PublicoAlvo
{
    [Description("Todos")]
    Todos = 0,
    
    [Description("Iniciante")]
    Iniciante = 1,

    [Description("Avancado")]
    Avancado = 2
}
