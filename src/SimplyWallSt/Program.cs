using Microsoft.Extensions.Options;
using SimplyWallSt;
using SimplyWallSt.Listing.Repository;
using SimplyWallSt.Listing.Repository.Company;
using SimplyWallSt.Listing.Repository.CompanyPriceClose;
using SimplyWallSt.Listing.Repository.CompanyScore;
using SimplyWallSt.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<CompanyRepositoryConfigs>(builder.Configuration.GetRequiredSection(CompanyRepositoryConfigs.ConfigName));
        builder.Services.Configure<CompanyApiConfigs>(builder.Configuration.GetRequiredSection(CompanyApiConfigs.ConfigName));

        // Add services to the container.
        builder.Services.AddLogging(builder => builder.AddConsole());
        builder.Services.AddSingleton<ICompanySqlConnectionFactory, CompanySqlConnectionFactory>();
        builder.Services.AddSingleton<IRepositoryModelToViewModelMapper, RepositoryModelToViewModelMapper>();
        builder.Services.AddSingleton<ICompanyRepository, DirectCompanyRepository>();
        builder.Services.AddSingleton<ICompanySearcher, CompanySearcher>();
        builder.Services.AddSingleton<ICompanyPriceCloseRepository>((sp) =>
            new CachedCompanyPriceCloseRepository(
                new DirectCompanyPriceCloseRepository(
                    sp.GetRequiredService<ICompanySqlConnectionFactory>(),
                    sp.GetRequiredService<ILogger<DirectCompanyPriceCloseRepository>>()),
                sp.GetRequiredService<IOptions<CompanyRepositoryConfigs>>()
            )
        );
        builder.Services.AddSingleton<ICompanyScoreRepository>((sp) =>
            new CachedCompanyScoreRepository(
                new DirectCompanyScoreRepository(
                    sp.GetRequiredService<ICompanySqlConnectionFactory>(),
                    sp.GetRequiredService<ILogger<DirectCompanyScoreRepository>>()),
                sp.GetRequiredService<IOptions<CompanyRepositoryConfigs>>()
            )
        );
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}