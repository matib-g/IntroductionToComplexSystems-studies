//
//  main.cpp
//  SitePercolation
//
//  Created by Mateusz Bulanda - Gorol on 20/03/2021.
//

#include <iostream>
#include <cstdlib>
#include <stdlib.h>  // for srand(), rand() and RAND_MAX
#include <math.h>  // for sqrt(), log() and sin()
#include <time.h>  // for time()
#include <fstream>
#include <sstream>
#include <random>
#include <map>
#include <chrono>

using namespace std;

struct params{
    int Max = 0;
    int nCluster = 0;
};


void show (int** A, int L){
    for (int i=0; i<L; i++){
        for (int j=0; j<L; j++) {
            cout << A[i][j] << " ";
        }
        cout << endl;
    }
    cout << endl << endl;
}


bool outOfRange (int i, int j, int L){
    if (i<0 || i>L) return true;
    if (j<0 || j>L) return true;
    else return false;
}


bool burnable (int** A, int i, int j, int L){
    if (outOfRange(i, j, L) == true) return false;
    if (A[i][j] == 1) return true;
    else return false;
}


/*bool burning (int** A, int L){
    for (int j=0; j<L; j++){
        if (A[0][j] == 1) A[0][j] = 2;
    }
    int t=2;
    bool end = false;
    while (end == false){
        end = true;
        for (int i=0; i<L; i++){
            for (int j=0; j<L; j++){
                if (A[i][j] == t){
                    end = false;
                    if (burnable(A, i-1, j-1, L) == true) A[i-1][j-1] = t;
                    if (burnable(A, i-1, j, L) == true) A[i-1][j] = t;
                    if (burnable(A, i, j-1, L) == true) A[i][j-1] = t;
                    if (burnable(A, i, j+1, L) == true) A[i][j+1] = t;
                    if (burnable(A, i+1, j, L) == true) {
                        A[i+1][j] = t;
                        if (i+1 == L-1) goto stop;
                    }
                    if (burnable(A, i+1, j+1, L)){
                        A[i+1][j+1] = t;
                        if (i+1 == L-1) goto stop;
                    }
                }
            }
        }
        //cout << t << endl;
        t++;
    }
    return false;
    stop:
    return true;
    
}*/



bool checkPath(int** a,int L) {
    //init burn side
    for (int i = 0; i < L; i++) {
        if (a[0][i] != 0) {
            a[0][i] = 2;
        }
    }

    //burn process
    bool burnFlag = true;
    bool endFlag = false;
    int burnLevel = 2;
    while (burnFlag) {
        burnFlag = false;
        for (int row = 0; row < L; row++) {
            for (int col = 0; col < L; col++) {
                if (a[row][col] == burnLevel) {
                    burnFlag = true;
                    if (row - 1 >= 0 && col - 1 >= 0){
                        if (a[row - 1][col - 1] == 1){
                            a[row - 1][col - 1] = burnLevel + 1;
                        }
                    }
                    //n1 row-1 col
                    if (row - 1 >= 0) {
                        if (a[row - 1][col] == 1) {
                            a[row - 1][col] = burnLevel + 1;
                        }
                    }
                    //n2 row col-1
                    if (col - 1 >= 0) {
                        if (a[row][col - 1] == 1) {
                            a[row][col - 1] = burnLevel + 1;
                        }
                    }
                    //n3 row col+1
                    if (col + 1 < L) {
                        if (a[row][col + 1] == 1) {
                            a[row][col + 1] = burnLevel + 1;
                        }
                    }
                    //n4 row+1 col
                    if (row + 1 < L) {
                        if (a[row + 1][col] == 1) {
                            a[row + 1][col] = burnLevel + 1;
                            if (row + 1 == L - 1) {
                                goto stop;
                            }
                        }
                    }
                    if (row + 1 < L && col + 1 < L){
                        if (a[row + 1][col + 1] == 1){
                            a[row + 1][col + 1] = burnLevel + 1;
                            if (row + 1 == L - 1) {
                                goto stop;
                            }
                        }
                    }
                }
            }
        }
        burnLevel++;
    }
    return false;
    stop:
    return true;
}







/*bool isEmpty (int** A, int i, int j, int L){
    if (outOfRange(i, j, L) == true) return true;
    if (A[i][j] == 1 || A[i][j] == 0) return true;
    else return A[i][j] == 0;
}


void mergeClusters(int** A, int s, int l, int L){
    for (int i=0; i<L; i++){
        for (int j=0; j<0; j++){
            if (A[i][j] == s) A[i][j] = l;
        }
    }
}



map<int,int> allClaster (int** A, int L){
    int k = 2;
    //params cl;
    map<int, int> clusters;
    for (int i=0; i<L; i++){
        for (int j=0; j<L; j++) {
            if (A[i][j]){
            //  if top and left are empty:
            if (isEmpty(A, i-1, j, L) && isEmpty(A, i, j-1, L)){
                k++;
                A[i][j] = k;
                clusters.insert({k, 1});
                //cl.nCluster++;
            }
            //  if one is occupied with 𝑘0:
            if ((!isEmpty(A, i-1, j, L) && isEmpty(A, i, j-1, L)) || (isEmpty(A, i-1, j, L) && !isEmpty(A, i, j-1, L))){
                if (!isEmpty(A, i-1, j, L)){
                    A[i][j] = A[i-1][j];
                    if (clusters[A[i-1][j]] >= 0) clusters[A[i-1][j]]++;
                }
                else {
                    A[i][j] = A[i][j-1];
                    if (clusters[A[i][j-1]] >= 0) clusters[A[i][j-1]]++;
                    
                }
            }
            //  if both are occupied:
            if (!isEmpty(A, i-1, j, L) && !isEmpty(A, i, j-1, L)){
                // (𝑘1 = 𝑘2)
                if (A[i-1][j] == A[i][j-1]){
                    A[i][j] = A[i-1][j];
                    if (clusters[A[i-1][j]] >= 0) clusters[A[i-1][j]]++;
                }
                // (𝑘1 ≠ 𝑘2)
                else {
                    int s = 0;
                    int l = 0;
                    if (clusters[A[i-1][j]] > clusters[A[i][j-1]]){
                        s = A[i][j-1];
                        l = A[i-1][j];
                    }
                    else {
                        s = A[i-1][j];
                        l = A[i][j-1];
                    }
                    A[i][j] = l;
                    clusters[l] = clusters[l] + clusters[s] + 1;
                    clusters.erase(s);
                    //cl.nCluster--;
                    mergeClusters(A, s, l, L);
                }
            }
            }
        }
    }
    return clusters;
}*/




bool isOccupied(int** a,int L,int row,int col) {
    if (row < 0 || row >= L) {
        return false;
    }
    if (col < 0 || col >= L) {
        return false;
    }
    return a[row][col] != 0;
}
void unionClusters(int** a, int L, int sm, int lg) {
    for (int i = 0; i < L; i++) {
        for (int j = 0; j < L; j++) {
            if (a[i][j] == sm) {
                a[i][j] = lg;
            }
        }
    }
}
map<int,int> findClusters(int** a, int L) {
    int k = 2;
    map<int, int> clusters;
    for (int row = 0; row < L; row++) {
        for (int col = 0; col < L; col++) {
            if (a[row][col]) {
                //top and left and top-left empty
                if (!isOccupied(a, L, row - 1, col) && !isOccupied(a, L, row, col - 1) && !isOccupied(a, L, row - 1, col - 1)) {
                    k++;
                    a[row][col] = k;
                    clusters.insert({ k, 1 });
                }
                //one is occupied with k0
                if ((isOccupied(a, L, row - 1, col) && !isOccupied(a, L, row, col - 1) && !isOccupied(a, L, row - 1, col - 1)) || (!isOccupied(a, L, row - 1, col) && isOccupied(a, L, row, col - 1) && !isOccupied(a, L, row - 1, col - 1)) || (!isOccupied(a, L, row - 1, col) && !isOccupied(a, L, row, col - 1) && isOccupied(a, L, row - 1, col - 1))) {
                    if ((isOccupied(a, L, row - 1, col))) {
                        a[row][col] = a[row-1][col];
                        if (clusters[a[row - 1][col]] >= 0) {
                            clusters[a[row - 1][col]]++;
                        }
                        
                    }
                    if (isOccupied(a, L, row, col - 1)) {
                        a[row][col] = a[row][col-1];
                        if (clusters[a[row][col - 1]] >= 0) {
                            clusters[a[row][col - 1]]++;
                        }
                        
                    }
                    else {
                        a[row][col] = a[row-1][col-1];
                        if (clusters[a[row-1][col-1]] >= 0) {
                            clusters[a[row-1][col-1]]++;
                        }
                    }
                    
                }
                //top and left occupied
                if (isOccupied(a, L, row - 1, col) && isOccupied(a, L, row, col - 1) && !isOccupied(a, L, row - 1, col - 1)) {
                    if (a[row - 1][col] == a[row][col - 1]) {
                        a[row][col] = a[row - 1][col];
                        if (clusters[a[row - 1][col]] >= 0) {
                            clusters[a[row - 1][col]]++;
                        }
                    }
                    //top and left occupied but k1!=k2
                    else {
                        int sm = 0;
                        int lg = 0;
                        if (clusters[a[row - 1][col]] > clusters[a[row][col - 1]]) {
                            sm = a[row][col - 1];
                            lg = a[row - 1][col];
                        }
                        else {
                            sm = a[row-1][col];
                            lg = a[row][col-1];
                        }
                        a[row][col] = lg;
                        clusters[lg] = clusters[lg]+ clusters[sm]+1;
                        clusters.erase(sm);
                        unionClusters(a, L, sm, lg);
                    }
                }
                //top and top-left occupied
                if (isOccupied(a, L, row - 1, col) && !isOccupied(a, L, row, col - 1) && isOccupied(a, L, row - 1, col - 1)) {
                    if (a[row - 1][col] == a[row - 1][col - 1]) {
                        a[row][col] = a[row - 1][col];
                        if (clusters[a[row - 1][col]] >= 0) {
                            clusters[a[row - 1][col]]++;
                        }
                    }
                    //top and top-left occupied but k1!=k2
                    else {
                        int sm = 0;
                        int lg = 0;
                        if (clusters[a[row - 1][col]] > clusters[a[row - 1][col - 1]]) {
                            sm = a[row - 1][col - 1];
                            lg = a[row - 1][col];
                        }
                        else {
                            sm = a[row-1][col];
                            lg = a[row - 1][col-1];
                        }
                        a[row][col] = lg;
                        clusters[lg] = clusters[lg]+ clusters[sm]+1;
                        clusters.erase(sm);
                        unionClusters(a, L, sm, lg);
                    }
                }
                //top-left and left occupied
                if (!isOccupied(a, L, row - 1, col) && isOccupied(a, L, row, col - 1) && isOccupied(a, L, row - 1, col - 1)) {
                    if (a[row - 1][col - 1] == a[row][col - 1]) {
                        a[row][col] = a[row - 1][col - 1];
                        if (clusters[a[row - 1][col - 1]] >= 0) {
                            clusters[a[row - 1][col - 1]]++;
                        }
                    }
                    //top-left and left occupied but k1!=k2
                    else {
                        int sm = 0;
                        int lg = 0;
                        if (clusters[a[row - 1][col - 1]] > clusters[a[row][col - 1]]) {
                            sm = a[row][col - 1];
                            lg = a[row - 1][col - 1];
                        }
                        else {
                            sm = a[row-1][col - 1];
                            lg = a[row][col-1];
                        }
                        a[row][col] = lg;
                        clusters[lg] = clusters[lg]+ clusters[sm]+1;
                        clusters.erase(sm);
                        unionClusters(a, L, sm, lg);
                    }
                }
                //top, top-left and left are occupied
                if (isOccupied(a, L, row - 1, col) && isOccupied(a, L, row - 1, col - 1) && isOccupied(a, L, row, col - 1)){
                    if (a[row - 1][col] == a[row - 1][col - 1] && a[row - 1][col] != a[row][col - 1]){
                        int sm = 0;
                        int lg = 0;
                        if (clusters[a[row - 1][col]] + clusters[a[row - 1][col - 1]] > clusters[a[row][col - 1]]){
                            sm = a[row][col - 1];
                            lg = a[row - 1][col];
                        }
                        else {
                            sm = a[row - 1][col];
                            lg = a[row][col - 1];
                        }
                        a[row][col] = lg;
                        clusters[lg] = clusters[lg]+ clusters[sm]+1;
                        clusters.erase(sm);
                        unionClusters(a, L, sm, lg);
                        
                    }
                    if (a[row - 1][col - 1] == a[row][col - 1] && a[row - 1][col - 1] != a[row - 1][col]){
                        int sm = 0;
                        int lg = 0;
                        if (clusters[a[row - 1][col - 1]] + clusters[a[row][col - 1]] > clusters[a[row - 1][col]]){
                            sm = a[row - 1][col];
                            lg = a[row - 1][col - 1];
                        }
                        else {
                            sm = a[row - 1][col - 1];
                            lg = a[row - 1][col];
                        }
                        a[row][col] = lg;
                        clusters[lg] = clusters[lg]+ clusters[sm]+1;
                        clusters.erase(sm);
                        unionClusters(a, L, sm, lg);
                    }
                    if (a[row - 1][col] == a[row][col - 1] && a[row - 1][col] != a[row - 1][col - 1]){
                        int sm = 0;
                        int lg = 0;
                        if (clusters[a[row - 1][col]] + clusters[a[row][col - 1]] > clusters[a[row - 1][col - 1]]){
                            sm = a[row - 1][col - 1];
                            lg = a[row - 1][col];
                        }
                        else {
                            sm = a[row - 1][col];
                            lg = a[row - 1][col - 1];
                        }
                        a[row][col] = lg;
                        clusters[lg] = clusters[lg]+ clusters[sm]+1;
                        clusters.erase(sm);
                        unionClusters(a, L, sm, lg);
                    }
                    if (a[row - 1][col - 1] == a[row - 1][col] && a[row - 1][col - 1] == a[row][col - 1]){
                        a[row][col] = a[row - 1][col - 1];
                        if (clusters[a[row - 1][col - 1]] >= 0) {
                            clusters[a[row - 1][col - 1]]++;
                        }
                    }
                }
            }
        }
    }
    return clusters;


}









int max(map<int, int> clusters){
    int max = 0;
    for (auto const& c : clusters){
        if (c.second > max) max = c.second;
    }
    return max;
}





int main(int argc, const char * argv[]) {
    
    //ifstream input("perc−ini.txt");
    int L = 10;
    int T = 10000;
    float p0 = 0.2;
    float pk = 0.8;
    float dp = 0.1;
    //input >> L >> T >> p0 >> pk >> dp;
    //cout << "Input: " << L << "  " << T << "  " << p0 << "  " << pk << "  " << dp << endl;
    
    /*fstream file1;
    string fileName1 = "Ave−L" + to_string(L) +" T " + to_string(T) +" .txt";
    file1.open(fileName1, ios::out | ios::trunc);
    if (file1.good()==false) cout << "Failed to open file!";
    
    file1 << "p  Pflow  <smax>" << endl;*/
    
    int ** A = new int * [L];
    int ** B = new int * [L];
    for (int i=0; i<L; i++){
        A[i] = new int [L];
        B[i] = new int [L];
    }
    
    //random
    unsigned seed = std::chrono::system_clock::now().time_since_epoch().count();
    std::default_random_engine generator(seed);
    std::uniform_real_distribution<float> distribution(0.0, 1.0);
    
    
    for (float p=p0; p<=pk+0.0003; p+=dp){
        fstream file2;
            string fileName2 = "Dist−p" + to_string(p) +" L" + to_string(L) +" T " + to_string(T) +" .txt";
            file2.open(fileName2, ios::out | ios::trunc);
            if (file2.good()==false) cout << "Failed to open file!";
        float pflow = 0;
        float smax = 0;
        
        map<int, float> cDist;
        
        for (int msc=0; msc<T; msc++){
            for (int i=0; i<L; i++){
                for (int j=0; j<L; j++){
                    float r = distribution(generator);
                    if (r<p) {
                        A[i][j] = 1;
                        B[i][j] = 1;
                    }
                    else {
                        A[i][j] = 0;
                        B[i][j] = 0;
                    }
                }
            }
            auto allClusters = findClusters(A, L);
            int max = 0;
            for (auto const& c : allClusters){
                cDist[c.second] += 1;
                if (c.second > max) max = c.second;
            }
            smax += max;
            bool isPath = checkPath(B, L);
            if (isPath == true) pflow += 1;
            
            }
        for (auto const& c : cDist){
            cDist[c.first] /= (float)T;
             //cout << smax << "  " << n << endl;
        }
        
        for (auto const& c : cDist){
            cout << c.first << "  " << c.second << endl;
            file2 << c.first << "  " << c.second << endl;
        }
        
        pflow = pflow / T;
        smax = smax / T;
        
        cout << p << "  " << pflow << "  " << smax << endl;
        //file1 << p << "  " << pflow << "  " << smax << endl;

            file2.close();
    }
    
    //file1.close();
    for (int i=0; i<L; i++){
        delete [] A[i];
        delete [] B[i];
    }
    delete [] A;
    delete [] B;
    return 0;
    
}
