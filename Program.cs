using Microsoft.AspNetCore.Builder;
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

app.MapGet("/tasks", async (ApiDbContext db) =>
{
    return await db.Tasks.ToListAsync();
});

app.MapGet("/tasks/{id}", async(int id, ApiDbContext db) =>
{
    //Using is Task to transform in a Task and return it in the body
    return await db.Tasks.FindAsync(id) is Task task ? Results.Ok(task) : Results.NotFound();
});

app.MapGet("/tasks/completed", async (ApiDbContext db) =>
{
    return await db.Tasks.Where(t => t.IsCompleted == true).ToListAsync();
});

app.MapPost("/tasks", async (Task task, ApiDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});

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
