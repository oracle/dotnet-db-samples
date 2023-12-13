//This sample shows how to use EF Core JSON columns with ODP.NET and Oracle Database.
//It creates an owned entity, inserts, queries, updates, and deletes JSON column data.
//It requires Oracle EF Core 8 or higher. Oracle Database 21c and higher supports JSON columns.
//Earlier database versions map aggregate types to NCLOB columns instead of JSON columns.

//Specify the user, password, and data source in the connection string below.

using Microsoft.EntityFrameworkCore;

namespace ODPJsonColumns
{
    class Program
    {
        public class ContactDetails
        {
            public Address Address { get; set; } = null!;
            public string? Phone { get; set; }
        }
    
        public class Address
        {
            public Address(string street, string city, string postcode, string country)
            {
                Street = street;
                City = city;
                Postcode = postcode;
                Country = country;
            }
      
            public string Street { get; set; }
            public string City { get; set; }
            public string Postcode { get; set; }
            public string Country { get; set; }
        }
    
        public class Author
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public ContactDetails? Contact { get; set; }
        }

        public class AuthorContext : DbContext
        {
            public DbSet<Author> Authors { get; set; }

            //To use Oracle database JSON columns, connect to Oracle Database 21c or higher version and
            // specify OracleSQLCompatibility to DatabaseVersion21 or higher.
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseOracle("User Id=<USER>; Password=<PASSWORD>; Data Source=<DATA SOURCE>"
                    , b => b.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion21)); ;
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Configuring the Contact column as a JSON column
                modelBuilder.Entity<Author>().OwnsOne(
                    author => author.Contact, ownedNavigationBuilder =>
                    {
                        ownedNavigationBuilder.ToJson();
                        ownedNavigationBuilder.OwnsOne(contactDetails => contactDetails.Address);
                    });
            }
        }

        public static void Main()
        {
            using (var context = new AuthorContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
          
                var author1 = new Author()
                {
                    Name = "John Smith",
                    Contact = new ContactDetails()
                    {
                    Phone = "555 123 4567",
                    Address = new Address("1 Any Street", "Austin", "78741", "US")
                    }
                };
                
                var author2 = new Author()
                {
                    Name = "Kim Jones",
                    Contact = new ContactDetails()
                    {
                        Phone = "555 234 5678",
                        Address = new Address("2 Any Street", "Redwood Shores", "94065", "US")
                    }
                };

                /*
                * Contact inserted into the JSON column as the following example:
                * 
                * {
                *    "Phone": "555 123 4567",
                *    "Address": {
                *      "City": "Austin",
                *      "Country": "US",
                *      "Postcode": "78741",
                *      "Street": "1 Any Street"
                *    }
                *  }
                */

                // Insert data
                Console.WriteLine("Inserting authors into table\n");
                context.Authors.Add(author1);
                context.Authors.Add(author2);
                context.SaveChanges();

                // Query to verify insert
                Console.WriteLine("Authors in table: ");
                var result = context.Authors.ToList();
                foreach (var author in result)
                {
                    Console.WriteLine($"{author.Name}");
                }
                Console.WriteLine();
                
                // Query using owned entity details from JSON column
                var result1 = context.Authors.Where(author => author.Contact.Address.City == "Austin").First();
                Console.WriteLine($"Author residing in Austin: {result1.Name}\n");
                
                // Update second author's phone number details
                Console.WriteLine("Updating second author's phone number");
                author2.Contact.Phone = "123 456 7890";
                context.SaveChanges();
          
                // Query to verify update
                var result2 = context.Authors.Where(author => author.Name == "Kim Jones").First();
                Console.WriteLine($"Retrieve author's new phone number {result2.Name}: {result2.Contact.Phone}\n");

                // Delete second author
                Console.WriteLine("Deleting author with street address '2 Any Street' from table");
                context.Remove(context.Authors.Single(author => author.Contact.Address.Street == "2 Any Street"));
                context.SaveChanges();
                Console.WriteLine("Delete complete.\n");

                // Query to verify delete
                Console.WriteLine("Authors remaining in table: ");
                var result3 = context.Authors.ToList();
                foreach(var author in result3)
                {
                    Console.WriteLine($"{author.Name}");
                }
            }
        }
    }
}

/* Copyright (c) 2023, Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/dotnet-db-samples/blob/master/LICENSE.txt
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/
