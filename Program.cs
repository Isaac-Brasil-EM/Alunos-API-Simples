using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AlunoDb>(opt => opt.UseInMemoryDatabase("AlunoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Bem Vindo a API de alunos!");

app.MapGet("/alunos", async (AlunoDb db) =>
    await db.Alunos.ToListAsync());

app.MapGet("/alunos/{id}", async (int id, AlunoDb db) =>
    await db.Alunos.FindAsync(id)
        is Aluno aluno
            ? Results.Ok(aluno)
            : Results.NotFound());

app.MapPost("/alunos", async (Aluno aluno, AlunoDb db) =>
{
    db.Alunos.Add(aluno);
    await db.SaveChangesAsync();

    return Results.Created($"/alunos/{aluno.Matricula}", aluno);
});

app.MapPut("/alunos/{id}", async (int id, Aluno inputAluno, AlunoDb db) =>
{
    var aluno = await db.Alunos.FindAsync(id);

    if (aluno is null) return Results.NotFound();

    aluno.Nome = inputAluno.Nome;
    aluno.Matricula = inputAluno.Matricula;
    aluno.Cpf = inputAluno.Cpf;
    aluno.Nascimento = inputAluno.Nascimento;
    aluno.Sexo= inputAluno.Sexo;


    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/alunos/{id}", async (int id, AlunoDb db) =>
{
    if (await db.Alunos.FindAsync(id) is Aluno aluno)
    {
        db.Alunos.Remove(aluno);
        await db.SaveChangesAsync();
        return Results.Ok(aluno);
    }

    return Results.NotFound();
});

app.Run();
public enum EnumeradorSexo
{
    Masculino = 0,
    Feminino = 1
}
class Aluno
{
    public string Matricula { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public DateTime Nascimento { get; set; }
    public EnumeradorSexo Sexo { get; set; }




}

class AlunoDb : DbContext
{
    public AlunoDb(DbContextOptions<AlunoDb> options)
        : base(options) { }

    public DbSet<Aluno> Alunos => Set<Aluno>();
}