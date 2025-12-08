using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Models;

namespace CheburechnayaAPI.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options) { }

        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryItem> DeliveryItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           // modelBuilder.Entity<Employee>()
                //.HasOne(e => e.Position)
                //.WithMany(p => p.Employees)
                //.HasForeignKey(e => e.PositionId);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Supplier)
                .WithMany(e => e.Deliveries)
                .HasForeignKey(d => d.SupplierId);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Employee)
                .WithMany()
                .HasForeignKey(d => d.EmployeeId);

            modelBuilder.Entity<DeliveryItem>()
                .HasOne(di => di.Delivery)
                .WithMany(d => d.DeliveryItems)
                .HasForeignKey(di => di.DeliveryId);

            modelBuilder.Entity<DeliveryItem>()
                .HasOne(di => di.Product)
                .WithMany(p => p.DeliveryItems)
                .HasForeignKey(di => di.ProductId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.EmployeeId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<Delivery>()
                .Property(d => d.DeliveryDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Delivery>()
                .Property(d => d.Status)
                .HasDefaultValue("Доставлено");

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasDefaultValue("Принят");
        }
    }
}