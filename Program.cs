using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TelemetryHttpTester;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CustomMetrics>();
OpenTelemetryBuilder otel = builder.Services.AddOpenTelemetry();
otel.WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddRuntimeInstrumentation()
    .AddProcessInstrumentation()
    .AddMeter("CustomMeter")
    .AddConsoleExporter()
    .AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("http://otel-collector:4318/");
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
    })
);

otel.WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddSource("CustomActivityName");
    tracing.AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("http://otel-collector:4318/");
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
    tracing.AddConsoleExporter();
});


builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;

    var resourceBuilder = ResourceBuilder
        .CreateDefault()
        .AddService(builder.Environment.ApplicationName);

    logging.SetResourceBuilder(resourceBuilder)
        .AddConsoleExporter();
    logging.AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("http://otel-collector:4318/");
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
