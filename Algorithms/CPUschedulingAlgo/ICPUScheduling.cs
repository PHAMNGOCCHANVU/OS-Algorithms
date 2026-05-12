using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.CPUschedulingAlgo;

/// <summary>
/// Interface for CPU scheduling algorithms.
/// </summary>
public interface ICPUScheduling
{
    /// <summary>
    /// Initialize algorithm with processes and parameters.
    /// </summary>
    void Initialize(List<Process> processes, int quantumTime = 0);

    /// <summary>
    /// Execute scheduling algorithm and return results.
    /// </summary>
    (List<Process> Processes, List<(int ProcessId, int EndTime)> GanttChart, double AvgWT, double AvgTAT, double Throughput) Execute();
}
