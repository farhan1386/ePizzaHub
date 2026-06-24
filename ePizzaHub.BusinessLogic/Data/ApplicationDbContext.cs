using ePizzaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.BusinessLogic.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Master Collection Database Tables
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserAddressModel> UserAddresses { get; set; }
        public DbSet<PizzaCategoryModel> PizzaCategories { get; set; }
        public DbSet<PizzaModel> Pizzas { get; set; }
        public DbSet<CartItemModel> CartItems { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderItemModel> OrderItems { get; set; }
        public DbSet<PaymentMethodModel> PaymentMethods { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<CouponModel> Coupons { get; set; }
        public DbSet<TaxSettingModel> TaxSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. Explicit Physical Table Routing Map Definitions
            builder.Entity<RoleModel>().ToTable("t_role");
            builder.Entity<UserModel>().ToTable("t_user");
            builder.Entity<UserAddressModel>().ToTable("t_user_address");
            builder.Entity<PizzaCategoryModel>().ToTable("t_pizza_category");
            builder.Entity<PizzaModel>().ToTable("t_pizza");
            builder.Entity<CartItemModel>().ToTable("t_cart_item");
            builder.Entity<OrderModel>().ToTable("t_order");
            builder.Entity<OrderItemModel>().ToTable("t_order_item");
            builder.Entity<PaymentMethodModel>().ToTable("t_payment_method");
            builder.Entity<PaymentModel>().ToTable("t_payment");
            builder.Entity<CouponModel>().ToTable("t_coupon");
            builder.Entity<TaxSettingModel>().ToTable("t_tax_setting");

            // 2. High Precision Schema Constraints Configuration Maps
            builder.Entity<PizzaModel>().Property(p => p.f_price).HasPrecision(18, 2);
            builder.Entity<OrderModel>().Property(o => o.f_total_amount).HasPrecision(18, 2);
            builder.Entity<OrderItemModel>().Property(oi => oi.f_unit_price).HasPrecision(18, 2);
            builder.Entity<PaymentModel>().Property(p => p.f_amount).HasPrecision(18, 2);
            builder.Entity<CouponModel>().Property(c => c.f_discount_percentage).HasPrecision(5, 2);
            builder.Entity<CouponModel>().Property(c => c.f_max_discount_amount).HasPrecision(18, 2);
            builder.Entity<TaxSettingModel>().Property(t => t.f_percentage).HasPrecision(5, 2);

            // 3. Enum DataType Storage String Conversions
            builder.Entity<OrderModel>().Property(o => o.f_order_status).HasConversion<string>();
            builder.Entity<PaymentModel>().Property(p => p.f_payment_status).HasConversion<string>();

            // 4. GLOBAL EXPLICIT FLUENT API COLUMN POSITION REMAPPING ENGINE
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
                {
                    // Force your absolute identity tracking tags to first positions
                    builder.Entity(entityType.ClrType).Property("f_uid").HasColumnOrder(0);
                    builder.Entity(entityType.ClrType).Property("f_iid").HasColumnOrder(1);

                    // Fetch only raw scalar fields declared directly inside the concrete child entity class
                    var scalarProperties = entityType.GetProperties()
                        .Where(p => !p.IsShadowProperty() && p.DeclaringType == entityType)
                        .Where(p => p.Name != "f_uid" && p.Name != "f_iid" &&
                                    p.Name != "f_create_date" && p.Name != "f_create_by" &&
                                    p.Name != "f_update_date" && p.Name != "f_update_by" &&
                                    p.Name != "f_delete_date" && p.Name != "f_delete_by")
                        .ToList();

                    // Sequentially place all child class fields beginning at position index 2
                    int middleIndex = 2;
                    foreach (var prop in scalarProperties)
                    {
                        builder.Entity(entityType.ClrType).Property(prop.Name).HasColumnOrder(middleIndex++);
                    }

                    // Force BaseModel audit traits to position themselves at the absolute end
                    builder.Entity(entityType.ClrType).Property("f_create_date").HasColumnOrder(100);
                    builder.Entity(entityType.ClrType).Property("f_create_by").HasColumnOrder(101);
                    builder.Entity(entityType.ClrType).Property("f_update_date").HasColumnOrder(102);
                    builder.Entity(entityType.ClrType).Property("f_update_by").HasColumnOrder(103);
                    builder.Entity(entityType.ClrType).Property("f_delete_date").HasColumnOrder(104);
                    builder.Entity(entityType.ClrType).Property("f_delete_by").HasColumnOrder(105);
                }
            }

            // 5. Global Relational Referential Integrity Rules
            builder.Entity<UserModel>()
                .HasOne(u => u.f_role)
                .WithMany()
                .HasForeignKey(u => u.f_role_uid)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserAddressModel>()
                .HasOne(ua => ua.f_user)
                .WithMany()
                .HasForeignKey(ua => ua.f_user_iid)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PizzaModel>()
                .HasOne(p => p.f_category)
                .WithMany()
                .HasForeignKey(p => p.f_category_uid)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CartItemModel>()
                .HasOne(c => c.f_pizza)
                .WithMany()
                .HasForeignKey(c => c.f_pizza_uid)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderItemModel>()
                .HasOne(oi => oi.f_order)
                .WithMany(o => o.f_order_items)
                .HasForeignKey(oi => oi.f_order_uid)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItemModel>()
                .HasOne(oi => oi.f_pizza)
                .WithMany()
                .HasForeignKey(oi => oi.f_pizza_uid)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PaymentModel>()
                .HasOne(p => p.f_order)
                .WithMany()
                .HasForeignKey(p => p.f_order_uid)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PaymentModel>()
                .HasOne(p => p.f_payment_method)
                .WithMany()
                .HasForeignKey(p => p.f_payment_method_uid)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. Global Active Row Query Filters for Soft Deletes
            builder.Entity<RoleModel>().HasQueryFilter(r => r.f_delete_date == null);
            builder.Entity<UserModel>().HasQueryFilter(u => u.f_delete_date == null);
            builder.Entity<PizzaCategoryModel>().HasQueryFilter(c => c.f_delete_date == null);
            builder.Entity<PizzaModel>().HasQueryFilter(p => p.f_delete_date == null);
            builder.Entity<OrderModel>().HasQueryFilter(o => o.f_delete_date == null);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseModel>();
            var currentTime = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.f_create_date = currentTime;
                    if (entry.Entity.f_uid == Guid.Empty)
                    {
                        entry.Entity.f_uid = Guid.NewGuid();
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(x => x.f_create_date).IsModified = false;
                    entry.Entity.f_update_date = currentTime;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
