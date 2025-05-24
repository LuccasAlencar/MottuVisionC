using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SistemaMottuAPI.Data; // Assuming original namespace
using SistemaMottuAPI.Model; // Assuming original namespace
using SistemaMottuAPI.DTO;  // Assuming original namespace
using System.ComponentModel.DataAnnotations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco Oracle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema de Motos API (FIAP)",
        Version = "v1",
        Description = "API para gerenciamento de sistema de controle de motos com reconhecimento por câmeras (FIAP Advanced Business Development with .NET)",

    });
    
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 2. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema Motos API v1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection(); 
app.UseCors("AllowAll");

// Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Attempting to delete database (if exists)...");
        await context.Database.EnsureDeletedAsync();
        logger.LogInformation("Database deleted (if existed).");

        logger.LogInformation("Attempting to apply migrations...");
        await context.Database.MigrateAsync(); 
        logger.LogInformation("Database schema created/updated via migrations.");

        logger.LogInformation("Attempting to seed data...");
        await SeedData(context); 
        logger.LogInformation("Data seeded.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialization.");
    }
}

static async Task SeedData(AppDbContext context)
{

    if (await context.Cargos.AnyAsync() &&
        await context.Usuarios.AnyAsync() &&
        await context.Motos.AnyAsync() &&
        await context.Cameras.AnyAsync() &&
        await context.Reconhecimentos.AnyAsync() &&
        await context.Registros.AnyAsync() &&
        await context.LogsAlteracoes.AnyAsync())
    {
        Console.WriteLine("Database already contains seed data. Skipping seeding.");
        return;
    }

    // Cargos
    if (!await context.Cargos.AnyAsync())
    {
        var cargos = new List<Cargo>
        {
            new() { Nome = "ADMIN", NivelPermissao = 5, Permissoes = "[\"create\",\"read\",\"update\",\"delete\"]", DataCadastro = DateTime.UtcNow },
            new() { Nome = "OPERADOR", NivelPermissao = 3, Permissoes = "[\"read\",\"update\"]", DataCadastro = DateTime.UtcNow },
            new() { Nome = "MANUTENCAO", NivelPermissao = 2, Permissoes = "[\"read\",\"update_status\"]", DataCadastro = DateTime.UtcNow },
            new() { Nome = "VISITANTE", NivelPermissao = 1, Permissoes = "[\"read\"]", DataCadastro = DateTime.UtcNow },
            new() { Nome = "AUDITOR", NivelPermissao = 4, Permissoes = "[\"read\",\"create_log\",\"read_log\"]", DataCadastro = DateTime.UtcNow }
        };
        context.Cargos.AddRange(cargos);
        await context.SaveChangesAsync(); 
    }


    // Usuários
    if (!await context.Usuarios.AnyAsync() && await context.Cargos.AnyAsync())
    {
        var adminCargo = await context.Cargos.FirstOrDefaultAsync(c => c.Nome == "ADMIN");
        var operadorCargo = await context.Cargos.FirstOrDefaultAsync(c => c.Nome == "OPERADOR");
        var manutencaoCargo = await context.Cargos.FirstOrDefaultAsync(c => c.Nome == "MANUTENCAO");
        var visitanteCargo = await context.Cargos.FirstOrDefaultAsync(c => c.Nome == "VISITANTE");
        var auditorCargo = await context.Cargos.FirstOrDefaultAsync(c => c.Nome == "AUDITOR");

        if (adminCargo != null && operadorCargo != null && manutencaoCargo != null && visitanteCargo != null && auditorCargo != null)
        {
            var usuarios = new List<Usuario>
            {
                new() { Nome = "João Silva", Email = "joao@ex.com", Senha = "$2y$12$hash1...", IdCargo = adminCargo.IdCargo, Ativo = "Sim" },
                new() { Nome = "Maria Souza", Email = "maria@ex.com", Senha = "$2y$12$hash2...", IdCargo = operadorCargo.IdCargo, Ativo = "Sim" },
                new() { Nome = "Paulo Costa", Email = "paulo@ex.com", Senha = "$2y$12$hash3...", IdCargo = manutencaoCargo.IdCargo, Ativo = "Sim" },
                new() { Nome = "Ana Pereira", Email = "ana@ex.com", Senha = "$2y$12$hash4...", IdCargo = visitanteCargo.IdCargo, Ativo = "Sim" },
                new() { Nome = "Carlos Lima", Email = "carlos@ex.com", Senha = "$2y$12$hash5...", IdCargo = auditorCargo.IdCargo, Ativo = "Sim" }
            };
            context.Usuarios.AddRange(usuarios);
            await context.SaveChangesAsync();
        }
    }

    // Motos
    if (!await context.Motos.AnyAsync())
    {
        var motos = new List<Moto>
        {
            new() { Placa = "ABC1234", Marca = "Honda", Modelo = "CB 500", Cor = "Preta", Presente = "Sim", ImagemReferencia = "ref1.jpg" },
            new() { Placa = "DEF5678", Marca = "Yamaha", Modelo = "YBR 125", Cor = "Vermelha", Presente = "Não", ImagemReferencia = "ref2.jpg"},
            new() { Placa = "GHI9012", Marca = "Suzuki", Modelo = "GSR 750", Cor = "Azul", Presente = "Sim", ImagemReferencia = "ref3.jpg"},
            new() { Placa = "JKL3456", Marca = "Kawasaki", Modelo = "Ninja 400", Cor = "Verde", Presente = "Sim", ImagemReferencia = "ref4.jpg"},
            new() { Placa = "MNO7890", Marca = "Ducati", Modelo = "Monster", Cor = "Branca", Presente = "Não", ImagemReferencia = "ref5.jpg" }
        };
        context.Motos.AddRange(motos);
        await context.SaveChangesAsync();
    }

    // Câmeras
    if (!await context.Cameras.AnyAsync())
    {
        var cameras = new List<Camera>
        {
            new() { Localizacao = "Portão de Entrada", Status = "ativo", UltimaVerificacao = DateTime.UtcNow },
            new() { Localizacao = "Área Interna 1", Status = "manutencao", UltimaVerificacao = DateTime.UtcNow },
            new() { Localizacao = "Garagem Norte", Status = "ativo", UltimaVerificacao = DateTime.UtcNow },
            new() { Localizacao = "Garagem Sul", Status = "inativo", UltimaVerificacao = DateTime.UtcNow },
            new() { Localizacao = "Saída", Status = "ativo", UltimaVerificacao = DateTime.UtcNow }
        };
        context.Cameras.AddRange(cameras);
        await context.SaveChangesAsync();
    }


    // Reconhecimentos 
    if (!await context.Reconhecimentos.AnyAsync() && await context.Motos.AnyAsync() && await context.Cameras.AnyAsync())
    {
        var moto1 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "ABC1234");
        var moto2 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "DEF5678");
        var moto3 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "GHI9012");
        var moto4 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "JKL3456");
        var moto5 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "MNO7890");

        var camera1 = await context.Cameras.FirstOrDefaultAsync(cam => cam.Localizacao == "Portão de Entrada");
        var camera2 = await context.Cameras.FirstOrDefaultAsync(cam => cam.Localizacao == "Área Interna 1");
        var camera3 = await context.Cameras.FirstOrDefaultAsync(cam => cam.Localizacao == "Garagem Norte");
        var camera5 = await context.Cameras.FirstOrDefaultAsync(cam => cam.Localizacao == "Saída");

        if (moto1 != null && moto2 != null && moto3 != null && moto4 != null && moto5 != null &&
            camera1 != null && camera2 != null && camera3 != null && camera5 != null)
        {
            var reconhecimentos = new List<Reconhecimento>
            {
                new() { IdMoto = moto1.IdMoto, IdCamera = camera1.IdCamera, Precisao = 0.9523m, ImagemCapturada = "rec1.jpg", ConfiancaMinima = 0.8000m, DataHora = DateTime.UtcNow.AddHours(-1) },
                new() { IdMoto = moto2.IdMoto, IdCamera = camera3.IdCamera, Precisao = 0.8734m, ImagemCapturada = "rec2.jpg", ConfiancaMinima = 0.8500m, DataHora = DateTime.UtcNow.AddHours(-2) },
                new() { IdMoto = moto3.IdMoto, IdCamera = camera1.IdCamera, Precisao = 0.9101m, ImagemCapturada = "rec3.jpg", ConfiancaMinima = 0.9000m, DataHora = DateTime.UtcNow.AddHours(-3) },
                new() { IdMoto = moto4.IdMoto, IdCamera = camera5.IdCamera, Precisao = 0.7822m, ImagemCapturada = "rec4.jpg", ConfiancaMinima = 0.7500m, DataHora = DateTime.UtcNow.AddHours(-4) },
                new() { IdMoto = moto5.IdMoto, IdCamera = camera2.IdCamera, Precisao = 0.9955m, ImagemCapturada = "rec5.jpg", ConfiancaMinima = 0.9500m, DataHora = DateTime.UtcNow.AddHours(-5) }
            };
            context.Reconhecimentos.AddRange(reconhecimentos);
            await context.SaveChangesAsync();
        }
    }

    // Registros
    if (!await context.Registros.AnyAsync() &&
        await context.Motos.AnyAsync() &&
        await context.Usuarios.AnyAsync() &&
        await context.Reconhecimentos.AnyAsync()) 
    {
        var moto1 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "ABC1234");
        var moto2 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "DEF5678");
        var moto3 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "GHI9012");
        var moto4 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "JKL3456");

        var usuario1 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "joao@ex.com");
        var usuario2 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "maria@ex.com");
        var usuario3 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "paulo@ex.com");
        var usuario5 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "carlos@ex.com");

        var rec1 = await context.Reconhecimentos.FirstOrDefaultAsync(r => r.IdMoto == moto1!.IdMoto);
        var rec2 = await context.Reconhecimentos.FirstOrDefaultAsync(r => r.IdMoto == moto2!.IdMoto);
        var rec3 = await context.Reconhecimentos.FirstOrDefaultAsync(r => r.IdMoto == moto3!.IdMoto);
        var rec4 = await context.Reconhecimentos.FirstOrDefaultAsync(r => r.IdMoto == moto4!.IdMoto);


        if(moto1 != null && moto2 != null && moto3 != null && moto4 != null &&
           usuario1 != null && usuario2 != null && usuario3 != null && usuario5 != null)
        {
            var registros = new List<Registro>
            {
                new() { IdMoto = moto1.IdMoto, IdUsuario = usuario2.IdUsuario, IdReconhecimento = rec1?.IdReconhecimento, Tipo = "entrada", ModoRegistro = "automatico", DataHora = DateTime.UtcNow.AddMinutes(-10) },
                new() { IdMoto = moto1.IdMoto, IdUsuario = usuario3.IdUsuario, IdReconhecimento = null, Tipo = "saida", ModoRegistro = "manual", DataHora = DateTime.UtcNow.AddMinutes(-5) },
                new() { IdMoto = moto2.IdMoto, IdUsuario = usuario2.IdUsuario, IdReconhecimento = rec2?.IdReconhecimento, Tipo = "entrada", ModoRegistro = "automatico", DataHora = DateTime.UtcNow.AddMinutes(-15) },
                new() { IdMoto = moto3.IdMoto, IdUsuario = usuario5.IdUsuario, IdReconhecimento = rec3?.IdReconhecimento, Tipo = "entrada", ModoRegistro = "automatico", DataHora = DateTime.UtcNow.AddMinutes(-20) },
                new() { IdMoto = moto4.IdMoto, IdUsuario = usuario1.IdUsuario, IdReconhecimento = rec4?.IdReconhecimento, Tipo = "saida", ModoRegistro = "automatico", DataHora = DateTime.UtcNow.AddMinutes(-25) }
            };
            context.Registros.AddRange(registros);
            await context.SaveChangesAsync();
        }
    }

    // Logs
    if (!await context.LogsAlteracoes.AnyAsync() && await context.Motos.AnyAsync() && await context.Usuarios.AnyAsync())
    {
        var moto1 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "ABC1234");
        var moto2 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "DEF5678");
        var moto3 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "GHI9012");
        var moto4 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "JKL3456");
        var moto5 = await context.Motos.FirstOrDefaultAsync(m => m.Placa == "MNO7890");

        var usuario1 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "joao@ex.com");
        var usuario2 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "maria@ex.com");
        var usuario3 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "paulo@ex.com");
        var usuario5_log = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "carlos@ex.com");


        if (moto1 != null && moto2 != null && moto3 != null && moto4 != null && moto5 != null &&
            usuario1 != null && usuario2 != null && usuario3 != null && usuario5_log != null)
        {
             var logs = new List<LogAlteracao>
            {
                new() { IdUsuario = usuario1.IdUsuario, IdMoto = moto1.IdMoto, TipoAcao = "edicao", CampoAlterado = "cor", ValorAntigo = "Preta", ValorNovo = "Azul", DataHora = DateTime.UtcNow.AddSeconds(-30) },
                new() { IdUsuario = usuario2.IdUsuario, IdMoto = moto2.IdMoto, TipoAcao = "edicao", CampoAlterado = "modelo", ValorAntigo = "YBR 125", ValorNovo = "YBR 150", DataHora = DateTime.UtcNow.AddSeconds(-35) },
                new() { IdUsuario = usuario3.IdUsuario, IdMoto = moto3.IdMoto, TipoAcao = "edicao", CampoAlterado = "imagem_referencia", ValorAntigo = "ref3.jpg", ValorNovo = "ref3_updated.jpg", DataHora = DateTime.UtcNow.AddSeconds(-40) },
                new() { IdUsuario = usuario5_log.IdUsuario, IdMoto = moto4.IdMoto, TipoAcao = "insercao", CampoAlterado = "presente", ValorAntigo = null, ValorNovo = "Sim", DataHora = DateTime.UtcNow.AddSeconds(-45) },
                new() { IdUsuario = usuario1.IdUsuario, IdMoto = moto5.IdMoto, TipoAcao = "exclusao", CampoAlterado = "imagem_referencia", ValorAntigo = "ref5.jpg", ValorNovo = null, DataHora = DateTime.UtcNow.AddSeconds(-50) }
            };
            context.LogsAlteracoes.AddRange(logs);
            await context.SaveChangesAsync();
        }
    }
    Console.WriteLine("Finished seeding data.");
}


// ENDPOINTS 
// CRUD CARGOS
var cargosGroup = app.MapGroup("/api/cargos").WithTags("Cargos");

cargosGroup.MapGet("/", async (AppDbContext db, int? skip, int? take, string? nome, int? nivelPermissao) =>
{
    var query = db.Cargos.AsQueryable();
    if (!string.IsNullOrEmpty(nome)) query = query.Where(c => c.Nome.ToLower().Contains(nome.ToLower()));
    if (nivelPermissao.HasValue) query = query.Where(c => c.NivelPermissao == nivelPermissao);
    if (skip.HasValue) query = query.Skip(skip.Value);
    if (take.HasValue) query = query.Take(take.Value);
    var cargos = await query.OrderBy(c => c.IdCargo).ToListAsync();
    return Results.Ok(cargos);
})
.WithName("GetCargos")
.WithSummary("Buscar cargos com filtros opcionais");

cargosGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
{
    var cargo = await db.Cargos.FindAsync(id);
    return cargo is not null ? Results.Ok(cargo) : Results.NotFound($"Cargo com ID {id} não encontrado.");
})
.WithName("GetCargoById")
.WithSummary("Buscar cargo por ID");

cargosGroup.MapGet("/nivel/{nivel:int}", async (int nivel, AppDbContext db) =>
{
    var cargos = await db.Cargos.Where(c => c.NivelPermissao == nivel).ToListAsync();
    return Results.Ok(cargos);
})
.WithName("GetCargosByNivel")
.WithSummary("Buscar cargos por nível de permissão");

cargosGroup.MapGet("/search/{termo}", async (string termo, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(termo)) return Results.BadRequest("Termo de busca não pode ser vazio.");
    var cargos = await db.Cargos.Where(c => c.Nome.ToLower().Contains(termo.ToLower())).ToListAsync();
    return Results.Ok(cargos);
})
.WithName("SearchCargos")
.WithSummary("Buscar cargos por termo no nome");

cargosGroup.MapPost("/", async (CargoDto cargoDto, AppDbContext db) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(cargoDto, new ValidationContext(cargoDto), validationResults, true))
    {
        return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }

    if (await db.Cargos.AnyAsync(c => c.Nome.ToLower() == cargoDto.Nome.ToLower()))
    {
        return Results.Conflict($"Cargo com nome '{cargoDto.Nome}' já existe.");
    }

    var cargo = new Cargo
    {
        Nome = cargoDto.Nome,
        NivelPermissao = cargoDto.NivelPermissao,
        Permissoes = cargoDto.Permissoes, 
        DataCadastro = DateTime.UtcNow
    };
    db.Cargos.Add(cargo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/cargos/{cargo.IdCargo}", cargo);
})
.WithName("CreateCargo")
.WithSummary("Criar novo cargo");

cargosGroup.MapPut("/{id:int}", async (int id, CargoDto cargoDto, AppDbContext db) =>
{
    var cargo = await db.Cargos.FindAsync(id);
    if (cargo is null) return Results.NotFound($"Cargo com ID {id} não encontrado.");

    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(cargoDto, new ValidationContext(cargoDto), validationResults, true))
    {
         return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }
    
    if (cargo.Nome.ToLower() != cargoDto.Nome.ToLower() &&
        await db.Cargos.AnyAsync(c => c.IdCargo != id && c.Nome.ToLower() == cargoDto.Nome.ToLower()))
    {
        return Results.Conflict($"Outro cargo com nome '{cargoDto.Nome}' já existe.");
    }

    cargo.Nome = cargoDto.Nome;
    cargo.NivelPermissao = cargoDto.NivelPermissao;
    cargo.Permissoes = cargoDto.Permissoes;
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateCargo")
.WithSummary("Atualizar cargo existente");

cargosGroup.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var cargo = await db.Cargos.FindAsync(id);
    if (cargo is null) return Results.NotFound($"Cargo com ID {id} não encontrado.");
    
    if (await db.Usuarios.AnyAsync(u => u.IdCargo == id))
    {
        return Results.Conflict($"Cargo com ID {id} está em uso e não pode ser excluído.");
    }

    db.Cargos.Remove(cargo);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteCargo")
.WithSummary("Deletar cargo");


// --- CRUD MOTOS ---
var motosGroup = app.MapGroup("/api/motos").WithTags("Motos");

motosGroup.MapGet("/", async (AppDbContext db, int? skip, int? take, string? marca, string? modelo, string? presente) =>
{
    var query = db.Motos.AsQueryable();
    if (!string.IsNullOrEmpty(marca)) query = query.Where(m => m.Marca.ToLower().Contains(marca.ToLower()));
    if (!string.IsNullOrEmpty(modelo)) query = query.Where(m => m.Modelo.ToLower().Contains(modelo.ToLower()));
    if (!string.IsNullOrEmpty(presente)) query = query.Where(m => m.Presente.ToLower() == presente.ToLower()); 
    if (skip.HasValue) query = query.Skip(skip.Value);
    if (take.HasValue) query = query.Take(take.Value);
    var motos = await query.OrderBy(m => m.IdMoto).ToListAsync();
    return Results.Ok(motos);
})
.WithName("GetMotos")
.WithSummary("Buscar motos com filtros opcionais");

motosGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
{
    var moto = await db.Motos.FindAsync(id);
    return moto is not null ? Results.Ok(moto) : Results.NotFound($"Moto com ID {id} não encontrada.");
})
.WithName("GetMotoById")
.WithSummary("Buscar moto por ID");

motosGroup.MapGet("/placa/{placa}", async (string placa, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(placa)) return Results.BadRequest("Placa não pode ser vazia.");
    var moto = await db.Motos.FirstOrDefaultAsync(m => m.Placa.ToLower() == placa.ToLower());
    return moto is not null ? Results.Ok(moto) : Results.NotFound($"Moto com placa {placa} não encontrada.");
})
.WithName("GetMotoByPlaca")
.WithSummary("Buscar moto por placa");

motosGroup.MapGet("/presentes", async (AppDbContext db) =>
{
    var motos = await db.Motos.Where(m => m.Presente.ToLower() == "sim").ToListAsync();
    return Results.Ok(motos);
})
.WithName("GetMotosPresentes")
.WithSummary("Buscar motos presentes no local");

motosGroup.MapPost("/", async (MotoDto motoDto, AppDbContext db) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(motoDto, new ValidationContext(motoDto), validationResults, true))
    {
        return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }

    if (await db.Motos.AnyAsync(m => m.Placa.ToLower() == motoDto.Placa.ToLower()))
    {
        return Results.Conflict($"Moto com placa '{motoDto.Placa}' já existe.");
    }

    var moto = new Moto
    {
        Placa = motoDto.Placa.ToUpper(), 
        Marca = motoDto.Marca,
        Modelo = motoDto.Modelo,
        Cor = motoDto.Cor,
        Presente = string.IsNullOrEmpty(motoDto.Presente) ? "Não" : motoDto.Presente, 
        ImagemReferencia = motoDto.ImagemReferencia,
    };
    db.Motos.Add(moto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/motos/{moto.IdMoto}", moto);
})
.WithName("CreateMoto")
.WithSummary("Cadastrar nova moto");

motosGroup.MapPut("/{id:int}", async (int id, MotoDto motoDto, AppDbContext db) =>
{
    var moto = await db.Motos.FindAsync(id);
    if (moto is null) return Results.NotFound($"Moto com ID {id} não encontrada.");

    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(motoDto, new ValidationContext(motoDto), validationResults, true))
    {
        return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }

    if (moto.Placa.ToLower() != motoDto.Placa.ToLower() &&
        await db.Motos.AnyAsync(m => m.IdMoto != id && m.Placa.ToLower() == motoDto.Placa.ToLower()))
    {
        return Results.Conflict($"Outra moto com placa '{motoDto.Placa}' já existe.");
    }

    moto.Placa = motoDto.Placa.ToUpper();
    moto.Marca = motoDto.Marca;
    moto.Modelo = motoDto.Modelo;
    moto.Cor = motoDto.Cor;
    moto.Presente = string.IsNullOrEmpty(motoDto.Presente) ? moto.Presente : motoDto.Presente;
    moto.ImagemReferencia = motoDto.ImagemReferencia;
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateMoto")
.WithSummary("Atualizar moto existente");

motosGroup.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var moto = await db.Motos.FindAsync(id);
    if (moto is null) return Results.NotFound($"Moto com ID {id} não encontrada.");

    if (await db.Registros.AnyAsync(r => r.IdMoto == id) ||
        await db.Reconhecimentos.AnyAsync(rec => rec.IdMoto == id) ||
        await db.LogsAlteracoes.AnyAsync(l => l.IdMoto == id))
    {
        return Results.Conflict($"Moto com ID {id} possui registros associados e não pode ser excluída. Considere inativá-la ou remover os registros dependentes primeiro.");
    }

    db.Motos.Remove(moto);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteMoto")
.WithSummary("Deletar moto");


// CRUD USUÁRIOS 
var usuariosGroup = app.MapGroup("/api/usuarios").WithTags("Usuarios");

usuariosGroup.MapGet("/", async (AppDbContext db, int? skip, int? take, string? nome, int? cargoId, string? ativo) =>
{
    var query = db.Usuarios.Include(u => u.Cargo).AsQueryable(); // Incluir dados do Cargo
    if (!string.IsNullOrEmpty(nome)) query = query.Where(u => u.Nome.ToLower().Contains(nome.ToLower()));
    if (cargoId.HasValue) query = query.Where(u => u.IdCargo == cargoId);
    if (!string.IsNullOrEmpty(ativo)) query = query.Where(u => u.Ativo.ToLower() == ativo.ToLower());
    if (skip.HasValue) query = query.Skip(skip.Value);
    if (take.HasValue) query = query.Take(take.Value);
    var usuarios = await query.OrderBy(u => u.IdUsuario).ToListAsync();
    return Results.Ok(usuarios.Select(u => new { u.IdUsuario, u.Nome, u.Email, u.IdCargo, CargoNome = u.Cargo?.Nome, u.Ativo })); // Retornar DTO se preferir
})
.WithName("GetUsuarios")
.WithSummary("Buscar usuários com filtros opcionais");

usuariosGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
{
    var usuario = await db.Usuarios.Include(u => u.Cargo).FirstOrDefaultAsync(u => u.IdUsuario == id);
    if (usuario is null) return Results.NotFound($"Usuário com ID {id} não encontrado.");
    return Results.Ok(new { usuario.IdUsuario, usuario.Nome, usuario.Email, usuario.IdCargo, CargoNome = usuario.Cargo?.Nome, usuario.Ativo });
})
.WithName("GetUsuarioById")
.WithSummary("Buscar usuário por ID");

usuariosGroup.MapPost("/", async (UsuarioDto usuarioDto, AppDbContext db) =>
    {
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(usuarioDto, new ValidationContext(usuarioDto), validationResults, true))
        {
            return Results.ValidationProblem(validationResults.ToDictionary(
                e => e.MemberNames.FirstOrDefault() ?? string.Empty,
                e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
            ));
        }

        if (await db.Usuarios.AnyAsync(u => u.Email.ToLower() == usuarioDto.Email.ToLower()))
        {
            return Results.Conflict($"Usuário com email '{usuarioDto.Email}' já existe.");
        }
        if (!await db.Cargos.AnyAsync(c => c.IdCargo == usuarioDto.IdCargo))
        {
            return Results.BadRequest($"Cargo com ID '{usuarioDto.IdCargo}' não encontrado.");
        }

        var usuario = new Usuario
        {
            Nome = usuarioDto.Nome,
            Email = usuarioDto.Email,
            Senha = usuarioDto.Senha, 
            IdCargo = usuarioDto.IdCargo,
            Ativo = usuarioDto.Ativo ?? "Sim"
        };
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        
        var responseUsuario = new {
            usuario.IdUsuario,
            usuario.Nome,
            usuario.Email,
            usuario.IdCargo,
            CargoNome = (await db.Cargos.FindAsync(usuario.IdCargo))?.Nome, 
            usuario.Ativo
        };
        return Results.Created($"/api/usuarios/{usuario.IdUsuario}", responseUsuario);
    })
    .WithName("CreateUsuario")
    .WithSummary("Criar novo usuário");

usuariosGroup.MapPut("/{id:int}", async (int id, UsuarioDto usuarioDto, AppDbContext db) =>
    {
        var usuario = await db.Usuarios.FindAsync(id);
        if (usuario is null) return Results.NotFound($"Usuário com ID {id} não encontrado.");

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(usuarioDto, new ValidationContext(usuarioDto), validationResults, true))
        {
            return Results.ValidationProblem(validationResults.ToDictionary(
                e => e.MemberNames.FirstOrDefault() ?? string.Empty,
                e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
            ));
        }


        if (usuario.Email.ToLower() != usuarioDto.Email.ToLower() &&
            await db.Usuarios.AnyAsync(u => u.IdUsuario != id && u.Email.ToLower() == usuarioDto.Email.ToLower()))
        {
            return Results.Conflict($"Outro usuário com email '{usuarioDto.Email}' já existe.");
        }
        if (!await db.Cargos.AnyAsync(c => c.IdCargo == usuarioDto.IdCargo))
        {
            return Results.BadRequest($"Cargo com ID '{usuarioDto.IdCargo}' não encontrado.");
        }

        usuario.Nome = usuarioDto.Nome;
        usuario.Email = usuarioDto.Email;
        if (!string.IsNullOrWhiteSpace(usuarioDto.Senha) && usuario.Senha != usuarioDto.Senha)
        {
            usuario.Senha = usuarioDto.Senha;
        }
        usuario.IdCargo = usuarioDto.IdCargo;
        usuario.Ativo = usuarioDto.Ativo ?? usuario.Ativo;
        await db.SaveChangesAsync();
        return Results.NoContent();
    })
    .WithName("UpdateUsuario")
    .WithSummary("Atualizar usuário existente. Forneça o campo 'senha' para alterá-la.");

usuariosGroup.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    if (usuario is null) return Results.NotFound($"Usuário com ID {id} não encontrado.");

    if (await db.Registros.AnyAsync(r => r.IdUsuario == id) ||
        await db.LogsAlteracoes.AnyAsync(l => l.IdUsuario == id))
    {
         return Results.Conflict($"Usuário com ID {id} possui registros associados e não pode ser excluído. Considere inativá-lo ('Ativo: Não').");
    }

    db.Usuarios.Remove(usuario);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteUsuario")
.WithSummary("Deletar usuário");


// CRUD REGISTROS
var registrosGroup = app.MapGroup("/api/registros").WithTags("Registros");

registrosGroup.MapGet("/", async (AppDbContext db, int? skip, int? take, string? tipo, string? modo, int? motoId, int? usuarioId) =>
{
    var query = db.Registros
        .Include(r => r.Moto)
        .Include(r => r.Usuario)
            .ThenInclude(u => u!.Cargo) 
        .Include(r => r.Reconhecimento)
            .ThenInclude(rec => rec!.Camera) 
        .AsQueryable();

    if (!string.IsNullOrEmpty(tipo)) query = query.Where(r => r.Tipo.ToLower() == tipo.ToLower());
    if (!string.IsNullOrEmpty(modo)) query = query.Where(r => r.ModoRegistro.ToLower() == modo.ToLower());
    if (motoId.HasValue) query = query.Where(r => r.IdMoto == motoId);
    if (usuarioId.HasValue) query = query.Where(r => r.IdUsuario == usuarioId);

    if (skip.HasValue) query = query.Skip(skip.Value);
    if (take.HasValue) query = query.Take(take.Value);

    var registros = await query.OrderByDescending(r => r.DataHora).ToListAsync();
    
    var response = registros.Select(r => new
    {
        r.IdRegistro,
        r.IdMoto,
        MotoPlaca = r.Moto?.Placa,
        r.IdUsuario,
        UsuarioNome = r.Usuario?.Nome,
        r.IdReconhecimento,
        ReconhecimentoCameraLocalizacao = r.Reconhecimento?.Camera?.Localizacao,
        r.DataHora,
        r.Tipo,
        r.ModoRegistro
    });
    return Results.Ok(response);
})
.WithName("GetRegistros")
.WithSummary("Buscar registros com filtros opcionais, incluindo dados relacionados.");

registrosGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
{
    var registro = await db.Registros
        .Include(r => r.Moto)
        .Include(r => r.Usuario)
            .ThenInclude(u => u!.Cargo)
        .Include(r => r.Reconhecimento)
            .ThenInclude(rec => rec!.Camera)
        .FirstOrDefaultAsync(r => r.IdRegistro == id);

    if (registro is null) return Results.NotFound($"Registro com ID {id} não encontrado.");

    return Results.Ok(new
    {
        registro.IdRegistro,
        registro.IdMoto,
        MotoPlaca = registro.Moto?.Placa,
        registro.IdUsuario,
        UsuarioNome = registro.Usuario?.Nome,
        registro.IdReconhecimento,
        ReconhecimentoDetalhes = registro.Reconhecimento != null ? new {
            registro.Reconhecimento.IdReconhecimento,
            registro.Reconhecimento.IdCamera,
            CameraLocalizacao = registro.Reconhecimento.Camera?.Localizacao,
            registro.Reconhecimento.Precisao
        } : null,
        registro.DataHora,
        registro.Tipo,
        registro.ModoRegistro
    });
})
.WithName("GetRegistroById")
.WithSummary("Buscar registro por ID com dados relacionados.");

registrosGroup.MapPost("/", async (RegistroDto registroDto, AppDbContext db) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(registroDto, new ValidationContext(registroDto), validationResults, true))
    {
        return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }
    if (!await db.Motos.AnyAsync(m => m.IdMoto == registroDto.IdMoto))
        return Results.BadRequest($"Moto com ID {registroDto.IdMoto} não encontrada.");
    if (!await db.Usuarios.AnyAsync(u => u.IdUsuario == registroDto.IdUsuario))
        return Results.BadRequest($"Usuário com ID {registroDto.IdUsuario} não encontrado.");
    if (registroDto.IdReconhecimento.HasValue && !await db.Reconhecimentos.AnyAsync(r => r.IdReconhecimento == registroDto.IdReconhecimento.Value))
        return Results.BadRequest($"Reconhecimento com ID {registroDto.IdReconhecimento.Value} não encontrado.");


    var registro = new Registro
    {
        IdMoto = registroDto.IdMoto,
        IdUsuario = registroDto.IdUsuario,
        IdReconhecimento = registroDto.IdReconhecimento, 
        Tipo = registroDto.Tipo,
        ModoRegistro = registroDto.ModoRegistro,
        DataHora = DateTime.UtcNow
    };
    db.Registros.Add(registro);
    await db.SaveChangesAsync();
    return Results.Created($"/api/registros/{registro.IdRegistro}", registro); 
})
.WithName("CreateRegistro")
.WithSummary("Criar novo registro");

registrosGroup.MapPut("/{id:int}", async (int id, RegistroDto registroDto, AppDbContext db) =>
{
    var registro = await db.Registros.FindAsync(id);
    if (registro is null) return Results.NotFound($"Registro com ID {id} não encontrado.");

    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(registroDto, new ValidationContext(registroDto), validationResults, true))
    {
         return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }

    if (registro.IdMoto != registroDto.IdMoto && !await db.Motos.AnyAsync(m => m.IdMoto == registroDto.IdMoto))
        return Results.BadRequest($"Moto com ID {registroDto.IdMoto} não encontrada.");
    if (registro.IdUsuario != registroDto.IdUsuario && !await db.Usuarios.AnyAsync(u => u.IdUsuario == registroDto.IdUsuario))
        return Results.BadRequest($"Usuário com ID {registroDto.IdUsuario} não encontrado.");
    if (registro.IdReconhecimento != registroDto.IdReconhecimento &&
        registroDto.IdReconhecimento.HasValue &&
        !await db.Reconhecimentos.AnyAsync(r => r.IdReconhecimento == registroDto.IdReconhecimento.Value))
        return Results.BadRequest($"Reconhecimento com ID {registroDto.IdReconhecimento.Value} não encontrado.");


    registro.IdMoto = registroDto.IdMoto;
    registro.IdUsuario = registroDto.IdUsuario;
    registro.IdReconhecimento = registroDto.IdReconhecimento;
    registro.Tipo = registroDto.Tipo;
    registro.ModoRegistro = registroDto.ModoRegistro;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateRegistro")
.WithSummary("Atualizar registro existente");

registrosGroup.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var registro = await db.Registros.FindAsync(id);
    if (registro is null) return Results.NotFound($"Registro com ID {id} não encontrado.");

    db.Registros.Remove(registro);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteRegistro")
.WithSummary("Deletar registro");


// CRUD CÂMERAS 
var camerasGroup = app.MapGroup("/api/cameras").WithTags("Cameras");

camerasGroup.MapGet("/", async (AppDbContext db) =>
{
    var cameras = await db.Cameras.OrderBy(c => c.IdCamera).ToListAsync();
    var response = cameras.Select(c => new CameraResponseDto
    {
        IdCamera = c.IdCamera,
        Localizacao = c.Localizacao,
        Status = c.Status,
        UltimaVerificacao = c.UltimaVerificacao
    });
    return Results.Ok(response);
})
.WithName("GetCameras")
.WithSummary("Buscar todas as câmeras");

camerasGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
{
    var camera = await db.Cameras.FindAsync(id);
    if (camera == null)
    {
        return Results.NotFound($"Câmera com ID {id} não encontrada.");
    }
    var response = new CameraResponseDto
    {
        IdCamera = camera.IdCamera,
        Localizacao = camera.Localizacao,
        Status = camera.Status,
        UltimaVerificacao = camera.UltimaVerificacao
    };
    return Results.Ok(response);
})
.WithName("GetCameraById")
.WithSummary("Buscar câmera por ID");

camerasGroup.MapPost("/", async (CameraCreateDto cameraDto, AppDbContext db) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(cameraDto, new ValidationContext(cameraDto), validationResults, true))
    {
        return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }

    var camera = new Camera
    {
        Localizacao = cameraDto.Localizacao,
        Status = cameraDto.Status ?? "ativo", 
        UltimaVerificacao = DateTime.UtcNow
    };
    db.Cameras.Add(camera);
    await db.SaveChangesAsync();

    var response = new CameraResponseDto
    {
        IdCamera = camera.IdCamera,
        Localizacao = camera.Localizacao,
        Status = camera.Status,
        UltimaVerificacao = camera.UltimaVerificacao
    };
    return Results.Created($"/api/cameras/{camera.IdCamera}", response);
})
.WithName("CreateCamera")
.WithSummary("Criar nova câmera");

camerasGroup.MapPut("/{id:int}", async (int id, CameraUpdateDto cameraDto, AppDbContext db) =>
{
    var camera = await db.Cameras.FindAsync(id);
    if (camera is null) return Results.NotFound($"Câmera com ID {id} não encontrada.");
    
    if (cameraDto.Localizacao != null && (cameraDto.Localizacao.Length < 3 || cameraDto.Localizacao.Length > 100))
    {
        return Results.BadRequest("Localização deve ter entre 3 e 100 caracteres.");
    }
    if (cameraDto.Status != null && (cameraDto.Status.Length < 3 || cameraDto.Status.Length > 20))
    {
         return Results.BadRequest("Status deve ter entre 3 e 20 caracteres.");
    }


    bool changed = false;
    if (cameraDto.Localizacao != null && camera.Localizacao != cameraDto.Localizacao)
    {
        camera.Localizacao = cameraDto.Localizacao;
        changed = true;
    }
    if (cameraDto.Status != null && camera.Status != cameraDto.Status)
    {
        camera.Status = cameraDto.Status;
        changed = true;
    }

    if (changed) {
        camera.UltimaVerificacao = DateTime.UtcNow; 
        await db.SaveChangesAsync();
    }
    return Results.NoContent();
})
.WithName("UpdateCamera")
.WithSummary("Atualizar câmera existente");

camerasGroup.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var camera = await db.Cameras.FindAsync(id);
    if (camera is null) return Results.NotFound($"Câmera com ID {id} não encontrada.");

    if (await db.Reconhecimentos.AnyAsync(r => r.IdCamera == id))
    {
        return Results.Conflict($"Câmera com ID {id} está em uso por Reconhecimentos e não pode ser excluída.");
    }

    db.Cameras.Remove(camera);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteCamera")
.WithSummary("Deletar câmera");


// CRUD RECONHECIMENTOS 
var reconhecimentosGroup = app.MapGroup("/api/reconhecimentos").WithTags("Reconhecimentos");

reconhecimentosGroup.MapGet("/", async (AppDbContext db, int? skip, int? take, int? motoId, int? cameraId, decimal? precisaoMinima) =>
{
    var query = db.Reconhecimentos
        .Include(r => r.Moto)
        .Include(r => r.Camera)
        .AsQueryable();

    if (motoId.HasValue) query = query.Where(r => r.IdMoto == motoId);
    if (cameraId.HasValue) query = query.Where(r => r.IdCamera == cameraId);
    if (precisaoMinima.HasValue) query = query.Where(r => r.Precisao >= precisaoMinima);

    if (skip.HasValue) query = query.Skip(skip.Value);
    if (take.HasValue) query = query.Take(take.Value);

    var reconhecimentos = await query.OrderByDescending(r => r.DataHora).ToListAsync();

    var response = reconhecimentos.Select(r => new ReconhecimentoResponseDto
    {
        IdReconhecimento = r.IdReconhecimento,
        IdMoto = r.IdMoto,
        IdCamera = r.IdCamera,
        DataHora = r.DataHora,
        Precisao = r.Precisao,
        ImagemCapturada = r.ImagemCapturada,
        ConfiancaMinima = r.ConfiancaMinima
    }).ToList();

    return Results.Ok(response);
})
.WithName("GetReconhecimentos")
.WithSummary("Buscar reconhecimentos com filtros opcionais.");

reconhecimentosGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
{
    var reconhecimento = await db.Reconhecimentos
        .Include(r => r.Moto)
        .Include(r => r.Camera)
        .FirstOrDefaultAsync(r => r.IdReconhecimento == id);

    if (reconhecimento is null) return Results.NotFound($"Reconhecimento com ID {id} não encontrado.");

    var response = new ReconhecimentoResponseDto
    {
        IdReconhecimento = reconhecimento.IdReconhecimento,
        IdMoto = reconhecimento.IdMoto,
        IdCamera = reconhecimento.IdCamera,
        DataHora = reconhecimento.DataHora,
        Precisao = reconhecimento.Precisao,
        ImagemCapturada = reconhecimento.ImagemCapturada,
        ConfiancaMinima = reconhecimento.ConfiancaMinima
    };
    return Results.Ok(response);
})
.WithName("GetReconhecimentoById")
.WithSummary("Buscar reconhecimento por ID.");

reconhecimentosGroup.MapPost("/", async (ReconhecimentoCreateDto dto, AppDbContext db) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true))
    {
         return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }
    if (!await db.Motos.AnyAsync(m => m.IdMoto == dto.IdMoto))
        return Results.BadRequest($"Moto com ID {dto.IdMoto} não encontrada.");
    if (!await db.Cameras.AnyAsync(c => c.IdCamera == dto.IdCamera))
        return Results.BadRequest($"Câmera com ID {dto.IdCamera} não encontrada.");

    var reconhecimento = new Reconhecimento
    {
        IdMoto = dto.IdMoto,
        IdCamera = dto.IdCamera,
        Precisao = dto.Precisao,
        ImagemCapturada = dto.ImagemCapturada,
        ConfiancaMinima = dto.ConfiancaMinima,
        DataHora = DateTime.UtcNow
    };
    db.Reconhecimentos.Add(reconhecimento);
    await db.SaveChangesAsync();

    var response = new ReconhecimentoResponseDto { }; 
    response.IdReconhecimento = reconhecimento.IdReconhecimento;
    response.IdMoto = reconhecimento.IdMoto;
    response.IdCamera = reconhecimento.IdCamera;
    response.DataHora = reconhecimento.DataHora;
    response.Precisao = reconhecimento.Precisao;
    response.ImagemCapturada = reconhecimento.ImagemCapturada;
    response.ConfiancaMinima = reconhecimento.ConfiancaMinima;

    return Results.Created($"/api/reconhecimentos/{reconhecimento.IdReconhecimento}", response);
})
.WithName("CreateReconhecimento")
.WithSummary("Criar novo reconhecimento.");

reconhecimentosGroup.MapPut("/{id:int}", async (int id, ReconhecimentoUpdateDto dto, AppDbContext db) =>
{
    var reconhecimento = await db.Reconhecimentos.FindAsync(id);
    if (reconhecimento is null) return Results.NotFound($"Reconhecimento com ID {id} não encontrado.");

    var validationResults = new List<ValidationResult>();
     if (!Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true))
    {
         return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }
    if (dto.IdMoto.HasValue && dto.IdMoto.Value != reconhecimento.IdMoto && !await db.Motos.AnyAsync(m => m.IdMoto == dto.IdMoto.Value))
        return Results.BadRequest($"Moto com ID {dto.IdMoto.Value} não encontrada.");
    if (dto.IdCamera.HasValue && dto.IdCamera.Value != reconhecimento.IdCamera && !await db.Cameras.AnyAsync(c => c.IdCamera == dto.IdCamera.Value))
         return Results.BadRequest($"Câmera com ID {dto.IdCamera.Value} não encontrada.");


    if (dto.IdMoto.HasValue) reconhecimento.IdMoto = dto.IdMoto.Value;
    if (dto.IdCamera.HasValue) reconhecimento.IdCamera = dto.IdCamera.Value;
    if (dto.Precisao.HasValue) reconhecimento.Precisao = dto.Precisao.Value;
    if (dto.ImagemCapturada != null) reconhecimento.ImagemCapturada = dto.ImagemCapturada;
    if (dto.ConfiancaMinima.HasValue) reconhecimento.ConfiancaMinima = dto.ConfiancaMinima.Value;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateReconhecimento")
.WithSummary("Atualizar reconhecimento existente.");

reconhecimentosGroup.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var reconhecimento = await db.Reconhecimentos.FindAsync(id);
    if (reconhecimento is null) return Results.NotFound($"Reconhecimento com ID {id} não encontrado.");

    if (await db.Registros.AnyAsync(r => r.IdReconhecimento == id))
    {
        return Results.Conflict($"Reconhecimento com ID {id} está em uso por Registros. Remova os registros dependentes ou desassocie o reconhecimento primeiro (definindo IdReconhecimento como null no registro).");
    }

    db.Reconhecimentos.Remove(reconhecimento);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteReconhecimento")
.WithSummary("Deletar reconhecimento.");

// CRUD LOG ALTERAÇÕES
var logsGroup = app.MapGroup("/api/logs").WithTags("LogAlteracoes");

logsGroup.MapGet("/", async (AppDbContext db, int? skip, int? take, int? usuarioId, int? motoId, string? tipoAcao) =>
{
    var query = db.LogsAlteracoes
        .Include(l => l.Usuario) 
        .Include(l => l.Moto)    
        .AsQueryable();

    if (usuarioId.HasValue) query = query.Where(l => l.IdUsuario == usuarioId);
    if (motoId.HasValue) query = query.Where(l => l.IdMoto == motoId);
    if (!string.IsNullOrEmpty(tipoAcao)) query = query.Where(l => l.TipoAcao.ToLower() == tipoAcao.ToLower());

    if (skip.HasValue) query = query.Skip(skip.Value);
    if (take.HasValue) query = query.Take(take.Value);

    var logs = await query.OrderByDescending(l => l.DataHora).ToListAsync();
    var response = logs.Select(l => new LogAlteracaoResponseDto
    {
        IdLog = l.IdLog,
        IdUsuario = l.IdUsuario,
        IdMoto = l.IdMoto,
        DataHora = l.DataHora,
        TipoAcao = l.TipoAcao,
        CampoAlterado = l.CampoAlterado,
        ValorAntigo = l.ValorAntigo,
        ValorNovo = l.ValorNovo
    });
    return Results.Ok(response);
})
.WithName("GetLogs")
.WithSummary("Buscar logs de alteração com filtros opcionais.");

logsGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
{
    var log = await db.LogsAlteracoes
        .Include(l => l.Usuario)
        .Include(l => l.Moto)
        .FirstOrDefaultAsync(l => l.IdLog == id);
    if (log is null) return Results.NotFound($"Log com ID {id} não encontrado.");

    var response = new LogAlteracaoResponseDto { };
    response.IdLog = log.IdLog;
    response.IdUsuario = log.IdUsuario;
    response.IdMoto = log.IdMoto;
    response.DataHora = log.DataHora;
    response.TipoAcao = log.TipoAcao;
    response.CampoAlterado = log.CampoAlterado;
    response.ValorAntigo = log.ValorAntigo;
    response.ValorNovo = log.ValorNovo;
    return Results.Ok(response);
})
.WithName("GetLogById")
.WithSummary("Buscar log de alteração por ID.");

logsGroup.MapPost("/", async (LogAlteracaoCreateDto dto, AppDbContext db) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true))
    {
        return Results.ValidationProblem(validationResults.ToDictionary(
            e => e.MemberNames.FirstOrDefault() ?? string.Empty,
            e => e.ErrorMessage?.Split(',') ?? new string[] { "Erro de validação." }
        ));
    }
    if (!await db.Usuarios.AnyAsync(u => u.IdUsuario == dto.IdUsuario))
        return Results.BadRequest($"Usuário com ID {dto.IdUsuario} não encontrado.");
    if (!await db.Motos.AnyAsync(m => m.IdMoto == dto.IdMoto))
        return Results.BadRequest($"Moto com ID {dto.IdMoto} não encontrada.");


    var log = new LogAlteracao
    {
        IdUsuario = dto.IdUsuario,
        IdMoto = dto.IdMoto,
        TipoAcao = dto.TipoAcao,
        CampoAlterado = dto.CampoAlterado,
        ValorAntigo = dto.ValorAntigo,
        ValorNovo = dto.ValorNovo,
        DataHora = DateTime.UtcNow
    };
    db.LogsAlteracoes.Add(log);
    await db.SaveChangesAsync();

    var response = new LogAlteracaoResponseDto { };
    response.IdLog = log.IdLog;
    return Results.Created($"/api/logs/{log.IdLog}", response);
})
.WithName("CreateLog")
.WithSummary("Criar novo log de alteração (uso interno geralmente).");


app.Run();