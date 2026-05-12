namespace PageReplacementDemo.Algorithms.BankerAlgo;

/// <summary>
/// Executor for Banker's Algorithm.
/// </summary>
public class BankerExecutor
{
    public static IBankerAlgorithm CreateAlgorithm()
    {
        return new BankerAlgorithm();
    }

    public static string GetAlgorithmName()
    {
        return "Banker's Algorithm (Deadlock Avoidance)";
    }

    public static (bool IsSafe, List<int> SafeSequence, string Message) ExecuteAlgorithm(
        List<int> totalResources, 
        List<List<int>> maxMatrix, 
        List<List<int>> allocationMatrix)
    {
        var algorithm = CreateAlgorithm();
        algorithm.Initialize(totalResources, maxMatrix, allocationMatrix);
        return algorithm.CheckSafeState();
    }
}
