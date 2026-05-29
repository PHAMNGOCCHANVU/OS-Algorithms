#include <bits/stdc++.h>

using namespace std;

struct Process {
    // Thông số đầu vào
    int id, at, bt, rt, prio; 
    // Thông số đầu ra
    int ct, tat, wt; 
};

// Sắp xếp tiến trình theo thời gian đến cho FCFS
bool cmpFcfs(Process a, Process b) {
    return a.at < b.at; 
}

// Sắp xếp tiến trình theo thời gian đến (Dùng chung)
bool cmpAt(Process a, Process b) { 
    return a.at < b.at; 
}

// In biểu đồ Gantt
void printGantt(const vector<pair<int, int>>& gantt) {
    cout << "\n--- GANTT CHART ---\n|";
    for (auto p : gantt) cout << " P" << p.first << " |";
    cout << "\n0";
    for (auto p : gantt) cout << setw(5) << p.second;
    cout << "\n"; 
}

// In bảng thống kê các chỉ số
pair<double, double> printTable(vector<Process>& procs) {
    double totalWt = 0, totalTat = 0; 
    int minAt = INT_MAX, maxCt = 0; 

    cout << "\nPID\tAT\tBT\tPR\tCT\tTAT\tWT\n";
    for (auto& p : procs) {
        totalWt += p.wt; 
        totalTat += p.tat;
        
        // Tìm khoảng thời gian chạy để tính Throughput
        if (p.at < minAt) minAt = p.at;
        if (p.ct > maxCt) maxCt = p.ct;

        cout << p.id << "\t" << p.at << "\t" << p.bt << "\t" << p.prio << "\t"
             << p.ct << "\t" << p.tat << "\t" << p.wt << "\n";
    }
    
    // Tính thông năng và trung bình
    double throughput = (double)procs.size() / (maxCt - minAt);
    double avgWt = totalWt / procs.size();
    double avgTat = totalTat / procs.size();
    
    cout << fixed << setprecision(2) << "Avg WT: " << avgWt 
         << " | Avg TAT: " << avgTat << "\n";
    cout << "Throughput: " << throughput << " processes/unit time\n";

    return make_pair(avgWt, avgTat);
}

// In quá trình thực thi (Có hỗ trợ delay)
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

        if (mode == 1) { // Chạy tự động có delay 0.8s
            this_thread::sleep_for(chrono::milliseconds(800));
        }
    }
    
    // Vẽ Gantt ngang
    cout << "\n|";
    for (auto p : gantt) {
        if (p.first == 0) cout << " IDLE |";
        else cout << " P" << p.first << " |";
    }
    cout << "\n0";
    for (auto p : gantt) cout << setw(6) << p.second;
    cout << "\n"; 
}

pair<double, double> solveFcfs(vector<Process> procs, int mode) {
    sort(procs.begin(), procs.end(), cmpFcfs); 
    int time = 0; // Đồng hồ hệ thống
    vector<pair<int, int>> gantt;
    
    for (int i = 0; i < procs.size(); i++) {
        if (time < procs[i].at) {
            gantt.push_back({0, procs[i].at}); // Ghi nhận CPU rảnh
            time = procs[i].at;
        }
        
        time += procs[i].bt; // Chạy một mạch không ngắt
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
            if (done[i] || procs[i].at > time) continue; 
            
            // Tìm tiến trình có Burst Time ngắn nhất
            if (idx == -1) idx = i; 
            else if (procs[i].bt < procs[idx].bt) idx = i;
            else if (procs[i].bt == procs[idx].bt && procs[i].at < procs[idx].at) idx = i;
        }

        if (idx == -1) {
            if (gantt.empty() || gantt.back().first != 0) gantt.push_back({0, time + 1});
            else gantt.back().second = time + 1;
            time++; 
        } else {
            time += procs[idx].bt; // Chạy không ngắt (Non-preemptive)
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
    
    for (int i = 0; i < n; i++) procs[i].rt = procs[i].bt; // Khởi tạo thời gian còn lại (RT)

    while (completed < n) {
        int idx = -1;
        for (int i = 0; i < n; i++) {
            if (procs[i].at > time || procs[i].rt == 0) continue;
            
            // Tìm tiến trình có Thời gian còn lại ngắn nhất
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

        // Ghi nhận sự chuyển đổi ngữ cảnh (Context Switch)
        if (procs[idx].id != prevId) {
            if (prevId != -1 && prevId != 0) gantt.push_back({prevId, time});
            prevId = procs[idx].id;
        }

        procs[idx].rt--; // Chạy từng đơn vị thời gian
        time++;

        if (procs[idx].rt == 0) { // Tiến trình đã chạy xong
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
    sort(procs.begin(), procs.end(), cmpAt); // Bắt buộc xếp theo thời gian đến

    queue<int> rq; // Hàng đợi Ready Queue
    int ptr = 0; // Con trỏ theo dõi tiến trình nạp vào

    // Nạp các tiến trình có sẵn ban đầu
    while (ptr < n && procs[ptr].at <= time) {
        rq.push(ptr);
        inQueue[ptr] = true;
        ptr++;
    }

    while (completed < n) {
        if (rq.empty()) { // Nếu hàng đợi trống thì tua thời gian
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

        // Chạy tối đa 1 Quantum Time
        int runTime = min(tq, procs[curr].rt);
        procs[curr].rt -= runTime;
        time += runTime;
        gantt.push_back({procs[curr].id, time});

        // QUAN TRỌNG: Nạp ngay tiến trình mới trước khi đẩy tiến trình cũ về cuối
        while (ptr < n && procs[ptr].at <= time) {
            if (!inQueue[ptr]) {
                rq.push(ptr);
                inQueue[ptr] = true;
            }
            ptr++;
        }

        if (procs[curr].rt == 0) { // Tiến trình hoàn thành
            procs[curr].ct = time;
            procs[curr].tat = time - procs[curr].at;
            procs[curr].wt = procs[curr].tat - procs[curr].bt;
            completed++;
        } else {
            rq.push(curr); // Chưa xong thì xếp hàng lại
        }
    }
    
    // Sắp xếp lại theo ID để in bảng chuẩn
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
            
            // Số Prio càng nhỏ thì ưu tiên càng cao
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

// In bảng so sánh 5 thuật toán
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
    int inputOpt, modeOpt;
    int n, tq;
    vector<Process> data;
    pair<double, double> results[6]; 

    while (true) {
        cout << "\n=== HE THONG MO PHONG DINH THOI CPU ===\n";
        cout << "1. Nhap du lieu thu cong\n";
        cout << "2. Doc du lieu tu file (Test Case)\n";
        cout << "0. Thoat chuong trinh\n";
        cout << "Chon (0-2): "; cin >> inputOpt;

        if (inputOpt == 0) {
            cout << "\nThoat chuong trinh. Tam biet!\n";
            break;
        }

        if (inputOpt != 1 && inputOpt != 2) {
            cout << "\nLua chon khong hop le. Vui long nhap lai!\n";
            continue;
        }

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
            int fileChoice;
            cout << "\nDanh sach cac Test Case mau:\n";
            cout << "1. Test Case 1 (Hieu ung Doan Tau - Dung cho FCFS)\n";
            cout << "2. Test Case 2 (Ngat quyen lien tuc - Dung cho SRTF/RR)\n";
            cout << "3. Test Case 3 (Uu tien dac biet - Dung cho Priority)\n";
            cout << "Chon file (1-3): "; cin >> fileChoice;

            // Xử lý tự động map đúng tên file có dấu cách của ông
            string filename = "";
            if (fileChoice == 1) filename = "TestCase1.txt";
            else if (fileChoice == 2) filename = "TestCase2.txt";
            else if (fileChoice == 3) filename = "TestCase3.txt";
            else filename = "TestCase" + to_string(fileChoice) + ".txt"; // Đề phòng ông gõ số khác
            
            ifstream file(filename);

            if (!file.is_open()) {
                cout << "\n[LOI] Khong tim thay file '" << filename << "' trong thu muc hien tai!\n";
                cout << "Vui long kiem tra lai xem file co dang nam chung thu muc voi file code khong.\n";
                continue; 
            }

            // Xử lý logic file: Đọc n đầu tiên, tq cuối cùng
            file >> n; 
            data.resize(n);
            for (int i = 0; i < n; i++) {
                file >> data[i].at >> data[i].bt >> data[i].prio; 
                data[i].id = i + 1; data[i].rt = 0; 
                data[i].ct = 0; data[i].tat = 0; data[i].wt = 0; 
            }
            file >> tq; // Đọc Quantum Time ở dòng cuối
            file.close(); 
            
            // In chi tiết bộ Test Case ra màn hình
            cout << "\n===================================================\n";
            cout << "       DA LOAD DU LIEU TU '" << filename << "'\n";
            cout << "===================================================\n";
            cout << "- So luong tien trinh : " << n << "\n";
            cout << "- Quantum Time (RR)   : " << tq << "\n";
            cout << "---------------------------------------------------\n";
            cout << "PID\tAT\tBT\tPR\n";
            for (const auto& p : data) {
                cout << p.id << "\t" << p.at << "\t" << p.bt << "\t" << p.prio << "\n";
            }
            cout << "===================================================\n";
        }

        while (true) {
            cout << "\n=== CHON CHE DO HIEN THI ===\n";
            cout << "0. In toan bo ket qua (khong delay)\n";
            cout << "1. Chay tu dong (Delay 0.8s moi buoc)\n";
            cout << "2. Quay lai chon du lieu\n";
            cout << "Chon (0-2): "; cin >> modeOpt;

            if (modeOpt == 2) {
                cout << "\nQuay lai menu chon du lieu...\n";
                break; 
            }

            if (modeOpt != 0 && modeOpt != 1) {
                cout << "\nLua chon khong hop le. Vui long nhap lai!\n";
                continue;
            }

            int algoChoice;
            do {
                cout << "\n===================================================\n";
                cout << "          BANG CHU THICH & CONG THUC TINH          \n";
                cout << "===================================================\n";
                cout << " PID : Process ID (Ma tien trinh)\n";
                cout << " AT  : Arrival Time (Thoi gian den)\n";
                cout << " BT  : Burst Time (Thoi gian thuc thi)\n";
                cout << " PR  : Priority (Do uu tien)\n";
                cout << " CT  : Completion Time (Thoi gian hoan thanh)\n";
                cout << " TAT : Turnaround Time (Thoi gian luu he thong)\n";
                cout << "       => Cong thuc: TAT = CT - AT\n";
                cout << " WT  : Waiting Time (Thoi gian cho)\n";
                cout << "       => Cong thuc: WT = TAT - BT\n";
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
                    break;
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