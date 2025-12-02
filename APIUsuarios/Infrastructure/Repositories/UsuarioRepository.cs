using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;
using APIUsuarios.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct)
    {
        // Retorna todos os usuários (inclui inativos; filtragem pode ser feita na camada de serviço)
        return await _context.Usuarios
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var emailNorm = NormalizeEmail(email);
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == emailNorm, ct);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken ct)
    {
        if (usuario == null) throw new ArgumentNullException(nameof(usuario));

        usuario.Email = NormalizeEmail(usuario.Email);

        // Regra: não permitir e-mail duplicado na criação.
        if (await EmailExistsAsync(usuario.Email, ct))
        {
            throw new InvalidOperationException("E-mail já cadastrado.");
        }

        await _context.Usuarios.AddAsync(usuario, ct);
    }

    public Task UpdateAsync(Usuario usuario, CancellationToken ct)
    {
        if (usuario == null) throw new ArgumentNullException(nameof(usuario));

        usuario.Email = NormalizeEmail(usuario.Email);
        _context.Usuarios.Update(usuario);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Usuario usuario, CancellationToken ct)
    {
        if (usuario == null) throw new ArgumentNullException(nameof(usuario));

        // Soft delete: apenas marcar como inativo.
        usuario.Ativo = false;
        _context.Usuarios.Update(usuario);
        return Task.CompletedTask;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        var emailNorm = NormalizeEmail(email);
        return await _context.Usuarios.AnyAsync(u => u.Email == emailNorm, ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _context.SaveChangesAsync(ct);

    private static string NormalizeEmail(string email)
        => (email ?? string.Empty).Trim().ToLowerInvariant();
}
