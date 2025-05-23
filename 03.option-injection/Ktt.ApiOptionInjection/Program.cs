using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

void Configure<TConfig>(string sectionName) where TConfig : class, new()
{
    builder.Services
        .AddSingleton(p => p.GetRequiredService<IOptions<TConfig>>().Value)
        .AddOptionsWithValidateOnStart<TConfig>()
        .BindConfiguration(sectionName)
        .Validate(options =>
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(options);
            if (!Validator.TryValidateObject(options, context, results, validateAllProperties: true))
            {
                throw new OptionsValidationException(
                    sectionName,
                    typeof(TConfig),
                    results.Select(r => $"[{sectionName}] {r.ErrorMessage}")
                );
            }
            return true;
        });
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
