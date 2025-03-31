using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

void Configure<TConfig>(string sectionName) where TConfig : class, new()
{
    builder.Services
        .AddSingleton(p => p.GetRequiredService<IOptions<TConfig>>().Value)
        .AddOptionsWithValidateOnStart<TConfig>()
        .BindConfiguration(sectionName)
        .ValidateDataAnnotations();
}

// Add config to the container.
Configure<SourceOptions>(SourceOptions.SectionName);
Configure<SupportedLanguageOptions>(SupportedLanguageOptions.SectionName);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

await app.RunAsync();
