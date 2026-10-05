using System;
using System.Collections.Generic;
using Commerce_Connector_Data.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Commerce_Connector_Data.FileProcessingEntities;

public partial class FileProcessing_Entities : DbContext
{
    public FileProcessing_Entities()
    {
    }

    public FileProcessing_Entities(DbContextOptions<FileProcessing_Entities> options)
        : base(options)
    {
    }

    public virtual DbSet<FileDetail> FileDetail { get; set; }

    public virtual DbSet<LocationConfiguration> LocationConfigurations { get; set; }

    public virtual DbSet<TemplateDetail> TemplateDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ACL-228\\SQL2022;Initial Catalog=FileProcessing_Database;Persist Security Info=False;User ID=sa;Password=Commerce#765;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileDetail>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__FileDeta__3214EC07C7D82231");

            entity.ToTable("FileDetail");

            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LocationType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModelType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.LocationConfig).WithMany(p => p.FileDetails)
                .HasForeignKey(d => d.LocationConfigId)
                .HasConstraintName("FK_FileDetail_LocationConfiguration");
        });

        modelBuilder.Entity<LocationConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Location__3214EC0759C1EFD9");

            entity.ToTable("LocationConfiguration");

            entity.Property(e => e.ApiEndpoint)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ApiKey)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ConnectionString)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContainerName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LocationType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ServerAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TemplateDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Template__3214EC0735B5161A");

            entity.Property(e => e.TemplateName).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
