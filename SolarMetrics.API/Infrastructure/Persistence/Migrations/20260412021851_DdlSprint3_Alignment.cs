using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarMetrics.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DdlSprint3_Alignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SM_AUDITORIA",
                columns: table => new
                {
                    ID_AUDITORIA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME_TABELA = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    OPERACAO = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    USUARIO_ORACLE = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    DATA_OPERACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DADOS_OLD = table.Column<string>(type: "CLOB", nullable: true),
                    DADOS_NEW = table.Column<string>(type: "CLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SM_AUDITORIA", x => x.ID_AUDITORIA);
                });

            migrationBuilder.CreateTable(
                name: "SM_LOGIN",
                columns: table => new
                {
                    USERNAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    PASSWORD = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SM_LOGIN", x => x.USERNAME);
                });

            migrationBuilder.CreateTable(
                name: "SM_SENSOR_LOGIN",
                columns: table => new
                {
                    USERNAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    PASSWORD = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    IS_SUPERUSER = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SM_SENSOR_LOGIN", x => x.USERNAME);
                });

            migrationBuilder.CreateTable(
                name: "SM_LOGIN_ROLES",
                columns: table => new
                {
                    SM_LOGIN_USERNAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    ROLES = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SM_LOGIN_ROLES", x => new { x.SM_LOGIN_USERNAME, x.ROLES });
                    table.ForeignKey(
                        name: "FK_SM_LOGIN_ROLES_SM_LOGIN_SM_LOGIN_USERNAME",
                        column: x => x.SM_LOGIN_USERNAME,
                        principalTable: "SM_LOGIN",
                        principalColumn: "USERNAME",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SM_USUARIO",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    TIPO_USER = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    USUARIO_USERNAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SM_USUARIO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SM_USUARIO_SM_LOGIN_USUARIO_USERNAME",
                        column: x => x.USUARIO_USERNAME,
                        principalTable: "SM_LOGIN",
                        principalColumn: "USERNAME");
                });

            migrationBuilder.CreateTable(
                name: "SM_SISTEMA",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    NOME_INSTALACAO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    DATA_INSTALACAO = table.Column<DateTime>(type: "DATE", nullable: false),
                    POTENCIA_TOTAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CLIENTE_ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SM_SISTEMA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SM_SISTEMA_SM_USUARIO_CLIENTE_ID",
                        column: x => x.CLIENTE_ID,
                        principalTable: "SM_USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SM_PAINEL_SOLAR",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    MODELO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    FABRICANTE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    POTENCIA_MAXIMA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATA_FABRICACAO = table.Column<DateTime>(type: "DATE", nullable: false),
                    EFICIENCIA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SISTEMA_ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SM_PAINEL_SOLAR", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SM_PAINEL_SOLAR_SM_SISTEMA_SISTEMA_ID",
                        column: x => x.SISTEMA_ID,
                        principalTable: "SM_SISTEMA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SM_SENSOR",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    TIPO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    LOCALIZACAO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    SISTEMA_ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    SENSOR_LOGIN_USERNAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SM_SENSOR", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SM_SENSOR_SM_SENSOR_LOGIN_SENSOR_LOGIN_USERNAME",
                        column: x => x.SENSOR_LOGIN_USERNAME,
                        principalTable: "SM_SENSOR_LOGIN",
                        principalColumn: "USERNAME");
                    table.ForeignKey(
                        name: "FK_SM_SENSOR_SM_SISTEMA_SISTEMA_ID",
                        column: x => x.SISTEMA_ID,
                        principalTable: "SM_SISTEMA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SM_MONITORAMENTO",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    PERIODO = table.Column<DateTime>(type: "DATE", nullable: false),
                    VALOR_LIDO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MEDIA_LEITURA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MAXIMA_LEITURA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SENSOR_ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SM_MONITORAMENTO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SM_MONITORAMENTO_SM_SENSOR_SENSOR_ID",
                        column: x => x.SENSOR_ID,
                        principalTable: "SM_SENSOR",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SM_MONITORAMENTO_SENSOR_ID",
                table: "SM_MONITORAMENTO",
                column: "SENSOR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SM_PAINEL_SOLAR_SISTEMA_ID",
                table: "SM_PAINEL_SOLAR",
                column: "SISTEMA_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SM_SENSOR_SENSOR_LOGIN_USERNAME",
                table: "SM_SENSOR",
                column: "SENSOR_LOGIN_USERNAME",
                unique: true,
                filter: "\"SENSOR_LOGIN_USERNAME\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SM_SENSOR_SISTEMA_ID",
                table: "SM_SENSOR",
                column: "SISTEMA_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SM_SISTEMA_CLIENTE_ID",
                table: "SM_SISTEMA",
                column: "CLIENTE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SM_USUARIO_EMAIL",
                table: "SM_USUARIO",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SM_USUARIO_USUARIO_USERNAME",
                table: "SM_USUARIO",
                column: "USUARIO_USERNAME",
                unique: true,
                filter: "\"USUARIO_USERNAME\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SM_AUDITORIA");

            migrationBuilder.DropTable(
                name: "SM_LOGIN_ROLES");

            migrationBuilder.DropTable(
                name: "SM_MONITORAMENTO");

            migrationBuilder.DropTable(
                name: "SM_PAINEL_SOLAR");

            migrationBuilder.DropTable(
                name: "SM_SENSOR");

            migrationBuilder.DropTable(
                name: "SM_SENSOR_LOGIN");

            migrationBuilder.DropTable(
                name: "SM_SISTEMA");

            migrationBuilder.DropTable(
                name: "SM_USUARIO");

            migrationBuilder.DropTable(
                name: "SM_LOGIN");
        }
    }
}
