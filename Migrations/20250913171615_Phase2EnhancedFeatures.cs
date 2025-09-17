using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SkillNet.Migrations
{
    public partial class Phase2EnhancedFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndorsementCount",
                table: "UserSkills",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "UserSkills",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDate",
                table: "UserSkills",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "UserSkills",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Skills",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Skills",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Skills",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "ConnectionStrength",
                table: "Connections",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "InteractionCount",
                table: "Connections",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastInteraction",
                table: "Connections",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MutualConnectionsCount",
                table: "Connections",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RelationshipNote",
                table: "Connections",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SavedSearches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    SearchTerm = table.Column<string>(maxLength: 500, nullable: false),
                    SearchType = table.Column<string>(nullable: true),
                    Filters = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUsed = table.Column<DateTime>(nullable: false),
                    EmailNotifications = table.Column<bool>(nullable: false),
                    NotificationFrequency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSearches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedSearches_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    SearchTerm = table.Column<string>(maxLength: 500, nullable: false),
                    SearchType = table.Column<string>(nullable: true),
                    Filters = table.Column<string>(nullable: true),
                    SearchDate = table.Column<DateTime>(nullable: false),
                    ResultCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillEndorsements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndorserId = table.Column<string>(nullable: false),
                    EndorsedUserId = table.Column<string>(nullable: false),
                    SkillId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillEndorsements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillEndorsements_AspNetUsers_EndorsedUserId",
                        column: x => x.EndorsedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillEndorsements_AspNetUsers_EndorserId",
                        column: x => x.EndorserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillEndorsements_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    SkillId = table.Column<int>(nullable: false),
                    Method = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Evidence = table.Column<string>(maxLength: 1000, nullable: true),
                    VerifierNotes = table.Column<string>(maxLength: 500, nullable: true),
                    VerifierId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    VerifiedDate = table.Column<DateTime>(nullable: true),
                    Score = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillVerifications_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillVerifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillVerifications_AspNetUsers_VerifierId",
                        column: x => x.VerifierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "Microsoft's object-oriented programming language", "fab fa-microsoft", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "Dynamic programming language for web development", "fab fa-js", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "High-level programming language for various applications", "fab fa-python", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "User interface and user experience design", "fas fa-paint-brush", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "Visual communication and problem-solving through design", "fas fa-palette", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "JavaScript library for building user interfaces", "fab fa-react", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "Progressive JavaScript framework", "fab fa-vuejs", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "JavaScript runtime for server-side development", "fab fa-node-js", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "Structured Query Language for database management", "fas fa-database", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "NoSQL document-oriented database", "fas fa-leaf", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "Planning and organizing project activities", "fas fa-tasks", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "Online marketing and promotion strategies", "fas fa-bullhorn", true });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Description", "Icon", "IsActive" },
                values: new object[] { "Creating engaging written content", "fas fa-pen", true });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Category", "Description", "Icon", "IsActive", "Name" },
                values: new object[,]
                {
                    { 19, "Data Science", "Analyzing and interpreting data", "fas fa-chart-line", true, "Data Analysis" },
                    { 18, "AI/ML", "Algorithms that learn from data", "fas fa-robot", true, "Machine Learning" },
                    { 17, "Cloud", "Amazon Web Services cloud platform", "fab fa-aws", true, "AWS" },
                    { 16, "DevOps", "Containerization platform", "fab fa-docker", true, "Docker" },
                    { 15, "Frontend", "TypeScript-based web application framework", "fab fa-angular", true, "Angular" },
                    { 20, "Security", "Protecting systems from digital attacks", "fas fa-shield-alt", true, "Cybersecurity" },
                    { 14, "Programming", "Object-oriented programming language", "fab fa-java", true, "Java" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearches_UserId",
                table: "SavedSearches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistories_UserId",
                table: "SearchHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillEndorsements_EndorsedUserId",
                table: "SkillEndorsements",
                column: "EndorsedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillEndorsements_EndorserId",
                table: "SkillEndorsements",
                column: "EndorserId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillEndorsements_SkillId",
                table: "SkillEndorsements",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillVerifications_SkillId",
                table: "SkillVerifications",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillVerifications_UserId",
                table: "SkillVerifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillVerifications_VerifierId",
                table: "SkillVerifications",
                column: "VerifierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedSearches");

            migrationBuilder.DropTable(
                name: "SearchHistories");

            migrationBuilder.DropTable(
                name: "SkillEndorsements");

            migrationBuilder.DropTable(
                name: "SkillVerifications");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DropColumn(
                name: "EndorsementCount",
                table: "UserSkills");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "UserSkills");

            migrationBuilder.DropColumn(
                name: "VerifiedDate",
                table: "UserSkills");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "UserSkills");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "ConnectionStrength",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "InteractionCount",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "LastInteraction",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "MutualConnectionsCount",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "RelationshipNote",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "AspNetUsers");
        }
    }
}
