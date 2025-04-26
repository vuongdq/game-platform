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
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
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
                    },
                    new Category
                    {
                        Name = "Arcade",
                        Description = "Classic arcade-style games",
                        ImageUrl = "https://i.imgur.com/4KQZQZQ.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Racing",
                        Description = "High-speed racing games",
                        ImageUrl = "https://i.imgur.com/3KQZQZQ.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Sports",
                        Description = "Sports and athletic games",
                        ImageUrl = "https://i.imgur.com/2KQZQZQ.jpg",
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
                var arcadeCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Arcade");
                var racingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Racing");
                var sportsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Sports");

                var games = new List<Game>
                {
                    // Platformer Games
                    new Game
                    {
                        Name = "Super Mario Flash",
                        Description = "A classic platformer featuring Mario in a Flash-based adventure",
                        ImageUrl = "https://i.imgur.com/1KQZQZQ.jpg",
                        GameUrl = "https://www.newgrounds.com/portal/view/123456",
                        CategoryId = platformerCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Name = "Fancy Pants Adventures",
                        Description = "A stylish platformer with fluid animation and challenging levels",
                        ImageUrl = "https://i.imgur.com/4KQZQZQ.jpg",
                        GameUrl = "https://www.newgrounds.com/portal/view/234567",
                        CategoryId = platformerCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Puzzle Games
                    new Game
                    {
                        Name = "2048",
                        Description = "Classic number sliding puzzle game",
                        ImageUrl = "https://i.imgur.com/2KQZQZQ.jpg",
                        GameUrl = "https://www.2048game.com",
                        CategoryId = puzzleCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Name = "Sudoku",
                        Description = "Classic number placement puzzle",
                        ImageUrl = "https://i.imgur.com/3KQZQZQ.jpg",
                        GameUrl = "https://www.sudoku.com",
                        CategoryId = puzzleCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Strategy Games
                    new Game
                    {
                        Name = "Bloons Tower Defense 5",
                        Description = "Defend your territory by strategically placing monkey towers to pop balloons",
                        ImageUrl = "https://i.imgur.com/5KQZQZQ.jpg",
                        GameUrl = "https://www.kongregate.com/games/NinjaKiwi/bloons-tower-defense-5",
                        CategoryId = strategyCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Name = "GemCraft",
                        Description = "A unique tower defense game where you craft and combine gems to create powerful towers",
                        ImageUrl = "https://i.imgur.com/6KQZQZQ.jpg",
                        GameUrl = "https://www.kongregate.com/games/gameinabottle/gemcraft",
                        CategoryId = strategyCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Adventure Games
                    new Game
                    {
                        Name = "Stick RPG",
                        Description = "A life simulation game where you control a stick figure in a city",
                        ImageUrl = "https://i.imgur.com/7KQZQZQ.jpg",
                        GameUrl = "https://www.newgrounds.com/portal/view/345678",
                        CategoryId = adventureCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Action Games
                    new Game
                    {
                        Name = "The Last Stand",
                        Description = "A zombie survival game where you must defend your barricade against waves of undead",
                        ImageUrl = "https://i.imgur.com/8KQZQZQ.jpg",
                        GameUrl = "https://www.armorgames.com/play/1234/the-last-stand",
                        CategoryId = actionCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Name = "Boxhead",
                        Description = "A zombie shooter game with simple graphics but intense action",
                        ImageUrl = "https://i.imgur.com/9KQZQZQ.jpg",
                        GameUrl = "https://www.armorgames.com/play/2345/boxhead",
                        CategoryId = actionCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Arcade Games
                    new Game
                    {
                        Name = "Pac-Man",
                        Description = "Classic maze chase arcade game",
                        ImageUrl = "https://i.imgur.com/1KQZQZQ.jpg",
                        GameUrl = "https://www.pacman.com",
                        CategoryId = arcadeCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Game
                    {
                        Name = "Tetris",
                        Description = "Classic block-stacking puzzle game",
                        ImageUrl = "https://i.imgur.com/2KQZQZQ.jpg",
                        GameUrl = "https://www.tetris.com",
                        CategoryId = arcadeCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Racing Games
                    new Game
                    {
                        Name = "Mario Kart",
                        Description = "Classic racing game with power-ups and obstacles",
                        ImageUrl = "https://i.imgur.com/3KQZQZQ.jpg",
                        GameUrl = "https://www.mariokart.com",
                        CategoryId = racingCategory?.Id ?? 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Sports Games
                    new Game
                    {
                        Name = "Basketball Stars",
                        Description = "Fast-paced basketball game with various modes",
                        ImageUrl = "https://i.imgur.com/4KQZQZQ.jpg",
                        GameUrl = "https://www.basketballstars.com",
                        CategoryId = sportsCategory?.Id ?? 0,
                        IsActive = true,
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
                        Password = BCrypt.Net.BCrypt.HashPassword("User123!"),
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "user2",
                        Email = "user2@example.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("User123!"),
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "gamer1",
                        Email = "gamer1@example.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Gamer123!"),
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "gamer2",
                        Email = "gamer2@example.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Gamer123!"),
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

    private static async Task CreateAdminUser(ApplicationDbContext context)
    {
        try
        {
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@gameplatform.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw to allow application to start
            Console.WriteLine($"Error creating admin user: {ex.Message}");
        }
    }
} 