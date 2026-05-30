using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.CPUschedulingAlgo;

/// <summary>
/// CPU Scheduling Logic - contains all 5 scheduling algorithms
/// FCFS, SJF, SRTF, Round Robin, Priority Scheduling
/// </summary>
public static class CPUSchedulingLogic
{
    /// <summary>
    /// FCFS (First Come First Served) - Non-preemptive
    /// </summary>
    public static (List<Process>, List<(int, int)>, double, double, double) SolveFCFS(List<Process> processes)
    {
        var procs = new List<Process>(processes);
        procs = procs.OrderBy(p => p.ArrivalTime).ToList();

        int time = 0;
        var gantt = new List<(int, int)>();
        double totalWT = 0, totalTAT = 0;
        int minAT = procs.Min(p => p.ArrivalTime);

        foreach (var proc in procs)
        {
            if (time < proc.ArrivalTime)
            {
                gantt.Add((0, proc.ArrivalTime)); // Idle period
                time = proc.ArrivalTime;
            }

            time += proc.BurstTime;
            proc.CompletionTime = time;
            proc.TurnaroundTime = proc.CompletionTime - proc.ArrivalTime;
            proc.WaitingTime = proc.TurnaroundTime - proc.BurstTime;

            totalWT += proc.WaitingTime;
            totalTAT += proc.TurnaroundTime;
            gantt.Add((proc.Id, time));
        }

        double avgWT = totalWT / procs.Count;
        double avgTAT = totalTAT / procs.Count;
        int maxCT = procs.Max(p => p.CompletionTime);
        double throughput = (double)procs.Count / (maxCT - minAT);

        return (procs, gantt, avgWT, avgTAT, throughput);
    }

    /// <summary>
    /// SJF (Shortest Job First) - Non-preemptive
    /// Tie-breaker: Earlier arrival time
    /// </summary>
    public static (List<Process>, List<(int, int)>, double, double, double) SolveSJF(List<Process> processes)
    {
        var procs = new List<Process>(processes);
        int n = procs.Count;
        int time = 0;
        int completed = 0;
        var gantt = new List<(int, int)>();
        var done = new bool[n];
        double totalWT = 0, totalTAT = 0;
        int minAT = procs.Min(p => p.ArrivalTime);

        while (completed < n)
        {
            int idx = -1;

            // Find ready process with shortest burst time
            for (int i = 0; i < n; i++)
            {
                if (done[i] || procs[i].ArrivalTime > time)
                    continue;

                if (idx == -1)
                    idx = i;
                else if (procs[i].BurstTime < procs[idx].BurstTime)
                    idx = i;
                else if (procs[i].BurstTime == procs[idx].BurstTime &&
                         procs[i].ArrivalTime < procs[idx].ArrivalTime)
                    idx = i;
            }

            if (idx == -1)
            {
                time++;
            }
            else
            {
                time += procs[idx].BurstTime;
                procs[idx].CompletionTime = time;
                procs[idx].TurnaroundTime = procs[idx].CompletionTime - procs[idx].ArrivalTime;
                procs[idx].WaitingTime = procs[idx].TurnaroundTime - procs[idx].BurstTime;

                totalWT += procs[idx].WaitingTime;
                totalTAT += procs[idx].TurnaroundTime;

                done[idx] = true;
                completed++;
                gantt.Add((procs[idx].Id, time));
            }
        }

        double avgWT = totalWT / procs.Count;
        double avgTAT = totalTAT / procs.Count;
        int maxCT = procs.Max(p => p.CompletionTime);
        double throughput = (double)procs.Count / (maxCT - minAT);

        return (procs, gantt, avgWT, avgTAT, throughput);
    }

    /// <summary>
    /// SRTF (Shortest Remaining Time First) - Preemptive
    /// Tie-breaker: Earlier arrival time
    /// </summary>
    public static (List<Process>, List<(int, int)>, double, double, double) SolveSRTF(List<Process> processes)
    {
        var procs = new List<Process>(processes);
        int n = procs.Count;
        int time = 0;
        int completed = 0;
        var gantt = new List<(int, int)>();
        int prevId = -1;
        double totalWT = 0, totalTAT = 0;
        int minAT = procs.Min(p => p.ArrivalTime);

        // Initialize remaining time
        foreach (var p in procs)
            p.RemainingTime = p.BurstTime;

        while (completed < n)
        {
            int idx = -1;

            // Find ready process with shortest remaining time
            for (int i = 0; i < n; i++)
            {
                if (procs[i].ArrivalTime > time || procs[i].RemainingTime == 0)
                    continue;

                if (idx == -1)
                    idx = i;
                else if (procs[i].RemainingTime < procs[idx].RemainingTime)
                    idx = i;
                else if (procs[i].RemainingTime == procs[idx].RemainingTime &&
                         procs[i].ArrivalTime < procs[idx].ArrivalTime)
                    idx = i;
            }

            if (idx == -1)
            {
                if (prevId != -1)
                    prevId = -1;
                time++;
                continue;
            }

            // Record context switch
            if (procs[idx].Id != prevId)
            {
                if (prevId != -1)
                    gantt.Add((prevId, time));
                prevId = procs[idx].Id;
            }

            procs[idx].RemainingTime--;
            time++;

            if (procs[idx].RemainingTime == 0)
            {
                procs[idx].CompletionTime = time;
                procs[idx].TurnaroundTime = procs[idx].CompletionTime - procs[idx].ArrivalTime;
                procs[idx].WaitingTime = procs[idx].TurnaroundTime - procs[idx].BurstTime;

                totalWT += procs[idx].WaitingTime;
                totalTAT += procs[idx].TurnaroundTime;

                gantt.Add((procs[idx].Id, time));
                prevId = -1;
                completed++;
            }
        }

        double avgWT = totalWT / procs.Count;
        double avgTAT = totalTAT / procs.Count;
        int maxCT = procs.Max(p => p.CompletionTime);
        double throughput = (double)procs.Count / (maxCT - minAT);

        return (procs, gantt, avgWT, avgTAT, throughput);
    }

    /// <summary>
    /// Round Robin - Preemptive with time quantum
    /// </summary>
    public static (List<Process>, List<(int, int)>, double, double, double) SolveRoundRobin(List<Process> processes, int quantumTime)
    {
        var procs = new List<Process>(processes);
        procs = procs.OrderBy(p => p.ArrivalTime).ToList();

        int n = procs.Count;
        int time = 0;
        int completed = 0;
        var gantt = new List<(int, int)>();
        var rq = new Queue<int>();
        var inQueue = new bool[n];
        double totalWT = 0, totalTAT = 0;
        int minAT = procs.Min(p => p.ArrivalTime);

        // Initialize remaining time
        foreach (var p in procs)
            p.RemainingTime = p.BurstTime;

        int ptr = 0;

        // Load initial processes
        while (ptr < n && procs[ptr].ArrivalTime <= time)
        {
            rq.Enqueue(ptr);
            inQueue[ptr] = true;
            ptr++;
        }

        while (completed < n)
        {
            if (rq.Count == 0)
            {
                if (ptr < n)
                {
                    gantt.Add((0, procs[ptr].ArrivalTime)); // Idle
                    time = procs[ptr].ArrivalTime;
                    while (ptr < n && procs[ptr].ArrivalTime <= time)
                    {
                        rq.Enqueue(ptr);
                        inQueue[ptr] = true;
                        ptr++;
                    }
                }
                continue;
            }

            int curr = rq.Dequeue();
            int runTime = Math.Min(quantumTime, procs[curr].RemainingTime);
            procs[curr].RemainingTime -= runTime;
            time += runTime;
            gantt.Add((procs[curr].Id, time));

            // Load new processes
            while (ptr < n && procs[ptr].ArrivalTime <= time)
            {
                if (!inQueue[ptr])
                {
                    rq.Enqueue(ptr);
                    inQueue[ptr] = true;
                }
                ptr++;
            }

            if (procs[curr].RemainingTime == 0)
            {
                procs[curr].CompletionTime = time;
                procs[curr].TurnaroundTime = time - procs[curr].ArrivalTime;
                procs[curr].WaitingTime = procs[curr].TurnaroundTime - procs[curr].BurstTime;

                totalWT += procs[curr].WaitingTime;
                totalTAT += procs[curr].TurnaroundTime;

                completed++;
            }
            else
            {
                rq.Enqueue(curr);
            }
        }

        // Reorder by ID for display
        procs = procs.OrderBy(p => p.Id).ToList();

        double avgWT = totalWT / procs.Count;
        double avgTAT = totalTAT / procs.Count;
        int maxCT = procs.Max(p => p.CompletionTime);
        double throughput = (double)procs.Count / (maxCT - minAT);

        return (procs, gantt, avgWT, avgTAT, throughput);
    }

    /// <summary>
    /// Priority Scheduling - Non-preemptive
    /// Lower priority number = higher priority
    /// Tie-breaker: Earlier arrival time
    /// </summary>
    public static (List<Process>, List<(int, int)>, double, double, double) SolvePriority(List<Process> processes)
    {
        var procs = new List<Process>(processes);
        int n = procs.Count;
        int time = 0;
        int completed = 0;
        var gantt = new List<(int, int)>();
        var done = new bool[n];
        double totalWT = 0, totalTAT = 0;
        int minAT = procs.Min(p => p.ArrivalTime);

        while (completed < n)
        {
            int idx = -1;

            // Find ready process with highest priority (lowest priority number)
            for (int i = 0; i < n; i++)
            {
                if (done[i] || procs[i].ArrivalTime > time)
                    continue;

                if (idx == -1)
                    idx = i;
                else if (procs[i].Priority < procs[idx].Priority)
                    idx = i;
                else if (procs[i].Priority == procs[idx].Priority &&
                         procs[i].ArrivalTime < procs[idx].ArrivalTime)
                    idx = i;
            }

            if (idx == -1)
            {
                time++;
            }
            else
            {
                time += procs[idx].BurstTime;
                procs[idx].CompletionTime = time;
                procs[idx].TurnaroundTime = procs[idx].CompletionTime - procs[idx].ArrivalTime;
                procs[idx].WaitingTime = procs[idx].TurnaroundTime - procs[idx].BurstTime;

                totalWT += procs[idx].WaitingTime;
                totalTAT += procs[idx].TurnaroundTime;

                done[idx] = true;
                completed++;
                gantt.Add((procs[idx].Id, time));
            }
        }

        double avgWT = totalWT / procs.Count;
        double avgTAT = totalTAT / procs.Count;
        int maxCT = procs.Max(p => p.CompletionTime);
        double throughput = (double)procs.Count / (maxCT - minAT);

        return (procs, gantt, avgWT, avgTAT, throughput);
    }
}
