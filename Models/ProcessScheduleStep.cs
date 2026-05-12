namespace PageReplacementDemo.Models;

/// <summary>
/// Represents a step in CPU scheduling algorithm execution.
/// </summary>
public class ProcessScheduleStep
{
    /// <summary>
    /// Process identifier
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// Arrival time of process
    /// </summary>
    public int ArrivalTime { get; set; }

    /// <summary>
    /// Burst time (execution time) of process
    /// </summary>
    public int BurstTime { get; set; }

    /// <summary>
    /// Priority level (for Priority Scheduling)
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Completion time
    /// </summary>
    public int CompletionTime { get; set; }

    /// <summary>
    /// Turnaround time (completion time - arrival time)
    /// </summary>
    public int TurnaroundTime { get; set; }

    /// <summary>
    /// Waiting time (turnaround time - burst time)
    /// </summary>
    public int WaitingTime { get; set; }

    /// <summary>
    /// Remaining time (for preemptive algorithms)
    /// </summary>
    public int RemainingTime { get; set; }

    /// <summary>
    /// Gantt chart: list of (processId, endTime) pairs
    /// </summary>
    public List<(int ProcessId, int EndTime)> GanttChart { get; set; } = new();

    /// <summary>
    /// Average waiting time across all processes
    /// </summary>
    public double AverageWaitingTime { get; set; }

    /// <summary>
    /// Average turnaround time across all processes
    /// </summary>
    public double AverageTurnaroundTime { get; set; }

    /// <summary>
    /// Throughput: number of processes / total time
    /// </summary>
    public double Throughput { get; set; }

    /// <summary>
    /// Step index in the scheduling sequence
    /// </summary>
    public int StepIndex { get; set; }

    /// <summary>
    /// Status message for this step
    /// </summary>
    public string Message { get; set; } = "";
}
