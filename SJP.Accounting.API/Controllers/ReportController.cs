using Microsoft.AspNetCore.Mvc;
using SJP.Accounting.Application.Contracts;

namespace SJP.Accounting.Api.Controllers;

[ApiController]
[Route("api/v1/reports")]
public sealed class ReportController : ControllerBase
{
    private readonly IEnumerable<ISJPAccountingReport> _reports;

    public ReportController(IEnumerable<ISJPAccountingReport> reports)
    {
        _reports = reports;
    }

    [HttpGet]
    public IActionResult GetReports()
    {
        var reports = _reports
            .Select(x => 
                new { x.ReportCode
                , x.ReportName
                , ExportUrl = Url.Action(nameof(Export), "Report", values: new { reportCode = x.ReportCode }, Request.Scheme )})
            .OrderBy(x => x.ReportName);

        return Ok(reports);
    }

    [HttpGet("{reportCode}")]
    public async Task<IActionResult> Export(string reportCode, string exportType = "Pdf" , CancellationToken cancellationToken = default)
    {
        var report = _reports.SingleOrDefault(x => x.ReportCode.Equals(reportCode, StringComparison.OrdinalIgnoreCase));
        if (report is null)
        {
            return NotFound();
        }
        var files = await report.ExportAsync(cancellationToken);
        var pdf = files[exportType];
        return File(pdf, "application/pdf", $"{report.ReportCode}.pdf");
    }
}