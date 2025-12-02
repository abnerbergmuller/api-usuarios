using System.Net.Mime;
using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace APIUsuarios.Controllers;

[ApiController]
[Route("usuarios")]
[Produces(MediaTypeNames.Application.Json)]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repo;

    public UsuariosController(IUsuarioRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ResponseUsuarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var usuarios = await _repo.GetAllAsync(ct);
        var result = usuarios.Select(MapToResponse);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResponseUsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var usuario = await _repo.GetByIdAsync(id, ct);
        if (usuario is null)
            return NotFound();

        return Ok(MapToResponse(usuario));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseUsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var entity = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = dto.Senha,
            Ativo = true
        };

        try
        {
            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);
        }
        catch (InvalidOperationException)
        {
            return Conflict(new { message = "Email já cadastrado" });
        }

        var response = MapToResponse(entity);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ResponseUsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound();

        // Verifica duplicidade de email (exceto o próprio usuário)
        var byEmail = await _repo.GetByEmailAsync(dto.Email, ct);
        if (byEmail is not null && byEmail.Id != id)
            return Conflict(new { message = "Email já cadastrado" });

        existing.Nome = dto.Nome;
        existing.Email = dto.Email;
        existing.Ativo = dto.Ativo;

        await _repo.UpdateAsync(existing, ct);
        await _repo.SaveChangesAsync(ct);

        return Ok(MapToResponse(existing));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound();

        await _repo.RemoveAsync(existing, ct);
        await _repo.SaveChangesAsync(ct);
        return NoContent();
    }

    private static ResponseUsuarioDto MapToResponse(Usuario u)
        => new ResponseUsuarioDto
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Ativo = u.Ativo
        };
}
