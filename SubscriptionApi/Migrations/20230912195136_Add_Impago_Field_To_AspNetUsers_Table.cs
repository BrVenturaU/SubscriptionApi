using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionApi.Migrations
{
    public partial class Add_Impago_Field_To_AspNetUsers_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Impago",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Impago",
                table: "AspNetUsers");
        }
    }
}
