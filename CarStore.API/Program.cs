using System.Text;
using CarStore.API.Middlewares;
using CarStore.Application;
using CarStore.InfraStructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddApplication();
    builder.Services.AddInfraStructure(builder.Configuration);
    
    #region Swagger Settings
    builder.Services.AddSwaggerGen(swagger =>
    {
        // This is to generate the default UI of swagger documentation
        swagger.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ASP .NET 9 Web API",
            Description = "Book Management"
        });
        // To Enable Authorization Using Swagger (JWT)
        swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your valid token in the text area"
        });
        swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                []
            }
        });
    });
    #endregion

    #region JWT Bearer Settings

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey"))),

                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["JwtSettings:issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer"),

                ValidateAudience = true,
                ValidAudience = builder.Configuration["JwtSettings:audience"] ?? throw new ArgumentNullException("JwtSettings:Audience"),
                
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    #endregion

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Public", policy =>
            policy.RequireAssertion(context =>
                context.Resource is HttpContext httpContext &&
                (httpContext.Request.Path.StartsWithSegments("/api/auth/signup") ||
                httpContext.Request.Path.StartsWithSegments("/api/auth/signin"))));
    });

}

var app = builder.Build();


    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();


{
    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.UseMiddleware<TokenValidationMiddleware>();

    app.MapControllers();

    app.Run();
}
