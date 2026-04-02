using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolfBuddy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGolfTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseName",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "Rounds");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Rounds",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Handicap",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoleNumber = table.Column<int>(type: "int", nullable: false),
                    Par = table.Column<int>(type: "int", nullable: false),
                    Yardage = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Holes_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoleId = table.Column<int>(type: "int", nullable: false),
                    RoundId = table.Column<int>(type: "int", nullable: false),
                    ShotNumber = table.Column<int>(type: "int", nullable: false),
                    DistanceYards = table.Column<int>(type: "int", nullable: false),
                    Club = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shots_Holes_HoleId",
                        column: x => x.HoleId,
                        principalTable: "Holes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shots_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_CourseId",
                table: "Rounds",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Holes_CourseId",
                table: "Holes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Shots_HoleId",
                table: "Shots",
                column: "HoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Shots_RoundId",
                table: "Shots",
                column: "RoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Courses_CourseId",
                table: "Rounds",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Courses_CourseId",
                table: "Rounds");

            migrationBuilder.DropTable(
                name: "Shots");

            migrationBuilder.DropTable(
                name: "Holes");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_CourseId",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Rounds");

            migrationBuilder.AddColumn<string>(
                name: "CourseName",
                table: "Rounds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalScore",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Handicap",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
