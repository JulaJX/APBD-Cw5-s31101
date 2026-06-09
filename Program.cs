using Microsoft.EntityFrameworkCore;
using APBD_DatabaseFirst.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();