using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.BankerAlgo;

/// <summary>
/// Interface for Banker's Algorithm (Deadlock Avoidance).
/// </summary>
public interface IBankerAlgorithm
{
    /// <summary>
    /// Initialize algorithm with system resources and process requirements.
    /// </summary>
    void Initialize(List<int> totalResources, List<List<int>> maxMatrix, List<List<int>> allocationMatrix);

    /// <summary>
    /// Check if system is in safe state.
    /// </summary>
    (bool IsSafe, List<int> SafeSequence, string Message) CheckSafeState();
}
