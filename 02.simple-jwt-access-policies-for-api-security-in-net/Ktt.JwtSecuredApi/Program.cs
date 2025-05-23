using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

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

Configure<JwtOptions>(JwtOptions.SectionName);

builder.Services.AddJwtAndAccessPolicies();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IUserNameAccessor, UserNameAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
