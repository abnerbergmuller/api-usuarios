using System.ComponentModel.DataAnnotations;

namespace APIUsuarios.Application.DTOs;

public class UpdateUsuarioDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
}
