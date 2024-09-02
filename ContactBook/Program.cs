using WebApplication1;
using WebApplication1.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddDbContext<UserRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<NoteRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<ElementRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<ContactRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<DeviceRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<ElementTagRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<EmailRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<EventRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<FrequencyRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<NotificationRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<PhoneNumberRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<SNNameRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<SocialNetworkRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<TagRepository>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
