using System.ComponentModel;

namespace MBA.EducaOn.Core.Enumerators;

/// <summary>
/// Enumerator para Tipo de Usuario
/// </summary>
public enum TipoUsuario
{
    [Description("Administrador")]
    Administrador,

    [Description("Aluno")]
    Aluno
}
