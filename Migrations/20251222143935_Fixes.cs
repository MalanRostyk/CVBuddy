using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuddy.Migrations
{
    /// <inheritdoc />
    public partial class Fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cvs_UserId",
                table: "Cvs");

            migrationBuilder.RenameColumn(
                name: "PersonalCharacteristics",
                table: "Cvs",
                newName: "PCIds");

            migrationBuilder.RenameColumn(
                name: "Certificates",
                table: "Cvs",
                newName: "CertIds");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Cvs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Interests",
                table: "Cvs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    CertId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CertName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertIds = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.CertId);
                    table.ForeignKey(
                        name: "FK_Certificate_Cvs_CertIds",
                        column: x => x.CertIds,
                        principalTable: "Cvs",
                        principalColumn: "Cid");
                });

            migrationBuilder.CreateTable(
                name: "PersonalCharacteristic",
                columns: table => new
                {
                    PCId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacteristicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCIds = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalCharacteristic", x => x.PCId);
                    table.ForeignKey(
                        name: "FK_PersonalCharacteristic_Cvs_PCIds",
                        column: x => x.PCIds,
                        principalTable: "Cvs",
                        principalColumn: "Cid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cvs_UserId",
                table: "Cvs",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_CertIds",
                table: "Certificate",
                column: "CertIds");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalCharacteristic_PCIds",
                table: "PersonalCharacteristic",
                column: "PCIds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropTable(
                name: "PersonalCharacteristic");

            migrationBuilder.DropIndex(
                name: "IX_Cvs_UserId",
                table: "Cvs");

            migrationBuilder.RenameColumn(
                name: "PCIds",
                table: "Cvs",
                newName: "PersonalCharacteristics");

            migrationBuilder.RenameColumn(
                name: "CertIds",
                table: "Cvs",
                newName: "Certificates");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Cvs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Interests",
                table: "Cvs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cvs_UserId",
                table: "Cvs",
                column: "UserId",
                unique: true);
        }
    }
}
