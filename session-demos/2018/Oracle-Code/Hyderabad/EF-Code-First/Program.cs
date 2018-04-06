using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.History;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCodeFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<OracleDbContext>());

            using (var ctx = new OracleDbContext())
            {
                var emp = new Employee
                {
                    Name = "Alex",
                    HireDate = DateTime.Now
                };

                ctx.Employees.Add(emp);
                ctx.SaveChanges();

                var dept = new Department
                {
                    Name = "Product Management",
                    ManagerId = emp.EmployeeId
                };

                ctx.Departments.Add(dept);
                ctx.SaveChanges();
            }

            Console.Write("Schema objects created and data entered.");
            Console.ReadKey(true);
        }
    }

    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public DateTime HireDate { get; set; }
        //public string Location { get; set; }
    }

    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        [ForeignKey("Manager")]
        public int ManagerId { get; set; }
        public Employee Manager { get; set; }
    }

    public class OracleDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("HR");
        }
    }
}

