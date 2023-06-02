using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add db in this case im using in memory db with Microsoft.EntityFrameworkCore.InMemory
builder.Services.AddDbContext<ApiDbContext>(options => 
    options.UseInMemoryDatabase("TasksDB")
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("phrases", async () =>
    await new HttpClient().GetStringAsync("https://ron-swanson-quotes.herokuapp.com/v2/quotes")
);



app.Run();

class Task
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsCompleted { get; set; }  
}

class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }
    public DbSet<Task> Tasks => Set<Task>();
}
