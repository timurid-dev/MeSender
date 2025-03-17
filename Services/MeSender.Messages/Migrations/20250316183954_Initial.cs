using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeSender.Messages.Migrations
{
    /// <inheritdoc />
#pragma warning disable MA0048
    internal sealed partial class Initial : Migration
#pragma warning restore MA0048
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreateTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdateTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
