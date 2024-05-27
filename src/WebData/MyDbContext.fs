module MyDbContext

open System
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Sqlite // replace with DB of your choice
open Microsoft.EntityFrameworkCore.Design
open Microsoft.Extensions.DependencyInjection // for IDesignTimeDbContextFactory


type ApplicationDbContext(options: DbContextOptions<ApplicationDbContext>) =
    inherit IdentityDbContext(options)

    // OPTIONAL. seed the database with some initial roles.
    override __.OnModelCreating(modelBuilder: ModelBuilder) =
        base.OnModelCreating(modelBuilder)

        modelBuilder
            .Entity<IdentityRole>()
            .HasData(
                [| IdentityRole(Name = "admin", NormalizedName = "ADMIN")
                   IdentityRole(Name = "user", NormalizedName = "USER") |]
            )
        |> ignore

type ApplicationDbContextFactory() =
    interface IDesignTimeDbContextFactory<ApplicationDbContext> with
        member __.CreateDbContext(args: string[]) =
            let optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            optionsBuilder.UseSqlite("Data Source=identity.db") |> ignore
            new ApplicationDbContext(optionsBuilder.Options)

let configureDBContextService (services: IServiceCollection) =
    services.AddDbContext<ApplicationDbContext>(fun options ->
        options.UseSqlite("Filename=identity.db") |> ignore)
    |> ignore

    services
        .AddIdentity<IdentityUser, IdentityRole>(fun options ->
            options.Password.RequireLowercase <- true
            options.Password.RequireUppercase <- true
            options.Password.RequireDigit <- true
            options.Lockout.MaxFailedAccessAttempts <- 5
            options.Lockout.DefaultLockoutTimeSpan <- TimeSpan.FromMinutes(15)
            options.User.RequireUniqueEmail <- true)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders() 
    |> ignore

    services
