using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.PageReplacementAlgo;

/// <summary>
/// Thuật toán OPT (Optimal) - Thay thế trang sẽ được tham chiếu xa nhất trong tương lai.
/// </summary>
public class OPTAlgorithm : IPageReplacement
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

        // List để tracking các trang đang có trong frame
        List<int> framesList = new List<int>(_frameCount);
        int totalFaults = 0;

        for (int i = 0; i < _referenceString.Length; i++)
        {
            int page = _referenceString[i];
            bool isFault = !framesList.Contains(page);
            int? victim = null;
            string message;

            if (isFault)
            {
                if (framesList.Count >= _frameCount)
                {
                    // Đầy: tìm trang trong frame sẽ xuất hiện xa nhất (hoặc không bao giờ) trong tương lai
                    victim = FindOptimalVictim(framesList, _referenceString, i);
                    framesList.Remove(victim.Value);

                    // Tìm vị trí victim trong mảy vật lý cố định và thay thế
                    int frameIndex = Array.IndexOf(memoryFrames, victim.Value);
                    memoryFrames[frameIndex] = page;

                    message = $"Page Fault! Thay trang {victim} bằng trang {page} (xa nhất trong tương lai)";
                }
                else
                {
                    // Tìm frame trống
                    int emptyIndex = Array.IndexOf(memoryFrames, -1);
                    memoryFrames[emptyIndex] = page;

                    message = $"Page Fault! Nạp trang {page} vào frame trống";
                }

                // Cập nhật tracker
                framesList.Add(page);
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

    /// <summary>
    /// Tìm trang trong frames sẽ xuất hiện xa nhất trong tương lai (hoặc không bao giờ).
    /// </summary>
    private static int FindOptimalVictim(List<int> framesList, int[] referenceString, int currentIndex)
    {
        int farthestIndex = -1;
        int victim = framesList[0];

        foreach (var page in framesList)
        {
            // Tìm vị trí xuất hiện tiếp theo của trang trong tương lai
            int nextUse = -1;
            for (int j = currentIndex + 1; j < referenceString.Length; j++)
            {
                if (referenceString[j] == page)
                {
                    nextUse = j;
                    break;
                }
            }

            if (nextUse == -1)
            {
                // Trang không xuất hiện trong tương lai → chọn ngay
                return page;
            }

            if (nextUse > farthestIndex)
            {
                farthestIndex = nextUse;
                victim = page;
            }
        }

        return victim;
    }
}
