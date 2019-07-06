/* Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/Oracle.NET/blob/master/LICENSE
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Data;
using Oracle.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;

namespace OracleEFCore
{
    class Program
    {
    
        // Using Stored Procedures in Oracle EF Core Sample Code
        // This sample code shows how to bind a parameter, return a result set, and call a stored
        //procedure using Oracle EF Core. Calling a stored procedure requires using the FromSQL
        //extension method and using anonymous PL/SQL.
        // Use the .sql file to create the stored procedure in the schema.
    
        public class BloggingContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // Add your User Id, Password, and Data Source
                optionsBuilder.UseOracle(@"User Id=<USER ID>;Password=<PASSWORD>;Data Source=<DATA SOURCE>;");
            }
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
            public List<Post> Posts { get; set; }
        }

        public class Post
        {
            public int PostId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public int BlogId { get; set; }
            public Blog Blog { get; set; }
        }
        
        static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                var blog = new Blog { Url = "https://blogs.oracle.com" };
                db.Blogs.Add(blog);
                db.SaveChanges();
            }

            // Demonstrate returning implicit REF Cursor from PL/SQL
            using (var db = new BloggingContext())
            {
                // Use anonymous PL/SQL to call stored procedure and return result set
                var blogs = db.Blogs
                    .FromSql("BEGIN GETALLBLOGS_IMPLICIT(); END;")
                    .ToList()
                    .OrderBy(Blog => Blog.BlogId);
            }

            // Demonstrate returning explicitly bound REF Cursor from PL/SQL
            using (var db = new BloggingContext())
            {
                // Create REF Cursor output parameter
                var allblogs = new OracleParameter("blogparam", OracleDbType.RefCursor, ParameterDirection.Output);

                // Use anonymous PL/SQL to call stored procedure, bind output parameter, and return result set
                var blogs = db.Blogs
                    .FromSql("BEGIN GETALLBLOGS(:blogparam); END;", new object[] { allblogs })
                    .ToList()
                    .OrderBy(Blog => Blog.BlogId);
            }
        }
    }
}
