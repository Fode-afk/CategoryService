using CategoryService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Application.Interfaces.Data;

public interface IAppDbContext
{
    DbSet<Category> Categories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
