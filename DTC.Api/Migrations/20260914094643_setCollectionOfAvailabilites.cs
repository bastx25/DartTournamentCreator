using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTC.Api.Migrations
{
    /// <inheritdoc />
    public partial class setCollectionOfAvailabilites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Availabilities_TournamentPlayerId",
                table: "Availabilities");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_TournamentPlayerId",
                table: "Availabilities",
                column: "TournamentPlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Availabilities_TournamentPlayerId",
                table: "Availabilities");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_TournamentPlayerId",
                table: "Availabilities",
                column: "TournamentPlayerId",
                unique: true);
        }
    }
}
