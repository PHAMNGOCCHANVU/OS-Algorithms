using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms;

/// <summary>
/// Thuật toán FIFO (First-In, First-Out) - Thay thế trang nạp vào đầu tiên.
/// </summary>
public class FIFOAlgorithm : IPageReplacement
{
    private int _pageCount;
    private int _frameCount;
    private int[] _referenceString = Array.Empty<int>();

    public void Initialize(int pageCount, int frameCount, int[] referenceString)
    {
        _pageCount = pageCount;
        _frameCount = frameCount;
        _referenceString = referenceString;
    }

    public IEnumerable<StepResult> ExecuteStepByStep()
    {
        // Mảng vật lý cố định để render ra UI
        int[] memoryFrames = new int[_frameCount];
        Array.Fill(memoryFrames, -1); // -1 = frame trống

        // Queue để tracking thứ tự FIFO
        Queue<int> fifoTracker = new Queue<int>(_frameCount);
        // HashSet để kiểm tra nhanh trang đã có trong frame chưa
        HashSet<int> inFrames = new HashSet<int>();
        int totalFaults = 0;

        for (int i = 0; i < _referenceString.Length; i++)
        {
            int page = _referenceString[i];
            bool isFault = !inFrames.Contains(page);
            int? victim = null;
            string message;

            if (isFault)
            {
                if (fifoTracker.Count >= _frameCount)
                {
                    // Đầy: lấy trang ở đầu hàng đợi
                    victim = fifoTracker.Dequeue();
                    inFrames.Remove(victim.Value);

                    // Tìm vị trí victim trong mảng vật lý cố định và thay thế
                    int frameIndex = Array.IndexOf(memoryFrames, victim.Value);
                    memoryFrames[frameIndex] = page;

                    message = $"Page Fault! Thay trang {victim} bằng trang {page}";
                }
                else
                {
                    // Tìm frame trống
                    int emptyIndex = Array.IndexOf(memoryFrames, -1);
                    memoryFrames[emptyIndex] = page;

                    message = $"Page Fault! Nạp trang {page} vào frame trống";
                }

                // Cập nhật tracker
                fifoTracker.Enqueue(page);
                inFrames.Add(page);
                totalFaults++;
            }
            else
            {
                message = $"Hit! Trang {page} đã có trong bộ nhớ";
            }

            yield return new StepResult(
                currentPage: page,
                frames: memoryFrames.ToArray(),
                isPageFault: isFault,
                victimPage: victim,
                message: message,
                stepIndex: i,
                totalPageFaults: totalFaults
            );
        }
    }
}