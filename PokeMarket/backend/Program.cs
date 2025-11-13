using PokeMarket.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using PokeMarket.Models.Settings;
using PokeMarket.Services;

var builder = WebApplication.CreateBuilder(args);

// Para enviar emails
builder.Services.AddScoped<EmailService>();

// Añade el servicio de CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular",
		policy =>
		{
			policy.WithOrigins("http://localhost:4200") // O https si es https
				  .AllowAnyHeader()
				  .AllowAnyMethod()
				  .AllowCredentials();
		});
});

// Configuración de controladores y enums
builder.Services.AddControllers().AddJsonOptions(opts =>
{
	opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configuración adicional de controladores (duplicada en tu código, pero la dejo solo una vez)
builder.Services.AddControllers();

// Configuración de archivos de configuración
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// JWT Key
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
	Console.WriteLine("JWT Key no encontrada. Revisa tu appsettings.json");
}
else
{
	Console.WriteLine("JWT Key cargada correctamente.");
}

// Configuración de EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Configuración de autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
		};
	});

// Base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
	new MySqlServerVersion(new Version(8, 0, 36)))
);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware para entornos de desarrollo y producción
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapGet("/api/hola", () => Results.Json(new { message = "Hola mundo son el back C#"}));
app.MapControllers();

app.Run();
