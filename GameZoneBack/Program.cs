using System.Text;
using System.Text.Json.Serialization;
using GameZoneBack.MiddleWare;
using GameZoneBack.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.


// JwtBearerDefaults.AuthenticationScheme tells ASP.NET Core that
// JWT Bearer authentication will be the default authentication method.
string Secretkey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // TokenValidationParameters define how incoming JWTs will be validated.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Ensures the token was issued by a trusted issuer.
            ValidateIssuer = true,


            // Ensures the token is intended for this API (audience check).
            ValidateAudience = true,


            // Ensures the token has not expired.
            ValidateLifetime = true,


            // Ensures the token signature is valid and was signed by the API.
            ValidateIssuerSigningKey = true,


            // The expected issuer value (must match the issuer used when creating the JWT).
            ValidIssuer = "GameZoneApi",


            // The expected audience value (must match the audience used when creating the JWT).
            ValidAudience = "GameZoneApiUsers",


            // The secret key used to validate the JWT signature.
            // This must be the same key used when generating the token.
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Secretkey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
// ===============================
// 1) Define the JWT Bearer security scheme
// ===============================
//
// This tells Swagger that our API uses JWT Bearer authentication
// through the HTTP Authorization header.
options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    // The name of the HTTP header where the token will be sent.
    Name = "Authorization",


    // Indicates this is an HTTP authentication scheme.
    Type = SecuritySchemeType.Http,


    // Specifies the authentication scheme name.
    // Must be exactly "Bearer" for JWT Bearer tokens.
    Scheme = "Bearer",


    // Optional metadata to describe the token format.
    BearerFormat = "JWT",


    // Specifies that the token is sent in the request header.
    In = ParameterLocation.Header,


    // Text shown in Swagger UI to guide the user.
    Description = "Enter: Bearer {your JWT token}"
});

    // ===============================
    // 2) Require the Bearer scheme for secured endpoints
    // ===============================
    //
    // This tells Swagger that endpoints protected by [Authorize]
    // require the Bearer token defined above.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                // Reference the previously defined "Bearer" security scheme.
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },


            // No scopes are required for JWT Bearer authentication.
            // This array is empty because JWT does not use OAuth scopes here.
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("GameZoneApiCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7294",
                "http://localhost:5133",
                "http://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("GameZoneApiCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();
