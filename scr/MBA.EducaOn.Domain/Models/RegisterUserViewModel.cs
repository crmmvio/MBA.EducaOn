using System.ComponentModel.DataAnnotations;

namespace MBA.EducaOn.Core.Models;

public class RegisterUserViewModel
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public required string UserName { get; set; } 

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6)]
    public required string Password { get; set; }

    [Compare("Password", ErrorMessage = "As senhas não conferem.")]
    public string? ConfirmPassword { get; set; }
}
