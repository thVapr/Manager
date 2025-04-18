
using System.Text;
using ManagerData.Authentication;
using ManagerData.Contexts;
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Authentication;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", 
    policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.WithOrigins(builder.Configuration.GetSection("Cors:Urls").Get<string[]>()!);
    }
));

builder.Services.AddControllers();

// Adding database contexts
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
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IJwtCreator,JwtCreator>();
builder.Services.AddSingleton<IAuthenticationRepository,AuthenticationRepository>();
builder.Services.AddSingleton<IAuthentication,Authentication>();
builder.Services.AddSingleton<IEncrypt,Encrypt>();

builder.Services.AddSingleton<IManagementRepository<WorkspaceDataModel>, WorkspaceRepository>();
builder.Services.AddSingleton<IManagementLogic<WorkspaceModel>, WorkspaceLogic>();

builder.Services.AddSingleton<IManagementRepository<PartDataModel>, PartRepository>();
builder.Services.AddSingleton<IPartLogic, PartLogic>();

builder.Services.AddSingleton<IMemberRepository, MemberRepository>();
builder.Services.AddSingleton<IMemberLogic, MemberLogic>();

builder.Services.AddSingleton<ITaskRepository, TaskRepository>();
builder.Services.AddSingleton<ITaskLogic, TaskLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
