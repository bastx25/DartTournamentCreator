using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTC.Api.Migrations
{
    /// <inheritdoc />
    public partial class addAvailabilityToTournamentPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_MatchParticipants_MatchParticipantId",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "MatchParticipantId",
                table: "Availabilities",
                newName: "TournamentPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Availabilities_MatchParticipantId",
                table: "Availabilities",
                newName: "IX_Availabilities_TournamentPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_TournamentPlayers_TournamentPlayerId",
                table: "Availabilities",
                column: "TournamentPlayerId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_TournamentPlayers_TournamentPlayerId",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "TournamentPlayerId",
                table: "Availabilities",
                newName: "MatchParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Availabilities_TournamentPlayerId",
                table: "Availabilities",
                newName: "IX_Availabilities_MatchParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_MatchParticipants_MatchParticipantId",
                table: "Availabilities",
                column: "MatchParticipantId",
                principalTable: "MatchParticipants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
