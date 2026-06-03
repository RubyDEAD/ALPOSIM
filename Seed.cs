// using alposim.Data;
// using alposim.Models;
//
// public static class SeedData
// {
//     public static void Initialize(AppDbContext context)
//     {
//         // Check if data already exists
//         if (context.Products.Any() || context.Sales.Any())
//         {
//             return;
//         }
//
//         // Seed Products
//         var products = new Product[]
//         {
//             new Product
//             {
//                 Id = Guid.NewGuid(),
//                 Name = "Organic Coffee Beans",
//                 ImageUrl = "https://via.placeholder.com/300?text=Coffee",
//                 Quantity = 50,
//                 Price = 12.99m,
//                 Metric = "kg",
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow
//             },
//             new Product
//             {
//                 Id = Guid.NewGuid(),
//                 Name = "Fresh Milk",
//                 ImageUrl = "https://via.placeholder.com/300?text=Milk",
//                 Quantity = 100,
//                 Price = 3.50m,
//                 Metric = "liter",
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow
//             },
//             new Product
//             {
//                 Id = Guid.NewGuid(),
//                 Name = "Whole Wheat Bread",
//                 ImageUrl = "https://via.placeholder.com/300?text=Bread",
//                 Quantity = 75,
//                 Price = 2.99m,
//                 Metric = "loaf",
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow
//             },
//             new Product
//             {
//                 Id = Guid.NewGuid(),
//                 Name = "Free-Range Eggs",
//                 ImageUrl = "https://via.placeholder.com/300?text=Eggs",
//                 Quantity = 200,
//                 Price = 5.99m,
//                 Metric = "dozen",
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow
//             },
//             new Product
//             {
//                 Id = Guid.NewGuid(),
//                 Name = "Cheddar Cheese",
//                 ImageUrl = "https://via.placeholder.com/300?text=Cheese",
//                 Quantity = 30,
//                 Price = 8.50m,
//                 Metric = "kg",
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow
//             }
//         };
//
//         context.Products.AddRange(products);
//         context.SaveChanges();
//
//         // Seed Sales with SaleItems
//         var sales = new Sale[]
//         {
//             new Sale
//             {
//                 Id = Guid.NewGuid(),
//                 TotalPrice = 22.48m,
//                 CreatedAt = DateTime.UtcNow.AddDays(-5),
//                 OnlinePayment = true,
//                 Items = new List<SaleItem>
//                 {
//                     new SaleItem
//                     {
//                         Id = Guid.NewGuid(),
//                         ProductId = products[0].Id,
//                         Quantity = 2,
//                         UnitPrice = 12.99m,
//                         TotalPrice = 25.98m
//                     }
//                 }
//             },
//             new Sale
//             {
//                 Id = Guid.NewGuid(),
//                 TotalPrice = 13.47m,
//                 CreatedAt = DateTime.UtcNow.AddDays(-3),
//                 OnlinePayment = false,
//                 Items = new List<SaleItem>
//                 {
//                     new SaleItem
//                     {
//                         Id = Guid.NewGuid(),
//                         ProductId = products[1].Id,
//                         Quantity = 3,
//                         UnitPrice = 3.50m,
//                         TotalPrice = 10.50m
//                     },
//                     new SaleItem
//                     {
//                         Id = Guid.NewGuid(),
//                         ProductId = products[2].Id,
//                         Quantity = 1,
//                         UnitPrice = 2.99m,
//                         TotalPrice = 2.99m
//                     }
//                 }
//             },
//             new Sale
//             {
//                 Id = Guid.NewGuid(),
//                 TotalPrice = 11.98m,
//                 CreatedAt = DateTime.UtcNow.AddDays(-1),
//                 OnlinePayment = true,
//                 Items = new List<SaleItem>
//                 {
//                     new SaleItem
//                     {
//                         Id = Guid.NewGuid(),
//                         ProductId = products[3].Id,
//                         Quantity = 2,
//                         UnitPrice = 5.99m,
//                         TotalPrice = 11.98m
//                     }
//                 }
//             }
//         };
//
//         context.Sales.AddRange(sales);
//         context.SaveChanges();
//     }
// }
