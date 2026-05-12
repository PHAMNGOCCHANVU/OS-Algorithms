using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.CPUschedulingAlgo;

/// <summary>
/// Round Robin (RR) - Preemptive scheduling.
/// Each process gets a time quantum. If not completed, goes to back of queue.
/// </summary>
public class RoundRobinAlgorithm : ICPUScheduling
{
    private List<Process> _processes = new();
    private List<(int ProcessId, int EndTime)> _ganttChart = new();
    private int _quantumTime = 0;

    public void Initialize(List<Process> processes, int quantumTime = 0)
    {
        _processes = new List<Process>(processes.OrderBy(p => p.ArrivalTime).ToList());
        _quantumTime = quantumTime > 0 ? quantumTime : 2; // Default quantum = 2
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

        var readyQueue = new Queue<int>();
        var inQueue = new bool[n];
        int ptr = 0;

        // Load initial processes
        while (ptr < n && _processes[ptr].ArrivalTime <= time)
        {
            readyQueue.Enqueue(ptr);
            inQueue[ptr] = true;
            ptr++;
        }

        while (completed < n)
        {
            if (readyQueue.Count == 0)
            {
                time = _processes[ptr].ArrivalTime;
                while (ptr < n && _processes[ptr].ArrivalTime <= time)
                {
                    readyQueue.Enqueue(ptr);
                    inQueue[ptr] = true;
                    ptr++;
                }
                continue;
            }

            int curr = readyQueue.Dequeue();
            int runTime = Math.Min(_quantumTime, _processes[curr].RemainingTime);

            _processes[curr].RemainingTime -= runTime;
            time += runTime;
            _ganttChart.Add((_processes[curr].Id, time));

            // Load new processes that arrived during execution
            while (ptr < n && _processes[ptr].ArrivalTime <= time)
            {
                if (!inQueue[ptr])
                {
                    readyQueue.Enqueue(ptr);
                    inQueue[ptr] = true;
                }
                ptr++;
            }

            if (_processes[curr].RemainingTime == 0)
            {
                _processes[curr].CompletionTime = time;
                _processes[curr].TurnaroundTime = _processes[curr].CompletionTime - _processes[curr].ArrivalTime;
                _processes[curr].WaitingTime = _processes[curr].TurnaroundTime - _processes[curr].BurstTime;

                totalWT += _processes[curr].WaitingTime;
                totalTAT += _processes[curr].TurnaroundTime;

                completed++;
            }
            else
            {
                readyQueue.Enqueue(curr);
            }
        }

        // Sort by ID for output
        _processes.Sort((a, b) => a.Id.CompareTo(b.Id));

        double avgWT = totalWT / _processes.Count;
        double avgTAT = totalTAT / _processes.Count;
        int maxCT = _processes.Max(p => p.CompletionTime);
        double throughput = (double)_processes.Count / (maxCT - minAT);

        return (_processes, _ganttChart, avgWT, avgTAT, throughput);
    }
}
