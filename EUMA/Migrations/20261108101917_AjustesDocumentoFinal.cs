using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EUMA.Migrations
{
    /// <inheritdoc />
    public partial class AjustesDocumentoFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Productos_Origen",
                table: "Productos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Pedidos_Cliente",
                table: "Pedidos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Pedidos_Estado",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Origen",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Localidad",
                table: "Clientes");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoPostal",
                table: "Pedidos",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Pedidos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Entrega",
                table: "Pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPago",
                table: "Pedidos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localidad",
                table: "Pedidos",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Categorias",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pedidos_Entrega",
                table: "Pedidos",
                sql: "[Entrega] IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pedidos_Estado",
                table: "Pedidos",
                sql: "[Estado] IN (0, 1, 2, 3, 4)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Pedidos_Entrega",
                table: "Pedidos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Pedidos_Estado",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "CodigoPostal",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Entrega",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "FechaPago",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Localidad",
                table: "Pedidos");

            migrationBuilder.AddColumn<int>(
                name: "Origen",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Pedidos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Clientes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localidad",
                table: "Clientes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Categorias",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Productos_Origen",
                table: "Productos",
                sql: "[Origen] IN (0, 1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pedidos_Cliente",
                table: "Pedidos",
                sql: "[Estado] = 0 OR [ClienteId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pedidos_Estado",
                table: "Pedidos",
                sql: "[Estado] IN (0, 1, 2, 3)");
        }
    }
}
