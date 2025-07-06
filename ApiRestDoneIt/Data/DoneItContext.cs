
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using ApiRestDoneIt.Models;

namespace ApiRestDoneIt.Data;

public partial class DoneItContext : DbContext
{
    public DoneItContext()
    {
    }

    public DoneItContext(DbContextOptions<DoneItContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Proyecto> Proyectos { get; set; }

    public virtual DbSet<Tarea> Tareas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.id_proyecto).HasName("PRIMARY");

            entity.ToTable("proyecto");

            entity.HasIndex(e => e.id_usuario, "id_usuario");

            entity.Property(e => e.id_proyecto).HasColumnName("id_proyecto");
            
            entity.Property(e => e.descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");

            entity.Property(e => e.fecha_creacion)
                .HasColumnType("date")
                .HasColumnName("fecha_creacion");

            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.HasOne(d => d.id_usuarioNavigation).WithMany(p => p.proyectos)
                .HasForeignKey(d => d.id_usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proyecto_ibfk_1");
        });

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.HasKey(e => e.id_tarea).HasName("PRIMARY");

            entity.ToTable("tarea");

            entity.HasIndex(e => e.id_proyecto, "id_proyecto");
            
            entity.Property(e => e.id_tarea).HasColumnName("id_tarea");

            entity.Property(e => e.descripcion).HasColumnType("text")
            .HasColumnName("descripcion");
            entity.Property(e => e.estado)
                .IsRequired()
                .HasColumnType("enum('Pendiente','En Proceso','Finalizado')");
            entity.Property(e => e.fecha_fin).HasColumnType("datetime");
            entity.Property(e => e.fecha_inicio).HasColumnType("datetime");
            entity.Property(e => e.prioridad)
                .IsRequired()
                .HasColumnType("enum('Bajo','Medio','Alto')");
            entity.Property(e => e.titulo)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("titulo");
            entity.Property(e => e.id_proyecto).HasColumnName("id_proyecto");

            entity.HasOne(d => d.id_proyectoNavigation).WithMany(p => p.tareas)
                .HasForeignKey(d => d.id_proyecto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tarea_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.id_usuario).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.HasIndex(e => e.nombre_usuario, "nombre_usuario").IsUnique();

            entity.Property(e => e.apellido)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.email)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.fecha_nacimiento)
                           .HasColumnType("date")
                           .HasColumnName("fecha_nacimiento");
            entity.Property(e => e.fecha_registro)
                .HasColumnType("date")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.nombre_usuario)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.password_hash)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.salt)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.token_recuperacion).HasMaxLength(255);
            entity.Property(e => e.vencimiento_token).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}