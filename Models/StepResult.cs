namespace PageReplacementDemo.Models;

/// <summary>
/// Lưu trữ trạng thái của một bước xử lý trong thuật toán.
/// </summary>
public class StepResult
{
    /// <summary>Trang đang được yêu cầu nạp.</summary>
    public int CurrentPage { get; }

    /// <summary>Trạng thái của các frame tại bước này (mảng copy).</summary>
    public int[] Frames { get; }

    /// <summary>Có xảy ra lỗi trang (page fault) hay không.</summary>
    public bool IsPageFault { get; }

    /// <summary>Trang bị đẩy ra ngoài (nếu có).</summary>
    public int? VictimPage { get; }

    /// <summary>Lời giải thích ngắn cho bước này.</summary>
    public string Message { get; }

    /// <summary>Số thứ tự bước (index trong reference string).</summary>
    public int StepIndex { get; }

    /// <summary>Tổng số page fault tính đến bước này.</summary>
    public int TotalPageFaults { get; }

    public StepResult(
        int currentPage,
        int[] frames,
        bool isPageFault,
        int? victimPage,
        string message,
        int stepIndex,
        int totalPageFaults)
    {
        CurrentPage = currentPage;
        Frames = frames;
        IsPageFault = isPageFault;
        VictimPage = victimPage;
        Message = message;
        StepIndex = stepIndex;
        TotalPageFaults = totalPageFaults;
    }
}