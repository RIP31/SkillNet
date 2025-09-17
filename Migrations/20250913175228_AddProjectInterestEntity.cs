using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SkillNet.Migrations
{
    public partial class AddProjectInterestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectInterests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ResponseDate = table.Column<DateTime>(nullable: true),
                    ResponseMessage = table.Column<string>(maxLength: 500, nullable: true),
                    MatchingScore = table.Column<double>(nullable: false),
                    IsHighPriority = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectInterests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectInterests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInterests_ProjectId",
                table: "ProjectInterests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInterests_UserId_ProjectId",
                table: "ProjectInterests",
                columns: new[] { "UserId", "ProjectId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectInterests");
        }
    }
}
