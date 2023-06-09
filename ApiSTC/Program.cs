using NSwag.Generation.Processors.Security;
using NSwag;
using ApiSTC.Helpers;
using ApiSTC.Data;
using Microsoft.EntityFrameworkCore;
using ApiSTC.Repositories;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient =
    builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret keyVaultSecret = await
    secretClient.GetSecretAsync("SqlAzure");
string connectionString = keyVaultSecret.Value;

// Add services to the container.
HelperOAuthToken helper = new HelperOAuthToken(builder.Configuration);
builder.Services.AddSingleton<HelperOAuthToken>();
builder.Services.AddAuthentication(helper.GetAuthenticationOptions()).AddJwtBearer(helper.GetJwtOptions());

//string connectionString = builder.Configuration.GetConnectionString("SqlAzure");
builder.Services.AddDbContext<STCContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<RepositoryPartidos>();
builder.Services.AddTransient<RepositoryCompeticion>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(document => {
    document.Title = "Api STC";
    document.Description = "Api STC";
    // CONFIGURAMOS LA SEGURIDAD JWT PARA SWAGGER,    // PERMITE A�ADIR EL TOKEN JWT A LA CABECERA.
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
    new NSwag.OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = ""
    }
        );
    document.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("swagger/v1/swagger.json", "STC");
    options.RoutePrefix = "";
});
if (app.Environment.IsDevelopment()) { 

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
