using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTC.Api.Migrations
{
    /// <inheritdoc />
    public partial class addTournamentCofig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreakBetweenMatchesMinutes",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "MatchDurationMinutes",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Tournaments");

            migrationBuilder.CreateTable(
                name: "TournamentConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    VersionNr = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    MatchDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    BreakBetweenMatchesMinutes = table.Column<int>(type: "int", nullable: false),
                    GroupCount = table.Column<int>(type: "int", nullable: false),
                    PlayersPerGroup = table.Column<int>(type: "int", nullable: true),
                    QualifiersPerGroup = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentConfigs_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TournamentConfigs_TournamentId",
                table: "TournamentConfigs",
                column: "TournamentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TournamentConfigs");

            migrationBuilder.AddColumn<int>(
                name: "BreakBetweenMatchesMinutes",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatchDurationMinutes",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
