using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using SJP.Accounting.Application;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.Reports.BalanceSheet;
using SJP.Accounting.Application.Reports.PartnerSettlement;
using SJP.Accounting.Infrastructure;
using SJP.Accounting.Infrastructure.Report;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices(builder.Configuration);

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}