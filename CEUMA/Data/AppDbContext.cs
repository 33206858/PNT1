using CEUMA.Models;
using Microsoft.EntityFrameworkCore;

namespace CEUMA.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Categoria>().HasIndex(c => c.Nombre).IsUnique();
        modelBuilder.Entity<Producto>().HasIndex(p => p.Codigo).IsUnique();
        modelBuilder.Entity<Cliente>().HasIndex(c => c.Email).IsUnique();
        modelBuilder.Entity<Producto>().Property(p => p.Precio).HasPrecision(18, 2);
        modelBuilder.Entity<DetallePedido>().Property(d => d.PrecioUnitario).HasPrecision(18, 2);
        modelBuilder.Entity<DetallePedido>().HasIndex(d => new { d.PedidoId, d.ProductoId }).IsUnique();

        modelBuilder.Entity<Producto>().HasOne(p => p.Categoria)
            .WithMany(c => c.Productos).HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Pedido>().HasOne(p => p.Cliente)
            .WithMany(c => c.Pedidos).HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DetallePedido>().HasOne(d => d.Producto)
            .WithMany(p => p.DetallesPedido).HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DetallePedido>().HasOne(d => d.Pedido)
            .WithMany(p => p.Detalles).HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Producto>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Productos_Precio", "[Precio] > 0");
            t.HasCheckConstraint("CK_Productos_Stock", "[Stock] >= 0");
            t.HasCheckConstraint("CK_Productos_Origen", "[Origen] IN (0, 1, 2)");
        });
        modelBuilder.Entity<DetallePedido>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_DetallesPedido_Cantidad", "[Cantidad] > 0");
            t.HasCheckConstraint("CK_DetallesPedido_Precio", "[PrecioUnitario] > 0");
        });
        modelBuilder.Entity<Pedido>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Pedidos_Estado", "[Estado] IN (0, 1, 2, 3)");
            t.HasCheckConstraint("CK_Pedidos_Cliente", "[Estado] = 0 OR [ClienteId] IS NOT NULL");
        });
    }
}
