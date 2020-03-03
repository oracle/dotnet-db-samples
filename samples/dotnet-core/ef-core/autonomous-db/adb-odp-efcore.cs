/* Copyright (c) 2020, Oracle and/or its affiliates. All rights reserved. */

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
using Microsoft.EntityFrameworkCore;

// ODP.NET namespace added to access OracleConfiguration class
using Oracle.ManagedDataAccess.Client;

namespace OracleEFCore_ADB
{
     // This sample code demonstrates using ODP.NET EF Core with an Oracle Autonomous Database
    class Program
    {
        public class BloggingContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // Set TnsAdmin value to directory location of tnsnames.ora and sqlnet.ora files
                OracleConfiguration.TnsAdmin = @"<DIRECTORY LOCATION>";

                // Set WalletLocation value to directory location of the ADB wallet (i.e. cwallet.sso)
                OracleConfiguration.WalletLocation = @"<DIRECTORY LOCATION>";

                // Configure ODP.NET connection string
                optionsBuilder.UseOracle(@"User Id=<USER>;Password=<PASSWORD>;Data Source=<TNS NAME>") ;
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

            using (var db = new BloggingContext())
            {
                var blogs = db.Blogs;
            }
        }
    }
}
