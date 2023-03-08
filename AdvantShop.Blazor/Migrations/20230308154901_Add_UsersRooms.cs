using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvantShop.Blazor.Migrations
{
    /// <inheritdoc />
    public partial class Add_UsersRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRoomUsers",
                columns: table => new
                {
                    RoomId = table.Column<long>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomUsers", x => new { x.RoomId, x.UserName });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRoomUsers");
        }
    }
}
