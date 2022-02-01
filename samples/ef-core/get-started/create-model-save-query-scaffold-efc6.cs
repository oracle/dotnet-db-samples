using Microsoft.EntityFrameworkCore;

namespace OracleEFCore6
{
    class Program
    {
        //This sample code demonstrates how to get started with Oracle Entity Framework Core 6 
        //It can connecto to on-premises Oracle DB or walletless Oracle Autonomous DB
        
        public class BloggingContext : DbContext
        {
            public DbSet<Blog>? Blogs { get; set; }
            public DbSet<Post>? Posts { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseOracle(@"User Id=blog;Password=<Password>;Data Source=<Net Service Name>");
            }
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string? Url { get; set; }
            //public int? Rating { get; set; }
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

            using (var db = new BloggingContext())
            {
                var blog = new Blog { Url = "https://blogs.oracle.com" };
                //var blog = new Blog { Url = "https://blogs.oracle.com", Rating = 10 };
                db.Blogs!.Add(blog);
                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                var blogs = db.Blogs;
                foreach (var item in blogs!)
                {
                    Console.WriteLine(item.Url);
                    //Console.WriteLine(item.Url + " has rating " + item.Rating );
                }
            }
            Console.ReadLine();
        }
    }
}


/* Copyright (c) 2022, Oracle and/or its affiliates. All rights reserved. */

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
