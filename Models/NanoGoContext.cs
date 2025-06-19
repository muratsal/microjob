using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace NanoGo.Models
{
    public partial class NanoGoContext : DbContext
    {
        public NanoGoContext()
        {
        }

        public NanoGoContext(DbContextOptions<NanoGoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Auth> Auths { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<EmailAttachment> EmailAttachments { get; set; }
        public virtual DbSet<EmailConfig> EmailConfigs { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Parameter> Parameters { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionLog> PermissionLogs { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleAuth> RoleAuths { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<VEmployee> VEmployees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=192.168.1.12\\SQLEXPRESS;Database=NanoGo;User Id=sa;Password=asrra1907;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auth>(entity =>
            {
                entity.Property(e => e.AuthCode).HasMaxLength(150);

                entity.Property(e => e.AuthDesc).HasMaxLength(150);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.AuthCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Auths_Users");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.AuthUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Auths_Users1");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(550);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(550);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(70);

                entity.Property(e => e.Logo).HasMaxLength(550);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.TaxAuthority).HasMaxLength(150);

                entity.Property(e => e.TaxNumber).HasMaxLength(20);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Website).HasMaxLength(150);

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.CompanyCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Companies_Users");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.CompanyUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Companies_Users1");
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.Property(e => e.CounterName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Prefix)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.CounterCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Counters_Users");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.CounterUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Counters_Users1");
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailFrom)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.EmailTo).HasMaxLength(1500);

                entity.Property(e => e.EmailToBcc)
                    .HasMaxLength(1500)
                    .HasColumnName("EmailToBCC");

                entity.Property(e => e.EmailToCc)
                    .HasMaxLength(1500)
                    .HasColumnName("EmailToCC");

                entity.Property(e => e.IsHtml).HasColumnName("IsHTML");

                entity.Property(e => e.SendDate).HasColumnType("datetime");

                entity.Property(e => e.Subject).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.EmailCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Emails_Users");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.EmailUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Emails_Users1");
            });

            modelBuilder.Entity<EmailAttachment>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FilePath).HasMaxLength(550);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.EmailAttachmentCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailAttachments_Users");

                entity.HasOne(d => d.Email)
                    .WithMany(p => p.EmailAttachments)
                    .HasForeignKey(d => d.EmailId)
                    .HasConstraintName("FK_EmailAttachments_Emails");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.EmailAttachmentUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailAttachments_Users1");
            });

            modelBuilder.Entity<EmailConfig>(entity =>
            {
                entity.Property(e => e.ConfigName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EnableSsl).HasColumnName("EnableSSL");

                entity.Property(e => e.Host).HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Port).HasMaxLength(10);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(150);

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.EmailConfigCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailConfigs_Users");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.EmailConfigUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailConfigs_Users1");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress).HasMaxLength(150);

                entity.Property(e => e.EndWorkDate).HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.GsmNumber).HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.IntegrationCode).HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WorkStartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Companies");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.EmployeeCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Users");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.EmployeeDepartments)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Parameters1");

                entity.HasOne(d => d.ResponsibleForNavigation)
                    .WithMany(p => p.InverseResponsibleForNavigation)
                    .HasForeignKey(d => d.ResponsibleFor)
                    .HasConstraintName("FK_Employees_Employees");

                entity.HasOne(d => d.Title)
                    .WithMany(p => p.EmployeeTitles)
                    .HasForeignKey(d => d.TitleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Parameters");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.EmployeeUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Users1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.EmployeeUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Employees_Users2");
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1500);

                entity.Property(e => e.FileOriginalName)
                    .IsRequired()
                    .HasMaxLength(1500);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.FileCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Files_Users");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.FileUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Files_Users1");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.InventoryDate).HasColumnType("datetime");

                entity.Property(e => e.InventoryName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Model).HasMaxLength(70);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.Picture).HasMaxLength(500);

                entity.Property(e => e.SerieNo).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Inventories_Companies");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.InventoryCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Inventories_Users");

                entity.HasOne(d => d.InventoryTypeNavigation)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(d => d.InventoryType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Inventories_Parameters");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.InventoryUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Inventories_Users1");
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(e => e.InventoryTransId);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.InventoryTransactionCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryTransactions_Users");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.InventoryTransactions)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_InventoryTransactions_Employees");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.InventoryTransactions)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryTransactions_Inventories");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.InventoryTransactionUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryTransactions_Users1");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FreeText1).HasMaxLength(50);

                entity.Property(e => e.FreeText2).HasMaxLength(50);

                entity.Property(e => e.FreeText3).HasMaxLength(50);

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.LogCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Logs_Users");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.LogUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Logs_Users1");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MenuIcon).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.State).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.MenuCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .HasConstraintName("FK_Menus_Menus");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.MenuUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .HasConstraintName("FK_Menus_Users");
            });

            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.HasKey(e => e.ParamId);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FreeText1).HasMaxLength(50);

                entity.Property(e => e.FreeText2).HasMaxLength(50);

                entity.Property(e => e.FreeText3).HasMaxLength(50);

                entity.Property(e => e.ParamCode).HasMaxLength(150);

                entity.Property(e => e.ParamDesc).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.ParameterCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Parameters_Users");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.ParameterUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Parameters_Users1");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Duties).HasMaxLength(500);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ApprovedWaitingEmployeeNavigation)
                    .WithMany(p => p.PermissionApprovedWaitingEmployeeNavigations)
                    .HasForeignKey(d => d.ApprovedWaitingEmployee)
                    .HasConstraintName("FK_Permissions_Employees1");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.PermissionCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permissions_Users");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.PermissionEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permissions_Permissions");

                entity.HasOne(d => d.PermissionTypeNavigation)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.PermissionType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permissions_Parameters");

                entity.HasOne(d => d.ProxyEmployeeNavigation)
                    .WithMany(p => p.PermissionProxyEmployeeNavigations)
                    .HasForeignKey(d => d.ProxyEmployee)
                    .HasConstraintName("FK_Permissions_Employees");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.PermissionUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permissions_Users1");
            });

            modelBuilder.Entity<PermissionLog>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.PermissionLogCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionLogs_Users");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.PermissionLogs)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionLogs_Employees");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionLogs)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionLogs_Permissions");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.PermissionLogUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionLogs_Users1");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.RoleDesc).HasMaxLength(150);

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.RoleCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Roles_Users");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.RoleUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Roles_Users1");
            });

            modelBuilder.Entity<RoleAuth>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Auth)
                    .WithMany(p => p.RoleAuths)
                    .HasForeignKey(d => d.AuthId)
                    .HasConstraintName("FK_RoleAuths_Auths");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.RoleAuthCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleAuths_Users");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleAuths)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_RoleAuths_Roles");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.RoleAuthUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleAuths_Users1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(70);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.LastLoginIp).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(70);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.ProfileImage).HasMaxLength(350);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserType).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.UserRoleCreatedUserNavigations)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Users");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Roles");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.UserRoleUpdatedUserNavigations)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Users1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoleUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Users2");
            });

            modelBuilder.Entity<VEmployee>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vEmployees");

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.CompanyName).HasMaxLength(550);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Department).HasMaxLength(150);

                entity.Property(e => e.EmailAddress).HasMaxLength(150);

                entity.Property(e => e.EndWorkDate).HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.GsmNumber).HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.IntegrationCode).HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.ResponsibleForName).HasMaxLength(250);

                entity.Property(e => e.Title).HasMaxLength(150);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.Property(e => e.WorkStartDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
