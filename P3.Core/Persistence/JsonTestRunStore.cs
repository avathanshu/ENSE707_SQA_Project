// JsonTestRunStore.cs
// A simple, dependency-free implementation of ITestRunStore that persists
// all test runs to a single JSON file on disk. This satisfies the project's
// "fully self-contained and offline" requirement (no database server, no
// external NuGet packages beyond the .NET base class library) and is more
// than enough for a semester-project's data volume.
//
// Storage strategy: the whole run history is kept in memory and rewritten
// to disk on every save ("read-modify-write"). That's simple and safe for
// a project of this scale; if the dataset ever got large, this would be the
// first thing to swap for SQLite behind the same ITestRunStore interface.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using P3.Core.Models;

namespace P3.Core.Persistence
{
    public class JsonTestRunStore : ITestRunStore
    {
        // Full path to the JSON file used as the backing store.
        private readonly string _filePath;

        // A lock object so concurrent calls (e.g. dashboard reading while a
        // new run is being saved) don't corrupt the file or the in-memory list.
        private readonly object _lock = new();

        // Options that make the JSON file human-readable (indented) so it's
        // easy to inspect/debug and diff in Git — useful while the team is
        // still agreeing on the data shape.
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        // Constructor: takes the path to the JSON file to use for storage.
        // If the file (or its parent directory) doesn't exist yet, create both
        // so the caller doesn't need to do any setup before first use.
        public JsonTestRunStore(string filePath)
        {
            _filePath = filePath;

            // Directory.GetParent can return null for a bare filename with no
            // folder component, so only create a directory if one was given.
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // If this is the first time the store is used, seed the file with
            // an empty JSON array so later reads don't have to special-case
            // a missing/empty file.
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        // Adds one new run to the store and writes the updated history to disk.
        public void SaveRun(TestRun run)
        {
            // lock (...) ensures only one thread at a time can read-modify-write
            // the file, preventing two simultaneous saves from overwriting
            // each other's changes.
            lock (_lock)
            {
                var allRuns = ReadAllFromDisk();  // load current history
                allRuns.Add(run);                 // append the new run
                WriteAllToDisk(allRuns);          // persist the full, updated list
            }
        }

        // Returns every stored run, ordered oldest-first so trend charts and
        // regression comparisons can rely on chronological order.
        public IReadOnlyList<TestRun> GetAllRuns()
        {
            lock (_lock)
            {
                return ReadAllFromDisk()
                    .OrderBy(r => r.Timestamp)   // ascending: oldest run first
                    .ToList();
            }
        }

        // Returns only the runs recorded against the given build ID.
        public IReadOnlyList<TestRun> GetRunsForBuild(string buildId)
        {
            lock (_lock)
            {
                return ReadAllFromDisk()
                    // StringComparison.OrdinalIgnoreCase avoids "build-1" vs
                    // "Build-1" being treated as different builds by accident.
                    .Where(r => string.Equals(r.BuildId, buildId, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(r => r.Timestamp)
                    .ToList();
            }
        }

        // Reads the JSON file from disk and deserializes it into a List<TestRun>.
        // Kept private: callers only ever see the interface methods above.
        private List<TestRun> ReadAllFromDisk()
        {
            var json = File.ReadAllText(_filePath);

            // Guard against an empty file (e.g. if it was manually cleared)
            // so JsonSerializer doesn't throw on an empty string.
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<TestRun>();
            }

            // Deserialize into a strongly-typed list. The "?? new()" fallback
            // covers the edge case where the file contains the literal "null".
            return JsonSerializer.Deserialize<List<TestRun>>(json, _jsonOptions) ?? new List<TestRun>();
        }

        // Serializes the given list back to the JSON file, overwriting it.
        private void WriteAllToDisk(List<TestRun> runs)
        {
            var json = JsonSerializer.Serialize(runs, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
    }
}
