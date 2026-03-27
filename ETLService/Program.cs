using Operations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRazorPages();


// 👇 Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutterApp", policy =>
    {
        policy.WithOrigins("http://localhost") // Flutter en emulador web (si aplica)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Solo si usas cookies o autenticación con credenciales
    });

    // Opción más permisiva (solo para desarrollo)
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
await new StartServices().StartServicesApp();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// 👇 Usar CORS (DEBE ir antes de UseAuthorization y UseRouting si los usas)
//app.UseCors("AllowAll"); // o "AllowFlutterApp" si prefieres ser más restrictivo

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);


app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();

app.Run();

