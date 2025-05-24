using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaMottuAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CAMERAS",
                columns: table => new
                {
                    ID_CAMERA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    LOCALIZACAO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false, defaultValueSql: "'ativo'"),
                    ULTIMA_VERIFICACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAMERAS", x => x.ID_CAMERA);
                });

            migrationBuilder.CreateTable(
                name: "CARGOS",
                columns: table => new
                {
                    ID_CARGO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NIVEL_PERMISSAO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PERMISSOES = table.Column<string>(type: "CLOB", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CARGOS", x => x.ID_CARGO);
                    table.CheckConstraint("CK_CARGO_NIVEL", "NIVEL_PERMISSAO BETWEEN 1 AND 5");
                });

            migrationBuilder.CreateTable(
                name: "MOTOS",
                columns: table => new
                {
                    ID_MOTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PLACA = table.Column<string>(type: "NVARCHAR2(7)", maxLength: 7, nullable: false),
                    MARCA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    MODELO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    COR = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    PRESENTE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, defaultValueSql: "'Não'"),
                    IMAGEM_REFERENCIA = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOTOS", x => x.ID_MOTO);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    SENHA = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false),
                    ID_CARGO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ATIVO = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, defaultValueSql: "'Sim'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.ID_USUARIO);
                    table.ForeignKey(
                        name: "FK_USUARIO_CARGO",
                        column: x => x.ID_CARGO,
                        principalTable: "CARGOS",
                        principalColumn: "ID_CARGO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RECONHECIMENTOS",
                columns: table => new
                {
                    ID_RECONHECIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_MOTO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_CAMERA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    PRECISAO = table.Column<decimal>(type: "DECIMAL(5,4)", precision: 5, scale: 4, nullable: false),
                    IMAGEM_CAPTURADA = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    CONFIANCA_MINIMA = table.Column<decimal>(type: "DECIMAL(5,4)", precision: 5, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RECONHECIMENTOS", x => x.ID_RECONHECIMENTO);
                    table.ForeignKey(
                        name: "FK_REC_CAMERA",
                        column: x => x.ID_CAMERA,
                        principalTable: "CAMERAS",
                        principalColumn: "ID_CAMERA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_REC_MOTO",
                        column: x => x.ID_MOTO,
                        principalTable: "MOTOS",
                        principalColumn: "ID_MOTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LOG_ALTERACOES",
                columns: table => new
                {
                    ID_LOG = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_MOTO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TIPO_ACAO = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    CAMPO_ALTERADO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    VALOR_ANTIGO = table.Column<string>(type: "CLOB", nullable: true),
                    VALOR_NOVO = table.Column<string>(type: "CLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOG_ALTERACOES", x => x.ID_LOG);
                    table.ForeignKey(
                        name: "FK_LOG_MOTO",
                        column: x => x.ID_MOTO,
                        principalTable: "MOTOS",
                        principalColumn: "ID_MOTO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LOG_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REGISTROS",
                columns: table => new
                {
                    ID_REGISTRO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_MOTO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_RECONHECIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TIPO = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    MODO_REGISTRO = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REGISTROS", x => x.ID_REGISTRO);
                    table.ForeignKey(
                        name: "FK_REG_MOTO",
                        column: x => x.ID_MOTO,
                        principalTable: "MOTOS",
                        principalColumn: "ID_MOTO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_REG_RECONHECIMENTO",
                        column: x => x.ID_RECONHECIMENTO,
                        principalTable: "RECONHECIMENTOS",
                        principalColumn: "ID_RECONHECIMENTO",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_REG_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CARGOS_NOME",
                table: "CARGOS",
                column: "NOME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_LOG_ACAO",
                table: "LOG_ALTERACOES",
                column: "TIPO_ACAO");

            migrationBuilder.CreateIndex(
                name: "IX_LOG_ALTERACOES_ID_MOTO",
                table: "LOG_ALTERACOES",
                column: "ID_MOTO");

            migrationBuilder.CreateIndex(
                name: "IX_LOG_ALTERACOES_ID_USUARIO",
                table: "LOG_ALTERACOES",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_MOTOS_PLACA",
                table: "MOTOS",
                column: "PLACA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_RECONHECIMENTO_DATA",
                table: "RECONHECIMENTOS",
                column: "DATA_HORA");

            migrationBuilder.CreateIndex(
                name: "IX_RECONHECIMENTOS_ID_CAMERA",
                table: "RECONHECIMENTOS",
                column: "ID_CAMERA");

            migrationBuilder.CreateIndex(
                name: "IX_RECONHECIMENTOS_ID_MOTO",
                table: "RECONHECIMENTOS",
                column: "ID_MOTO");

            migrationBuilder.CreateIndex(
                name: "IDX_REGISTRO_TIPO",
                table: "REGISTROS",
                column: "TIPO");

            migrationBuilder.CreateIndex(
                name: "IX_REGISTROS_ID_MOTO",
                table: "REGISTROS",
                column: "ID_MOTO");

            migrationBuilder.CreateIndex(
                name: "IX_REGISTROS_ID_RECONHECIMENTO",
                table: "REGISTROS",
                column: "ID_RECONHECIMENTO");

            migrationBuilder.CreateIndex(
                name: "IX_REGISTROS_ID_USUARIO",
                table: "REGISTROS",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_EMAIL",
                table: "USUARIOS",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_ID_CARGO",
                table: "USUARIOS",
                column: "ID_CARGO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOG_ALTERACOES");

            migrationBuilder.DropTable(
                name: "REGISTROS");

            migrationBuilder.DropTable(
                name: "RECONHECIMENTOS");

            migrationBuilder.DropTable(
                name: "USUARIOS");

            migrationBuilder.DropTable(
                name: "CAMERAS");

            migrationBuilder.DropTable(
                name: "MOTOS");

            migrationBuilder.DropTable(
                name: "CARGOS");
        }
    }
}
