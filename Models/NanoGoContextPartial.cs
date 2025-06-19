using System;
using NanoGo.Models.Procedure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace NanoGo.Models
{
    public partial class NanoGoContext : DbContext
    {
        public virtual DbSet<GetNextCounterValueReturnValue> GetNextCounterValueReturnValueDbSet { get; set; }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GetNextCounterValueReturnValue>(entity =>
            {
                entity.HasNoKey();
            });
        }
    }
}
