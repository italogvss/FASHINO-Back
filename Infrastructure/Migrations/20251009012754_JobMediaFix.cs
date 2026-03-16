using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class JobMediaFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobMedia_JobExecution_JobExecutionId",
                table: "JobMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_JobMedia_Medias_MediaId",
                table: "JobMedia");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobExecutionId",
                table: "JobMedia",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobMedia_JobExecution_MediaId",
                table: "JobMedia",
                column: "MediaId",
                principalTable: "JobExecution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobMedia_Medias_JobExecutionId",
                table: "JobMedia",
                column: "JobExecutionId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobMedia_JobExecution_MediaId",
                table: "JobMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_JobMedia_Medias_JobExecutionId",
                table: "JobMedia");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobExecutionId",
                table: "JobMedia",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_JobMedia_JobExecution_JobExecutionId",
                table: "JobMedia",
                column: "JobExecutionId",
                principalTable: "JobExecution",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobMedia_Medias_MediaId",
                table: "JobMedia",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
