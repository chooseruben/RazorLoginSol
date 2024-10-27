using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.test;

public partial class tempcontext : DbContext
{
    public tempcontext()
    {
    }

    public tempcontext(DbContextOptions<tempcontext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:zoodotnet.database.windows.net,1433;Initial Catalog=zooDB;Persist Security Info=False;User ID=login;Password=Zoo1234!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK_Employee_Employee_ID");

            entity.ToTable("Employee", tb => tb.HasTrigger("check_employee_over_18"));

            entity.HasIndex(e => e.EmployeeId, "Employee$Employee_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Ssn, "Employee$SSN_UNIQUE").IsUnique();

            entity.HasIndex(e => e.EmployeeEmail, "EmployeeEmailUNIQUE").IsUnique();

            entity.HasIndex(e => e.SupervisorId, "Employee_ibfk_1");

            entity.HasIndex(e => e.ShopId, "Employee_ibfk_2");

            entity.HasIndex(e => e.FoodStoreId, "Employee_ibfk_3");

            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.DateOfEmployment)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Date_of_employment");
            entity.Property(e => e.Degree)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.EmployeeAddress)
                .HasMaxLength(120)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_Address");
            entity.Property(e => e.EmployeeDob)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_DOB");
            entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(45)
                .IsFixedLength()
                .HasColumnName("Employee_Email");
            entity.Property(e => e.EmployeeFirstName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_First_name");
            entity.Property(e => e.EmployeeLastName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_Last_name");
            entity.Property(e => e.EmployeePhoneNumber)
                .HasMaxLength(10)
                .HasColumnName("Employee_Phone_number");
            entity.Property(e => e.EmployeeSalary)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_Salary");
            entity.Property(e => e.FoodStoreId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Food_store_ID");
            entity.Property(e => e.ShopId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Shop_ID");
            entity.Property(e => e.Ssn).HasColumnName("SSN");
            entity.Property(e => e.SupervisorId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Supervisor_ID");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK_Event_Event_ID");

            entity.ToTable("Event");

            entity.HasIndex(e => e.EventId, "Event$Event_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.EventEmployeeRepId, "Event_ibfk_1");

            entity.Property(e => e.EventId)
                .ValueGeneratedNever()
                .HasColumnName("Event_ID");
            entity.Property(e => e.EventDate).HasColumnName("Event_date");
            entity.Property(e => e.EventEmployeeRepId).HasColumnName("Event_employee_rep_ID");
            entity.Property(e => e.EventEndTime).HasColumnName("Event_end_time");
            entity.Property(e => e.EventLocation)
                .HasMaxLength(70)
                .IsFixedLength()
                .HasColumnName("Event_Location");
            entity.Property(e => e.EventName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Event_name");
            entity.Property(e => e.EventStartTime).HasColumnName("Event_start_time");

            entity.HasOne(d => d.EventEmployeeRep).WithMany(p => p.Events)
                .HasForeignKey(d => d.EventEmployeeRepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event$Event_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
