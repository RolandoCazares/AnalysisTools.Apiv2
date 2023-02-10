
using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Helpers;
using analysistools.api.Repositories;
using analysistools.api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using (var db = new AppDbContext())
{
    //Create Database if not exists
    db.Database.EnsureCreated();
    // Login to all server when the app starts
    //NetworkCredential networkCredential = NetworkCredential.Instance;
    //var opticalServers = db.OpticalStations.ToList();
    //foreach (var opticalServer in opticalServers)
    //{
    //    var credential = db.WindowsCredentials.FirstOrDefault(c => c.Id == opticalServer.CredentialId);
    //    networkCredential.LogIn(opticalServer.IpAddress, credential.Username, credential.Password);
    //}
    //var ticketServers = db.TicketServers.ToList();
    //foreach (var ticketServer in ticketServers)
    //{
    //    var credential = db.WindowsCredentials.FirstOrDefault(c => c.Id == ticketServer.CredentialId);
    //    networkCredential.LogIn(ticketServer.IpAddress, credential.Username, credential.Password);
    //}
    db.SaveChanges();
}

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5005");

// Register services (in order to use dependency injection)
builder.Services.AddScoped<IMesRepository, MesRepository>();
builder.Services.AddScoped<IAccessRepository, AccessRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IComponentsService, ComponentsService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Swagger config for authentication.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

// Authentication settings
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddCors(p => p.AddPolicy("corsconfig", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("corsconfig");

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();

