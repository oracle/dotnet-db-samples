using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OracleEFCore
{
    class Program
    {

        public class BloggingContext : DbContext
        {
            public DbSet<Blog>? Blogs { get; set; }
            public DbSet<Post>? Posts { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                //Enter user id, passowrd, and data source info
                optionsBuilder.UseOracle(@"User Id=<USER>;Password=<PASSWORD>;Data Source=<DATA SOURCE>");
            }
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string? Url { get; set; }
            public int Rating { get; set; }
            public List<Post>? Posts { get; set; }
        }

        public class Post
        {
            public int PostId { get; set; }
            public string? Title { get; set; }
            public string? Content { get; set; }

            public int BlogId { get; set; }
            public Blog? Blog { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("This app is using .NET version: {0}", Environment.Version.ToString());
            Console.WriteLine();

            using (var db = new BloggingContext())
            {
                var blog = new Blog { Url = "https://blogs.oracle.com" };
                db.Blogs!.Add(blog);
                db.SaveChanges();
            }

            Console.WriteLine("Here are the Blog URLs in the database:");
            using (var db = new BloggingContext())
            {
                var blogs = db.Blogs;
                foreach (var item in blogs!)
                {
                    Console.WriteLine(item.Url);
                }
            }
        }
    }
}

/* Copyright (c) 2021, Oracle and/or its affiliates. All rights reserved. */

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
