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
    /// Load Deadlock test case from file
    /// Format:
    /// Line 1: P R (number of processes, number of resources)
    /// Lines 2 to P+1: Max[i][0] Max[i][1] ... (max resources for process i)
    /// Lines P+2 to 2P+1: Allocation[i][0] Allocation[i][1] ... (allocated resources for process i)
    /// Line 2P+2: Available[0] Available[1] ... (available resources)
    /// </summary>
    public static (int NumProc, int NumRes, int[][] Max, int[][] Allocation, int[] Available) 
        LoadDeadlockTestCase(string filename)
    {
        var filePath = Path.Combine(TestDataFolder, filename);
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Test case file not found: {filePath}");

        var lines = File.ReadAllLines(filePath);
        if (lines.Length < 2)
            throw new InvalidOperationException("Invalid test case file format");

        var firstLine = lines[0].Trim().Split();
        int p = int.Parse(firstLine[0]);
        int r = int.Parse(firstLine[1]);

        // Read Max matrix
        var max = new int[p][];
        for (int i = 0; i < p; i++)
        {
            var parts = lines[i + 1].Trim().Split();
            max[i] = Array.ConvertAll(parts, int.Parse);
        }

        // Read Allocation matrix
        var allocation = new int[p][];
        for (int i = 0; i < p; i++)
        {
            var parts = lines[p + i + 1].Trim().Split();
            allocation[i] = Array.ConvertAll(parts, int.Parse);
        }

        // Read Available resources
        var availParts = lines[2 * p + 1].Trim().Split();
        var available = Array.ConvertAll(availParts, int.Parse);

        return (p, r, max, allocation, available);
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
