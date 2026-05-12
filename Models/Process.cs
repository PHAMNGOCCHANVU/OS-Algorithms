namespace PageReplacementDemo.Models;

/// <summary>
/// Represents a process in CPU scheduling.
/// </summary>
public class Process
{
    /// <summary>
    /// Process ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Arrival Time (AT) - when process arrives in the system
    /// </summary>
    public int ArrivalTime { get; set; }

    /// <summary>
    /// Burst Time (BT) - total time process needs to execute
    /// </summary>
    public int BurstTime { get; set; }

    /// <summary>
    /// Priority - lower number = higher priority
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Remaining Time (RT) - time left to execute (for preemptive algorithms)
    /// </summary>
    public int RemainingTime { get; set; }

    /// <summary>
    /// Completion Time (CT) - when process finishes
    /// </summary>
    public int CompletionTime { get; set; }

    /// <summary>
    /// Turnaround Time (TAT) = CT - AT
    /// </summary>
    public int TurnaroundTime { get; set; }

    /// <summary>
    /// Waiting Time (WT) = TAT - BT
    /// </summary>
    public int WaitingTime { get; set; }

    public Process(int id, int arrivalTime, int burstTime, int priority = 0)
    {
        Id = id;
        ArrivalTime = arrivalTime;
        BurstTime = burstTime;
        Priority = priority;
        RemainingTime = burstTime;
        CompletionTime = 0;
        TurnaroundTime = 0;
        WaitingTime = 0;
    }

    public override string ToString()
    {
        return $"P{Id}(AT:{ArrivalTime},BT:{BurstTime},PR:{Priority})";
    }
}
