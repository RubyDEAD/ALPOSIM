using alposim.Data;
using alposim.Models;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Products.Any() || context.Sales.Any())
        {
            return;
        }

        // Categories
        var categories = new List<Category>
        {
            new() { Name = "Construction Materials" },
            new() { Name = "Electrical Supplies" },
            new() { Name = "Plumbing Supplies" },
            new() { Name = "Paint & Finishes" },
            new() { Name = "Hand Tools" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        // Products
        var products = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "CEM-001",
                Name = "Portland Cement 40kg",
                CategoryId = categories[0].Id,
                Quantity = 150,
                OriginalPrice = 220m,
                SellingPrice = 255m,
                Metric = "bag",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "REB-001",
                Name = "10mm Deformed Rebar",
                CategoryId = categories[0].Id,
                Quantity = 300,
                OriginalPrice = 180m,
                SellingPrice = 220m,
                Metric = "piece",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "WIR-001",
                Name = "THHN Wire #12",
                CategoryId = categories[1].Id,
                Quantity = 500,
                OriginalPrice = 18m,
                SellingPrice = 25m,
                Metric = "meter",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "SWI-001",
                Name = "Light Switch",
                CategoryId = categories[1].Id,
                Quantity = 120,
                OriginalPrice = 45m,
                SellingPrice = 65m,
                Metric = "piece",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "PVC-001",
                Name = "PVC Pipe 1/2 inch",
                CategoryId = categories[2].Id,
                Quantity = 200,
                OriginalPrice = 70m,
                SellingPrice = 95m,
                Metric = "piece",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "ELB-001",
                Name = "PVC Elbow 1/2 inch",
                CategoryId = categories[2].Id,
                Quantity = 400,
                OriginalPrice = 8m,
                SellingPrice = 12m,
                Metric = "piece",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "PNT-001",
                Name = "Latex Paint White 4L",
                CategoryId = categories[3].Id,
                Quantity = 80,
                OriginalPrice = 580m,
                SellingPrice = 700m,
                Metric = "can",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "ROL-001",
                Name = "Paint Roller",
                CategoryId = categories[3].Id,
                Quantity = 100,
                OriginalPrice = 60m,
                SellingPrice = 90m,
                Metric = "piece",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "HAM-001",
                Name = "Claw Hammer",
                CategoryId = categories[4].Id,
                Quantity = 50,
                OriginalPrice = 180m,
                SellingPrice = 250m,
                Metric = "piece",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new()
            {
                Id = Guid.NewGuid(),
                ProductCode = "PLI-001",
                Name = "Combination Pliers",
                CategoryId = categories[4].Id,
                Quantity = 60,
                OriginalPrice = 150m,
                SellingPrice = 220m,
                Metric = "piece",
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Sales
        var sales = new List<Sale>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SaleCode = "SALE-0001",
                TotalPrice = 730m,
                ReceivedCash = 1000m,
                OnlinePayment = false,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                Items = new List<SaleItem>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = products[0].Id,
                        Quantity = 2,
                        CostPrice = 220m,
                        UnitPrice = 255m,
                        TotalPrice = 510m
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = products[3].Id,
                        Quantity = 2,
                        CostPrice = 45m,
                        UnitPrice = 65m,
                        TotalPrice = 130m
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = products[5].Id,
                        Quantity = 5,
                        CostPrice = 8m,
                        UnitPrice = 12m,
                        TotalPrice = 60m
                    }
                }
            },

            new()
            {
                Id = Guid.NewGuid(),
                SaleCode = "SALE-0002",
                TotalPrice = 1240m,
                ReceivedCash = 1240m,
                OnlinePayment = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Items = new List<SaleItem>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = products[6].Id,
                        Quantity = 1,
                        CostPrice = 580m,
                        UnitPrice = 700m,
                        TotalPrice = 700m
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = products[8].Id,
                        Quantity = 2,
                        CostPrice = 180m,
                        UnitPrice = 250m,
                        TotalPrice = 500m
                    }
                }
            },

            new()
            {
                Id = Guid.NewGuid(),
                SaleCode = "SALE-0003",
                TotalPrice = 915m,
                ReceivedCash = 1000m,
                OnlinePayment = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Items = new List<SaleItem>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = products[1].Id,
                        Quantity = 3,
                        CostPrice = 180m,
                        UnitPrice = 220m,
                        TotalPrice = 660m
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = products[4].Id,
                        Quantity = 1,
                        CostPrice = 70m,
                        UnitPrice = 95m,
                        TotalPrice = 95m
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = products[7].Id,
                        Quantity = 2,
                        CostPrice = 60m,
                        UnitPrice = 90m,
                        TotalPrice = 180m
                    }
                }
            }
        };

        context.Sales.AddRange(sales);
        context.SaveChanges();
    }
}