using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using ManagerData.Contexts;
using ManagerData.Management;
using ManagerLogic.Management;
using ManagerData.Authentication;
using ManagerData.DataModels;
using ManagerData.DataModels.Authentication;
using ManagerLogic.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", 
    policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:Urls").Get<string[]>()!);
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    }
));

builder.Services.AddControllers();

builder.Services.AddDbContext<AuthenticationDbContext>();
builder.Services.AddDbContext<MainDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecureKey"]!)), 
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            []
        }
    });
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSingleton<IJwtCreator,JwtCreator>();
builder.Services.AddSingleton<IAuthenticationRepository,AuthenticationRepository>();
builder.Services.AddSingleton<IAuthentication,Authentication>();
builder.Services.AddSingleton<IEncrypt,Encrypt>();

builder.Services.AddSingleton<IPartRepository, PartRepository>();
builder.Services.AddSingleton<IPartLogic, PartLogic>();

builder.Services.AddSingleton<IMemberRepository, MemberRepository>();
builder.Services.AddSingleton<IMemberLogic, MemberLogic>();

builder.Services.AddSingleton<ITaskRepository, TaskRepository>();
builder.Services.AddSingleton<ITaskLogic, TaskLogic>();
builder.Services.AddSingleton<IRoleRepository, RoleRepository>();

var app = builder.Build();

app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
