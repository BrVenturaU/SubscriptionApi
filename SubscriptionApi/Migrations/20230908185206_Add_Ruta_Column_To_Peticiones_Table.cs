using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionApi.Migrations
{
    public partial class Add_Ruta_Column_To_Peticiones_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ruta",
                table: "Peticiones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ruta",
                table: "Peticiones");
        }
    }
}
