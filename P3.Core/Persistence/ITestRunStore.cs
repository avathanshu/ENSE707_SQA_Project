// ITestRunStore.cs
// Abstraction over "however we persist test runs". The regression detector,
// dashboard, and report exporter all talk to this interface instead of a
// concrete database/file class — so we can swap JSON-file storage for
// SQLite/EF Core later (e.g. for the final submission) without changing
// any code that consumes test runs.

using System.Collections.Generic;
using P3.Core.Models;

namespace P3.Core.Persistence
{
    public interface ITestRunStore
    {
        // Persist a single new test run. Implementations must be safe to call
        // repeatedly as new runs complete (this is how "results are persisted
        // across multiple test runs/builds" is satisfied).
        void SaveRun(TestRun run);

        // Return every run ever stored, ordered oldest-first by Timestamp.
        // Used to build the full historical trend view.
        IReadOnlyList<TestRun> GetAllRuns();

        // Return only the runs recorded against a specific build ID.
        // A single build can have multiple runs (e.g. one per load profile).
        IReadOnlyList<TestRun> GetRunsForBuild(string buildId);
    }
}
