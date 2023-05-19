using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT id FROM AspNetRoles WHERE Id = 'a9e9e697-d960-498f-b1a6-f2d89d5372b4')
                BEGIN
	                INSERT AspNetRoles (Id, [Name], [NormalizedName])
	                VALUES ('a9e9e697-d960-498f-b1a6-f2d89d5372b4', 'admin', 'ADMIN')
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE AspNetRoles WHERE Id = 'a9e9e697-d960-498f-b1a6-f2d89d5372b4'");
        }
    }
}