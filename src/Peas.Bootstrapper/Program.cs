var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOrderService(builder.Configuration);
builder.Services.AddDapperExtension();
builder.Services.AddDotBPEWithHttpApi();



var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(o => {
    o.DocumentTitle = "Peas API Document";
});

app.UseEndpoints(endpoints =>
{
    endpoints.ScanMapServices("order");
    endpoints.MapGet("/", async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync("<html><head><meta http-equiv=\"refresh\" content=\"5;url=/swagger\"/></head><body>Welcome to DotBPE RPC Service.</body></html>");
    });
});

app.Run();
