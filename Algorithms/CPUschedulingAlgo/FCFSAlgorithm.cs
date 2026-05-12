using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.CPUschedulingAlgo;

/// <summary>
/// FCFS (First Come First Served) - Non-preemptive scheduling.
/// Processes are executed in the order they arrive.
/// </summary>
public class FCFSAlgorithm : ICPUScheduling
{
    private List<Process> _processes = new();
    private List<(int ProcessId, int EndTime)> _ganttChart = new();

    public void Initialize(List<Process> processes, int quantumTime = 0)
    {
        _processes = processes.OrderBy(p => p.ArrivalTime).ToList();
        _ganttChart.Clear();
    }

    public (List<Process>, List<(int, int)>, double, double, double) Execute()
    {
        int time = 0;
        double totalWT = 0, totalTAT = 0;
        int minAT = _processes.Min(p => p.ArrivalTime);

        for (int i = 0; i < _processes.Count; i++)
        {
            // If CPU is idle, fast forward to process arrival
            if (time < _processes[i].ArrivalTime)
                time = _processes[i].ArrivalTime;

            // Execute process
            time += _processes[i].BurstTime;
            _processes[i].CompletionTime = time;
            _processes[i].TurnaroundTime = _processes[i].CompletionTime - _processes[i].ArrivalTime;
            _processes[i].WaitingTime = _processes[i].TurnaroundTime - _processes[i].BurstTime;

            totalWT += _processes[i].WaitingTime;
            totalTAT += _processes[i].TurnaroundTime;

            _ganttChart.Add((_processes[i].Id, time));
        }

        double avgWT = totalWT / _processes.Count;
        double avgTAT = totalTAT / _processes.Count;
        int maxCT = _processes.Max(p => p.CompletionTime);
        double throughput = (double)_processes.Count / (maxCT - minAT);

        return (_processes, _ganttChart, avgWT, avgTAT, throughput);
    }
}
