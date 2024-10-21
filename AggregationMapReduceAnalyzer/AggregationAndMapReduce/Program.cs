using AggregationAndMapReduce.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDataGenerationService, DataGenerationService>();
builder.Services.AddScoped<IMapReduceService, MapReduceService>();
builder.Services.AddScoped<IAggregationService, AggregationService>();
builder.Services.AddScoped<IPerformanceComparisonService, PerformanceComparisonService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
