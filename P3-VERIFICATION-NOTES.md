\# P3 Build \& Verification Notes — 16 Aug 2026



\- Merged latest `main` into `skDev` (P1 load generator + StoreSim changes)

\- `dotnet build` on P3.Core and P3.Dashboard: both succeed clean on .NET 8 (SDK 10.0.302)

\- Ran P3.Dashboard against SampleData/sample\_runs.json:

&#x20; - Live Dashboard correctly displays endpoint metrics and regression severity

&#x20;   (POST /checkout: Major, 95.2% vs baseline; GET /cart: None, 2.5%)

&#x20; - Historical Trends charts render correctly for both endpoints

\- Verified QA report export (Markdown) against P3 acceptance criteria:

&#x20; - Breach log correctly lists the Major regression with endpoint, build IDs, % change

&#x20; - Anomaly log correctly filters on AnomalyDetected == true and shows the

&#x20;   fallback message when nothing is flagged

&#x20; - Historical trend table matches dashboard chart data

\- Known issue: P2.LoadGenerator is missing its .csproj on main, so the full

&#x20; .slnx doesn't build — flagged to teammate; built P3.Core/P3.Dashboard directly as workaround

