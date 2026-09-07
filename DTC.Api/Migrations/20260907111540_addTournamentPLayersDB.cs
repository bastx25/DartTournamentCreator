using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTC.Api.Migrations
{
    /// <inheritdoc />
    public partial class addTournamentPLayersDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPlayers_TournamentPlayer_TournamentPlayerId",
                table: "GroupPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchParticipants_TournamentPlayer_TournamentPlayerId",
                table: "MatchParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayer_Players_PlayerId",
                table: "TournamentPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayer_Tournaments_TournamentId",
                table: "TournamentPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentPlayer",
                table: "TournamentPlayer");

            migrationBuilder.RenameTable(
                name: "TournamentPlayer",
                newName: "TournamentPlayers");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentPlayer_TournamentId_PlayerId",
                table: "TournamentPlayers",
                newName: "IX_TournamentPlayers_TournamentId_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentPlayer_PlayerId",
                table: "TournamentPlayers",
                newName: "IX_TournamentPlayers_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentPlayers",
                table: "TournamentPlayers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPlayers_TournamentPlayers_TournamentPlayerId",
                table: "GroupPlayers",
                column: "TournamentPlayerId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchParticipants_TournamentPlayers_TournamentPlayerId",
                table: "MatchParticipants",
                column: "TournamentPlayerId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayers_Players_PlayerId",
                table: "TournamentPlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayers_Tournaments_TournamentId",
                table: "TournamentPlayers",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPlayers_TournamentPlayers_TournamentPlayerId",
                table: "GroupPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchParticipants_TournamentPlayers_TournamentPlayerId",
                table: "MatchParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayers_Players_PlayerId",
                table: "TournamentPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayers_Tournaments_TournamentId",
                table: "TournamentPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentPlayers",
                table: "TournamentPlayers");

            migrationBuilder.RenameTable(
                name: "TournamentPlayers",
                newName: "TournamentPlayer");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentPlayers_TournamentId_PlayerId",
                table: "TournamentPlayer",
                newName: "IX_TournamentPlayer_TournamentId_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentPlayers_PlayerId",
                table: "TournamentPlayer",
                newName: "IX_TournamentPlayer_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentPlayer",
                table: "TournamentPlayer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPlayers_TournamentPlayer_TournamentPlayerId",
                table: "GroupPlayers",
                column: "TournamentPlayerId",
                principalTable: "TournamentPlayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchParticipants_TournamentPlayer_TournamentPlayerId",
                table: "MatchParticipants",
                column: "TournamentPlayerId",
                principalTable: "TournamentPlayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayer_Players_PlayerId",
                table: "TournamentPlayer",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayer_Tournaments_TournamentId",
                table: "TournamentPlayer",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
