namespace PageReplacementDemo.Algorithms.DeadlockDetection;

/// <summary>
/// Deadlock Detector using Banker's Algorithm
/// Detects safe/unsafe states
/// </summary>
public class DeadlockDetector : IDeadlockAlgorithm
{
    private int _numProcesses;
    private int _numResources;
    private int[][] _max;
    private int[][] _allocation;
    private int[] _available;
    private int[][] _need;

    public DeadlockDetector(int[][] max, int[][] allocation, int[] available)
    {
        _numProcesses = max.Length;
        _numResources = max[0].Length;
        _max = CopyMatrix(max);
        _allocation = CopyMatrix(allocation);
        _available = new int[_numResources];
        Array.Copy(available, _available, _numResources);

        // Calculate Need matrix
        _need = new int[_numProcesses][];
        for (int i = 0; i < _numProcesses; i++)
        {
            _need[i] = new int[_numResources];
            for (int j = 0; j < _numResources; j++)
            {
                _need[i][j] = _max[i][j] - _allocation[i][j];
            }
        }
    }

    public (bool IsSafe, List<int> SafeSequence) Detect()
    {
        var safeSequence = new List<int>();
        var finish = new bool[_numProcesses];
        var work = new int[_numResources];
        Array.Copy(_available, work, _numResources);

        for (int count = 0; count < _numProcesses; count++)
        {
            bool found = false;

            for (int i = 0; i < _numProcesses; i++)
            {
                if (finish[i])
                    continue;

                // Check if process i can be satisfied
                bool canSatisfy = true;
                for (int j = 0; j < _numResources; j++)
                {
                    if (_need[i][j] > work[j])
                    {
                        canSatisfy = false;
                        break;
                    }
                }

                if (canSatisfy)
                {
                    // Release resources
                    for (int j = 0; j < _numResources; j++)
                    {
                        work[j] += _allocation[i][j];
                    }

                    finish[i] = true;
                    safeSequence.Add(i);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // Unsafe state - cannot find a process that can complete
                return (false, new List<int>());
            }
        }

        // All processes can complete - safe state
        return (true, safeSequence);
    }

    public int[][] GetNeedMatrix() => CopyMatrix(_need);
    public int[][] GetAllocationMatrix() => CopyMatrix(_allocation);
    public int[] GetAvailableResources() => new int[_numResources];
    public int NumProcesses => _numProcesses;
    public int NumResources => _numResources;

    private static int[][] CopyMatrix(int[][] matrix)
    {
        var copy = new int[matrix.Length][];
        for (int i = 0; i < matrix.Length; i++)
        {
            copy[i] = new int[matrix[i].Length];
            Array.Copy(matrix[i], copy[i], matrix[i].Length);
        }
        return copy;
    }
}
