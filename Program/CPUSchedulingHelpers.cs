using PageReplacementDemo.Algorithms.CPUschedulingAlgo;
using PageReplacementDemo.Models;

namespace PageReplacementDemo.Program;

/// <summary>
/// CPU Scheduling specific helpers for display and comparison
/// </summary>
public static class CPUSchedulingHelpers
{
    /// <summary>
    /// Display results with Gantt chart and metrics
    /// </summary>
    public static void DisplayCPUSchedulingResults(int algorithmChoice, 
        List<Process> processes, 
        List<(int, int)> ganttChart, 
        double avgWT, 
        double avgTAT, 
        double throughput)
    {
        string algorithmName = GetAlgorithmName(algorithmChoice);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n╔════════════════════ {algorithmName.PadRight(20)} ════════════════════╗");
        Console.ResetColor();

        // Display Gantt Chart
        GanttChart.Display(ganttChart);

        // Display results table
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("PID\tAT\tBT\tPR\tCT\tTAT\tWT");
        Console.ResetColor();

        foreach (var proc in processes)
        {
            Console.WriteLine($"{proc.Id}\t{proc.ArrivalTime}\t{proc.BurstTime}\t{proc.Priority}\t" +
                $"{proc.CompletionTime}\t{proc.TurnaroundTime}\t{proc.WaitingTime}");
        }

        // Display metrics
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nAvg WT: {avgWT:F2} | Avg TAT: {avgTAT:F2} | Throughput: {throughput:F4}");
        Console.ResetColor();
    }

    /// <summary>
    /// Display comparison of all 5 algorithms
    /// </summary>
    public static void DisplayComparisonTable(Dictionary<int, (double AvgWT, double AvgTAT, double Throughput)> results)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                         BANG SO SANH THUAT TOAN                      ║");
        Console.WriteLine("╠═══════════════════╦═════════════════╦═════════════════╦══════════════╣");
        Console.WriteLine("║  Thuat Toan       ║   Avg WT        ║   Avg TAT       ║ Throughput   ║");
        Console.WriteLine("╠═══════════════════╬═════════════════╬═════════════════╬══════════════╣");

        for (int i = 1; i <= 5; i++)
        {
            if (results.ContainsKey(i))
            {
                var (avgWT, avgTAT, throughput) = results[i];
                string name = GetAlgorithmName(i).PadRight(17);
                Console.WriteLine($"║ {name} ║ {avgWT,13:F2} ║ {avgTAT,13:F2} ║ {throughput,10:F4} ║");
            }
        }

        Console.WriteLine("╚═══════════════════╩═════════════════╩═════════════════╩══════════════╝");
        Console.ResetColor();
    }

    /// <summary>
    /// Get algorithm name from choice
    /// </summary>
    public static string GetAlgorithmName(int choice)
    {
        return choice switch
        {
            1 => "FCFS",
            2 => "SJF",
            3 => "SRTF",
            4 => "Round Robin",
            5 => "Priority",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Run all algorithms and return results for comparison
    /// </summary>
    public static Dictionary<int, (double AvgWT, double AvgTAT, double Throughput)> RunAllAlgorithms(
        List<Process> processes, 
        int quantumTime)
    {
        var results = new Dictionary<int, (double, double, double)>();

        // Make copies for each algorithm
        var copyProcesses = () => processes.Select(p => new Process(p)).ToList();

        // FCFS
        Console.WriteLine("\n>>> [1] FCFS");
        var (procsFCFS, ganttFCFS, wtFCFS, tatFCFS, tpFCFS) = CPUSchedulingLogic.SolveFCFS(copyProcesses());
        DisplayCPUSchedulingResults(1, procsFCFS, ganttFCFS, wtFCFS, tatFCFS, tpFCFS);
        results[1] = (wtFCFS, tatFCFS, tpFCFS);

        // SJF
        Console.WriteLine("\n>>> [2] SJF");
        var (procsSJF, ganttSJF, wtSJF, tatSJF, tpSJF) = CPUSchedulingLogic.SolveSJF(copyProcesses());
        DisplayCPUSchedulingResults(2, procsSJF, ganttSJF, wtSJF, tatSJF, tpSJF);
        results[2] = (wtSJF, tatSJF, tpSJF);

        // SRTF
        Console.WriteLine("\n>>> [3] SRTF");
        var (procsSRTF, ganttSRTF, wtSRTF, tatSRTF, tpSRTF) = CPUSchedulingLogic.SolveSRTF(copyProcesses());
        DisplayCPUSchedulingResults(3, procsSRTF, ganttSRTF, wtSRTF, tatSRTF, tpSRTF);
        results[3] = (wtSRTF, tatSRTF, tpSRTF);

        // Round Robin
        Console.WriteLine("\n>>> [4] ROUND ROBIN");
        var (procsRR, ganttRR, wtRR, tatRR, tpRR) = CPUSchedulingLogic.SolveRoundRobin(copyProcesses(), quantumTime);
        DisplayCPUSchedulingResults(4, procsRR, ganttRR, wtRR, tatRR, tpRR);
        results[4] = (wtRR, tatRR, tpRR);

        // Priority
        Console.WriteLine("\n>>> [5] PRIORITY");
        var (procsPrio, ganttPrio, wtPrio, tatPrio, tpPrio) = CPUSchedulingLogic.SolvePriority(copyProcesses());
        DisplayCPUSchedulingResults(5, procsPrio, ganttPrio, wtPrio, tatPrio, tpPrio);
        results[5] = (wtPrio, tatPrio, tpPrio);

        return results;
    }
}
