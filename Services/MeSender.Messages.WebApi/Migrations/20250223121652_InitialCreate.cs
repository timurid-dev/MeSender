using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeSender.Messages.WebApi.Migrations
{
    /// <inheritdoc />
#pragma warning disable MA0048
    public sealed partial class InitialCreate : Migration
#pragma warning restore MA0048
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", value: true),
                    Text = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
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
