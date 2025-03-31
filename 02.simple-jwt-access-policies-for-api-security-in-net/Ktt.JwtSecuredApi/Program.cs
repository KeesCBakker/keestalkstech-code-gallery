using Microsoft.Extensions.Options;

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
        .ValidateDataAnnotations();
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
