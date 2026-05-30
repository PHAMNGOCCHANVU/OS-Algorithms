namespace PageReplacementDemo.Models;

/// <summary>
/// Result of deadlock detection analysis
/// </summary>
public class DeadlockResult
{
    public bool IsSafe { get; set; }
    public List<int> SafeSequence { get; set; } = new();
    public int NumProcesses { get; set; }
    public int NumResources { get; set; }
    public int[][] Max { get; set; } = Array.Empty<int[]>();
    public int[][] Allocation { get; set; } = Array.Empty<int[]>();
    public int[][] Need { get; set; } = Array.Empty<int[]>();
    public int[] Available { get; set; } = Array.Empty<int>();
}
