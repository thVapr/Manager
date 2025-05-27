using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using ManagerData.Contexts;
using ManagerData.Management;
using ManagerLogic.Management;
using ManagerData.Authentication;
using ManagerLogic.Authentication;
using ManagerLogic.Background;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", 
    policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:Urls").Get<string[]>()!);
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    }
));

builder.Services.AddControllers();

builder.Services.AddDbContext<AuthenticationDbContext>();
builder.Services.AddDbContext<MainDbContext>();

builder.Services.AddScoped<IFileRepository, FileRepository>();

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
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/hubs/update")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
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

builder.Services.AddSingleton<IUserIdProvider,HubUserIdProvider>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IJwtCreator,JwtCreator>();
builder.Services.AddScoped<IAuthenticationRepository,AuthenticationRepository>();
builder.Services.AddScoped<IAuthentication,Authentication>();
builder.Services.AddScoped<IEncrypt,Encrypt>();

builder.Services.AddScoped<IPartRepository, PartRepository>();
builder.Services.AddScoped<IPartLogic, PartLogic>();
builder.Services.AddScoped<IPathHelper, PathHelper>();

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberLogic, MemberLogic>();

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskLogic, TaskLogic>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IFileLogic, FileLogic>();

builder.Services.AddScoped<IBackgroundTaskRepository, BackgroundTaskRepository>();
builder.Services.AddHostedService<MessengerHostService>();

var app = builder.Build();

app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); 
app.UseAuthorization();

app.MapHub<UpdateHub>("/hubs/update");
app.MapControllers();

app.Run();
