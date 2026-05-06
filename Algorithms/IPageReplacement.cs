using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms;

/// <summary>
/// Interface chuẩn cho mọi thuật toán thay thế trang.
/// </summary>
public interface IPageReplacement
{
    /// <summary>
    /// Khởi tạo dữ liệu đầu vào cho thuật toán.
    /// </summary>
    void Initialize(int pageCount, int frameCount, int[] referenceString);

    /// <summary>
    /// Trả về kết quả từng bước một (step-by-step) sử dụng yield return.
    /// </summary>
    IEnumerable<StepResult> ExecuteStepByStep();
}