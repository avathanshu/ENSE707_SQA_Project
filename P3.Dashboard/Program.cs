// Program.cs
// Startup/entry point for the dashboard. Wires up Blazor Server hosting and
// registers P3.Core's services (store, detector, exporter) for dependency
// injection so pages can just @inject them instead of "new"-ing them up.

using P3.Core.Persistence;
using P3.Core.Regression;
using P3.Core.Reporting;

// WebApplication.CreateBuilder sets up configuration, logging, and DI
// container using ASP.NET Core's default conventions.
var builder = WebApplication.CreateBuilder(args);

// Registers the services needed to host Razor Pages + interactive Blazor
// Server components (this is the "classic" Blazor Server hosting model).
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// --- P3.Core service registrations ---
// AddSingleton: one shared instance for the whole app's lifetime. That's
// appropriate here because JsonTestRunStore itself is thread-safe (it locks
// internally) and we want every page/user to see the same persisted data.

// Path.Combine builds an OS-agnostic path (works on Windows, macOS, Linux)
// to the shared JSON store, kept OUTSIDE the dashboard project folder so
// P1/P2's tooling and this dashboard can point at the exact same file.
var dataFilePath = Path.Combine(builder.Environment.ContentRootPath, "..", "SampleData", "runs.json");
builder.Services.AddSingleton<ITestRunStore>(new JsonTestRunStore(dataFilePath));

// The regression detector is stateless (only reads its constructor-supplied
// thresholds), so a singleton is fine here too.
builder.Services.AddSingleton<IRegressionDetector>(new ThresholdRegressionDetector());

// Registered as its interface so pages depend on IReportExporter, not the
// concrete Markdown implementation — swapping in a PDF exporter later
// wouldn't require changing any page code.
builder.Services.AddSingleton<IReportExporter, MarkdownReportExporter>();

var app = builder.Build();

// Standard ASP.NET Core middleware pipeline setup.
if (!app.Environment.IsDevelopment())
{
    // In production, show a friendly error page instead of a stack trace.
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();   // serves wwwroot/ files (css, images, etc.)
app.UseRouting();       // enables endpoint routing used below

// Maps the SignalR hub Blazor Server uses to push UI updates to the browser.
app.MapBlazorHub();

// Any URL that doesn't match a Razor Page falls back to _Host.cshtml, which
// bootstraps the Blazor app (App.razor) client-side.
app.MapFallbackToPage("/_Host");

// Starts the web server and blocks until the process is stopped.
app.Run();
