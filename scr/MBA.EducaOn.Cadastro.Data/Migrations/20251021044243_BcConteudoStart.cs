using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace MBA.EducaOn.GestaoConteudo.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage(Justification = "EF Migrations não será testada neste momento")]
    public partial class BcConteudoStart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", unicode: false, maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", unicode: false, maxLength: 1000, nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    CargaHoraria = table.Column<int>(type: "INTEGER", nullable: false),
                    PublicoAlvo = table.Column<string>(type: "TEXT", unicode: false, maxLength: 300, nullable: false),
                    Objetivo = table.Column<string>(type: "TEXT", unicode: false, maxLength: 500, nullable: false),
                    Requisitos = table.Column<string>(type: "TEXT", unicode: false, maxLength: 500, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConteudoDescricao = table.Column<string>(type: "TEXT", unicode: false, maxLength: 1000, nullable: false),
                    Revisao = table.Column<int>(type: "INTEGER", nullable: false),
                    ConteudoProgramatico_DataRevisao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CURSO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aulas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", unicode: false, maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", unicode: false, maxLength: 500, nullable: false),
                    Ordem = table.Column<int>(type: "INTEGER", nullable: false),
                    CursoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AULA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aulas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Materiais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", unicode: false, maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", unicode: false, maxLength: 500, nullable: false),
                    UrlArquivo = table.Column<string>(type: "TEXT", unicode: false, maxLength: 1000, nullable: false),
                    UrlLinkSiteReferencia = table.Column<string>(type: "TEXT", unicode: false, maxLength: 1000, nullable: false),
                    AulaId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATERIAL", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MATERIAI_AULAS",
                        column: x => x.AulaId,
                        principalTable: "Aulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aulas_CursoId",
                table: "Aulas",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiais_AulaId",
                table: "Materiais",
                column: "AulaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Materiais");

            migrationBuilder.DropTable(
                name: "Aulas");

            migrationBuilder.DropTable(
                name: "Cursos");
        }
    }
}
