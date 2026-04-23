using Microsoft.EntityFrameworkCore;
using Pharmacy.Api.Models;

namespace Pharmacy.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Order> Orders => Set<Order>();
}