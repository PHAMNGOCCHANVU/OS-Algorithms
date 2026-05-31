using PageReplacementDemo.Models;

namespace PageReplacementDemo.Program;

/// <summary>
/// Helper class for loading test data from files
/// </summary>
public static class FileHelpers
{
    private static readonly string TestDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestData");

    /// <summary>
    /// Load CPU Scheduling test case from file
    /// Format: 
    /// Line 1: N (number of processes)
    /// Lines 2 to N+1: AT BT PR (arrival time, burst time, priority)
    /// Line N+2: QuantumTime
    /// </summary>
    public static (List<Process> Processes, int QuantumTime) LoadCPUSchedulingTestCase(string filename)
    {
        var filePath = Path.Combine(TestDataFolder, filename);
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Test case file not found: {filePath}");

        var lines = File.ReadAllLines(filePath);
        if (lines.Length < 2)
            throw new InvalidOperationException("Invalid test case file format");

        int n = int.Parse(lines[0].Trim());
        var processes = new List<Process>();

        for (int i = 0; i < n; i++)
        {
            var parts = lines[i + 1].Trim().Split();
            if (parts.Length < 2)
                throw new InvalidOperationException($"Invalid process format at line {i + 2}");

            int at = int.Parse(parts[0]);
            int bt = int.Parse(parts[1]);
            int pr = parts.Length > 2 ? int.Parse(parts[2]) : 0;

            processes.Add(new Process(i + 1, at, bt, pr));
        }

        int quantum = lines.Length > n + 1 ? int.Parse(lines[n + 1].Trim()) : 3;

        return (processes, quantum);
    }

    /// <summary>
    /// Load Deadlock test case from file (same format as OS/deadlock.cpp readFromFile).
    /// Format:
    /// Line 1: P R (number of processes, number of resources)
    /// Line 2: Total[0] Total[1] ... (total resources in the system)
    /// Lines 3 to P+2: Max[i][0] Max[i][1] ...
    /// Lines P+3 to 2P+2: Allocation[i][0] Allocation[i][1] ...
    /// Available and Need are computed by CalculateInitialState after load.
    /// </summary>
    public static (int NumProc, int NumRes, int[] Total, int[][] Max, int[][] Allocation)
        LoadDeadlockTestCase(string filename)
    {
        var filePath = Path.Combine(TestDataFolder, filename);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Test case file not found: {filePath}");

        var lines = File.ReadAllLines(filePath)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .ToArray();

        if (lines.Length < 2)
            throw new InvalidOperationException("Invalid test case file format");

        var firstLine = lines[0].Split();
        int p = int.Parse(firstLine[0]);
        int r = int.Parse(firstLine[1]);

        int expectedLines = 2 + p + p;
        if (lines.Length < expectedLines)
            throw new InvalidOperationException(
                $"Invalid test case file format: expected {expectedLines} lines, got {lines.Length}");

        var total = lines[1].Split().Select(int.Parse).ToArray();
        if (total.Length != r)
            throw new InvalidOperationException($"Total resources line must have {r} values");

        var max = new int[p][];
        for (int i = 0; i < p; i++)
        {
            var parts = lines[i + 2].Split().Select(int.Parse).ToArray();
            if (parts.Length != r)
                throw new InvalidOperationException($"Max matrix row P{i + 1} must have {r} values");
            max[i] = parts;
        }

        var allocation = new int[p][];
        for (int i = 0; i < p; i++)
        {
            var parts = lines[i + 2 + p].Split().Select(int.Parse).ToArray();
            if (parts.Length != r)
                throw new InvalidOperationException($"Allocation matrix row P{i + 1} must have {r} values");
            allocation[i] = parts;
        }

        return (p, r, total, max, allocation);
    }

    /// <summary>
    /// List all available test cases
    /// </summary>
    public static List<string> GetAvailableCPUTestCases()
    {
        if (!Directory.Exists(TestDataFolder))
            return new List<string>();

        var files = Directory.GetFiles(TestDataFolder, "CPUScheduling_*.txt");
        return files.Select(f => Path.GetFileName(f)).ToList();
    }

    /// <summary>
    /// List all available deadlock test cases
    /// </summary>
    public static List<string> GetAvailableDeadlockTestCases()
    {
        if (!Directory.Exists(TestDataFolder))
            return new List<string>();

        var files = Directory.GetFiles(TestDataFolder, "Deadlock_*.txt");
        return files.Select(f => Path.GetFileName(f)).ToList();
    }
}
