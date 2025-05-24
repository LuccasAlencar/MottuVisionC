using Microsoft.EntityFrameworkCore;
using SistemaMottuAPI.Model;

namespace SistemaMottuAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets para cada entidade
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Moto> Motos { get; set; }
    public DbSet<Camera> Cameras { get; set; }
    public DbSet<Reconhecimento> Reconhecimentos { get; set; }
    public DbSet<Registro> Registros { get; set; }
    public DbSet<LogAlteracao> LogsAlteracoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações para Cargo
        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.ToTable("CARGOS", t => t.HasCheckConstraint("CK_CARGO_NIVEL", "NIVEL_PERMISSAO BETWEEN 1 AND 5")); 
            entity.HasKey(e => e.IdCargo);
            entity.Property(e => e.IdCargo).HasColumnName("ID_CARGO");
            entity.Property(e => e.Nome).HasColumnName("NOME").HasMaxLength(50).IsRequired();
            entity.Property(e => e.NivelPermissao).HasColumnName("NIVEL_PERMISSAO").IsRequired();
            entity.Property(e => e.Permissoes).HasColumnName("PERMISSOES").HasColumnType("CLOB").IsRequired();
            
            entity.HasIndex(e => e.Nome).IsUnique();
        });

        // Configurações para Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("USUARIOS");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");
            entity.Property(e => e.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("EMAIL").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Senha).HasColumnName("SENHA").HasMaxLength(60).IsRequired();
            entity.Property(e => e.IdCargo).HasColumnName("ID_CARGO").IsRequired();
            entity.Property(e => e.Ativo).HasColumnName("ATIVO").HasMaxLength(3).HasDefaultValueSql("'Sim'"); 
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasOne(d => d.Cargo)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdCargo)
                .HasConstraintName("FK_USUARIO_CARGO");
        });

        // Configurações para Moto
        modelBuilder.Entity<Moto>(entity =>
        {
            entity.ToTable("MOTOS");
            entity.HasKey(e => e.IdMoto);
            entity.Property(e => e.IdMoto).HasColumnName("ID_MOTO");
            entity.Property(e => e.Placa).HasColumnName("PLACA").HasMaxLength(7).IsRequired();
            entity.Property(e => e.Marca).HasColumnName("MARCA").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Modelo).HasColumnName("MODELO").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Cor).HasColumnName("COR").HasMaxLength(20).IsRequired();
            entity.Property(e => e.ImagemReferencia).HasColumnName("IMAGEM_REFERENCIA").HasMaxLength(255);
            entity.Property(e => e.Presente).HasColumnName("PRESENTE").HasMaxLength(3).HasDefaultValueSql("'Não'"); 
            entity.HasIndex(e => e.Placa).IsUnique();
        });

        // Configurações para Camera
        modelBuilder.Entity<Camera>(entity =>
        {
            entity.ToTable("CAMERAS" ); 
            entity.HasKey(e => e.IdCamera);
            entity.Property(e => e.IdCamera).HasColumnName("ID_CAMERA");
            entity.Property(e => e.Localizacao).HasColumnName("LOCALIZACAO").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasColumnName("STATUS").HasMaxLength(20).HasDefaultValueSql("'ativo'");
            entity.Property(e => e.UltimaVerificacao).HasColumnName("ULTIMA_VERIFICACAO").HasDefaultValueSql("CURRENT_TIMESTAMP");

        });

        // Configurações para Reconhecimento
        modelBuilder.Entity<Reconhecimento>(entity =>
        {
            entity.ToTable("RECONHECIMENTOS");
            entity.HasKey(e => e.IdReconhecimento);
            entity.Property(e => e.IdReconhecimento).HasColumnName("ID_RECONHECIMENTO");
            entity.Property(e => e.IdMoto).HasColumnName("ID_MOTO").IsRequired();
            entity.Property(e => e.IdCamera).HasColumnName("ID_CAMERA").IsRequired();
            entity.Property(e => e.DataHora).HasColumnName("DATA_HORA").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Precisao).HasColumnName("PRECISAO").HasPrecision(5, 4).IsRequired();
            entity.Property(e => e.ImagemCapturada).HasColumnName("IMAGEM_CAPTURADA").HasMaxLength(255).IsRequired();
            entity.Property(e => e.ConfiancaMinima).HasColumnName("CONFIANCA_MINIMA").HasPrecision(5, 4).IsRequired();

            entity.HasIndex(e => e.DataHora).HasDatabaseName("IDX_RECONHECIMENTO_DATA");

            entity.HasOne(d => d.Moto)
                .WithMany(p => p.Reconhecimentos)
                .HasForeignKey(d => d.IdMoto)
                .HasConstraintName("FK_REC_MOTO");

            entity.HasOne(d => d.Camera)
                .WithMany(p => p.Reconhecimentos)
                .HasForeignKey(d => d.IdCamera)
                .HasConstraintName("FK_REC_CAMERA");
        });

        // Configurações para Registro
        modelBuilder.Entity<Registro>(entity =>
        {
            entity.ToTable("REGISTROS");
            entity.HasKey(e => e.IdRegistro);
            entity.Property(e => e.IdRegistro).HasColumnName("ID_REGISTRO");
            entity.Property(e => e.IdMoto).HasColumnName("ID_MOTO").IsRequired();
            entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO").IsRequired();
            entity.Property(e => e.IdReconhecimento).HasColumnName("ID_RECONHECIMENTO");
            entity.Property(e => e.DataHora).HasColumnName("DATA_HORA").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Tipo).HasColumnName("TIPO").HasMaxLength(10).IsRequired();
            entity.Property(e => e.ModoRegistro).HasColumnName("MODO_REGISTRO").HasMaxLength(10).IsRequired();

            entity.HasIndex(e => e.Tipo).HasDatabaseName("IDX_REGISTRO_TIPO");

            entity.HasOne(d => d.Moto)
                .WithMany(p => p.Registros)
                .HasForeignKey(d => d.IdMoto)
                .HasConstraintName("FK_REG_MOTO");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.Registros)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_REG_USUARIO");

            entity.HasOne(d => d.Reconhecimento)
                .WithMany(p => p.Registros)
                .HasForeignKey(d => d.IdReconhecimento)
                .HasConstraintName("FK_REG_RECONHECIMENTO")
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configurações para LogAlteracao
        modelBuilder.Entity<LogAlteracao>(entity =>
        {
            entity.ToTable("LOG_ALTERACOES"); 
            entity.HasKey(e => e.IdLog);
            entity.Property(e => e.IdLog).HasColumnName("ID_LOG");
            entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO").IsRequired();
            entity.Property(e => e.IdMoto).HasColumnName("ID_MOTO").IsRequired();
            entity.Property(e => e.DataHora).HasColumnName("DATA_HORA").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.TipoAcao).HasColumnName("TIPO_ACAO").HasMaxLength(10).IsRequired();
            entity.Property(e => e.CampoAlterado).HasColumnName("CAMPO_ALTERADO").HasMaxLength(50).IsRequired();
            entity.Property(e => e.ValorAntigo).HasColumnName("VALOR_ANTIGO").HasColumnType("CLOB");
            entity.Property(e => e.ValorNovo).HasColumnName("VALOR_NOVO").HasColumnType("CLOB");

            entity.HasIndex(e => e.TipoAcao).HasDatabaseName("IDX_LOG_ACAO");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.LogsAlteracoes)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_LOG_USUARIO");

            entity.HasOne(d => d.Moto)
                .WithMany(p => p.LogsAlteracoes)
                .HasForeignKey(d => d.IdMoto)
                .HasConstraintName("FK_LOG_MOTO");
        });
    }
}