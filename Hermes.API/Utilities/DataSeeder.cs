using Hermes.Domain.Entities;
using Hermes.Domain.Enums;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.API.Utilities;

public class DataSeeder(HermesDbContext context)
{
    public async Task SeedAsync()
    {
        // 1. Seed Roles
        if (!await context.Roles.AnyAsync())
        {
            await context.Roles.AddRangeAsync(
                new Role { Name = "Admin" },
                new Role { Name = "User" },
                new Role { Name = "Seller" }
            );
            await context.SaveChangesAsync();
        }

        // 2. Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var electronics = new Category { Name = "Electronics", Description = "All things electronic" };
            var fashion = new Category { Name = "Fashion", Description = "Clothing and accessories" };
            var home = new Category { Name = "Home", Description = "Furniture, decor, and appliances" };
            var books = new Category { Name = "Books", Description = "Fiction, non-fiction, and more" };

            await context.Categories.AddRangeAsync(electronics, fashion, home, books);
            await context.SaveChangesAsync();

            // Seed Subcategories
            var smartphones = new Category
            {
                Name = "Smartphones",
                Description = "Latest smartphones from top brands",
                ParentCategoryId = electronics.Id
            };

            var laptops = new Category
            {
                Name = "Laptops",
                Description = "High-performance laptops for work and play",
                ParentCategoryId = electronics.Id
            };

            var womenswear = new Category
            {
                Name = "Women's Wear",
                Description = "Trendy and stylish clothing for women",
                ParentCategoryId = fashion.Id
            };

            var menswear = new Category
            {
                Name = "Men's Wear",
                Description = "Classic and modern apparel for men",
                ParentCategoryId = fashion.Id
            };

            var furniture = new Category
            {
                Name = "Furniture",
                Description = "High-quality furniture for your home",
                ParentCategoryId = home.Id
            };

            var decor = new Category
            {
                Name = "Decor",
                Description = "Decorative items for your home",
                ParentCategoryId = home.Id
            };

            await context.Categories.AddRangeAsync(smartphones, laptops, womenswear, menswear, furniture, decor);
            await context.SaveChangesAsync();
        }

        // 3. Seed Users
        if (!await context.Users.AnyAsync())
        {
            var admin = new User
            {
                Username = "admin",
                Email = "admin@hermes.com",
                PasswordHash = "5v/JL+P4JDF4HXco6t2O7rKYMUSgJVlXCRDqFuJV20o=",
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "1234567890",
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "Anytown",
                    State = "CA",
                    PostalCode = "12345",
                    Country = "US"
                },
                Role = "Admin",
                PasswordResetToken = "1234567890"
            };

            var user1 = new User
            {
                Username = "user1",
                Email = "user1@hermes.com",
                PasswordHash = "5v/JL+P4JDF4HXco6t2O7rKYMUSgJVlXCRDqFuJV20o=",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "9876543210",
                Address = new Address
                {
                    Street = "456 Elm St",
                    City = "Springfield",
                    State = "IL",
                    PostalCode = "62701",
                    Country = "US"
                },
                Role = "User",
                PasswordResetToken = "1234567890"

            };

            var user2 = new User
            {
                Username = "user2",
                Email = "user2@hermes.com",
                PasswordHash = "5v/JL+P4JDF4HXco6t2O7rKYMUSgJVlXCRDqFuJV20o=",
                FirstName = "Jane",
                LastName = "Smith",
                PhoneNumber = "5551234567",
                Address = new Address
                {
                    Street = "789 Oak Ave",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                    Country = "US"
                },
                Role = "User",
                PasswordResetToken = "1234567890"
            };

            var seller1 = new User
            {
                Username = "seller1",
                Email = "seller1@hermes.com",
                PasswordHash = "5v/JL+P4JDF4HXco6t2O7rKYMUSgJVlXCRDqFuJV20o=",
                FirstName = "Bob",
                LastName = "Johnson",
                PhoneNumber = "4445556667",
                Address = new Address
                {
                    Street = "1011 Pine St",
                    City = "Los Angeles",
                    State = "CA",
                    PostalCode = "90001",
                    Country = "US"
                },
                Role = "Seller",
                Rating = 4,
                PasswordResetToken = "1234567890"
            };

            var seller2 = new User
            {
                Username = "seller2",
                Email = "seller2@hermes.com",
                PasswordHash = "5v/JL+P4JDF4HXco6t2O7rKYMUSgJVlXCRDqFuJV20o=",
                FirstName = "Alice",
                LastName = "Williams",
                PhoneNumber = "3334445556",
                Address = new Address
                {
                    Street = "1213 Maple Dr",
                    City = "Chicago",
                    State = "IL",
                    PostalCode = "60601",
                    Country = "US"
                },
                Role = "Seller",
                Rating = 5,
                PasswordResetToken = "1234567890"
            };

            await context.Users.AddRangeAsync(admin, user1, user2, seller1, seller2);
            await context.SaveChangesAsync();
        }

        // 4. Seed Products
        if (!await context.Products.AnyAsync())
        {
            // Electronics
            var iphone14Pro = new Product
            {
                Name = "iPhone 14 Pro",
                Description = "Apple's latest flagship smartphone with a powerful A16 Bionic chip.",
                Price = 999,
                ImageUrl = "https://www.apple.com/v/iphone/home/images/hero/iphone_14_pro__b4r3f67v6s9g_large.jpg",
                Weight = 0.22,
                WeightUnit = "kg",
                Height = 0.15,
                HeightUnit = "cm",
                Width = 0.074,
                WidthUnit = "cm",
                Length = 0.078,
                LengthUnit = "cm",
                Tags = new List<string> { "Apple", "Smartphone", "Pro", "Camera", "A16 Bionic" },
                HostedAt = HostedAt.Store,
                CategoryId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "Smartphones"))!.Id,
                SellerId = (await context.Users.FirstOrDefaultAsync(u => u.Username == "seller1"))!.Id
            };

            var macbookPro = new Product
            {
                Name = "MacBook Pro",
                Description = "Apple's most powerful laptop with an M2 Pro chip and a stunning Retina display.",
                Price = 1999,
                ImageUrl = "https://www.apple.com/v/macbook-pro/home/images/hero/macbook_pro_14_inch__fm00w7k2p29k_large.jpg",
                Weight = 1.6,
                WeightUnit = "kg",
                Height = 1.6,
                HeightUnit = "cm",
                Width = 31.2,
                WidthUnit = "cm",
                Length = 22.1,
                LengthUnit = "cm",
                Tags = new List<string> { "Apple", "Laptop", "Pro", "M2 Pro", "Retina" },
                HostedAt = HostedAt.Store,
                CategoryId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "Laptops"))!.Id,
                SellerId = (await context.Users.FirstOrDefaultAsync(u => u.Username == "seller2"))!.Id
            };

            // Fashion
            var womensDress = new Product
            {
                Name = "Summer Floral Dress",
                Description = "A flowy and stylish floral dress for summer.",
                Price = 49,
                ImageUrl = "https://www.example.com/womens-dress.jpg",
                Weight = 0.5,
                WeightUnit = "kg",
                Height = 1.2,
                HeightUnit = "cm",
                Width = 0.8,
                WidthUnit = "cm",
                Length = 0.6,
                LengthUnit = "cm",
                Tags = new List<string> { "Women", "Dress", "Summer", "Floral", "Fashion" },
                HostedAt = HostedAt.Store,
                CategoryId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "Women's Wear"))!.Id,
                SellerId = (await context.Users.FirstOrDefaultAsync(u => u.Username == "seller1"))!.Id
            };

            var mensShirt = new Product
            {
                Name = "Classic Cotton Shirt",
                Description = "A comfortable and versatile cotton shirt for everyday wear.",
                Price = 29,
                ImageUrl = "https://www.example.com/mens-shirt.jpg",
                Weight = 0.3,
                WeightUnit = "kg",
                Height = 0.7,
                HeightUnit = "cm",
                Width = 0.5,
                WidthUnit = "cm",
                Length = 0.6,
                LengthUnit = "cm",
                Tags = new List<string> { "Men", "Shirt", "Cotton", "Classic", "Fashion" },
                HostedAt = HostedAt.Store,
                CategoryId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "Men's Wear"))!.Id,
                SellerId = (await context.Users.FirstOrDefaultAsync(u => u.Username == "seller2"))!.Id
            };

            // Home
            var sofa = new Product
            {
                Name = "Modern Leather Sofa",
                Description = "A stylish and comfortable leather sofa for your living room.",
                Price = 1299,
                ImageUrl = "https://www.example.com/leather-sofa.jpg",
                Weight = 80,
                WeightUnit = "kg",
                Height = 0.9,
                HeightUnit = "cm",
                Width = 2.2,
                WidthUnit = "cm",
                Length = 1.8,
                LengthUnit = "cm",
                Tags = new List<string> { "Sofa", "Leather", "Modern", "Furniture", "Home" },
                HostedAt = HostedAt.Store,
                CategoryId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "Furniture"))!.Id,
                SellerId = (await context.Users.FirstOrDefaultAsync(u => u.Username == "seller1"))!.Id
            };

            var lamp = new Product
            {
                Name = "Table Lamp",
                Description = "A stylish and functional table lamp for your bedroom or living room.",
                Price = 49,
                ImageUrl = "https://www.example.com/table-lamp.jpg",
                Weight = 2,
                WeightUnit = "kg",
                Height = 0.5,
                HeightUnit = "cm",
                Width = 0.3,
                WidthUnit = "cm",
                Length = 0.3,
                LengthUnit = "cm",
                Tags = new List<string> { "Lamp", "Table", "Decor", "Lighting", "Home" },
                HostedAt = HostedAt.Store,
                CategoryId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "Decor"))!.Id,
                SellerId = (await context.Users.FirstOrDefaultAsync(u => u.Username == "seller2"))!.Id
            };

            // Books
            var fictionBook = new Product
            {
                Name = "The Great Gatsby",
                Description = "A classic American novel by F. Scott Fitzgerald.",
                Price = 12.99m,
                ImageUrl = "https://www.example.com/great-gatsby.jpg",
                Weight = 0.3,
                WeightUnit = "kg",
                Height = 0.2,
                HeightUnit = "cm",
                Width = 0.15,
                WidthUnit = "cm",
                Length = 0.05,
                LengthUnit = "cm",
                Tags = new List<string> { "Fiction", "Classic", "Literature", "Books" },
                HostedAt = HostedAt.Warehouse,
                CategoryId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "Books"))!.Id,
                SellerId = (await context.Users.FirstOrDefaultAsync(u => u.Username == "seller1"))!.Id
            };

            var nonFictionBook = new Product
            {
                Name = "Sapiens",
                Description = "A groundbreaking book that explores the history of humankind.",
                Price = 19.99m,
                ImageUrl = "https://www.example.com/sapiens.jpg",
                Weight = 0.5,
                WeightUnit = "kg",
                Height = 0.25,
                HeightUnit = "cm",
                Width = 0.2,
                WidthUnit = "cm",
                Length = 0.1,
                LengthUnit = "cm",
                Tags = new List<string> { "Non-Fiction", "History", "Humanity", "Books" },
                HostedAt = HostedAt.Warehouse,
                CategoryId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "Books"))!.Id,
                SellerId = (await context.Users.FirstOrDefaultAsync(u => u.Username == "seller2"))!.Id
            };

            await context.Products.AddRangeAsync(iphone14Pro, macbookPro, womensDress, mensShirt, sofa, lamp,
                fictionBook, nonFictionBook);
            await context.SaveChangesAsync();

            // Seed Product Variants
            var iphone14ProVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductId = iphone14Pro.Id,
                    SKU = "IPHONE14PRO-128GB-BLUE",
                    ImageUrl = "https://www.apple.com/v/iphone/home/images/hero/iphone_14_pro__b4r3f67v6s9g_large.jpg",
                    PriceAdjustment = 0,
                    Quantity = 100,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Storage", Value = "128GB" },
                        new() { Name = "Color", Value = "Blue" }
                    }
                },
                new()
                {
                    ProductId = iphone14Pro.Id,
                    SKU = "IPHONE14PRO-256GB-BLACK",
                    ImageUrl = "https://www.apple.com/v/iphone/home/images/hero/iphone_14_pro__b4r3f67v6s9g_large.jpg",
                    PriceAdjustment = 100,
                    Quantity = 50,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Storage", Value = "256GB" },
                        new() { Name = "Color", Value = "Black" }
                    }
                },
                new()
                {
                    ProductId = iphone14Pro.Id,
                    SKU = "IPHONE14PRO-512GB-SILVER",
                    ImageUrl = "https://www.apple.com/v/iphone/home/images/hero/iphone_14_pro__b4r3f67v6s9g_large.jpg",
                    PriceAdjustment = 200,
                    Quantity = 25,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Storage", Value = "512GB" },
                        new() { Name = "Color", Value = "Silver" }
                    }
                }
            };

            await context.ProductVariants.AddRangeAsync(iphone14ProVariants);
            await context.SaveChangesAsync();

            // Create Inventory for each product variant
            foreach (var variant in iphone14ProVariants)
            {
                await context.Inventories.AddAsync(new Inventory
                {
                    ProductVariantId = variant.Id,
                    QuantityOnHand = variant.Quantity,
                    ReservedQuantity = 0
                });
            }

            var macbookProVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductId = macbookPro.Id,
                    SKU = "MBP-14-M2PRO-16GB-512GB",
                    ImageUrl = "https://www.apple.com/v/macbook-pro/home/images/hero/macbook_pro_14_inch__fm00w7k2p29k_large.jpg",
                    PriceAdjustment = 0,
                    Quantity = 30,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Processor", Value = "M2 Pro" },
                        new() { Name = "Memory", Value = "16GB" },
                        new() { Name = "Storage", Value = "512GB" }
                    }
                },
                new()
                {
                    ProductId = macbookPro.Id,
                    SKU = "MBP-14-M2PRO-32GB-1TB",
                    ImageUrl = "https://www.apple.com/v/macbook-pro/home/images/hero/macbook_pro_14_inch__fm00w7k2p29k_large.jpg",
                    PriceAdjustment = 300,
                    Quantity = 15,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Processor", Value = "M2 Pro" },
                        new() { Name = "Memory", Value = "32GB" },
                        new() { Name = "Storage", Value = "1TB" }
                    }
                }
            };

            await context.ProductVariants.AddRangeAsync(macbookProVariants);
            await context.SaveChangesAsync();

            // Create Inventory for each product variant
            foreach (var variant in macbookProVariants)
            {
                await context.Inventories.AddAsync(new Inventory
                {
                    ProductVariantId = variant.Id,
                    QuantityOnHand = variant.Quantity,
                    ReservedQuantity = 0
                });
            }

            
            var womensDressVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductId = womensDress.Id,
                    SKU = "WD-S-RED",
                    ImageUrl = "https://www.example.com/womens-dress-red.jpg",
                    PriceAdjustment = 0,
                    Quantity = 50,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Size", Value = "S" },
                        new() { Name = "Color", Value = "Red" }
                    }
                },
                new()
                {
                    ProductId = womensDress.Id,
                    SKU = "WD-M-BLUE",
                    ImageUrl = "https://www.example.com/womens-dress-blue.jpg",
                    PriceAdjustment = 0,
                    Quantity = 40,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Size", Value = "M" },
                        new() { Name = "Color", Value = "Blue" }
                    }
                },
                new()
                {
                    ProductId = womensDress.Id,
                    SKU = "WD-L-GREEN",
                    ImageUrl = "https://www.example.com/womens-dress-green.jpg",
                    PriceAdjustment = 0,
                    Quantity = 30,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Size", Value = "L" },
                        new() { Name = "Color", Value = "Green" }
                    }
                }
            };

            await context.ProductVariants.AddRangeAsync(womensDressVariants);
            await context.SaveChangesAsync();

            // Create Inventory for each product variant
            foreach (var variant in womensDressVariants)
            {
                await context.Inventories.AddAsync(new Inventory
                {
                    ProductVariantId = variant.Id,
                    QuantityOnHand = variant.Quantity,
                    ReservedQuantity = 0
                });
            }

            // Variants for Men's Shirt
            var mensShirtVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductId = mensShirt.Id,
                    SKU = "MS-S-WHITE",
                    ImageUrl = "https://www.example.com/mens-shirt-white.jpg",
                    PriceAdjustment = 0,
                    Quantity = 70,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Size", Value = "S" },
                        new() { Name = "Color", Value = "White" }
                    }
                },
                new()
                {
                    ProductId = mensShirt.Id,
                    SKU = "MS-M-BLUE",
                    ImageUrl = "https://www.example.com/mens-shirt-blue.jpg",
                    PriceAdjustment = 0,
                    Quantity = 60,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Size", Value = "M" },
                        new() { Name = "Color", Value = "Blue" }
                    }
                },
                new()
                {
                    ProductId = mensShirt.Id,
                    SKU = "MS-L-BLACK",
                    ImageUrl = "https://www.example.com/mens-shirt-black.jpg",
                    PriceAdjustment = 0,
                    Quantity = 50,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Size", Value = "L" },
                        new() { Name = "Color", Value = "Black" }
                    }
                }
            };

            await context.ProductVariants.AddRangeAsync(mensShirtVariants);
            await context.SaveChangesAsync();

            // Create Inventory for each product variant
            foreach (var variant in mensShirtVariants)
            {
                await context.Inventories.AddAsync(new Inventory
                {
                    ProductVariantId = variant.Id,
                    QuantityOnHand = variant.Quantity,
                    ReservedQuantity = 0
                });
            }

            // Variants for Modern Leather Sofa
            var sofaVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductId = sofa.Id,
                    SKU = "SOFA-BROWN",
                    ImageUrl = "https://www.example.com/leather-sofa-brown.jpg",
                    PriceAdjustment = 0,
                    Quantity = 10,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Color", Value = "Brown" }
                    }
                },
                new()
                {
                    ProductId = sofa.Id,
                    SKU = "SOFA-BLACK",
                    ImageUrl = "https://www.example.com/leather-sofa-black.jpg",
                    PriceAdjustment = 0,
                    Quantity = 5,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Color", Value = "Black" }
                    }
                }
            };

            await context.ProductVariants.AddRangeAsync(sofaVariants);
            await context.SaveChangesAsync();

            // Create Inventory for each product variant
            foreach (var variant in sofaVariants)
            {
                await context.Inventories.AddAsync(new Inventory
                {
                    ProductVariantId = variant.Id,
                    QuantityOnHand = variant.Quantity,
                    ReservedQuantity = 0
                });
            }

            // Variants for Table Lamp
            var lampVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductId = lamp.Id,
                    SKU = "LAMP-GOLD",
                    ImageUrl = "https://www.example.com/table-lamp-gold.jpg",
                    PriceAdjustment = 0,
                    Quantity = 20,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Color", Value = "Gold" }
                    }
                },
                new()
                {
                    ProductId = lamp.Id,
                    SKU = "LAMP-SILVER",
                    ImageUrl = "https://www.example.com/table-lamp-silver.jpg",
                    PriceAdjustment = 0,
                    Quantity = 15,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Color", Value = "Silver" }
                    }
                }
            };

            await context.ProductVariants.AddRangeAsync(lampVariants);
            await context.SaveChangesAsync();

            // Create Inventory for each product variant
            foreach (var variant in lampVariants)
            {
                await context.Inventories.AddAsync(new Inventory
                {
                    ProductVariantId = variant.Id,
                    QuantityOnHand = variant.Quantity,
                    ReservedQuantity = 0
                });
            }

            // Variants for The Great Gatsby
            var fictionBookVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductId = fictionBook.Id,
                    SKU = "GATSBY-PAPERBACK",
                    ImageUrl = "https://www.example.com/great-gatsby-paperback.jpg",
                    PriceAdjustment = 0,
                    Quantity = 100,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Format", Value = "Paperback" }
                    }
                },
                new()
                {
                    ProductId = fictionBook.Id,
                    SKU = "GATSBY-HARDCOVER",
                    ImageUrl = "https://www.example.com/great-gatsby-hardcover.jpg",
                    PriceAdjustment = 5,
                    Quantity = 50,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Format", Value = "Hardcover" }
                    }
                }
            };

            await context.ProductVariants.AddRangeAsync(fictionBookVariants);
            await context.SaveChangesAsync();

            // Create Inventory for each product variant
            foreach (var variant in fictionBookVariants)
            {
                await context.Inventories.AddAsync(new Inventory
                {
                    ProductVariantId = variant.Id,
                    QuantityOnHand = variant.Quantity,
                    ReservedQuantity = 0
                });
            }

            // Variants for Sapiens
            var nonFictionBookVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductId = nonFictionBook.Id,
                    SKU = "SAPIENS-PAPERBACK",
                    ImageUrl = "https://www.example.com/sapiens-paperback.jpg",
                    PriceAdjustment = 0,
                    Quantity = 80,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Format", Value = "Paperback" }
                    }
                },
                new()
                {
                    ProductId = nonFictionBook.Id,
                    SKU = "SAPIENS-HARDCOVER",
                    ImageUrl = "https://www.example.com/sapiens-hardcover.jpg",
                    PriceAdjustment = 5,
                    Quantity = 40,
                    InStock = true,
                    Options = new List<ProductVariantOption>
                    {
                        new() { Name = "Format", Value = "Hardcover" }
                    }
                }
            };

            await context.ProductVariants.AddRangeAsync(nonFictionBookVariants);
            await context.SaveChangesAsync();

            // Create Inventory for each product variant
            foreach (var variant in nonFictionBookVariants)
            {
                await context.Inventories.AddAsync(new Inventory
                {
                    ProductVariantId = variant.Id,
                    QuantityOnHand = variant.Quantity,
                    ReservedQuantity = 0
                });
            }

            
            await context.SaveChangesAsync();
        }
        
        // 6. Seed Reviews
        if (!await context.Reviews.AnyAsync())
        {
            var iphone14Pro = await context.Products.FirstOrDefaultAsync(p => p.Name == "iPhone 14 Pro");
            var macbookPro = await context.Products.FirstOrDefaultAsync(p => p.Name == "MacBook Pro");
            var womensDress = await context.Products.FirstOrDefaultAsync(p => p.Name == "Summer Floral Dress");
            var mensShirt = await context.Products.FirstOrDefaultAsync(p => p.Name == "Classic Cotton Shirt");
            var sofa = await context.Products.FirstOrDefaultAsync(p => p.Name == "Modern Leather Sofa");
            var lamp = await context.Products.FirstOrDefaultAsync(p => p.Name == "Table Lamp");
            var fictionBook = await context.Products.FirstOrDefaultAsync(p => p.Name == "The Great Gatsby");
            var nonFictionBook = await context.Products.FirstOrDefaultAsync(p => p.Name == "Sapiens");

            var user1 = await context.Users.FirstOrDefaultAsync(u => u.Username == "user1");
            var user2 = await context.Users.FirstOrDefaultAsync(u => u.Username == "user2");

            await context.Reviews.AddRangeAsync(
                new Review
                {
                    ProductId = iphone14Pro!.Id,
                    UserId = user1!.Id,
                    Rating = 4,
                    ReviewText = "Excellent phone! Great camera and performance."
                },
                new Review
                {
                    ProductId = macbookPro!.Id,
                    UserId = user2!.Id,
                    Rating = 5,
                    ReviewText = "Amazing laptop! Super fast and the screen is beautiful."
                },
                new Review
                {
                    ProductId = womensDress!.Id,
                    UserId = user1.Id,
                    Rating = 3,
                    ReviewText = "Nice dress, but a bit too short for me."
                },
                new Review
                {
                    ProductId = mensShirt!.Id,
                    UserId = user2.Id,
                    Rating = 5,
                    ReviewText = "Great shirt! Comfortable and stylish."
                },
                new Review
                {
                    ProductId = sofa!.Id,
                    UserId = user1.Id,
                    Rating = 4,
                    ReviewText = "Very comfortable sofa, but a bit pricey."
                },
                new Review
                {
                    ProductId = lamp!.Id,
                    UserId = user2.Id,
                    Rating = 5,
                    ReviewText = "Love this lamp! It looks great in my living room."
                },
                new Review
                {
                    ProductId = fictionBook!.Id,
                    UserId = user1.Id,
                    Rating = 4,
                    ReviewText = "A classic! Enjoyed reading it again."
                },
                new Review
                {
                    ProductId = nonFictionBook!.Id,
                    UserId = user2.Id,
                    Rating = 5,
                    ReviewText = "Fascinating read! Highly recommend it."
                }
            );

            await context.SaveChangesAsync();
        }

        // 7. Seed Coupons
        if (!await context.Coupons.AnyAsync())
        {
            await context.Coupons.AddRangeAsync(
                new Coupon
                {
                    Code = "WELCOME10",
                    Description = "Welcome discount coupon",
                    CouponType = CouponType.Percentage,
                    DiscountAmount = 10,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    MinimumOrderAmount = 50,
                    IsActive = true
                },
                new Coupon
                {
                    Code = "SUMMER20",
                    Description = "Summer discount coupon",
                    CouponType = CouponType.Percentage,
                    DiscountAmount = 20,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    MinimumOrderAmount = 100,
                    IsActive = true
                }
            );

            await context.SaveChangesAsync();
        }

        // 8. Seed Carts
        if (!await context.Carts.AnyAsync())
        {
            var user1 = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            var user2 = await context.Users.FirstOrDefaultAsync(u => u.Username == "user2");
            var iphone14Pro = await context.Products.FirstOrDefaultAsync(p => p.Name == "iPhone 14 Pro");
            var macbookPro = await context.Products.FirstOrDefaultAsync(p => p.Name == "MacBook Pro");

            await context.Carts.AddRangeAsync(
                new Cart
                {
                    UserId = user1!.Id,
                    TotalPrice = iphone14Pro!.Price,
                    ShippingCost = 0,
                    TaxAmount = 0,
                    CartItems = new List<CartItem>
                    {
                        new()
                        {
                            ProductId = iphone14Pro.Id,
                            Quantity = 1,
                            PriceAtPurchase = iphone14Pro.Price
                        }
                    }
                },
                new Cart
                {
                    UserId = user2!.Id,
                    TotalPrice = macbookPro!.Price,
                    ShippingCost = 0,
                    TaxAmount = 0,
                    CartItems = new List<CartItem>
                    {
                        new()
                        {
                            ProductId = macbookPro.Id,
                            Quantity = 2,
                            PriceAtPurchase = macbookPro.Price
                        }
                    }
                }
            );
            await context.SaveChangesAsync();
        }
        
        // 9. Seed Orders
        if (!await context.Orders.AnyAsync())
        {
            var user1 = await context.Users.FirstOrDefaultAsync(u => u.Username == "user1");
            var user2 = await context.Users.FirstOrDefaultAsync(u => u.Username == "user2");
            var iphone14Pro = await context.Products.FirstOrDefaultAsync(p => p.Name == "iPhone 14 Pro");
            var macbookPro = await context.Products.FirstOrDefaultAsync(p => p.Name == "MacBook Pro");

            await context.Orders.AddRangeAsync(
                new Order
                {
                    UserId = user1!.Id,
                    OrderStatus = OrderStatus.Pending,
                    ShippingAddress = new Address
                    {
                        Street = "123 Main St",
                        City = "Anytown",
                        State = "CA",
                        PostalCode = "12345",
                        Country = "US"
                    },
                    BillingAddress = new Address
                    {
                        Street = "456 Elm St",
                        City = "Springfield",
                        State = "IL",
                        PostalCode = "62701",
                        Country = "US"
                    },
                    Currency = "USD",
                    TotalAmount = 1000,
                    AppliedCouponCode = "",
                    CheckoutSessionId = "",
                    PaymentIntentId = "",
                    OrderItems = new List<OrderItem>
                    {
                        new()
                        {
                            ProductId = iphone14Pro!.Id,
                            Quantity = 1,
                            PriceAtPurchase = 999
                        }
                    }
                },
                new Order
                {
                    UserId = user2!.Id,
                    OrderStatus = OrderStatus.Shipped,
                    ShippingAddress = new Address
                    {
                        Street = "789 Oak Ave",
                        City = "New York",
                        State = "NY",
                        PostalCode = "10001",
                        Country = "US"
                    },
                    BillingAddress = new Address
                    {
                        Street = "789 Oak Ave",
                        City = "New York",
                        State = "NY",
                        PostalCode = "10001",
                        Country = "US"
                    },
                    Currency = "USD",
                    TotalAmount = 2000,
                    AppliedCouponCode = "",
                    CheckoutSessionId = "",
                    PaymentIntentId = "",
                    OrderItems = new List<OrderItem>
                    {
                        new()
                        {
                            ProductId = macbookPro!.Id,
                            Quantity = 1,
                            PriceAtPurchase = 1999
                        }
                    }
                }
            );

            await context.SaveChangesAsync();
        }
    }
}