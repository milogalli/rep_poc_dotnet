using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<OrderServiceOptions>(builder.Configuration.GetSection("ProductService"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IOrderService, OrderManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();