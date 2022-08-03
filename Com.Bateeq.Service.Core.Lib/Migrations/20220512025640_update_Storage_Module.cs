using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.Migrations
{
    public partial class update_Storage_Module : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StorageId",
                table: "ModuleSources",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StorageId",
                table: "ModuleDestinations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleSources_StorageId",
                table: "ModuleSources",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleDestinations_StorageId",
                table: "ModuleDestinations",
                column: "StorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleDestinations_Storages_StorageId",
                table: "ModuleDestinations",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleSources_Storages_StorageId",
                table: "ModuleSources",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleDestinations_Storages_StorageId",
                table: "ModuleDestinations");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleSources_Storages_StorageId",
                table: "ModuleSources");

            migrationBuilder.DropIndex(
                name: "IX_ModuleSources_StorageId",
                table: "ModuleSources");

            migrationBuilder.DropIndex(
                name: "IX_ModuleDestinations_StorageId",
                table: "ModuleDestinations");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "ModuleSources");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "ModuleDestinations");
        }
    }
}
