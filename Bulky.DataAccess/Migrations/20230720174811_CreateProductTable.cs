using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreateProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "Joseph Murphy", "", "", 100.0, 105.0, 90.0, "The Power of Your Subconscious Mind" },
                    { 2, "Darius Foroux", "", "", 140.0, 150.0, 125.0, "Do It Today" },
                    { 3, "thibaut meurisse and Kerry J Donovan", "", "", 13000.0, 13105.0, 11000.0, "Master Your Emotions" },
                    { 4, "", "", "", 169.0, 179.0, 149.0, "Energize Your Mind" },
                    { 5, "", "", "", 89.0, 99.0, 70.0, "Brain Activity Book for Kids" },
                    { 6, "", "", "", 125.0, 139.0, 105.0, "The Secret Garden" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
