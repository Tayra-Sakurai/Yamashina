// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takatsuki.Models;
using Windows.Storage;

namespace Takatsuki.Contexts
{
    public class TakatsukiContext : DbContext
    {
        public DbSet<BalanceSheet> BalanceSheet { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public string DbPath { get; private init; }

        public TakatsukiContext ()
        {
            string folderPath = ApplicationData.Current.LocalFolder.Path;
            DbPath = Path.Combine(folderPath, "Takatsuki.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                $"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BalanceSheet>(
                t =>
                {
                    t.ToTable("BalanceSheet");
                    t.HasKey(e => e.Id);
                    t.HasOne(e => e.Method)
                    .WithMany(e => e.Entries)
                    .HasForeignKey(e => e.MethodId)
                    .IsRequired();
                    t.Property(e => e.DateTime)
                    .IsRequired();
                    t.Property(e => e.ItemName)
                    .HasDefaultValue("")
                    .IsRequired();
                }
            );
            modelBuilder.Entity<PaymentMethod>()
                .HasKey(e => e.Id);
        }
    }
}
