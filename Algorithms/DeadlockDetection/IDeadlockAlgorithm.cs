namespace PageReplacementDemo.Algorithms.DeadlockDetection;

/// <summary>
/// Interface for deadlock detection algorithms
/// </summary>
public interface IDeadlockAlgorithm
{
    /// <summary>
    /// Detect if system is in safe state
    /// Returns: (IsSafe, SafeSequence)
    /// </summary>
    (bool IsSafe, List<int> SafeSequence) Detect();
}
