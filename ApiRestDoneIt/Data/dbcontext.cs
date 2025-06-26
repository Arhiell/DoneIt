using Microsoft.EntityFrameworkCore;

public class DoneItContext : DbContext
{
	public DoneItContext(DbContextOptions<DoneItContext> options) : base(options) { }

	public DbSet<Usuario> Usuarios { get; set; }
	public DbSet<Proyecto> Proyectos { get; set; }
	public DbSet<Tarea> Tareas { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Usuario>().ToTable("Usuario");
		modelBuilder.Entity<Proyecto>().ToTable("Proyecto");
		modelBuilder.Entity<Tarea>().ToTable("Tarea");

		// Configurar ENUMs como string
		modelBuilder.Entity<Tarea>()
			.Property(t => t.Estado)
			.HasConversion<string>();

		modelBuilder.Entity<Tarea>()
			.Property(t => t.Prioridad)
			.HasConversion<string>();
	}
}
