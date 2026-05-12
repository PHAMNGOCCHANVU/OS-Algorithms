namespace PageReplacementDemo.Models;

/// <summary>
/// Represents a step in Banker's Algorithm execution.
/// </summary>
public class BankerStep
{
    /// <summary>
    /// Total resources available in the system
    /// </summary>
    public List<int> TotalResources { get; set; } = new();

    /// <summary>
    /// Currently available resources
    /// </summary>
    public List<int> AvailableResources { get; set; } = new();

    /// <summary>
    /// Maximum demand matrix - what each process may need
    /// </summary>
    public List<List<int>> MaxMatrix { get; set; } = new();

    /// <summary>
    /// Allocation matrix - currently allocated resources
    /// </summary>
    public List<List<int>> AllocationMatrix { get; set; } = new();

    /// <summary>
    /// Need matrix - still needed by each process (Max - Allocation)
    /// </summary>
    public List<List<int>> NeedMatrix { get; set; } = new();

    /// <summary>
    /// Safe sequence of processes (if safe state exists)
    /// </summary>
    public List<int> SafeSequence { get; set; } = new();

    /// <summary>
    /// Whether system is in safe state
    /// </summary>
    public bool IsSafeState { get; set; }

    /// <summary>
    /// Detailed step-by-step message
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// Step index in the verification process
    /// </summary>
    public int StepIndex { get; set; }

    /// <summary>
    /// Number of processes
    /// </summary>
    public int ProcessCount { get; set; }

    /// <summary>
    /// Number of resource types
    /// </summary>
    public int ResourceCount { get; set; }
}
