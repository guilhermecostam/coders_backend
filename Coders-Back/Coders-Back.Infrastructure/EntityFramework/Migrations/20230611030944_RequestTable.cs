using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coders_Back.Infrastructure.Migrations
{
    public partial class RequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectJoinRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectJoinRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collaborators_UserId_ProjectId",
                table: "Collaborators",
                columns: new[] { "UserId", "ProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJoinRequests_UserId_ProjectId",
                table: "ProjectJoinRequests",
                columns: new[] { "UserId", "ProjectId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectJoinRequests");

            migrationBuilder.DropIndex(
                name: "IX_Collaborators_UserId_ProjectId",
                table: "Collaborators");
        }
    }
}
