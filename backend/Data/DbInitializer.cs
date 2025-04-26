using GamePlatform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GamePlatform.Data;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, IConfiguration configuration)
    {
        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if admin user exists
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
            if (adminUser == null)
            {
                var admin = new User
                {
                    Username = "admin",
                    Email = "admin@gameplatform.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }

            // Add sample categories if none exist
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category
                    {
                        Name = "Platformer",
                        Description = "Jump and run games with platforming elements",
                        ImageUrl = "https://i.imgur.com/8KQZQZQ.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Puzzle",
                        Description = "Brain-teasing games that challenge your mind",
                        ImageUrl = "https://i.imgur.com/9KQZQZQ.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Strategy",
                        Description = "Games that require planning and tactical thinking",
                        ImageUrl = "https://i.imgur.com/7KQZQZQ.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Adventure",
                        Description = "Story-driven games with exploration",
                        ImageUrl = "https://i.imgur.com/6KQZQZQ.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Action",
                        Description = "Fast-paced games with combat and adventure",
                        ImageUrl = "https://i.imgur.com/5KQZQZQ.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Add sample games if none exist
            if (!await context.Games.AnyAsync())
            {
                var platformerCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Platformer");
                var puzzleCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Puzzle");
                var strategyCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Strategy");
                var adventureCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Adventure");
                var actionCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Action");

                var games = new List<Game>
                {
                    new Game
                    {
                        Title = "Super Mario Flash",
                        Description = "A classic platformer featuring Mario in a Flash-based adventure",
                        ImageUrl = "https://i.imgur.com/1KQZQZQ.jpg",
                        GameUrl = "https://www.newgrounds.com/portal/view/123456",
                        CategoryId = platformerCategory.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Title = "Bloons Tower Defense 5",
                        Description = "Defend your territory by strategically placing monkey towers to pop balloons",
                        ImageUrl = "https://i.imgur.com/2KQZQZQ.jpg",
                        GameUrl = "https://www.kongregate.com/games/NinjaKiwi/bloons-tower-defense-5",
                        CategoryId = strategyCategory.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Title = "The Last Stand",
                        Description = "A zombie survival game where you must defend your barricade against waves of undead",
                        ImageUrl = "https://i.imgur.com/3KQZQZQ.jpg",
                        GameUrl = "https://www.armorgames.com/play/1234/the-last-stand",
                        CategoryId = actionCategory.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Title = "Fancy Pants Adventures",
                        Description = "A stylish platformer with fluid animation and challenging levels",
                        ImageUrl = "https://i.imgur.com/4KQZQZQ.jpg",
                        GameUrl = "https://www.newgrounds.com/portal/view/234567",
                        CategoryId = platformerCategory.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Title = "GemCraft",
                        Description = "A unique tower defense game where you craft and combine gems to create powerful towers",
                        ImageUrl = "https://i.imgur.com/5KQZQZQ.jpg",
                        GameUrl = "https://www.kongregate.com/games/gameinabottle/gemcraft",
                        CategoryId = strategyCategory.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Title = "Stick RPG",
                        Description = "A life simulation game where you control a stick figure in a city",
                        ImageUrl = "https://i.imgur.com/6KQZQZQ.jpg",
                        GameUrl = "https://www.newgrounds.com/portal/view/345678",
                        CategoryId = adventureCategory.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Title = "Boxhead",
                        Description = "A zombie shooter game with simple graphics but intense action",
                        ImageUrl = "https://i.imgur.com/7KQZQZQ.jpg",
                        GameUrl = "https://www.armorgames.com/play/2345/boxhead",
                        CategoryId = actionCategory.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Title = "Cursed Treasure",
                        Description = "A tower defense game where you play as the villain protecting your treasure",
                        ImageUrl = "https://i.imgur.com/8KQZQZQ.jpg",
                        GameUrl = "https://www.kongregate.com/games/IriySoft/cursed-treasure",
                        CategoryId = strategyCategory.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                await context.Games.AddRangeAsync(games);
                await context.SaveChangesAsync();
            }

            // Add sample regular users if none exist
            if (!await context.Users.AnyAsync(u => u.Role == "User"))
            {
                var users = new List<User>
                {
                    new User
                    {
                        Username = "user1",
                        Email = "user1@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "user2",
                        Email = "user2@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("DbInitializer");
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
} 