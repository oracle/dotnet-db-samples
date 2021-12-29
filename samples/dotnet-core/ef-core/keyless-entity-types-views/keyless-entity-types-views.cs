using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Oracle_KET_Views
{
    public class Program
    {
        private static void Main()
        {
            SetupDatabase();

            using (var db = new BloggingContext())
            {
                #region Query
                var postCounts = db.BlogPostCounts.ToList();

                Console.WriteLine("View Results:");
                foreach (var postCount in postCounts)
                {
                    Console.WriteLine($"{postCount.BlogName} has {postCount.PostCount} posts.");
                }

                Console.WriteLine();
                Console.WriteLine("Materialized View Results:");
                var mvPostCounts = db.MVBlogPostCounts.ToList();

                foreach (var mvPostCount in mvPostCounts)
                {
                    Console.WriteLine($"{mvPostCount.BlogName} has {mvPostCount.PostCount} posts.");
                }

                #endregion
            }
        }

        private static void SetupDatabase()
        {
            using (var db = new BloggingContext())
            {
                if (db.Database.EnsureCreated())
                {
                    db.Blogs.Add(
                        new Blog
                        {
                            Name = "Fish Blog",
                            Url = "http://sample.com/blogs/fish",
                            Posts = new List<Post>
                            {
                                new Post { Title = "Fish care 101" },
                                new Post { Title = "Caring for tropical fish" },
                                new Post { Title = "Types of ornamental fish" }
                            }
                        });

                    db.Blogs.Add(
                        new Blog
                        {
                            Name = "Cats Blog",
                            Url = "http://sample.com/blogs/cats",
                            Posts = new List<Post>
                            {
                                new Post { Title = "Cat care 101" },
                                new Post { Title = "Caring for tropical cats" },
                                new Post { Title = "Types of ornamental cats" }
                            }
                        });

                    db.Blogs.Add(
                        new Blog
                        {
                            Name = "Catfish Blog",
                            Url = "http://sample.com/blogs/catfish",
                            Posts = new List<Post>
                            {
                                new Post { Title = "Catfish care 101" },
                                new Post { Title = "History of the catfish name" }
                            }
                        });

                    db.SaveChanges();

                    #region View
                    db.Database.ExecuteSqlRaw(
                        "CREATE VIEW \"View_BlogPostCounts\" AS " +
                        "SELECT b.\"Name\", Count(p.\"PostId\") as \"PostCount\" " +
                        "FROM \"Blogs\" b " +
                        "JOIN \"Posts\" p on p.\"BlogId\" = b.\"BlogId\" " +
                        "GROUP BY b.\"Name\";");
                    #endregion

                    #region MaterializedView
                    db.Database.ExecuteSqlRaw(
                        "CREATE MATERIALIZED VIEW \"MView_BlogPostCounts\" " +
                        "BUILD IMMEDIATE REFRESH FORCE ON DEMAND AS " +
                        "SELECT b.\"Name\", Count(p.\"PostId\") as \"PostCount\" " +
                        "FROM \"Blogs\" b " +
                        "JOIN \"Posts\" p on p.\"BlogId\" = b.\"BlogId\" " +
                        "GROUP BY b.\"Name\";");
                    #endregion
                }
            }
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        #region DbSet
        public DbSet<BlogPostsCount> BlogPostCounts { get; set; }
        public DbSet<BlogPostsCount> MVBlogPostCounts { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Enter User Id, Password, and Data Source values
            optionsBuilder
                .UseOracle(
                    @"User Id=<USER ID>;Password=<PASSWORD>;Data Source=<DATA SOURCE>");
        }

        #region Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<BlogPostsCount>(eb =>
                {
                    eb.HasNoKey();
                    eb.ToView("View_BlogPostCounts");
                    eb.Property(v => v.BlogName).HasColumnName("Name");
                })
                .Entity<MVBlogPostsCount>(eb =>
                {
                    eb.HasNoKey();
                    eb.ToView("MView_BlogPostCounts");
                    eb.Property(v => v.BlogName).HasColumnName("Name");
                });
        }
        #endregion
    }

    #region Entities
    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
    }
    #endregion

    #region KeylessEntityType
    public class BlogPostsCount
    {
        public string BlogName { get; set; }
        public int PostCount { get; set; }
    }
    public class MVBlogPostsCount
    {
        public string BlogName { get; set; }
        public int PostCount { get; set; }
    }
    #endregion
}

/* Copyright (c) 2020, Oracle and/or its affiliates. All rights reserved. */
/* Copyright (c) .NET Foundation                                          */

/******************************************************************************
 *   Licensed under the Apache License, Version 2.0 (the "License");
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *   limitations under the License.
 * 
 *****************************************************************************/
