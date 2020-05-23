using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dictionary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Word",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(maxLength: 200, nullable: false),
                    Spelling = table.Column<string>(maxLength: 200, nullable: true),
                    SpellingAudioUrl = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WordClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordClass", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Phase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(maxLength: 2000, nullable: false),
                    DictionaryId = table.Column<int>(nullable: false),
                    WordId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phase_Dictionary_DictionaryId",
                        column: x => x.DictionaryId,
                        principalTable: "Dictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Phase_Word_WordId",
                        column: x => x.WordId,
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordForm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormType = table.Column<string>(nullable: true),
                    Content = table.Column<string>(maxLength: 200, nullable: false),
                    WordId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordForm_Word_WordId",
                        column: x => x.WordId,
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WordRelativeWord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsSynomym = table.Column<bool>(nullable: false),
                    WordClass = table.Column<string>(nullable: true),
                    RelWord = table.Column<string>(nullable: true),
                    WordId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordRelativeWord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordRelativeWord_Word_WordId",
                        column: x => x.WordId,
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Definition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(maxLength: 1000, nullable: false),
                    DictionaryId = table.Column<int>(nullable: false),
                    WordClassId = table.Column<int>(nullable: false),
                    WordId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Definition_Dictionary_DictionaryId",
                        column: x => x.DictionaryId,
                        principalTable: "Dictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Definition_WordClass_WordClassId",
                        column: x => x.WordClassId,
                        principalTable: "WordClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Definition_Word_WordId",
                        column: x => x.WordId,
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhaseDefinition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(nullable: true),
                    PhaseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhaseDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhaseDefinition_Phase_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "Phase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sample = table.Column<string>(maxLength: 2000, nullable: false),
                    Translation = table.Column<string>(maxLength: 2000, nullable: true),
                    DefinitionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usage_Definition_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "Definition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhaseUsage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sample = table.Column<string>(nullable: true),
                    Translation = table.Column<string>(nullable: true),
                    PhaseDefinitionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhaseUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhaseUsage_PhaseDefinition_PhaseDefinitionId",
                        column: x => x.PhaseDefinitionId,
                        principalTable: "PhaseDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Definition_DictionaryId",
                table: "Definition",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Definition_WordClassId",
                table: "Definition",
                column: "WordClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Definition_WordId",
                table: "Definition",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Phase_DictionaryId",
                table: "Phase",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Phase_WordId",
                table: "Phase",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_PhaseDefinition_PhaseId",
                table: "PhaseDefinition",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PhaseUsage_PhaseDefinitionId",
                table: "PhaseUsage",
                column: "PhaseDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Usage_DefinitionId",
                table: "Usage",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Word_Content",
                table: "Word",
                column: "Content",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WordForm_WordId",
                table: "WordForm",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordRelativeWord_WordId",
                table: "WordRelativeWord",
                column: "WordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhaseUsage");

            migrationBuilder.DropTable(
                name: "Usage");

            migrationBuilder.DropTable(
                name: "WordForm");

            migrationBuilder.DropTable(
                name: "WordRelativeWord");

            migrationBuilder.DropTable(
                name: "PhaseDefinition");

            migrationBuilder.DropTable(
                name: "Definition");

            migrationBuilder.DropTable(
                name: "Phase");

            migrationBuilder.DropTable(
                name: "WordClass");

            migrationBuilder.DropTable(
                name: "Dictionary");

            migrationBuilder.DropTable(
                name: "Word");
        }
    }
}
