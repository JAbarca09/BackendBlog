using Backend_Blog.Services;
using Backend_Blog.Services.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//cors policy
builder.Services.AddCors(options => {
    options.AddPolicy("BlogPolicy", 
    builder => {
        builder.WithOrigins("http://localhost:3000", "https://abarcajblog.azurewebsites.net")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BlogItemService>();
builder.Services.AddScoped<PasswordService>();

var connectionString = builder.Configuration.GetConnectionString("MyBlogString");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));


builder.Services.AddControllers();
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

// app.UseHttpsRedirection();

//enacting cors policy
app.UseCors("BlogPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
