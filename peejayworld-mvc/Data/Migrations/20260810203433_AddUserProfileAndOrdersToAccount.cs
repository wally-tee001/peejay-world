using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace peejayworld_mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileAndOrdersToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ShippingAddress = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    BillingAddress = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CardBrand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CardLast4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    CardExpiry = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_user_profiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_profiles");
        }
    }
}
