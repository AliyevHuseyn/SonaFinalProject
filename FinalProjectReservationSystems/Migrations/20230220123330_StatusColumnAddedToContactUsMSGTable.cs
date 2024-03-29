﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectReservationSystems.Migrations
{
    public partial class StatusColumnAddedToContactUsMSGTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "ContactUsMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ContactUsMessages");
        }
    }
}
