using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.BankerAlgo;

/// <summary>
/// Banker's Algorithm - Deadlock Avoidance.
/// Determines if system is in safe state and finds safe sequence.
/// </summary>
public class BankerAlgorithm : IBankerAlgorithm
{
    private List<int> _totalResources = new();
    private List<List<int>> _maxMatrix = new();
    private List<List<int>> _allocationMatrix = new();
    private List<int> _availableResources = new();
    private List<List<int>> _needMatrix = new();

    public void Initialize(List<int> totalResources, List<List<int>> maxMatrix, List<List<int>> allocationMatrix)
    {
        _totalResources = new List<int>(totalResources);
        _maxMatrix = DeepCopyMatrix(maxMatrix);
        _allocationMatrix = DeepCopyMatrix(allocationMatrix);

        CalculateAvailableAndNeed();
    }

    public (bool IsSafe, List<int> SafeSequence, string Message) CheckSafeState()
    {
        int n = _maxMatrix.Count;
        int m = _totalResources.Count;

        var work = new List<int>(_availableResources);
        var finish = new bool[n];
        var safeSequence = new List<int>();
        var message = new System.Text.StringBuilder();

        message.AppendLine("\n=== BANKER'S ALGORITHM - SAFE STATE CHECK ===\n");
        message.AppendLine("STEP 1: Calculate Available Resources");
        message.Append("Available = Total - Sum(Allocation) = ");
        for (int j = 0; j < m; j++)
            message.Append($"{_availableResources[j]} ");
        message.AppendLine("\n");

        message.AppendLine("STEP 2: Calculate Need Matrix (Need = Max - Allocation)");
        for (int i = 0; i < n; i++)
        {
            message.Append($"P{i}: Need = [");
            for (int j = 0; j < m; j++)
                message.Append($"{_needMatrix[i][j]} ");
            message.AppendLine("]");
        }
        message.AppendLine();

        message.AppendLine("STEP 3: Find Safe Sequence\n");

        int pass = 1;
        int count = 0;

        while (count < n)
        {
            bool found = false;
            message.AppendLine($"--- PASS {pass++} ---");
            message.Append("Work (Available) = [");
            for (int j = 0; j < m; j++)
                message.Append($"{work[j]} ");
            message.AppendLine("]");

            for (int p = 0; p < n; p++)
            {
                if (finish[p])
                    continue;

                message.Append($"P{p}: Need = [");
                for (int j = 0; j < m; j++)
                    message.Append($"{_needMatrix[p][j]} ");
                message.Append("] vs Work = [");
                for (int j = 0; j < m; j++)
                    message.Append($"{work[j]} ");
                message.Append("] → ");

                bool canExecute = true;
                for (int j = 0; j < m; j++)
                {
                    if (_needMatrix[p][j] > work[j])
                    {
                        canExecute = false;
                        break;
                    }
                }

                if (canExecute)
                {
                    message.AppendLine("CAN EXECUTE");
                    // Process finishes, release resources
                    for (int j = 0; j < m; j++)
                        work[j] += _allocationMatrix[p][j];

                    finish[p] = true;
                    safeSequence.Add(p);
                    found = true;
                    count++;
                }
                else
                {
                    message.AppendLine("CANNOT EXECUTE (waiting)");
                }
            }

            if (!found && count < n)
            {
                message.AppendLine("\n❌ DEADLOCK DETECTED: No process can proceed!");
                return (false, new List<int>(), message.ToString());
            }
            message.AppendLine();
        }

        message.AppendLine("=== RESULT ===");
        message.Append("✅ SAFE STATE: Safe Sequence = < ");
        for (int i = 0; i < safeSequence.Count; i++)
        {
            message.Append($"P{safeSequence[i]}");
            if (i < safeSequence.Count - 1) message.Append(", ");
        }
        message.AppendLine(" >");

        return (true, safeSequence, message.ToString());
    }

    private void CalculateAvailableAndNeed()
    {
        int n = _maxMatrix.Count;
        int m = _totalResources.Count;

        // Calculate available
        _availableResources = new List<int>(m);
        for (int j = 0; j < m; j++)
        {
            int sumAlloc = 0;
            for (int i = 0; i < n; i++)
                sumAlloc += _allocationMatrix[i][j];
            _availableResources.Add(_totalResources[j] - sumAlloc);
        }

        // Calculate need
        _needMatrix = new List<List<int>>(n);
        for (int i = 0; i < n; i++)
        {
            var need = new List<int>(m);
            for (int j = 0; j < m; j++)
                need.Add(_maxMatrix[i][j] - _allocationMatrix[i][j]);
            _needMatrix.Add(need);
        }
    }

    private static List<List<int>> DeepCopyMatrix(List<List<int>> matrix)
    {
        var copy = new List<List<int>>();
        foreach (var row in matrix)
            copy.Add(new List<int>(row));
        return copy;
    }
}
