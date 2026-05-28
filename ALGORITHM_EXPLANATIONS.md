# Giải Thích Code C# - Thuật Toán Thay Thế Trang (Page Replacement Algorithms)

**Hướng dẫn:** Từng dòng code C# và cách implement logic từ pseudocode

## Mục lục
1. [FIFO - First In First Out](#fifo)
2. [LRU - Least Recently Used](#lru)
3. [Clock (Second-Chance)](#clock)
4. [OPT - Optimal](#opt)

---

## <a name="fifo"></a>1. FIFO (First-In, First-Out)

### Cấu Trúc Dữ Liệu Chính

```csharp
int[] memoryFrames = new int[_frameCount];
Array.Fill(memoryFrames, -1); // -1 = frame trống

Queue<int> fifoTracker = new Queue<int>(_frameCount);
HashSet<int> inFrames = new HashSet<int>();
int totalFaults = 0;
```

**Giải thích từng phần:**

1. **`int[] memoryFrames`** - Mảng vật lý cố định
   - Lưu trữ các số hiệu trang trong bộ nhớ (physical frames)
   - Kích thước = `_frameCount` (số lượng frame)
   - Ban đầu fill -1 nghĩa là "frame trống"

2. **`Queue<int> fifoTracker`** - Hàng đợi FIFO
   - Tracking thứ tự các trang được nạp vào
   - Trang ở đầu queue = trang nạp vào đầu tiên = sẽ bị thay thế khi PAGE FAULT
   - Dùng `Enqueue()` để thêm, `Dequeue()` để lấy ra

3. **`HashSet<int> inFrames`** - Kiểm tra nhanh
   - Lưu danh sách trang đang có trong bộ nhớ
   - `Contains()` chạy O(1) → Nhanh hơn vòng lặp qua mảng
   - Dùng để kiểm tra HIT hay PAGE FAULT

### Vòng Lặp Xử Lý Từng Trang

```csharp
for (int i = 0; i < _referenceString.Length; i++)
{
    int page = _referenceString[i];
    bool isFault = !inFrames.Contains(page);
    int? victim = null;
    string message;
```

**Từng bước:**

- **`for (int i = ...)`** - Duyệt qua chuỗi tham chiếu trang
- **`int page = _referenceString[i]`** - Lấy trang hiện tại
- **`bool isFault = !inFrames.Contains(page)`** 
  - Nếu trang KHÔNG có trong `inFrames` → isFault = true
  - Nếu trang đã có → isFault = false (HIT)
- **`int? victim = null`** - Nullable int, lưu trang bị thay thế (null nếu ko thay thế)

### Xử Lý Page Fault

```csharp
if (isFault)
{
    if (fifoTracker.Count >= _frameCount)
    {
        // TH1: Bộ nhớ đầy → Thay thế trang ở đầu queue
        victim = fifoTracker.Dequeue();  // Lấy trang cũ nhất
        inFrames.Remove(victim.Value);   // Xoá khỏi HashSet

        // Tìm vị trí của victim trong mảy và thay thế
        int frameIndex = Array.IndexOf(memoryFrames, victim.Value);
        memoryFrames[frameIndex] = page;

        message = $"Page Fault! Thay trang {victim} bằng trang {page}";
    }
    else
    {
        // TH2: Có frame trống → Nạp vào frame trống
        int emptyIndex = Array.IndexOf(memoryFrames, -1);
        memoryFrames[emptyIndex] = page;

        message = $"Page Fault! Nạp trang {page} vào frame trống";
    }

    // Cập nhật tracker
    fifoTracker.Enqueue(page);    // Thêm trang mới vào cuối queue
    inFrames.Add(page);            // Thêm vào HashSet
    totalFaults++;
}
```

**Chi tiết từng dòng:**

1. **`if (isFault)` - Nếu xảy ra PAGE FAULT**
   
2. **`if (fifoTracker.Count >= _frameCount)` - Bộ nhớ đầy?**
   
   - **`victim = fifoTracker.Dequeue()`**
     - Lấy trang ở đầu queue (FIFO = trang nạp vào đầu tiên)
     - `Dequeue()` vừa lấy vừa xoá khỏi queue
   
   - **`inFrames.Remove(victim.Value)`**
     - Xoá trang cũ khỏi HashSet tracking
     - `.Value` vì `victim` là nullable int (phải extract value)
   
   - **`int frameIndex = Array.IndexOf(memoryFrames, victim.Value)`**
     - Tìm vị trí của trang cũ trong mảy
     - Trả về index hoặc -1 nếu không tìm thấy
   
   - **`memoryFrames[frameIndex] = page`**
     - Thay thế trang cũ = trang mới
     
3. **`else` - Bộ nhớ chưa đầy**
   
   - **`int emptyIndex = Array.IndexOf(memoryFrames, -1)`**
     - Tìm frame trống (giá trị -1)
   
   - **`memoryFrames[emptyIndex] = page`**
     - Nạp trang mới vào frame trống

4. **Cập nhật Tracker**
   
   - **`fifoTracker.Enqueue(page)`** - Thêm trang mới vào cuối queue
   - **`inFrames.Add(page)`** - Thêm trang mới vào HashSet
   - **`totalFaults++`** - Tăng bộ đếm PAGE FAULT

### Trả Về Kết Quả (yield return)

```csharp
yield return new StepResult(
    currentPage: page,
    frames: memoryFrames.ToArray(),
    isPageFault: isFault,
    victimPage: victim,
    message: message,
    stepIndex: i,
    totalPageFaults: totalFaults
);
```

- **`yield return`** - C# keyword tạo ra `IEnumerable` (generator)
  - Mỗi lần gọi return, hàm "tạm dừng" lại
  - Lần gọi tiếp theo sẽ tiếp tục từ chỗ dừng
  - Dùng để trả về kết quả từng bước (step-by-step)
- **`memoryFrames.ToArray()`** - Copy array hiện tại
- **Named parameters** - Rõ ràng, dễ hiểu

---

---

## <a name="lru"></a>2. LRU (Least Recently Used)

### Cấu Trúc Dữ Liệu Chính

```csharp
int[] memoryFrames = new int[_frameCount];
Array.Fill(memoryFrames, -1);

List<int> lruTracker = new List<int>(_frameCount);
int totalFaults = 0;
```

**Giải thích:**

1. **`List<int> lruTracker`** - Danh sách tracking LRU
   - **Phần tử đầu** = LRU nhất (Least Recently Used)
   - **Phần tử cuối** = MRU nhất (Most Recently Used - vừa dùng)
   - Dùng `List` vì cần:
     - Truy cập từng phần tử: `lruTracker[0]` lấy LRU
     - Xoá từ vị trí bất kỳ: `Remove()` hoặc `RemoveAt()`
     - Thêm vào cuối: `Add()`

### So Sánh FIFO vs LRU về Cấu Trúc

| FIFO | LRU |
|------|-----|
| `Queue<int>` - FIFO order | `List<int>` - LRU order |
| Không cập nhật khi HIT | **Cập nhật khi HIT** |
| Trang cũ ở đầu | Trang ít dùng ở đầu |

### Xử Lý Khi PAGE FAULT

```csharp
if (isFault)
{
    if (lruTracker.Count >= _frameCount)
    {
        // TH1: Bộ nhớ đầy
        victim = lruTracker[0];           // Trang LRU nhất ở đầu list
        lruTracker.RemoveAt(0);           // Xoá phần tử đầu

        int frameIndex = Array.IndexOf(memoryFrames, victim.Value);
        memoryFrames[frameIndex] = page;

        message = $"Page Fault! Thay trang {victim} bằng trang {page}";
    }
    else
    {
        // TH2: Có frame trống
        int emptyIndex = Array.IndexOf(memoryFrames, -1);
        memoryFrames[emptyIndex] = page;

        message = $"Page Fault! Nạp trang {page} vào frame trống";
    }

    // Cập nhật tracker - thêm vào cuối (trở thành MRU)
    lruTracker.Add(page);
    totalFaults++;
}
```

**Chi tiết:**

1. **`victim = lruTracker[0]`**
   - Lấy trang ở đầu list (LRU nhất)
   - Khác FIFO dùng `Dequeue()` vì List không có phương thức đó

2. **`lruTracker.RemoveAt(0)`**
   - Xoá phần tử tại index 0
   - Sau này tất cả phần tử phía sau sẽ dịch lên

3. **`lruTracker.Add(page)`**
   - Thêm trang mới vào **cuối** list
   - Trang mới trở thành MRU (Most Recently Used)

### Xử Lý Khi HIT - **Điểm Khác Biệt Chính**

```csharp
else
{
    // Hit: di chuyển trang được tham chiếu lên cuối (MRU nhất)
    lruTracker.Remove(page);   // Xoá trang từ vị trí hiện tại
    lruTracker.Add(page);      // Thêm lại vào cuối (MRU)
    message = $"Hit! Trang {page} đã có trong bộ nhớ, chuyển lên MRU";
}
```

**Tại sao cần cập nhật khi HIT?**

- **FIFO**: Không quan tâm trang được dùng hay không, chỉ quan tâm thứ tự nạp vào
- **LRU**: **Cập nhật** mỗi khi trang được sử dụng để phản ánh "recently"

**Ví dụ:**
```
Giả sử danh sách là: [1, 2, 3] (1=LRU, 3=MRU)

Nếu request trang 1:
  - Xoá 1: [2, 3]
  - Thêm 1: [2, 3, 1]
  - Kết quả: 1 trở thành MRU vì nó vừa được dùng
```

### So Sánh Chi Tiết: Queue vs List

```csharp
// ===== FIFO dùng Queue =====
Queue<int> q = new Queue<int>();
q.Enqueue(1);      // Thêm vào cuối
var x = q.Dequeue(); // Lấy + xoá từ đầu

// ===== LRU dùng List =====
List<int> l = new List<int>();
l.Add(1);          // Thêm vào cuối
var x = l[0];      // Lấy từ đầu (không xoá)
l.RemoveAt(0);     // Xoá riêng
l.Remove(page);    // Xoá giá trị (ở vị trí bất kỳ)
```

**Tại sao LRU cần List mà không dùng Queue?**
- FIFO: Chỉ thêm vào cuối, lấy từ đầu → Queue đủ
- LRU: Cần xoá **bất kỳ** phần tử (khi HIT) → List linh hoạt hơn

---

---

## <a name="clock"></a>3. Clock (Second-Chance Algorithm)

### Cấu Trúc Dữ Liệu Chính - Tuple

```csharp
// Tuple: mỗi frame lưu (page, useBit)
(int page, bool useBit)[] frames = new (int page, bool useBit)[_frameCount];

// Khởi tạo tất cả frame là trống
for (int i = 0; i < _frameCount; i++)
{
    frames[i] = (-1, false);
}

int pointer = 0;
```

**Giải thích Chi Tiết:**

1. **Tuple `(int, bool)`** - C# feature tạo kiểu dữ liệu kết hợp
   ```csharp
   // Cách viết
   (int page, bool useBit)[] frames;
   
   // Cách sử dụng
   frames[0] = (page: 5, useBit: true);  // Named tuple
   frames[0] = (5, true);                 // Positional
   
   // Truy cập
   int pageNum = frames[0].page;
   bool used = frames[0].useBit;
   ```

2. **Con trỏ Vòng Tròn** - `int pointer`
   - Tracking vị trí hiện tại trong mảy frames
   - Quay vòng từ 0 → _frameCount-1 → 0 → ...
   - Dùng phép modulo để quay vòng: `pointer = (pointer + 1) % _frameCount`

### Kiểm Tra HIT

```csharp
// Tìm trang trong frames
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
```

**Điểm Chính:**
- Duyệt toàn bộ frames để tìm trang
- Nếu tìm thấy: Set `useBit = true` (đánh dấu là vừa được dùng)
- `useBit = true` = trang được cấp "cơ hội thứ hai"

### Xử Lý Page Fault - Vòng Lặp Clock

```csharp
else
{
    // Page Fault: tìm trang hy sinh theo thuật toán Clock
    while (true)
    {
        if (frames[pointer].page == -1)
        {
            // Frame trống → Nạp vào
            victim = null;
            frames[pointer] = (page, true);
            message = $"Page Fault! Nạp trang {page} vào frame trống tại vị trí {pointer}";
            pointer = (pointer + 1) % _frameCount;
            break;
        }

        if (!frames[pointer].useBit)
        {
            // useBit == 0 → Thay thế trang này
            victim = frames[pointer].page;
            message = $"Page Fault! Thay trang {victim} bằng trang {page}...";
            frames[pointer] = (page, true);
            pointer = (pointer + 1) % _frameCount;
            break;
        }
        else
        {
            // useBit == 1 → Set = 0, di chuyển pointer (cấp cơ hội thứ hai)
            frames[pointer].useBit = false;
            pointer = (pointer + 1) % _frameCount;
        }
    }
    totalFaults++;
}
```

**Vòng Lặp While(true) - Giải Thích Chi Tiết:**

Vòng lặp này quay tròn từ con trỏ hiện tại cho đến khi tìm được trang để thay thế.

**Trường Hợp 1: Frame Trống**
```csharp
if (frames[pointer].page == -1)
{
    victim = null;  // Không có trang bị thay thế
    frames[pointer] = (page, true);  // Nạp trang mới
    message = $"...";
    pointer = (pointer + 1) % _frameCount;  // Quay tròn
    break;  // Thoát vòng lặp
}
```

**Trường Hợp 2: useBit = 0 (Trang Không Được Dùng)**
```csharp
if (!frames[pointer].useBit)  // Kiểm tra: nếu useBit == false
{
    victim = frames[pointer].page;
    frames[pointer] = (page, true);
    pointer = (pointer + 1) % _frameCount;
    break;  // Thoát - tìm được trang để thay thế
}
```

**Trường Hợp 3: useBit = 1 (Trang Vừa Được Dùng)**
```csharp
else
{
    // "Cơ hội thứ hai" - set useBit = 0 và tiếp tục
    frames[pointer].useBit = false;
    pointer = (pointer + 1) % _frameCount;
    // Không break - tiếp tục quay vòng
}
```

### Con Trỏ Vòng Tròn - Modulo Operator

```csharp
pointer = (pointer + 1) % _frameCount;
```

**Ví dụ với 3 frames:**
```
pointer = 0 → (0 + 1) % 3 = 1
pointer = 1 → (1 + 1) % 3 = 2
pointer = 2 → (2 + 1) % 3 = 0  ← Quay về 0 (vòng tròn)
```

### Tạo Mảy Display

```csharp
int[] displayFrames = new int[_frameCount];
for (int j = 0; j < _frameCount; j++)
{
    displayFrames[j] = frames[j].page;  // Lấy .page từ tuple
}

yield return new StepResult(
    currentPage: page,
    frames: displayFrames,
    ...
);
```

**Tại sao cần lấy `.page` riêng?**
- `frames` có kiểu `(int page, bool useBit)[]`
- `StepResult` chỉ cần mảy trang, không cần useBit
- Lấy `.page` từ mỗi tuple để tạo mảy mới `int[]`

### So Sánh: FIFO vs Clock

| FIFO | Clock |
|------|-------|
| `Queue<int>` | `(int, bool)[]` tuple |
| Không cập nhật khi HIT | Set `useBit = 1` khi HIT |
| Thay ngay trang ở đầu | Quay vòng tìm `useBit = 0` |
| 1 vòng lặp đơn | Vòng lặp While với 3 TH |

---

---

## <a name="opt"></a>4. OPT (Optimal Algorithm)

### Cấu Trúc Dữ Liệu Chính

```csharp
int[] memoryFrames = new int[_frameCount];
Array.Fill(memoryFrames, -1);

List<int> framesList = new List<int>(_frameCount);
int totalFaults = 0;
```

**Giải thích:**

- **`List<int> framesList`** - Danh sách các trang hiện có trong bộ nhớ
  - Dùng `List` vì cần gọi hàm `FindOptimalVictim()` pass vào
  - Phiên bản đơn giản hơn LRU (không cần tracking thứ tự)

### Xử Lý Page Fault

```csharp
if (isFault)
{
    if (framesList.Count >= _frameCount)
    {
        // Bộ nhớ đầy: tìm trang sẽ dùng xa nhất trong tương lai
        victim = FindOptimalVictim(framesList, _referenceString, i);
        framesList.Remove(victim.Value);

        int frameIndex = Array.IndexOf(memoryFrames, victim.Value);
        memoryFrames[frameIndex] = page;

        message = $"Page Fault! Thay trang {victim} bằng trang {page}...";
    }
    else
    {
        // Có frame trống
        int emptyIndex = Array.IndexOf(memoryFrames, -1);
        memoryFrames[emptyIndex] = page;

        message = $"Page Fault! Nạp trang {page} vào frame trống";
    }

    framesList.Add(page);
    totalFaults++;
}
else
{
    message = $"Hit! Trang {page} đã có trong bộ nhớ";
}
```

**Điểm khác biệt:**
- **FIFO**: Thay trang nạp vào đầu tiên
- **LRU**: Thay trang không dùng lâu nhất
- **OPT**: Gọi `FindOptimalVictim()` để tìm trang sẽ dùng **xa nhất trong tương lai**

### Hàm Tìm Trang Tối Ưu - Chi Tiết Từng Dòng

```csharp
private static int FindOptimalVictim(
    List<int> framesList, 
    int[] referenceString, 
    int currentIndex)
{
    int farthestIndex = -1;      // Vị trí tương lai xa nhất
    int victim = framesList[0];  // Trang mặc định thay thế
```

**Tham số:**
- **`framesList`** - Các trang trong bộ nhớ hiện tại
- **`referenceString`** - Toàn bộ chuỗi tham chiếu (để nhìn tương lai)
- **`currentIndex`** - Vị trí hiện tại trong chuỗi

**Biến:**
- **`farthestIndex`** - Tracking vị trí xuất hiện xa nhất
- **`victim`** - Trang sẽ thay thế (khởi tạo = trang đầu)

### Vòng Lặp Tìm Trang Xa Nhất

```csharp
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
```

**Giải thích từng dòng:**

1. **`foreach (var page in framesList)`**
   - Duyệt qua từng trang hiện có trong bộ nhớ

2. **`int nextUse = -1`**
   - -1 = trang không xuất hiện trong tương lai

3. **`for (int j = currentIndex + 1; ...)`**
   - Bắt đầu tìm từ vị trí sau hiện tại
   - `currentIndex` là vị trí hiện tại
   - `j = currentIndex + 1` là vị trí kế tiếp (tương lai)

4. **`if (referenceString[j] == page) { nextUse = j; break; }`**
   - Nếu tìm thấy trang → ghi nhận vị trí và thoát loop

### Logic So Sánh

```csharp
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
```

**Trường Hợp 1: Trang Không Xuất Hiện Lại**
```csharp
if (nextUse == -1)
{
    return page;  // Chọn ngay trang này
}
```

**Lý do:** Trang này sẽ không được dùng nữa → thay nó là lựa chọn tối ưu nhất.

**Trường Hợp 2: So Sánh Vị Trí Xa Nhất**
```csharp
if (nextUse > farthestIndex)
{
    farthestIndex = nextUse;
    victim = page;
}
```

**Logic:**
- Lần lặp 1: Trang A xuất hiện tại index 15
  - `farthestIndex = -1` → `15 > -1` ✓
  - `victim = A`, `farthestIndex = 15`

- Lần lặp 2: Trang B xuất hiện tại index 20
  - `20 > 15` ✓
  - `victim = B`, `farthestIndex = 20`

- Lần lặp 3: Trang C xuất hiện tại index 18
  - `18 > 20` ✗ (không cập nhật)
  - Vẫn giữ `victim = B`

**Kết quả:** Chọn trang B (xuất hiện xa nhất tại index 20)

### Ví Dụ Trace Code

```
Chuỗi: [1, 2, 3, 1, 4, 2, 5, 1]
Frame: [1, 2, 3]
Hiện tại ở index 4 (trang 4)

Bước 1: Duyệt trang 1 trong frame
  - Tìm từ index 5 trở đi
  - index 5: 2 (không match)
  - index 6: 5 (không match)
  - index 7: 1 (MATCH!) → nextUse = 7
  - 7 > -1 ✓ → victim = 1, farthestIndex = 7

Bước 2: Duyệt trang 2 trong frame
  - Tìm từ index 5 trở đi
  - index 5: 2 (MATCH!) → nextUse = 5
  - 5 > 7 ✗ → giữ nguyên victim = 1

Bước 3: Duyệt trang 3 trong frame
  - Tìm từ index 5 trở đi
  - index 5, 6, 7: không có 3
  - nextUse = -1 → return 3 (trang này không dùng nữa)

KẾT QUẢ: Chọn trang 3 để thay thế
(vì trang 3 không xuất hiện trong tương lai)
```

### So Sánh 4 Thuật Toán - Code Level

| Aspect | FIFO | LRU | Clock | OPT |
|--------|------|-----|-------|-----|
| **Cấu trúc** | `Queue<int>` | `List<int>` | `(int,bool)[]` | `List<int>` |
| **Kiểm tra HIT** | HashSet | List.Contains | Array loop | (không cập nhật) |
| **Xử lý HIT** | Bỏ qua | Remove+Add | Set useBit=1 | Bỏ qua |
| **Lựa chọn victim** | `Dequeue()` | `[0]` từ list | While loop | `FindOptimalVictim()` |
| **Độ phức tạp code** | Đơn giản | Trung bình | Phức tạp | Rất phức tạp |

---

## Tóm Tắt C# Features Được Dùng

### 1. **Tuple** (Clock)
```csharp
(int page, bool useBit)[] frames;  // Named tuple
frames[0] = (5, true);             // Gán giá trị
int p = frames[0].page;            // Truy cập .page
```

### 2. **Nullable Type** (Tất cả)
```csharp
int? victim = null;  // Nullable int
if (victim == null) { }
victim.Value         // Extract giá trị
```

### 3. **Yield Return** (Tất cả)
```csharp
IEnumerable<T> Method()
{
    yield return value1;  // Tạm dừng
    yield return value2;  // Tiếp tục
}
```

### 4. **Collection Types**
- **Queue** (FIFO): Enqueue/Dequeue → FIFO order
- **List** (LRU, OPT): Add/Remove/RemoveAt → Linh hoạt
- **HashSet** (FIFO): Contains/Add/Remove → O(1) lookup

### 5. **Array Methods**
```csharp
Array.Fill(arr, value);           // Fill mảy
Array.IndexOf(arr, value);        // Tìm index
memoryFrames.ToArray();           // Convert IEnumerable → array
```

### 6. **String Interpolation** (Tất cả)
```csharp
$"Page Fault! Thay trang {victim} bằng trang {page}"
```

---

## Bảng Tóm Tắt Cơ Chế Thay Thế

```
FIFO:
└─ Chọn trang nạp vào đầu tiên (FIFO)

LRU:
├─ Nếu trang đã có: Di chuyển lên MRU
└─ Nếu đầy: Thay trang LRU (không dùng lâu nhất)

Clock:
├─ Sử dụng use bit và con trỏ vòng
├─ Nếu use bit = 1: Set = 0, chuyển sang trang kế tiếp
└─ Nếu use bit = 0: Thay thế trang đó

OPT:
├─ Nhìn vào tương lai
└─ Thay trang sẽ dùng xa nhất (hoặc không dùng)
```

---

## Kết Luận

- **FIFO**: Đơn giản nhưng kém hiệu quả
- **LRU**: Hiệu quả tốt, phổ biến trong thực tế (Windows, Linux)
- **Clock**: Cân bằng tốt giữa đơn giản và hiệu quả (dùng nhiều)
- **OPT**: Tối ưu lý thuyết, không thể cài đặt thực tế nhưng dùng để so sánh
