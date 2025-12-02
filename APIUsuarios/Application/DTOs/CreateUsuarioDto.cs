using System.ComponentModel.DataAnnotations;

namespace APIUsuarios.Application.DTOs;

public class CreateUsuarioDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Senha { get; set; } = string.Empty;
}
