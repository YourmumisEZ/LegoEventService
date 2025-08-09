using Amazon.DynamoDBv2;
using Amazon.Runtime;
using LegoEventService.BL.Interfaces;
using LegoEventService.BL.Services;
using LegoEventService.Infra.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Amazon;
using Serilog;
using AWS.Logger.SeriLog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.AwsCloudWatch;
using Amazon.CloudWatchLogs;
using LegoEventService.Mutations;
using LegoEventService.Queries;
using Amazon.DynamoDBv2.DataModel;
namespace LegoEventService
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtSettings = new JwtSettings();

            builder.Configuration.GetSection("JWT").Bind(jwtSettings);

            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.AmazonCloudWatch(
                        logGroup: "LegoEventService",
                        logStreamPrefix: DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                        cloudWatchClient: new AmazonCloudWatchLogsClient())
                    .WriteTo.Console()
                    .CreateLogger();

            builder.Services.AddGraphQLServer()
                            .AddMutationConventions()
                            .AddQueryType<EventsQuery>()
                            .AddMutationType<SignupMutation>()
                            .AddDefaultTransactionScopeHandler();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                   .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        };
                    });

            builder.Services.AddAWSService<IAmazonDynamoDB>();
            builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
            builder.Services.AddScoped<IEventsService, EventsService>();
            builder.Services.AddScoped<ISignupsService, SignupsService>();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("IsUser", policy =>
                     policy.RequireAssertion(context =>
                     context.User.HasClaim(c => (c.Type == "Role" && c.Value == "User"))));
                options.AddPolicy("IsAdmin", policy =>
                     policy.RequireAssertion(context =>
                     context.User.HasClaim(c => (c.Type == "Role" && c.Value == "Admin"))));
                options.AddPolicy("IsUserOrAdmin", policy =>
                     policy.RequireAssertion(context =>
                     context.User.HasClaim(c => (c.Type == "Role" && (c.Value == "User" || c.Value == "Admin")))));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Add bearer token"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                      new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                      {
                          Reference = new Microsoft.OpenApi.Models.OpenApiReference
                          {
                              Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                              Id = "Bearer"
                          }
                      },
                      Array.Empty<string>()
                    }
                });
            });

            builder.Host.UseSerilog();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapGraphQL();

            app.Run();
        }
    }
}
