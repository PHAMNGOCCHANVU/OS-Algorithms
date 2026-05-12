using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.CPUschedulingAlgo;

/// <summary>
/// Priority Scheduling - Non-preemptive scheduling.
/// Among ready processes, execute the one with highest priority (lowest priority number).
/// </summary>
public class PriorityAlgorithm : ICPUScheduling
{
    private List<Process> _processes = new();
    private List<(int ProcessId, int EndTime)> _ganttChart = new();

    public void Initialize(List<Process> processes, int quantumTime = 0)
    {
        _processes = new List<Process>(processes);
        _ganttChart.Clear();
    }

    public (List<Process>, List<(int, int)>, double, double, double) Execute()
    {
        int n = _processes.Count;
        int time = 0;
        int completed = 0;
        double totalWT = 0, totalTAT = 0;
        int minAT = _processes.Min(p => p.ArrivalTime);

        var done = new bool[n];

        while (completed < n)
        {
            int idx = -1;

            // Find ready process with highest priority (lowest priority number)
            for (int i = 0; i < n; i++)
            {
                if (done[i] || _processes[i].ArrivalTime > time)
                    continue;

                if (idx == -1)
                    idx = i;
                else if (_processes[i].Priority < _processes[idx].Priority)
                    idx = i;
                else if (_processes[i].Priority == _processes[idx].Priority && 
                         _processes[i].ArrivalTime < _processes[idx].ArrivalTime)
                    idx = i;
            }

            if (idx == -1)
            {
                time++;
            }
            else
            {
                time += _processes[idx].BurstTime;
                _processes[idx].CompletionTime = time;
                _processes[idx].TurnaroundTime = _processes[idx].CompletionTime - _processes[idx].ArrivalTime;
                _processes[idx].WaitingTime = _processes[idx].TurnaroundTime - _processes[idx].BurstTime;

                totalWT += _processes[idx].WaitingTime;
                totalTAT += _processes[idx].TurnaroundTime;

                done[idx] = true;
                completed++;
                _ganttChart.Add((_processes[idx].Id, time));
            }
        }

        double avgWT = totalWT / _processes.Count;
        double avgTAT = totalTAT / _processes.Count;
        int maxCT = _processes.Max(p => p.CompletionTime);
        double throughput = (double)_processes.Count / (maxCT - minAT);

        return (_processes, _ganttChart, avgWT, avgTAT, throughput);
    }
}
