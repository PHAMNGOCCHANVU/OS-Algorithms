using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.PageReplacementAlgo;

/// <summary>
/// Thuật toán Clock (Second-Chance) - Sử dụng use bit và con trỏ vòng tròn.
/// </summary>
public class ClockAlgorithm : IPageReplacement
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
        // Mảng frame: mỗi phần tử lưu (page, useBit)
        // useBit = true nếu trang vừa được tham chiếu/nạp
        (int page, bool useBit)[] frames = new (int page, bool useBit)[_frameCount];
        // Khởi tạo tất cả frame là trống (page = -1)
        for (int i = 0; i < _frameCount; i++)
        {
            frames[i] = (-1, false);
        }

        int pointer = 0;
        int totalFaults = 0;

        for (int i = 0; i < _referenceString.Length; i++)
        {
            int page = _referenceString[i];
            bool isFault = true;
            int? victim = null;
            string message = "";

            // Kiểm tra xem trang đã có trong frame chưa
            int hitIndex = -1;
            for (int j = 0; j < _frameCount; j++)
            {
                if (frames[j].page == page)
                {
                    hitIndex = j;
                    break;
                }
            }

            if (hitIndex >= 0)
            {
                // Hit: set useBit = true
                isFault = false;
                frames[hitIndex].useBit = true;
                message = $"Hit! Trang {page} đã có trong bộ nhớ, set use_bit = 1";
            }
            else
            {
                // Page Fault: tìm trang hy sinh theo thuật toán Clock
                while (true)
                {
                    if (frames[pointer].page == -1)
                    {
                        // Frame trống
                        victim = null;
                        frames[pointer] = (page, true);
                        message = $"Page Fault! Nạp trang {page} vào frame trống tại vị trí {pointer}";
                        pointer = (pointer + 1) % _frameCount;
                        break;
                    }

                    if (!frames[pointer].useBit)
                    {
                        // useBit == 0: thay thế trang này
                        victim = frames[pointer].page;
                        message = $"Page Fault! Thay trang {victim} bằng trang {page} tại vị trí {pointer}";
                        frames[pointer] = (page, true);
                        pointer = (pointer + 1) % _frameCount;
                        break;
                    }
                    else
                    {
                        // useBit == 1: set về 0 và di chuyển pointer
                        frames[pointer].useBit = false;
                        pointer = (pointer + 1) % _frameCount;
                    }
                }

                totalFaults++;
            }

            // Tạo mảng frames để hiển thị
            int[] displayFrames = new int[_frameCount];
            for (int j = 0; j < _frameCount; j++)
            {
                displayFrames[j] = frames[j].page;
            }

            yield return new StepResult(
                currentPage: page,
                frames: displayFrames,
                isPageFault: isFault,
                victimPage: victim,
                message: message,
                stepIndex: i,
                totalPageFaults: totalFaults
            );
        }
    }
}
