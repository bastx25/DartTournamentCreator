using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTC.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddKnockoutDrawMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KnockoutDrawMode",
                table: "TournamentConfigs",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KnockoutDrawMode",
                table: "TournamentConfigs");
        }
    }
}
