using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ePizzaHub.BusinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_coupon",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_discount_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    f_expiry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    f_is_active = table.Column<bool>(type: "bit", nullable: false),
                    f_max_discount_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_coupon", x => x.f_uid);
                });

            migrationBuilder.CreateTable(
                name: "t_order",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_contact_phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_customer_user_uid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_delivery_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_is_paid = table.Column<bool>(type: "bit", nullable: false),
                    f_order_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_total_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_order", x => x.f_uid);
                });

            migrationBuilder.CreateTable(
                name: "t_payment_method",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_is_active = table.Column<bool>(type: "bit", nullable: false),
                    f_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_payment_method", x => x.f_uid);
                });

            migrationBuilder.CreateTable(
                name: "t_pizza_category",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_is_active = table.Column<bool>(type: "bit", nullable: false),
                    f_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_pizza_category", x => x.f_uid);
                });

            migrationBuilder.CreateTable(
                name: "t_role",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_is_active = table.Column<bool>(type: "bit", nullable: false),
                    f_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_role", x => x.f_uid);
                });

            migrationBuilder.CreateTable(
                name: "t_tax_setting",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_is_active = table.Column<bool>(type: "bit", nullable: false),
                    f_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    f_tax_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_tax_setting", x => x.f_uid);
                });

            migrationBuilder.CreateTable(
                name: "t_payment",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    f_order_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_payment_method_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_payment_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_transaction_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_payment", x => x.f_uid);
                    table.ForeignKey(
                        name: "FK_t_payment_t_order_f_order_uid",
                        column: x => x.f_order_uid,
                        principalTable: "t_order",
                        principalColumn: "f_uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_payment_t_payment_method_f_payment_method_uid",
                        column: x => x.f_payment_method_uid,
                        principalTable: "t_payment_method",
                        principalColumn: "f_uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_pizza",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_category_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_is_available = table.Column<bool>(type: "bit", nullable: false),
                    f_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_pizza", x => x.f_uid);
                    table.ForeignKey(
                        name: "FK_t_pizza_t_pizza_category_f_category_uid",
                        column: x => x.f_category_uid,
                        principalTable: "t_pizza_category",
                        principalColumn: "f_uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_user",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_fname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_is_active = table.Column<bool>(type: "bit", nullable: false),
                    f_lname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_role_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user", x => x.f_uid);
                    table.ForeignKey(
                        name: "FK_t_user_t_role_f_role_uid",
                        column: x => x.f_role_uid,
                        principalTable: "t_role",
                        principalColumn: "f_uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_cart_item",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_customer_session_uid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_pizza_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_quantity = table.Column<int>(type: "int", nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_cart_item", x => x.f_uid);
                    table.ForeignKey(
                        name: "FK_t_cart_item_t_pizza_f_pizza_uid",
                        column: x => x.f_pizza_uid,
                        principalTable: "t_pizza",
                        principalColumn: "f_uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_order_item",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_order_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_pizza_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_quantity = table.Column<int>(type: "int", nullable: false),
                    f_unit_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_order_item", x => x.f_uid);
                    table.ForeignKey(
                        name: "FK_t_order_item_t_order_f_order_uid",
                        column: x => x.f_order_uid,
                        principalTable: "t_order",
                        principalColumn: "f_uid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_order_item_t_pizza_f_pizza_uid",
                        column: x => x.f_pizza_uid,
                        principalTable: "t_pizza",
                        principalColumn: "f_uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_user_address",
                columns: table => new
                {
                    f_uid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_iid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_address_line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_city = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_is_default = table.Column<bool>(type: "bit", nullable: false),
                    f_postal_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_user_iid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    f_create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_create_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_update_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    f_delete_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_delete_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user_address", x => x.f_uid);
                    table.ForeignKey(
                        name: "FK_t_user_address_t_user_f_user_iid",
                        column: x => x.f_user_iid,
                        principalTable: "t_user",
                        principalColumn: "f_uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_cart_item_f_pizza_uid",
                table: "t_cart_item",
                column: "f_pizza_uid");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_item_f_order_uid",
                table: "t_order_item",
                column: "f_order_uid");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_item_f_pizza_uid",
                table: "t_order_item",
                column: "f_pizza_uid");

            migrationBuilder.CreateIndex(
                name: "IX_t_payment_f_order_uid",
                table: "t_payment",
                column: "f_order_uid");

            migrationBuilder.CreateIndex(
                name: "IX_t_payment_f_payment_method_uid",
                table: "t_payment",
                column: "f_payment_method_uid");

            migrationBuilder.CreateIndex(
                name: "IX_t_pizza_f_category_uid",
                table: "t_pizza",
                column: "f_category_uid");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_f_role_uid",
                table: "t_user",
                column: "f_role_uid");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_address_f_user_iid",
                table: "t_user_address",
                column: "f_user_iid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_cart_item");

            migrationBuilder.DropTable(
                name: "t_coupon");

            migrationBuilder.DropTable(
                name: "t_order_item");

            migrationBuilder.DropTable(
                name: "t_payment");

            migrationBuilder.DropTable(
                name: "t_tax_setting");

            migrationBuilder.DropTable(
                name: "t_user_address");

            migrationBuilder.DropTable(
                name: "t_pizza");

            migrationBuilder.DropTable(
                name: "t_order");

            migrationBuilder.DropTable(
                name: "t_payment_method");

            migrationBuilder.DropTable(
                name: "t_user");

            migrationBuilder.DropTable(
                name: "t_pizza_category");

            migrationBuilder.DropTable(
                name: "t_role");
        }
    }
}
