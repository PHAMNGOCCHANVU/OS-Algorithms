using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.CPUschedulingAlgo;

/// <summary>
/// Executor for CPU Scheduling algorithms.
/// Responsible for creating and executing the selected algorithm.
/// </summary>
public class CPUSchedulingExecutor
{
    public static ICPUScheduling CreateAlgorithm(int choice)
    {
        return choice switch
        {
            1 => new FCFSAlgorithm(),
            2 => new SJFAlgorithm(),
            3 => new SRTFAlgorithm(),
            4 => new RoundRobinAlgorithm(),
            5 => new PriorityAlgorithm(),
            _ => throw new ArgumentException("Invalid algorithm choice")
        };
    }

    public static string GetAlgorithmName(int choice)
    {
        return choice switch
        {
            1 => "FCFS (First Come First Served)",
            2 => "SJF (Shortest Job First)",
            3 => "SRTF (Shortest Remaining Time First)",
            4 => "Round Robin",
            5 => "Priority Scheduling",
            _ => "Unknown"
        };
    }

    public static (List<Process> Processes, List<(int, int)> GanttChart, double AvgWT, double AvgTAT, double Throughput) 
        ExecuteAlgorithm(List<Process> processes, int algorithmChoice, int quantumTime = 0)
    {
        var algorithm = CreateAlgorithm(algorithmChoice);
        algorithm.Initialize(processes, quantumTime);
        return algorithm.Execute();
    }
}
