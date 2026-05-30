namespace PageReplacementDemo.Algorithms.DeadlockDetection;

/// <summary>
/// Data model lưu trữ trạng thái hệ thống resource allocation
/// </summary>
public class DeadlockSystemData
{
    /// <summary>
    /// Số tiến trình
    /// </summary>
    public int NumProcesses { get; set; }

    /// <summary>
    /// Số loại tài nguyên
    /// </summary>
    public int NumResources { get; set; }

    /// <summary>
    /// Tổng tài nguyên có sẵn (Total resources)
    /// </summary>
    public int[]? Total { get; set; }

    /// <summary>
    /// Tài nguyên khả dụng (Available resources)
    /// </summary>
    public int[]? Available { get; set; }

    /// <summary>
    /// Ma trận cấp phát (Allocation matrix) - Process × Resource
    /// </summary>
    public int[][]? Allocation { get; set; }

    /// <summary>
    /// Ma trận nhu cầu tối đa (Max matrix) - Process × Resource
    /// </summary>
    public int[][]? Max { get; set; }

    /// <summary>
    /// Ma trận nhu cầu (Need matrix = Max - Allocation)
    /// </summary>
    public int[][]? Need { get; set; }

    /// <summary>
    /// Ma trận yêu cầu hiện tại (Request matrix) - Process × Resource
    /// </summary>
    public int[][]? RequestMatrix { get; set; }

    /// <summary>
    /// Danh sách tiến trình đã bị hệ thống hủy (Terminated processes)
    /// </summary>
    public bool[]? Terminated { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public DeadlockSystemData()
    {
        NumProcesses = 0;
        NumResources = 0;
    }

    /// <summary>
    /// Constructor với tham số
    /// </summary>
    public DeadlockSystemData(int numProc, int numRes)
    {
        NumProcesses = numProc;
        NumResources = numRes;

        Total = new int[numRes];
        Available = new int[numRes];

        Allocation = new int[numProc][];
        Max = new int[numProc][];
        Need = new int[numProc][];
        RequestMatrix = new int[numProc][];
        Terminated = new bool[numProc];

        for (int i = 0; i < numProc; i++)
        {
            Allocation[i] = new int[numRes];
            Max[i] = new int[numRes];
            Need[i] = new int[numRes];
            RequestMatrix[i] = new int[numRes];
            Terminated[i] = false;
        }
    }

    /// <summary>
    /// In các thông số hệ thống
    /// </summary>
    public override string ToString()
    {
        var result = new System.Text.StringBuilder();
        result.AppendLine($"Processes: {NumProcesses}, Resources: {NumResources}");
        result.AppendLine($"Total: [{string.Join(" ", Total ?? Array.Empty<int>())}]");
        result.AppendLine($"Available: [{string.Join(" ", Available ?? Array.Empty<int>())}]");
        return result.ToString();
    }
}
