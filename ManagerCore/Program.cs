
using System.Text;
using ManagerData.Authentication;
using ManagerData.Contexts;
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic;
using ManagerLogic.Authentication;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Constants = ManagerCore.Constants;


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
builder.Services.AddDbContext<ManagerDbContext>();

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.SecureKey)), 
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IJwtCreator,JwtCreator>();
builder.Services.AddScoped<IAuthenticationRepository,AuthenticationRepository>();
builder.Services.AddScoped<IAuthentication,Authentication>();
builder.Services.AddScoped<IEncrypt,Encrypt>();

builder.Services.AddSingleton<IManagementRepository<CompanyDataModel>, CompanyRepository>();
builder.Services.AddSingleton<IManagementLogic<CompanyModel>, CompanyLogic>();

builder.Services.AddSingleton<IManagementRepository<DepartmentDataModel>, DepartmentRepository>();
builder.Services.AddSingleton<IManagementLogic<DepartmentModel>, DepartmentLogic>();

builder.Services.AddSingleton<IManagementRepository<ProjectDataModel>, ProjectRepository>();
builder.Services.AddSingleton<IProjectLogic, ProjectLogic>();

builder.Services.AddSingleton<IManagementRepository<EmployeeDataModel>, EmployeeRepository>();
builder.Services.AddSingleton<IEmployeeLogic, EmployeeLogic>();

builder.Services.AddSingleton<IManagementRepository<TaskDataModel>, TaskRepository>();
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
