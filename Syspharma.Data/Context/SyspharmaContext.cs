using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Entities;
using System;
using System.Collections.Generic;

namespace Syspharma.Data.Context;

public partial class SyspharmaContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public SyspharmaContext(DbContextOptions<SyspharmaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<CategoriaServicio> CategoriaServicios { get; set; }
    public virtual DbSet<Cita> Citas { get; set; }
    public virtual DbSet<Compra> Compras { get; set; }
    public virtual DbSet<CompraDetalle> CompraDetalles { get; set; }
    public virtual DbSet<EstadosCitum> EstadosCita { get; set; }
    public virtual DbSet<EstadosCompra> EstadosCompras { get; set; }
    public virtual DbSet<EstadosPedido> EstadosPedidos { get; set; }
    public virtual DbSet<EstadosProveedor> EstadosProveedors { get; set; }
    public virtual DbSet<EstadosVentum> EstadosVenta { get; set; }
    public virtual DbSet<Gasto> Gastos { get; set; }
    public virtual DbSet<DisponibilidadHorario> DisponibilidadHorarios { get; set; }
    public virtual DbSet<DisponibilidadBloqueo> DisponibilidadBloqueos { get; set; }
    public virtual DbSet<Medico> Medicos { get; set; }
    public virtual DbSet<MedicoHorario> MedicoHorarios { get; set; }
    public virtual DbSet<MedicoDiaNoDisponible> MedicoDiasNoDisponibles { get; set; }
    public virtual DbSet<MetodosPago> MetodosPagos { get; set; }
    public virtual DbSet<Pedido> Pedidos { get; set; }
    public virtual DbSet<PedidoDetalle> PedidoDetalles { get; set; }
    public virtual DbSet<Permiso> Permisos { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<ProductoMedicamento> ProductoMedicamentos { get; set; }
    public virtual DbSet<Proveedore> Proveedores { get; set; }
    public virtual new DbSet<Role> Roles { get; set; }
    public virtual DbSet<RolesPermiso> RolesPermisos { get; set; }
    public virtual DbSet<Servicio> Servicios { get; set; }
    public virtual DbSet<TiposDocumento> TiposDocumentos { get; set; }
    public virtual DbSet<Turno> Turnos { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<VCompraDetalle> VCompraDetalles { get; set; }
    public virtual DbSet<VResumenTurno> VResumenTurnos { get; set; }
    public virtual DbSet<VVentaDetalle> VVentaDetalles { get; set; }
    public virtual DbSet<Venta> Ventas { get; set; }
    public virtual DbSet<VentaDetalle> VentaDetalles { get; set; }
    // Nueva DbSet agregada
    public virtual DbSet<DetalleDevolucion> DetallesDevolucion { get; set; }
    public virtual DbSet<VentaDetalleServicio> VentaDetalleServicios { get; set; }

    public virtual DbSet<EstadoDevolucion> EstadosDevoluciones { get; set; }
    public virtual DbSet<Devolucion> Devoluciones { get; set; }
    public virtual DbSet<DetalleDevolucion> DetallesDevoluciones { get; set; }

    // ✅ Nuevo DbSet para configuraciones
    public virtual DbSet<Configuracion> Configuraciones { get; set; }

    public virtual DbSet<Lote> Lotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83F3635F0B3");
            entity.ToTable("categorias");
            entity.HasIndex(e => e.Nombre, "UQ__categori__72AFBCC6D44E580C").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre).HasMaxLength(100).HasColumnName("nombre");
        });

        modelBuilder.Entity<CategoriaServicio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83F83A344EC");
            entity.ToTable("categoria_servicios");
            entity.HasIndex(e => e.Nombre, "UQ__categori__72AFBCC6A71D1D12").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre).HasMaxLength(100).HasColumnName("nombre");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__citas__3213E83FC793410B");
            entity.ToTable("citas");
            entity.HasIndex(e => new { e.MedicoId, e.Fecha }, "idx_citas_medico_fecha").IsDescending(false, true);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EstadoId).HasColumnName("estadoId");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Hora).HasColumnName("hora");
            entity.Property(e => e.MedicoId).HasColumnName("medicoId");
            entity.Property(e => e.Notas).HasColumnName("notas");
            entity.Property(e => e.PacienteDocumento).HasMaxLength(20).HasColumnName("pacienteDocumento");
            entity.Property(e => e.PacienteEmail).HasMaxLength(100).HasColumnName("pacienteEmail");
            entity.Property(e => e.PacienteNombre).HasMaxLength(200).HasColumnName("pacienteNombre");
            entity.Property(e => e.PacienteTelefono).HasMaxLength(20).HasColumnName("pacienteTelefono");
            entity.Property(e => e.Precio).HasColumnType("decimal(12, 2)").HasColumnName("precio");
            entity.Property(e => e.ServicioId).HasColumnName("servicioId");
            entity.Property(e => e.ServicioNombre).HasMaxLength(100).HasColumnName("servicioNombre");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.PedidoId).HasColumnName("pedidoId");
            entity.Property(e => e.VentaId).HasColumnName("ventaId");
            entity.HasOne(d => d.Estado).WithMany(p => p.Cita).HasForeignKey(d => d.EstadoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Citas_Estado");
            entity.HasOne(d => d.Medico).WithMany(p => p.Cita).HasForeignKey(d => d.MedicoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Citas_Medicos");
            entity.HasOne(d => d.Servicio).WithMany(p => p.Cita).HasForeignKey(d => d.ServicioId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Citas_Servicios");
            entity.HasOne(d => d.Usuario).WithMany(p => p.Cita).HasForeignKey(d => d.UsuarioId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Citas_Usuarios");
            entity.HasOne(d => d.Pedido).WithMany().HasForeignKey(d => d.PedidoId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Citas_Pedidos");
            entity.HasOne(d => d.Venta).WithMany().HasForeignKey(d => d.VentaId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Citas_Ventas");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__compras__3213E83F8314C9C0");
            entity.ToTable("compras");
            entity.HasIndex(e => e.NumeroCompra, "UQ__compras__6EB8ED51AA2AF7FC").IsUnique();
            entity.HasIndex(e => e.ProveedorId, "idx_compras_proveedor");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EstadoId).HasColumnName("estadoId");
            entity.Property(e => e.FechaCompra).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCompra");
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime").HasColumnName("fechaEntrega");
            entity.Property(e => e.Iva).HasColumnType("decimal(12, 2)").HasColumnName("iva");
            entity.Property(e => e.Notas).HasColumnName("notas");
            entity.Property(e => e.NumeroCompra).HasMaxLength(50).HasColumnName("numeroCompra");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.ProveedorId).HasColumnName("proveedorId");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)").HasColumnName("subtotal");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)").HasColumnName("total");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.HasOne(d => d.Estado).WithMany(p => p.Compras).HasForeignKey(d => d.EstadoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Compras_Estado");
            entity.HasOne(d => d.Proveedor).WithMany(p => p.Compras).HasForeignKey(d => d.ProveedorId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Compras_Proveedores");
            entity.HasOne(d => d.Usuario).WithMany(p => p.Compras).HasForeignKey(d => d.UsuarioId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Compras_Usuarios");
        });

        modelBuilder.Entity<CompraDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__compra_d__3213E83F3E646B3C");
            entity.ToTable("compra_detalles");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CompraId).HasColumnName("compraId");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)").HasColumnName("precioUnitario");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)").HasColumnName("subtotal");
            entity.Property(e => e.Lote).HasMaxLength(100).HasColumnName("lote");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fechaVencimiento");
            entity.HasOne(d => d.Compra).WithMany(p => p.CompraDetalles).HasForeignKey(d => d.CompraId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CompraDetalles_Compras");
            entity.HasOne(d => d.Producto).WithMany(p => p.CompraDetalles).HasForeignKey(d => d.ProductoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CompraDetalles_Productos");
        });

        modelBuilder.Entity<EstadosCitum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__estados___3213E83FD50846F6");
            entity.ToTable("estados_cita");
            entity.HasIndex(e => e.Nombre, "UQ__estados___72AFBCC60D8E2BC0").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasMaxLength(30).HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadosCompra>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__estados___3213E83F596D49DE");
            entity.ToTable("estados_compra");
            entity.HasIndex(e => e.Nombre, "UQ__estados___72AFBCC605F19684").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasMaxLength(30).HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadosPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__estados___3213E83FD81DA3EB");
            entity.ToTable("estados_pedido");
            entity.HasIndex(e => e.Nombre, "UQ__estados___72AFBCC6CC1257F9").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasMaxLength(30).HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadosProveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__estados___3213E83FCFECEA39");
            entity.ToTable("estados_proveedor");
            entity.HasIndex(e => e.Nombre, "UQ__estados___72AFBCC66CC17226").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasMaxLength(30).HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadosVentum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__estados___3213E83FDCF557D9");
            entity.ToTable("estados_venta");
            entity.HasIndex(e => e.Nombre, "UQ__estados___72AFBCC61A2305EF").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasMaxLength(30).HasColumnName("nombre");
        });

        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__gastos__3213E83F9BA914EE");
            entity.ToTable("gastos");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoria).HasMaxLength(20).HasDefaultValue("operacional").HasColumnName("categoria");
            entity.Property(e => e.Comprobante).HasMaxLength(100).HasColumnName("comprobante");
            entity.Property(e => e.Concepto).HasMaxLength(200).HasColumnName("concepto");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.FechaGasto).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaGasto");
            entity.Property(e => e.Monto).HasColumnType("decimal(12, 2)").HasColumnName("monto");
            entity.Property(e => e.TurnoId).HasColumnName("turnoId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Anulado).HasDefaultValue(false).HasColumnName("anulado");
            entity.Property(e => e.FechaAnulacion).HasColumnType("datetime").HasColumnName("fechaAnulacion");
            entity.Property(e => e.MotivoAnulacion).HasMaxLength(255).HasColumnName("motivoAnulacion");
            entity.HasOne(d => d.Turno).WithMany(p => p.Gastos).HasForeignKey(d => d.TurnoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Gastos_Turnos");
            entity.HasOne(d => d.Usuario).WithMany(p => p.Gastos).HasForeignKey(d => d.UsuarioId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Gastos_Usuarios");
        });

        modelBuilder.Entity<DisponibilidadHorario>(entity =>
        {
            entity.ToTable("disponibilidad_horarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MedicoId).HasColumnName("medicoId");
            entity.Property(e => e.DiaSemana).HasColumnName("diaSemana");
            entity.Property(e => e.MananaInicio).HasMaxLength(5).HasColumnName("mananaInicio");
            entity.Property(e => e.MananaFin).HasMaxLength(5).HasColumnName("mananaFin");
            entity.Property(e => e.TardeInicio).HasMaxLength(5).HasColumnName("tardeInicio");
            entity.Property(e => e.TardeFin).HasMaxLength(5).HasColumnName("tardeFin");
            entity.HasOne(d => d.Medico).WithMany().HasForeignKey(d => d.MedicoId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_DisponibilidadHorarios_Medicos");
        });

        modelBuilder.Entity<DisponibilidadBloqueo>(entity =>
        {
            entity.ToTable("disponibilidad_bloqueos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MedicoId).HasColumnName("medicoId");
            entity.Property(e => e.FechaInicio).HasColumnName("fechaInicio").HasColumnType("date");
            entity.Property(e => e.FechaFin).HasColumnName("fechaFin").HasColumnType("date");
            entity.Property(e => e.Motivo).HasMaxLength(255).HasColumnName("motivo");
            entity.HasOne(d => d.Medico).WithMany().HasForeignKey(d => d.MedicoId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_DisponibilidadBloqueos_Medicos");
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__medicos__3213E83F08A4C5C1");
            entity.ToTable("medicos");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiasLaborales).HasMaxLength(50).HasColumnName("diasLaborales");
            entity.Property(e => e.Documento).HasMaxLength(20).HasColumnName("documento");
            entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
            entity.Property(e => e.Especialidad).HasMaxLength(100).HasColumnName("especialidad");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.HoraFin).HasColumnName("horaFin");
            entity.Property(e => e.HoraInicio).HasColumnName("horaInicio");
            entity.Property(e => e.Intervalo).HasDefaultValue(30).HasColumnName("intervalo");
            entity.Property(e => e.Nombre).HasMaxLength(150).HasColumnName("nombre");
            entity.Property(e => e.Telefono).HasMaxLength(20).HasColumnName("telefono");
        });

        modelBuilder.Entity<MedicoHorario>(entity =>
        {
            entity.ToTable("MedicoHorario");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Medico)
                  .WithMany(m => m.Horarios)
                  .HasForeignKey(e => e.MedicoId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.MedicoId, e.DiaSemana }).IsUnique();
        });

        modelBuilder.Entity<MedicoDiaNoDisponible>(entity =>
        {
            entity.ToTable("MedicoDiaNoDisponible");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Medico)
                  .WithMany(m => m.DiasNoDisponibles)
                  .HasForeignKey(e => e.MedicoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MetodosPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__metodos___3213E83FBFEBC96F");
            entity.ToTable("metodos_pago");
            entity.HasIndex(e => e.Nombre, "UQ__metodos___72AFBCC64CAE685E").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__pedidos__3213E83F5DD7EBD1");
            entity.ToTable("pedidos");
            entity.HasIndex(e => e.NumeroPedido, "UQ__pedidos__90DD6149AD9BA5B6").IsUnique();
            entity.HasIndex(e => e.EstadoId, "idx_pedidos_estado");
            entity.HasIndex(e => e.FechaCreacion, "idx_pedidos_fecha").IsDescending();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClienteDocumento).HasMaxLength(20).HasColumnName("clienteDocumento");
            entity.Property(e => e.ClienteEmail).HasMaxLength(100).HasColumnName("clienteEmail");
            entity.Property(e => e.ClienteNombre).HasMaxLength(150).HasColumnName("clienteNombre");
            entity.Property(e => e.ClienteTelefono).HasMaxLength(20).HasColumnName("clienteTelefono");
            entity.Property(e => e.EstadoId).HasColumnName("estadoId");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime").HasColumnName("fechaEntrega");
            entity.Property(e => e.Iva).HasColumnType("decimal(12, 2)").HasColumnName("iva");
            entity.Property(e => e.MetodoPagoId).HasColumnName("metodoPagoId");
            entity.Property(e => e.Notas).HasColumnName("notas");
            entity.Property(e => e.NumeroPedido).HasMaxLength(50).HasColumnName("numeroPedido");
            entity.Property(e => e.Origen).HasMaxLength(20).HasDefaultValue("web").HasColumnName("origen");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)").HasColumnName("subtotal");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)").HasColumnName("total");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.HasOne(d => d.Estado).WithMany(p => p.Pedidos).HasForeignKey(d => d.EstadoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Pedidos_Estado");
            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Pedidos).HasForeignKey(d => d.MetodoPagoId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Pedidos_MetodoPago");
            entity.HasOne(d => d.Usuario).WithMany(p => p.Pedidos).HasForeignKey(d => d.UsuarioId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Pedidos_Usuarios");
        });

        modelBuilder.Entity<PedidoDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__pedido_d__3213E83F48F2B407");
            entity.ToTable("pedido_detalles");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Nombre).HasMaxLength(200).HasColumnName("nombre");
            entity.Property(e => e.PedidoId).HasColumnName("pedidoId");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)").HasColumnName("precioUnitario");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)").HasColumnName("subtotal");
            entity.HasOne(d => d.Pedido).WithMany(p => p.PedidoDetalles).HasForeignKey(d => d.PedidoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PedidoDetalles_Pedidos");
            entity.HasOne(d => d.Producto).WithMany(p => p.PedidoDetalles).HasForeignKey(d => d.ProductoId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_PedidoDetalles_Productos");
        });

        // ✅ CONFIGURACIÓN DE GASTO (LIMPIA)
        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__gastos__3213E83F9BA914EE");
            entity.ToTable("gastos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TurnoId).HasColumnName("turnoId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Concepto).HasMaxLength(200).HasColumnName("concepto");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Monto).HasColumnType("decimal(12, 2)").HasColumnName("monto");
            entity.Property(e => e.Categoria)
                .HasMaxLength(20)
                .HasDefaultValue("operacional")
                .HasColumnName("categoria");
            entity.Property(e => e.Comprobante).HasMaxLength(100).HasColumnName("comprobante");
            entity.Property(e => e.FechaGasto)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaGasto");

            entity.HasOne(d => d.Turno)
                .WithMany(p => p.Gastos)
                .HasForeignKey(d => d.TurnoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gastos_Turnos");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.Gastos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gastos_Usuarios");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__producto__3213E83F11CBC83A");
            entity.ToTable("productos");
            entity.HasIndex(e => new { e.CategoriaId, e.Estado }, "idx_productos_categoria_estado");
            entity.HasIndex(e => e.Nombre, "idx_productos_nombre");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoriaId");
            entity.Property(e => e.CodigoBarras).HasMaxLength(100).HasColumnName("codigoBarras");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Imagen).HasColumnName("imagen");
            entity.Property(e => e.Nombre).HasMaxLength(200).HasColumnName("nombre");
            entity.Property(e => e.Precio).HasColumnType("decimal(12, 2)").HasColumnName("precio");
            entity.Property(e => e.PrecioCompra).HasColumnType("decimal(12, 2)").HasColumnName("precioCompra");
            entity.Property(e => e.Marca).HasMaxLength(100).HasColumnName("marca");
            entity.Property(e => e.Presentacion).HasMaxLength(150).HasColumnName("presentacion");
            entity.Property(e => e.ProveedorId).HasColumnName("proveedorId");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.UltimaActualizacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("ultimaActualizacion");
            entity.Property(e => e.FechaVencimientoProxima).HasColumnName("fechaVencimientoProxima");
            entity.Property(e => e.PorcentajeIva).HasColumnType("decimal(5, 2)").HasDefaultValue(0.00m).HasColumnName("porcentajeIva");
            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos).HasForeignKey(d => d.CategoriaId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Productos_Categorias");
            entity.HasOne(d => d.Proveedor).WithMany(p => p.Productos).HasForeignKey(d => d.ProveedorId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Productos_Proveedores");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__proveedo__3213E83FFD96C03C");
            entity.ToTable("proveedores");
            entity.HasIndex(e => e.Nombre, "UQ__proveedo__72AFBCC64091315F").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contacto).HasMaxLength(150).HasColumnName("contacto");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.Documento).HasMaxLength(50).HasColumnName("documento");
            entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
            entity.Property(e => e.EstadoId).HasColumnName("estadoId");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre).HasMaxLength(150).HasColumnName("nombre");
            entity.Property(e => e.Telefono).HasMaxLength(20).HasColumnName("telefono");
            entity.Property(e => e.TipoDocumentoId).HasColumnName("tipoDocumentoId");
            entity.HasOne(d => d.Estado).WithMany(p => p.Proveedores).HasForeignKey(d => d.EstadoId).HasConstraintName("FK_Proveedores_Estado");
            entity.HasOne(d => d.TipoDocumento).WithMany(p => p.Proveedores).HasForeignKey(d => d.TipoDocumentoId).HasConstraintName("FK_Proveedores_TipoDocumento");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3213E83F06400409");
            entity.ToTable("roles");
            entity.HasIndex(e => e.Nombre, "UQ__roles__72AFBCC689F96075").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
        });

        modelBuilder.Entity<RolesPermiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles_pe__3213E83F567DFDB5");
            entity.ToTable("roles_permisos");
            entity.HasIndex(e => new { e.RoleId, e.PermisoId }, "UQ_Role_Permiso").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaAsignacion");
            entity.Property(e => e.PermisoId).HasColumnName("permisoId");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.HasOne(d => d.Permiso).WithMany(p => p.RolesPermisos).HasForeignKey(d => d.PermisoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RolesPermisos_Permisos");
            entity.HasOne(d => d.Role).WithMany(p => p.RolesPermisos).HasForeignKey(d => d.RoleId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RolesPermisos_Roles");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__servicio__3213E83F9123C840");
            entity.ToTable("servicios");
            entity.HasIndex(e => e.CategoriaId, "idx_servicios_categoria");
            entity.HasIndex(e => e.Estado, "idx_servicios_estado");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoriaId");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Duracion).HasColumnName("duracion");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre).HasMaxLength(150).HasColumnName("nombre");
            entity.Property(e => e.Precio).HasColumnType("decimal(12, 2)").HasColumnName("precio");
            entity.HasOne(d => d.Categoria).WithMany(p => p.Servicios).HasForeignKey(d => d.CategoriaId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Servicios_Categoria");
        });

        modelBuilder.Entity<TiposDocumento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tipos_do__3213E83FCA8B537B");
            entity.ToTable("tipos_documento");
            entity.HasIndex(e => e.Nombre, "UQ__tipos_do__72AFBCC640703EE9").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__turnos__3213E83F9A3D7F5A");
            entity.ToTable("turnos");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Diferencia).HasColumnType("decimal(12, 2)").HasColumnName("diferencia");
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("activo").HasColumnName("estado");
            entity.Property(e => e.FechaApertura).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaApertura");
            entity.Property(e => e.FechaCierre).HasColumnType("datetime").HasColumnName("fechaCierre");
            entity.Property(e => e.MontoBase).HasColumnType("decimal(12, 2)").HasColumnName("montoBase");
            entity.Property(e => e.MontoFinal).HasColumnType("decimal(12, 2)").HasColumnName("montoFinal");
            entity.Property(e => e.Notas).HasColumnName("notas");
            entity.Property(e => e.ResumenErroresCaja).HasColumnType("decimal(12, 2)").HasColumnName("resumenErroresCaja");
            entity.Property(e => e.ResumenServicios).HasColumnName("resumenServicios");
            entity.Property(e => e.ResumenVentas).HasColumnName("resumenVentas");
            entity.Property(e => e.TotalGastos).HasColumnType("decimal(12, 2)").HasColumnName("totalGastos");
            entity.Property(e => e.TotalVentas).HasColumnType("decimal(12, 2)").HasColumnName("totalVentas");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.HasOne(d => d.Usuario).WithMany(p => p.Turnos).HasForeignKey(d => d.UsuarioId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Turnos_Usuarios");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.Documento).HasMaxLength(20).HasColumnName("documento");
            entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre).HasMaxLength(150).HasColumnName("nombre");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Telefono).HasMaxLength(20).HasColumnName("telefono");
            entity.Property(e => e.TipoDocumentoId).HasColumnName("tipoDocumentoId");
            entity.Property(e => e.UltimoAcceso).HasColumnType("datetime").HasColumnName("ultimoAcceso");
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.HasOne(d => d.Role).WithMany(p => p.Usuarios).HasForeignKey(d => d.RoleId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Usuarios_Roles");
            entity.HasOne(d => d.TipoDocumento).WithMany(p => p.Usuarios).HasForeignKey(d => d.TipoDocumentoId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Usuarios_TipoDocumento");
        });

        modelBuilder.Entity<VCompraDetalle>(entity =>
        {
            entity.HasNoKey().ToView("v_compra_detalles");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CompraId).HasColumnName("compraId");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)").HasColumnName("precioUnitario");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.ProductoNombre).HasMaxLength(200).HasColumnName("productoNombre");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)").HasColumnName("subtotal");
            entity.Property(e => e.SubtotalCalculado).HasColumnType("decimal(23, 2)").HasColumnName("subtotal_calculado");
        });

        modelBuilder.Entity<VResumenTurno>(entity =>
        {
            entity.HasNoKey().ToView("v_resumen_turnos");
            entity.Property(e => e.Empleado).HasMaxLength(150).HasColumnName("empleado");
            entity.Property(e => e.Estado).HasMaxLength(20).HasColumnName("estado");
            entity.Property(e => e.FechaApertura).HasColumnType("datetime").HasColumnName("fechaApertura");
            entity.Property(e => e.FechaCierre).HasColumnType("datetime").HasColumnName("fechaCierre");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MontoBase).HasColumnType("decimal(12, 2)").HasColumnName("montoBase");
            entity.Property(e => e.TotalGastos).HasColumnType("decimal(38, 2)").HasColumnName("totalGastos");
            entity.Property(e => e.TotalVentas).HasColumnType("decimal(38, 2)").HasColumnName("totalVentas");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ventas__3213E83F3E2DA856");
            entity.ToTable("ventas");
            entity.HasIndex(e => e.NumeroVenta, "UQ__ventas__44FDAC498B274DB5").IsUnique();
            entity.HasIndex(e => new { e.TurnoId, e.FechaVenta }, "idx_ventas_turno_fecha").IsDescending(false, true);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClienteDocumento).HasMaxLength(20).HasColumnName("clienteDocumento");
            entity.Property(e => e.ClienteNombre).HasMaxLength(150).HasColumnName("clienteNombre");
            entity.Property(e => e.ClienteTelefono).HasMaxLength(20).HasColumnName("clienteTelefono");
            entity.Property(e => e.EstadoId).HasColumnName("estadoId");
            entity.Property(e => e.FechaVenta).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaVenta");
            entity.Property(e => e.Iva).HasColumnType("decimal(12, 2)").HasColumnName("iva");
            entity.Property(e => e.MetodoPagoId).HasColumnName("metodoPagoId");
            entity.Property(e => e.Notas).HasColumnName("notas");
            entity.Property(e => e.NumeroVenta).HasMaxLength(50).HasColumnName("numeroVenta");
            entity.Property(e => e.PorcentajeIva).HasColumnType("decimal(5, 2)").HasColumnName("porcentajeIva");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)").HasColumnName("subtotal");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)").HasColumnName("total");
            entity.Property(e => e.TurnoId).HasColumnName("turnoId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Origen).HasMaxLength(20).HasDefaultValue("CAJA").HasColumnName("origen");
            entity.Property(e => e.PedidoId).HasColumnName("pedidoId");
            entity.Property(e => e.ReferenciasPago).HasMaxLength(255).HasColumnName("referenciasPago");
            entity.HasOne(d => d.Estado).WithMany(p => p.Venta).HasForeignKey(d => d.EstadoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Ventas_Estado");
            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Venta).HasForeignKey(d => d.MetodoPagoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Ventas_MetodoPago");
            entity.HasOne(d => d.Turno).WithMany(p => p.Venta).HasForeignKey(d => d.TurnoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Ventas_Turnos");
            entity.HasOne(d => d.Usuario).WithMany(p => p.Venta).HasForeignKey(d => d.UsuarioId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Ventas_Usuarios");
            entity.HasOne(d => d.Pedido).WithMany().HasForeignKey(d => d.PedidoId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Ventas_Pedidos");
        });

        modelBuilder.Entity<VentaDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__venta_de__3213E83FD3C3548D");
            entity.ToTable("venta_detalles");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Descuento).HasColumnType("decimal(12, 2)").HasColumnName("descuento");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)").HasColumnName("precioUnitario");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)").HasColumnName("subtotal");
            entity.Property(e => e.VentaId).HasColumnName("ventaId");
            entity.Property(e => e.LoteId).HasColumnName("loteId");
            entity.HasOne(d => d.Producto).WithMany(p => p.VentaDetalles).HasForeignKey(d => d.ProductoId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VentaDetalles_Productos");
            entity.HasOne(d => d.Venta).WithMany(p => p.VentaDetalles).HasForeignKey(d => d.VentaId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_VentaDetalles_Ventas");
            entity.HasOne(d => d.Lote).WithMany().HasForeignKey(d => d.LoteId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_VentaDetalles_Lotes");
        });

        modelBuilder.Entity<VentaDetalleServicio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__venta_detalles_servicios");
            entity.ToTable("venta_detalles_servicios");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.VentaId).HasColumnName("ventaId");
            entity.Property(e => e.ServicioId).HasColumnName("servicioId");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)").HasColumnName("precioUnitario");
            entity.Property(e => e.Descuento).HasColumnType("decimal(12, 2)").HasColumnName("descuento");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)").HasColumnName("subtotal");
            entity.Property(e => e.CitaId).HasColumnName("citaId");

            entity.HasOne(d => d.Venta)
                .WithMany(p => p.VentaDetallesServicios)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VentaDetalleServicios_Ventas");

            entity.HasOne(d => d.Servicio)
                .WithMany(p => p.VentaDetallesServicios)
                .HasForeignKey(d => d.ServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VentaDetalleServicios_Servicios");

            entity.HasOne(d => d.Cita)
                .WithMany()
                .HasForeignKey(d => d.CitaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_VentaDetalleServicios_Citas");
        });

        modelBuilder.Entity<ProductoMedicamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_producto_medicamento");
            entity.ToTable("producto_medicamento");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.Composicion).HasColumnName("composicion");
            entity.Property(e => e.Concentracion).HasColumnName("concentracion");
            entity.Property(e => e.ViaAdministracion).HasColumnName("viaAdministracion");
            entity.Property(e => e.RegistroSanitario).HasColumnName("registroSanitario");

            entity.Property(e => e.RequiereFormula)
                .HasColumnName("requiereFormula")
                .HasColumnType("bit");

            entity.HasOne(d => d.Producto)
                .WithOne(p => p.ProductoMedicamento)
                .HasForeignKey<ProductoMedicamento>(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductoMedicamento_Productos");
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_lotes");
            entity.ToTable("lotes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.CompraId).HasColumnName("compraId");
            entity.Property(e => e.NumeroLote).HasMaxLength(100).HasColumnName("numeroLote");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fechaVencimiento");
            entity.Property(e => e.CostoUnitario).HasColumnType("decimal(12, 2)").HasColumnName("costoUnitario");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaCreacion");

            entity.HasOne(d => d.Producto)
                .WithMany(p => p.Lotes)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Lotes_Productos");

            entity.HasOne(d => d.Compra)
                .WithMany()
                .HasForeignKey(d => d.CompraId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Lotes_Compras");
        });

        // --- DEVOLUCIONES ---

        modelBuilder.Entity<EstadoDevolucion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosDevoluciones");
            entity.ToTable("EstadosDevoluciones");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
            entity.Property(e => e.Activo).HasDefaultValue(true).HasColumnName("activo");
        });

        modelBuilder.Entity<Devolucion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Devoluciones");
            entity.ToTable("Devoluciones");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.VentaId).HasColumnName("ventaId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.EstadoId).HasDefaultValue(1).HasColumnName("estadoId");
            entity.Property(e => e.Motivo).HasMaxLength(255).HasColumnName("motivo");
            entity.Property(e => e.Observaciones).HasMaxLength(500).HasColumnName("observaciones");
            entity.Property(e => e.TotalDevolucion).HasColumnType("decimal(18, 2)").HasColumnName("totalDevolucion");
            entity.Property(e => e.FechaDevolucion).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("fechaDevolucion");
            entity.Property(e => e.FechaGestion).HasColumnType("datetime").HasColumnName("fechaGestion");
            entity.Property(e => e.UsuarioGestionId).HasColumnName("usuarioGestionId");

            entity.HasOne(d => d.Venta)
                .WithMany()
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Dev_Venta");

            entity.HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Dev_Usuario");

            entity.HasOne(d => d.Estado)
                .WithMany()
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Dev_Estado");
        });

        modelBuilder.Entity<DetalleDevolucion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DetallesDevoluciones");
            entity.ToTable("DetallesDevoluciones");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DevolucionId).HasColumnName("devolucionId");
            entity.Property(e => e.DetalleVentaId).HasColumnName("detalleVentaId");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.CantidadDevuelta).HasColumnName("cantidadDevuelta");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)").HasColumnName("precioUnitario");
            entity.Property(e => e.SubtotalDevuelto).HasColumnType("decimal(18, 2)").HasColumnName("subtotalDevuelto");

            entity.HasOne(d => d.Devolucion)
                .WithMany(d => d.Detalles)
                .HasForeignKey(d => d.DevolucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetDev_Devolucion");

            entity.HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetDev_Producto");

            entity.HasOne(d => d.DetalleVenta)
                .WithMany()
                .HasForeignKey(d => d.DetalleVentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetDev_DetalleVenta");
        });

        // ✅ Mapeo de Configuracion
        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("configuracion");
            entity.HasIndex(e => e.Clave).IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Clave).HasMaxLength(100).HasColumnName("clave");
            entity.Property(e => e.Valor).HasMaxLength(500).HasColumnName("valor");
            entity.Property(e => e.Descripcion).HasMaxLength(300).HasColumnName("descripcion");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
        });

        modelBuilder.Entity<Devolucion>(entity =>
        {
            entity.ToTable("Devoluciones");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.VentaId).HasColumnName("ventaId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.EstadoId).HasColumnName("estadoId");
            entity.Property(e => e.Motivo).HasMaxLength(500).HasColumnName("motivo");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.TotalDevolucion).HasColumnType("decimal(12,2)").HasColumnName("totalDevolucion");
            entity.Property(e => e.FechaDevolucion).HasColumnType("datetime").HasColumnName("fechaDevolucion");
            entity.Property(e => e.FechaGestion).HasColumnType("datetime").HasColumnName("fechaGestion");
            entity.Property(e => e.UsuarioGestionId).HasColumnName("usuarioGestionId");
            entity.HasOne(d => d.Venta).WithMany().HasForeignKey(d => d.VentaId).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.Usuario).WithMany().HasForeignKey(d => d.UsuarioId).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.Estado).WithMany(e => e.Devoluciones).HasForeignKey(d => d.EstadoId).OnDelete(DeleteBehavior.ClientSetNull);
        });
        modelBuilder.Entity<DetalleDevolucion>(entity =>
        {
            entity.ToTable("DetallesDevoluciones");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DevolucionId).HasColumnName("devolucionId");
            entity.Property(e => e.DetalleVentaId).HasColumnName("detalleVentaId");
            entity.Property(e => e.ProductoId).HasColumnName("productoId");
            entity.Property(e => e.CantidadDevuelta).HasColumnName("cantidadDevuelta");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12,2)").HasColumnName("precioUnitario");
            entity.Property(e => e.SubtotalDevuelto).HasColumnType("decimal(12,2)").HasColumnName("subtotalDevuelto");
            entity.HasOne(d => d.Devolucion).WithMany(d => d.Detalles).HasForeignKey(d => d.DevolucionId).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.Producto).WithMany().HasForeignKey(d => d.ProductoId).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.DetalleVenta).WithMany().HasForeignKey(d => d.DetalleVentaId).OnDelete(DeleteBehavior.ClientSetNull);
        });
        modelBuilder.Entity<EstadoDevolucion>(entity =>
        {
            entity.ToTable("EstadosDevoluciones");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
            entity.Property(e => e.Activo).HasColumnName("activo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
