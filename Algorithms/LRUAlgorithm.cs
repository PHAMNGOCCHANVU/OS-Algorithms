using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms;

/// <summary>
/// Thuật toán LRU (Least Recently Used) - Thay thế trang không được dùng lâu nhất.
/// </summary>
public class LRUAlgorithm : IPageReplacement
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

        // List để tracking thứ tự LRU: phần tử đầu = LRU nhất, phần tử cuối = MRU nhất
        List<int> lruTracker = new List<int>(_frameCount);
        int totalFaults = 0;

        for (int i = 0; i < _referenceString.Length; i++)
        {
            int page = _referenceString[i];
            bool isFault = !lruTracker.Contains(page);
            int? victim = null;
            string message;

            if (isFault)
            {
                if (lruTracker.Count >= _frameCount)
                {
                    // Đầy: lấy trang LRU nhất (ở đầu tracker)
                    victim = lruTracker[0];
                    lruTracker.RemoveAt(0);

                    // Tìm vị trí victim trong mảy vật lý cố định và thay thế
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

                // Cập nhật tracker - thêm vào cuối (MRU nhất)
                lruTracker.Add(page);
                totalFaults++;
            }
            else
            {
                // Hit: di chuyển trang được tham chiếu lên cuối (MRU nhất)
                lruTracker.Remove(page);
                lruTracker.Add(page);
                message = $"Hit! Trang {page} đã có trong bộ nhớ, chuyển lên MRU";
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