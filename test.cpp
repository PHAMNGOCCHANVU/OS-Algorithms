#include <bits/stdc++.h>

using namespace std;

struct Process {
    int id, at, bt, rt, prio; 
    int ct, tat, wt; 
};

// Sắp xếp tiến trình theo thời gian đến (AT)
bool cmpAt(Process a, Process b) { 
    return a.at < b.at; 
}

// In biểu đồ thực thi (Gantt Chart)
void printSimulation(const vector<pair<int, int>>& gantt, int mode) {
    cout << "\n--- QUA TRINH THUC THI (GANTT) ---\n";
    int prevTime = 0;

    for (auto p : gantt) {
        if (p.first == 0) {
            cout << "[ " << prevTime << " -> " << p.second << " ] : CPU Nhan roi (Idle)\n";
        } else {
            cout << "[ " << prevTime << " -> " << p.second << " ] : Tien trinh P" << p.first << " chay\n";
        }
        
        prevTime = p.second;

        if (mode == 1) { // Chạy tự động với delay 0.8s
            this_thread::sleep_for(chrono::milliseconds(800));
        }
    }
    
    // In biểu đồ Gantt dạng thanh ngang
    cout << "\n|";
    for (auto p : gantt) {
        if (p.first == 0) cout << " IDLE |";
        else cout << " P" << p.first << " |";
    }
    cout << "\n0";
    for (auto p : gantt) cout << setw(6) << p.second;
    cout << "\n"; 
}

// In bảng thống kê và trả về (Avg WT, Avg TAT)
pair<double, double> printTable(vector<Process>& procs) {
    double totalWt = 0, totalTat = 0; 
    int minAt = INT_MAX, maxCt = 0; 

    cout << "\nPID\tAT\tBT\tPR\tCT\tTAT\tWT\n";
    for (auto& p : procs) {
        totalWt += p.wt; 
        totalTat += p.tat;
        if (p.at < minAt) minAt = p.at;
        if (p.ct > maxCt) maxCt = p.ct;
        cout << p.id << "\t" << p.at << "\t" << p.bt << "\t" << p.prio << "\t"
             << p.ct << "\t" << p.tat << "\t" << p.wt << "\n";
    }
    
    double throughput = (double)procs.size() / (maxCt - minAt);
    double avgWt = totalWt / procs.size();
    double avgTat = totalTat / procs.size();
    
    cout << fixed << setprecision(2) << "Avg WT: " << avgWt 
         << " | Avg TAT: " << avgTat << "\n";
    cout << "Throughput: " << throughput << " processes/unit time\n";
    
    // Trả về kết quả trung bình
    return make_pair(avgWt, avgTat);
}

pair<double, double> solveFcfs(vector<Process> procs, int mode) {
    sort(procs.begin(), procs.end(), cmpAt); 
    int time = 0; 
    vector<pair<int, int>> gantt;
    
    for (int i = 0; i < procs.size(); i++) {
        if (time < procs[i].at) {
            gantt.push_back({0, procs[i].at}); // Ghi nhận CPU ở trạng thái nghỉ (Idle)
            time = procs[i].at; 
        }
        time += procs[i].bt; // Thực thi hết thời gian yêu cầu (BT)
        procs[i].ct = time; 
        procs[i].tat = procs[i].ct - procs[i].at;
        procs[i].wt = procs[i].tat - procs[i].bt;
        gantt.push_back({procs[i].id, time}); 
    }
    printSimulation(gantt, mode);
    return printTable(procs); 
}

pair<double, double> solveSjf(vector<Process> procs, int mode) {
    int n = procs.size(), time = 0, completed = 0;
    vector<pair<int,int>> gantt;
    vector<bool> done(n, false); 

    while (completed < n) {
        int idx = -1;
        for (int i = 0; i < n; i++) {
            if (done[i] || procs[i].at > time) continue; // Bỏ qua tiến trình chưa đến hoặc đã xong
            
            // Ưu tiên BT nhỏ nhất, nếu bằng thì ưu tiên đến trước
            if (idx == -1) idx = i; 
            else if (procs[i].bt < procs[idx].bt) idx = i;
            else if (procs[i].bt == procs[idx].bt && procs[i].at < procs[idx].at) idx = i;
        }

        if (idx == -1) { // Hệ thống rảnh rỗi
            if (gantt.empty() || gantt.back().first != 0) gantt.push_back({0, time + 1});
            else gantt.back().second = time + 1;
            time++; 
        } else {
            time += procs[idx].bt; 
            procs[idx].ct = time;
            procs[idx].tat = procs[idx].ct - procs[idx].at;
            procs[idx].wt = procs[idx].tat - procs[idx].bt;
            done[idx] = true;
            completed++;
            gantt.push_back({procs[idx].id, time});
        }
    }
    printSimulation(gantt, mode);
    return printTable(procs); 
}

pair<double, double> solveSrtf(vector<Process> procs, int mode) {
    int n = procs.size(), time = 0, completed = 0, prevId = -1;
    vector<pair<int,int>> gantt;
    
    for (int i = 0; i < n; i++) procs[i].rt = procs[i].bt; // Khởi tạo thời gian còn lại (RT = BT)

    while (completed < n) {
        int idx = -1;
        for (int i = 0; i < n; i++) {
            if (procs[i].at > time || procs[i].rt == 0) continue;
            
            // Ưu tiên tiến trình có RT nhỏ nhất
            if (idx == -1) idx = i;
            else if (procs[i].rt < procs[idx].rt) idx = i;
            else if (procs[i].rt == procs[idx].rt && procs[i].at < procs[idx].at) idx = i;
        }

        if (idx == -1) {
            if (gantt.empty() || gantt.back().first != 0) gantt.push_back({0, time + 1});
            else gantt.back().second = time + 1;
            prevId = 0;
            time++; 
            continue;
        }

        // Ghi nhận quá trình chuyển ngữ cảnh (Context Switch)
        if (procs[idx].id != prevId) {
            if (prevId != -1 && prevId != 0) gantt.push_back({prevId, time});
            prevId = procs[idx].id;
        }

        procs[idx].rt--; // Thực thi 1 đơn vị thời gian
        time++;

        if (procs[idx].rt == 0) { // Tiến trình đã hoàn thành
            procs[idx].ct = time;
            procs[idx].tat = time - procs[idx].at;
            procs[idx].wt = procs[idx].tat - procs[idx].bt;
            gantt.push_back({procs[idx].id, time});
            prevId = -1;
            completed++;
        }
    }
    printSimulation(gantt, mode);
    return printTable(procs); 
}

pair<double, double> solveRr(vector<Process> procs, int tq, int mode) {
    int n = procs.size(), time = 0, completed = 0;
    vector<pair<int,int>> gantt;
    vector<bool> inQueue(n, false); 
    
    for (int i = 0; i < n; i++) procs[i].rt = procs[i].bt;
    sort(procs.begin(), procs.end(), cmpAt); // Bắt buộc duyệt theo thời gian đến

    queue<int> rq; // Hàng đợi thực thi (Ready Queue)
    int ptr = 0; // Con trỏ theo dõi tiến trình tiếp theo

    // Nạp các tiến trình đã đến vào hàng đợi
    while (ptr < n && procs[ptr].at <= time) {
        rq.push(ptr);
        inQueue[ptr] = true;
        ptr++;
    }

    while (completed < n) {
        if (rq.empty()) { // Nếu hàng đợi trống, tua thời gian đến tiến trình kế tiếp
            if (gantt.empty() || gantt.back().first != 0) gantt.push_back({0, procs[ptr].at});
            else gantt.back().second = procs[ptr].at;
            
            time = procs[ptr].at; 
            while (ptr < n && procs[ptr].at <= time) {
                rq.push(ptr);
                inQueue[ptr] = true;
                ptr++;
            }
            continue;
        }

        int curr = rq.front();
        rq.pop();

        // Chạy tối đa 1 Quantum hoặc đến khi xong
        int runTime = min(tq, procs[curr].rt);
        procs[curr].rt -= runTime;
        time += runTime;
        gantt.push_back({procs[curr].id, time});

        // QUAN TRỌNG: Nạp tiến trình mới đến trước khi đẩy tiến trình đang chạy về cuối hàng đợi
        while (ptr < n && procs[ptr].at <= time) {
            if (!inQueue[ptr]) {
                rq.push(ptr);
                inQueue[ptr] = true;
            }
            ptr++;
        }

        if (procs[curr].rt == 0) { // Cập nhật chỉ số khi hoàn thành
            procs[curr].ct = time;
            procs[curr].tat = time - procs[curr].at;
            procs[curr].wt = procs[curr].tat - procs[curr].bt;
            completed++;
        } else {
            rq.push(curr); // Đẩy lại vào hàng đợi để xoay vòng
        }
    }
    
    // Sắp xếp lại theo ID để in bảng
    sort(procs.begin(), procs.end(), [](Process a, Process b){ return a.id < b.id; });
    printSimulation(gantt, mode);
    return printTable(procs); 
}

pair<double, double> solvePriority(vector<Process> procs, int mode) {
    int n = procs.size(), time = 0, completed = 0;
    vector<pair<int,int>> gantt;
    vector<bool> done(n, false);

    while (completed < n) {
        int idx = -1;
        for (int i = 0; i < n; i++) {
            if (done[i] || procs[i].at > time) continue;
            
            // Số PR càng nhỏ, độ ưu tiên càng cao
            if (idx == -1) idx = i;
            else if (procs[i].prio < procs[idx].prio) idx = i;
            else if (procs[i].prio == procs[idx].prio && procs[i].at < procs[idx].at) idx = i;
        }

        if (idx == -1) {
            if (gantt.empty() || gantt.back().first != 0) gantt.push_back({0, time + 1});
            else gantt.back().second = time + 1;
            time++;
        } else {
            time += procs[idx].bt;
            procs[idx].ct = time;
            procs[idx].tat = procs[idx].ct - procs[idx].at;
            procs[idx].wt = procs[idx].tat - procs[idx].bt;
            
            done[idx] = true;
            completed++;
            gantt.push_back({procs[idx].id, time});
        }
    }
    printSimulation(gantt, mode);
    return printTable(procs); 
}

// In bảng so sánh hiệu năng
void printComparison(pair<double, double> res[]) {
    cout << "\n=================================================\n";
    cout << "                  BANG SO SANH                   \n";
    cout << "=================================================\n";
    cout << "| Thuat toan      | Avg WT        | Avg TAT       |\n";
    cout << "=================================================\n";
    cout << "| [1] FCFS        | " << left << setw(13) << res[1].first << " | " << setw(13) << res[1].second << " |\n";
    cout << "| [2] SJF         | " << left << setw(13) << res[2].first << " | " << setw(13) << res[2].second << " |\n";
    cout << "| [3] SRTF        | " << left << setw(13) << res[3].first << " | " << setw(13) << res[3].second << " |\n";
    cout << "| [4] Round Robin | " << left << setw(13) << res[4].first << " | " << setw(13) << res[4].second << " |\n";
    cout << "| [5] Priority    | " << left << setw(13) << res[5].first << " | " << setw(13) << res[5].second << " |\n";
    cout << "=================================================\n";
}

int main() {
    int inputOpt, modeOpt, algoChoice;
    int n, tq;
    vector<Process> data;
    // Mảng lưu kết quả (dùng index 1-5 để dễ quản lý)
    pair<double, double> results[6]; 

    // LỚP 1: Vòng lặp chính ngoài cùng (Chọn Nguồn Dữ Liệu)
    while (true) {
        cout << "\n=== HE THONG MO PHONG DINH THOI CPU ===\n";
        cout << "1. Nhap du lieu thu cong\n";
        cout << "2. Dung bo Test Case mau (co san)\n";
        cout << "0. Thoat chuong trinh\n";
        cout << "Chon (0-2): "; cin >> inputOpt;

        if (inputOpt == 0) {
            cout << "\nThoat chuong trinh. Tam biet!\n";
            break; // Thoát hẳn chương trình
        }

        if (inputOpt != 1 && inputOpt != 2) {
            cout << "\nLua chon khong hop le. Vui long nhap lai!\n";
            continue;
        }

        // Tải dữ liệu dựa trên lựa chọn
        if (inputOpt == 1) {
            cout << "Nhap so luong tien trinh: "; cin >> n;
            data.resize(n);
            for (int i = 0; i < n; i++) {
                cout << "Tien trinh " << i + 1 << " (AT BT PR): ";
                cin >> data[i].at >> data[i].bt >> data[i].prio;
                data[i].id = i + 1; data[i].rt = 0; 
                data[i].ct = 0; data[i].tat = 0; data[i].wt = 0; 
            }
            cout << "Nhap Quantum Time (cho Round Robin): "; cin >> tq;
        } else {
            // Dữ liệu mẫu đối chiếu với slide
            n = 5; tq = 3;
            data = {
                {1, 0, 5, 0, 2, 0, 0, 0}, 
                {2, 1, 3, 0, 3, 0, 0, 0},
                {3, 2, 8, 0, 1, 0, 0, 0},
                {4, 3, 2, 0, 4, 0, 0, 0},
                {5, 4, 4, 0, 5, 0, 0, 0}
            };
            cout << "\nDa load Test Case: 5 tien trinh, Quantum = 3.\n";
        }

        // LỚP 2: Vòng lặp Chọn Chế độ hiển thị
        while (true) {
            cout << "\n=== CHON CHE DO HIEN THI ===\n";
            cout << "0. In toan bo ket qua (khong delay)\n";
            cout << "1. Chay tu dong (Delay 0.8s moi buoc)\n";
            cout << "2. Quay lai chon du lieu\n";
            cout << "Chon (0-2): "; cin >> modeOpt;

            if (modeOpt == 2) {
                cout << "\nQuay lai menu chon du lieu...\n";
                break; // Thoát Lớp 2, về lại Lớp 1 (Chọn Nguồn Dữ Liệu)
            }

            if (modeOpt != 0 && modeOpt != 1) {
                cout << "\nLua chon khong hop le. Vui long nhap lai!\n";
                continue;
            }

            // LỚP 3: Vòng lặp Chọn Thuật toán
            do {
                cout << "\n===================================================\n";
                cout << "          BANG CHU THICH TU VIET TAT               \n";
                cout << " AT : Thoi gian den     | CT : Thoi gian hoan thanh\n";
                cout << " BT : Thoi gian phuc vu | TAT: Thoi gian luu he thong\n";
                cout << " PR : Do uu tien        | WT : Thoi gian cho\n";
                cout << "===================================================\n";

                cout << "\nChon thuat toan dinh thoi CPU:\n\n";
                cout << "  [1] FCFS (First Come First Served)\n";
                cout << "  [2] SJF (Shortest Job First)\n";
                cout << "  [3] SRTF (Shortest Remaining Time First)\n";
                cout << "  [4] Round Robin\n";
                cout << "  [5] Priority Scheduling\n";
                cout << "  [6] Chay tat ca & Xem bang so sanh (Xem Full)\n\n";
                cout << "  [0] Quay lai\n\n";
                
                cout << "Nhap lua chon (0-6): ";
                cin >> algoChoice;

                if (algoChoice == 0) {
                    cout << "\nQuay lai menu chon che do hien thi...\n";
                    break; // Thoát Lớp 3, về lại Lớp 2 (Chọn Chế độ)
                }

                switch (algoChoice) {
                    case 1: 
                        cout << "\n>>> 1. FCFS\n"; 
                        solveFcfs(data, modeOpt); 
                        break;
                    case 2: 
                        cout << "\n>>> 2. SJF\n"; 
                        solveSjf(data, modeOpt); 
                        break;
                    case 3: 
                        cout << "\n>>> 3. SRTF\n"; 
                        solveSrtf(data, modeOpt); 
                        break;
                    case 4: 
                        cout << "\n>>> 4. ROUND ROBIN\n"; 
                        solveRr(data, tq, modeOpt); 
                        break;
                    case 5: 
                        cout << "\n>>> 5. PRIORITY\n"; 
                        solvePriority(data, modeOpt); 
                        break;
                    case 6:
                        cout << "\n>>> DANG CHAY TOAN BO THUAT TOAN...\n";
                        // Chạy và lưu kết quả vào mảng kèm tiêu đề
                        cout << "\n>>> [1] KET QUA THUAT TOAN FCFS <<<";
                        results[1] = solveFcfs(data, modeOpt);
                        
                        cout << "\n>>> [2] KET QUA THUAT TOAN SJF <<<";
                        results[2] = solveSjf(data, modeOpt);
                        
                        cout << "\n>>> [3] KET QUA THUAT TOAN SRTF <<<";
                        results[3] = solveSrtf(data, modeOpt);
                        
                        cout << "\n>>> [4] KET QUA THUAT TOAN ROUND ROBIN <<<";
                        results[4] = solveRr(data, tq, modeOpt);
                        
                        cout << "\n>>> [5] KET QUA THUAT TOAN PRIORITY <<<";
                        results[5] = solvePriority(data, modeOpt);
                        
                        // Gọi hàm in bảng tổng hợp
                        printComparison(results);
                        break;
                    default:
                        cout << "Lua chon khong hop le! Vui long nhap lai.\n";
                }
            } while (algoChoice != 0); 
        }
    }
    
    return 0;
}