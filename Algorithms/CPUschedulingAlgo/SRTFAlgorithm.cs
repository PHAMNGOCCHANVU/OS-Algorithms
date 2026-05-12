using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.CPUschedulingAlgo;

/// <summary>
/// SRTF (Shortest Remaining Time First) - Preemptive scheduling.
/// Among ready processes, execute the one with smallest remaining time.
/// </summary>
public class SRTFAlgorithm : ICPUScheduling
{
    private List<Process> _processes = new();
    private List<(int ProcessId, int EndTime)> _ganttChart = new();

    public void Initialize(List<Process> processes, int quantumTime = 0)
    {
        _processes = new List<Process>(processes);
        _ganttChart.Clear();

        // Initialize remaining time
        foreach (var p in _processes)
            p.RemainingTime = p.BurstTime;
    }

    public (List<Process>, List<(int, int)>, double, double, double) Execute()
    {
        int n = _processes.Count;
        int time = 0;
        int completed = 0;
        double totalWT = 0, totalTAT = 0;
        int minAT = _processes.Min(p => p.ArrivalTime);
        int prevId = -1;

        while (completed < n)
        {
            int idx = -1;

            // Find ready process with smallest remaining time
            for (int i = 0; i < n; i++)
            {
                if (_processes[i].ArrivalTime > time || _processes[i].RemainingTime == 0)
                    continue;

                if (idx == -1)
                    idx = i;
                else if (_processes[i].RemainingTime < _processes[idx].RemainingTime)
                    idx = i;
                else if (_processes[i].RemainingTime == _processes[idx].RemainingTime && 
                         _processes[i].ArrivalTime < _processes[idx].ArrivalTime)
                    idx = i;
            }

            if (idx == -1)
            {
                if (prevId != -1)
                    prevId = -1;
                time++;
                continue;
            }

            // Record context switch for Gantt chart
            if (_processes[idx].Id != prevId)
            {
                if (prevId != -1)
                    _ganttChart.Add((prevId, time));
                prevId = _processes[idx].Id;
            }

            _processes[idx].RemainingTime--;
            time++;

            if (_processes[idx].RemainingTime == 0)
            {
                _processes[idx].CompletionTime = time;
                _processes[idx].TurnaroundTime = _processes[idx].CompletionTime - _processes[idx].ArrivalTime;
                _processes[idx].WaitingTime = _processes[idx].TurnaroundTime - _processes[idx].BurstTime;

                totalWT += _processes[idx].WaitingTime;
                totalTAT += _processes[idx].TurnaroundTime;

                _ganttChart.Add((_processes[idx].Id, time));
                prevId = -1;
                completed++;
            }
        }

        double avgWT = totalWT / _processes.Count;
        double avgTAT = totalTAT / _processes.Count;
        int maxCT = _processes.Max(p => p.CompletionTime);
        double throughput = (double)_processes.Count / (maxCT - minAT);

        return (_processes, _ganttChart, avgWT, avgTAT, throughput);
    }
}
