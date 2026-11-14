using EUMA.Models;
using Microsoft.EntityFrameworkCore;

namespace EUMA.Data;

// El contexto es el puente entre las clases del modelo y la base de datos.
public class AppDbContext : DbContext
{
    // Las opciones (SQL Server y la cadena de conexión) se configuran en Program.cs.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Un DbSet por tabla. EF los completa solo; "= null!" evita la advertencia del compilador.
    // Carrito e ItemCarrito no están acá porque no van a la base (viven en la sesión).
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Pedido> Pedidos { get; set; } = null!;
    public DbSet<DetallePedido> DetallesPedido { get; set; } = null!;

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
        });
        modelBuilder.Entity<DetallePedido>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_DetallesPedido_Cantidad", "[Cantidad] > 0");
            t.HasCheckConstraint("CK_DetallesPedido_Precio", "[PrecioUnitario] > 0");
        });
        modelBuilder.Entity<Pedido>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Pedidos_Estado", "[Estado] IN (0, 1, 2, 3, 4)");
            t.HasCheckConstraint("CK_Pedidos_Entrega", "[Entrega] IN (0, 1)");
        });
    }
}
