namespace PageReplacementDemo.Models.PageReplacementAlgo;

/// <summary>
/// Lưu trữ kết quả so sánh của các thuật toán thay thế trang.
/// </summary>
public class ComparisonResult
{
    /// <summary>Tên thuật toán.</summary>
    public string AlgorithmName { get; set; } = "";

    /// <summary>Tổng số page fault.</summary>
    public int PageFaults { get; set; }

    /// <summary>Tổng số tham chiếu trang.</summary>
    public int TotalReferences { get; set; }

    /// <summary>Tỷ lệ cache hit (%).</summary>
    public double HitRate { get; set; }

    /// <summary>Tỷ lệ page fault (%).</summary>
    public double FaultRate { get; set; }

    /// <summary>Xếp hạng dựa trên số page fault (1 = tốt nhất).</summary>
    public int Rank { get; set; }
}
