using ePizzaHub.Models;
using ePizzaHub.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.BusinessLogic.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (await context.Roles.AnyAsync())
            {
                return;
            }

            Guid systemAdminCreatorUid = Guid.Parse("00000000-0000-0000-0000-000000000001");

            // 1. Seed Core Identity Group Security Roles
            var adminRole = new RoleModel
            {
                f_uid = Guid.NewGuid(),
                f_name = "Administrator",
                f_create_date = DateTime.Now,
                f_create_by = systemAdminCreatorUid
            };
            var customerRole = new RoleModel
            {
                f_uid = Guid.NewGuid(),
                f_name = "Customer",
                f_create_date = DateTime.Now,
                f_create_by = systemAdminCreatorUid
            };
            await context.Roles.AddRangeAsync(new List<RoleModel> { adminRole, customerRole });
            await context.SaveChangesAsync();

            // 2. Seed Master User Accounts
            var adminUser = new UserModel
            {
                f_uid = Guid.NewGuid(),
                f_email = "admin@epizzahub.com",
                f_password_hash = "ADMIN_SECURE_CRYPTO_HASH_VALUE",
                f_fname = "Farhan",
                f_lname = "Ahmed",
                f_phone = "+919876543210",
                f_role_uid = adminRole.f_uid,
                f_create_date = DateTime.Now,
                f_create_by = systemAdminCreatorUid
            };

            var customerUser = new UserModel
            {
                f_uid = Guid.NewGuid(),
                f_email = "customer@gmail.com",
                f_password_hash = "CUSTOMER_SECURE_CRYPTO_HASH_VALUE",
                f_fname = "John",
                f_lname = "Doe",
                f_phone = "+1234567890",
                f_role_uid = customerRole.f_uid,
                f_create_date = DateTime.Now,
                f_create_by = adminUser.f_uid
            };

            await context.Users.AddRangeAsync(new List<UserModel> { adminUser, customerUser });
            await context.SaveChangesAsync();

            // 3. Seed Saved Delivery Customer Target Addresses
            var customerAddress = new UserAddressModel
            {
                f_uid = Guid.NewGuid(),
                f_user_iid = customerUser.f_uid,
                f_address_line = "123 Innovation Boulevard",
                f_city = "Jaipur",
                f_postal_code = "302001",
                f_is_default = true,
                f_create_date = DateTime.Now,
                f_create_by = customerUser.f_uid
            };
            await context.UserAddresses.AddAsync(customerAddress);
            await context.SaveChangesAsync();

            // 4. Seed Platform Financial Taxes & Marketing Discount Vouchers
            var vatTax = new TaxSettingModel
            {
                f_uid = Guid.NewGuid(),
                f_tax_name = "VAT",
                f_percentage = 5.00m,
                f_create_date = DateTime.Now,
                f_create_by = adminUser.f_uid
            };
            await context.TaxSettings.AddAsync(vatTax);

            var promoCoupon = new CouponModel
            {
                f_uid = Guid.NewGuid(),
                f_code = "PIZZA50",
                f_discount_percentage = 10.00m,
                f_max_discount_amount = 5.00m,
                f_expiry_date = DateTime.Now.AddMonths(1),
                f_create_date = DateTime.Now,
                f_create_by = adminUser.f_uid
            };
            await context.Coupons.AddAsync(promoCoupon);

            var cardPaymentMethod = new PaymentMethodModel
            {
                f_uid = Guid.NewGuid(),
                f_name = "Credit / Debit Card",
                f_create_date = DateTime.Now,
                f_create_by = adminUser.f_uid
            };
            await context.PaymentMethods.AddAsync(cardPaymentMethod);
            await context.SaveChangesAsync();

            // 5. Seed Catalog Menu Group Aggregations Categories
            var vegCategory = new PizzaCategoryModel
            {
                f_uid = Guid.NewGuid(),
                f_name = "Vegetarian Feasts",
                f_create_date = DateTime.Now,
                f_create_by = adminUser.f_uid
            };
            var nonVegCategory = new PizzaCategoryModel
            {
                f_uid = Guid.NewGuid(),
                f_name = "Meat Lovers Choice",
                f_create_date = DateTime.Now,
                f_create_by = adminUser.f_uid
            };
            await context.PizzaCategories.AddRangeAsync(new List<PizzaCategoryModel> { vegCategory, nonVegCategory });
            await context.SaveChangesAsync();

            // 6. Seed Specific Pizza Menu Items with explicit .png paths
            var margherita = new PizzaModel
            {
                f_uid = Guid.NewGuid(),
                f_name = "Classic Margherita",
                f_description = "Traditional fresh mozzarella cheese, rich herb tomato base sauce, and basil leaves.",
                f_price = 10.99m,
                f_image_url = "/images/margherita.jpg",
                f_is_available = true,
                f_category_uid = vegCategory.f_uid,
                f_create_date = DateTime.Now,
                f_create_by = adminUser.f_uid
            };

            var pepperoni = new PizzaModel
            {
                f_uid = Guid.NewGuid(),
                f_name = "Spicy Pepperoni Supreme",
                f_description = "Loaded with double premium cured pepperoni slices, mozzarella, and hot jalapeños.",
                f_price = 14.49m,
                f_image_url = "/images/pepperoni.jpg",
                f_is_available = true,
                f_category_uid = nonVegCategory.f_uid,
                f_create_date = DateTime.Now,
                f_create_by = adminUser.f_uid
            };

            var bbqChicken = new PizzaModel
            {
                f_uid = Guid.NewGuid(),
                f_name = "Smoky BBQ Chicken",
                f_description = "Grilled chicken chunks, red sweet onions, bell peppers, and signature smoky BBQ drizzle.",
                f_price = 13.99m,
                f_image_url = "/images/bbq-chicken.jpg",
                f_is_available = true,
                f_category_uid = nonVegCategory.f_uid,
                f_create_date = DateTime.Now,
                f_create_by = adminUser.f_uid
            };

            await context.Pizzas.AddRangeAsync(new List<PizzaModel> { margherita, pepperoni, bbqChicken });
            await context.SaveChangesAsync();

            // 7. Seed Active Client Cookie Baskets Sessions
            string sessionUid = "GUEST_SESSION_XYZ_98765";
            var sampleCartItem = new CartItemModel
            {
                f_uid = Guid.NewGuid(),
                f_customer_session_uid = sessionUid,
                f_pizza_uid = margherita.f_uid,
                f_quantity = 2,
                f_create_date = DateTime.Now,
                f_create_by = customerUser.f_uid
            };
            await context.CartItems.AddAsync(sampleCartItem);
            await context.SaveChangesAsync();

            // 8. Seed Historical Checkout Order Header Logs
            var sampleOrder = new OrderModel
            {
                f_uid = Guid.NewGuid(),
                f_customer_user_uid = customerUser.f_uid.ToString(),
                f_total_amount = 39.97m,
                f_delivery_address = customerAddress.f_address_line,
                f_contact_phone = customerUser.f_phone,
                f_order_status = OrderStatus.Baking,
                f_is_paid = true,
                f_create_date = DateTime.Now,
                f_create_by = customerUser.f_uid
            };
            await context.Orders.AddAsync(sampleOrder);
            await context.SaveChangesAsync();
            // 9. Seed Multi-Item Order Snapshot Extensions 
            var orderItem1 = new OrderItemModel
            {
                f_uid = Guid.NewGuid(),
                f_order_uid = sampleOrder.f_uid,
                f_pizza_uid = margherita.f_uid,
                f_quantity = 1,
                f_unit_price = 10.99m,
                f_create_date = DateTime.Now,
                f_create_by = customerUser.f_uid
            };

            var orderItem2 = new OrderItemModel
            {
                f_uid = Guid.NewGuid(),
                f_order_uid = sampleOrder.f_uid,
                f_pizza_uid = pepperoni.f_uid,
                f_quantity = 2,
                f_unit_price = 14.49m,
                f_create_date = DateTime.Now,
                f_create_by = customerUser.f_uid
            };
            await context.OrderItems.AddRangeAsync(new List<OrderItemModel> { orderItem1, orderItem2 });
            await context.SaveChangesAsync();

            // 10. Seed Core Financial Checkout Settlement Logs
            var paymentLog = new PaymentModel
            {
                f_uid = Guid.NewGuid(),
                f_order_uid = sampleOrder.f_uid,
                f_payment_method_uid = cardPaymentMethod.f_uid,
                f_transaction_id = "TXN_STRIPE_PRODUCTION_8877",
                f_amount = 39.97m,
                f_payment_status = PaymentStatus.Success,
                f_create_date = DateTime.Now,
                f_create_by = customerUser.f_uid
            };
            await context.Payments.AddAsync(paymentLog);
            await context.SaveChangesAsync();
        }
    }
}