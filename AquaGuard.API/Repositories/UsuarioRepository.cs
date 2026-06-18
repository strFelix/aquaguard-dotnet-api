using AquaGuard.API.Data;
using AquaGuard.API.Models;
using AquaGuard.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquaGuard.API.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context) { }

    public async Task<Usuario?> GetByEmailAsync(string email) =>
        await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
}