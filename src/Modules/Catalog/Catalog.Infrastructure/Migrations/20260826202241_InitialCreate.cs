using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ListPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "catalog",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CategoryId",
                schema: "catalog",
                table: "Courses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Status_CategoryId_PublishedAt",
                schema: "catalog",
                table: "Courses",
                columns: new[] { "Status", "CategoryId", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Status_PublishedAt",
                schema: "catalog",
                table: "Courses",
                columns: new[] { "Status", "PublishedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "catalog");
        }
    }
}
