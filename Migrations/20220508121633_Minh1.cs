using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace demoasm2.Migrations
{
    public partial class Minh1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Cart");
        }
    }
}
