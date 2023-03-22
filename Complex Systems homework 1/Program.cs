using System;
using System.IO;
using System.Threading;
using System.Linq;

namespace Complex_Systems_homework_1
{
    class Variables
    {
        public int L { get; set; } //dlugosc boku
        public int T { get; set; } //ilosc krokow
        public double p0 { get; set; } //minimalna wartosc p
        public double p { get; set; }
        public double pk { get; set; } //maksymalna wartosc p
        public double dp { get; set; } //krok p
        public int[,] out1 { get; set; }
        public double[] out2 { get; set; }
        public double[,] zapis { get; set; }
        public double[,] zapis_klustrow { get; set; }

        public void Parameters() //odczytaj zmienne z pliku tekstowego
        {
            string[] lines = new string[6];
            int counter = 0;

            try
            {
                using (StreamReader sr = new StreamReader("Parameters.txt"))
                {
                    string line = sr.ReadLine();

                    while (line != null)
                    {
                        lines[counter] = line.Substring(line.IndexOf(@": ") + 1);
                        line = sr.ReadLine();
                        counter++;
                    }

                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            L = Convert.ToInt32(lines[0]);
            T = Convert.ToInt32(lines[1]);
            p0 = Convert.ToDouble(lines[2]);
            pk = Convert.ToDouble(lines[3]);
            dp = Convert.ToDouble(lines[4]);
            p = p0;
        }

        public int[,] MakeLattice() //utworz plansze
        {
            int[,] Lttc = new int[L, L]; //make lattice

            var rand = new Random(); //new Random class for RNG

            int counter = 0;

            for(int x = 0; x < L; x++)
            {
                for(int y = 0; y < L; y++)
                {
                    if (rand.NextDouble() < p)
                    {
                        Lttc[x, y] = 1;
                        counter++;
                    }
                    else
                    {
                        Lttc[x, y] = 0;
                    }
                }
            }

            //Console.WriteLine("Number of filled slots: {0}", counter);
            return Lttc;
        }

        public int[,] CopyLattice(int[,] Lattice)
        {
            int[,] CopyLattice = new int[L, L];

            for(int x = 0; x < L; x++)
            {
                for(int y = 0; y < L; y++)
                {
                    CopyLattice[x, y] = Lattice[x, y];
                }
            }

            return CopyLattice;
        }

        public int BurningMethod(int[,] Lttic) //zobacz czy jest droga od poczatku do konca
        {
            int BurningEnd = 0;

            for(int i = 0; i < L; i++) // podpal drzewa na gorze
            {

                if (Lttic[0, i] == 1)
                {
                    Lttic[0, i] = 2;
                } 
            }

            for(int t = 2; (t + 1) < T; t++) //petla na iteracje
            {
                for(int x = 0; x < L; x++)
                {
                    for(int y = 0; y < L; y++)
                    {

                        if (Lttic[x, y] == t) //znajdz palace sie drzewa
                        {

                            if (x > 0) //warunki brzegowe
                            {

                                if (Lttic[x - 1, y] == 1) //podpal okoliczne drzewa
                                {
                                    Lttic[x - 1, y] = t + 1;
                                    BurningEnd = t + 1;
                                }
                            }

                            if (x < L - 1)
                            {

                                if (Lttic[x + 1, y] == 1)
                                {
                                    Lttic[x + 1, y] = t + 1;
                                    BurningEnd = t + 1;
                                }
                            }

                            if (y > 0)
                            {

                                if (Lttic[x, y - 1] == 1)
                                {
                                    Lttic[x, y - 1] = t + 1;
                                    BurningEnd = t + 1;
                                }
                            }

                            if (y < L - 1)
                            {

                                if (Lttic[x, y + 1] == 1)
                                {
                                    Lttic[x, y + 1] = t + 1;
                                    BurningEnd = t + 1;
                                }
                            }
                            
                        }

                        if (Lttic[L - 1, y] == t + 1) //sprawdz czy doszlismy do konca
                        {
                            return t;
                        }
                    }
                }

                if (BurningEnd == t) //nie podpalilismy zadnego drzewa - nie ma czego palic i koniec
                {
                    break;
                }
                else //podpalilismy cos, idz dalej
                {
                    continue;
                }
            }

            return 0;
        }

        public int ClusterCount2(int[,] Lattice) //liczenie z naprawianiem indeksow
        {
            int[] M_k = new int[L * L + 1];
            for (int i = 0; i < L * L; i++)
            {
                M_k[i] = 0;
            }

            int k = 2;
            M_k[k] = 1;

            for (int x = 0; x < L; x++)
            {

                for (int y = 0; y < L; y++)
                {

                    if (Lattice[x, y] == 1)
                    {

                        if (x == 0) //jestesmy w pierwszym gornym rzedzie
                        {

                            if (y == 0) //jestesmy w lewym gornym rogu - nowy kluster
                            {
                                Lattice[x, y] = k; //przypisz wartosc polu
                                k++; //zwieksz licznik klustra
                                M_k[k]++; //zwieksz licznik obiektow w klustrze
                            }
                            else //nie jestesmy w lewnym gornym rogu
                            {

                                if (Lattice[x, y - 1] > 1) //mamy kluster po lewej
                                {
                                    Lattice[x, y] = Lattice[x, y - 1]; //przypisz wartosc lewego klustra obecnemu polu
                                    M_k[Lattice[x, y]]++; //zwieksz licznik obiektow
                                }
                                else //nie mamy klustra po lewej - nowy kluster
                                {
                                    Lattice[x, y] = k;
                                    k++;
                                    M_k[k]++;
                                }
                            }
                        }
                        else //nie jestesmy w pierwszym gornym rzedzie
                        {

                            if (y == 0) //jestesmy na lewym brzegu
                            {

                                if (Lattice[x - 1, y] > 1) //sprawdz gorne pole
                                {
                                    Lattice[x, y] = Lattice[x - 1, y];
                                    M_k[Lattice[x, y]]++;
                                }
                                else //gorne pole puste - nowy kluster
                                {
                                    Lattice[x, y] = k;
                                    k++;
                                    M_k[k]++;
                                }

                            }
                            else //nie jestesmy na lewym brzegu
                            {

                                if (Lattice[x - 1, y] > 1) //gorne pole zajete
                                {

                                    if (Lattice[x, y - 1] > 1) //lewe pole zajete
                                    {

                                        if (Lattice[x - 1, y] == Lattice[x, y - 1]) //obydwa indeksy to te same klustry
                                        {
                                            Lattice[x, y] = Lattice[x - 1, y];
                                            M_k[Lattice[x - 1, y]]++;
                                        }
                                        else //nie sa tymi samymi klustrami
                                        {

                                             if (Lattice[x - 1, y] < Lattice[x, y - 1]) //gorne pole ma nizszy indeks niz lewe
                                             {
                                                Lattice[x, y] = Lattice[x - 1, y];
                                                M_k[Lattice[x - 1, y]] += M_k[Lattice[x, y - 1]] + 1; //przerzuc ilosc obiektow na kluster o nizszym indeksie
                                                FixLattice(Lattice, Lattice[x, y - 1], Lattice[x - 1, y]); //napraw indeksy
                                                M_k[Lattice[x, y - 1]] = 0; //wyzeruj nieistniejacy kluster
                                            }
                                             else //lewe pole ma nizszy indeks
                                             {
                                                Lattice[x, y] = Lattice[x, y - 1];
                                                M_k[Lattice[x, y - 1]] += M_k[Lattice[x - 1, y]] + 1;
                                                FixLattice(Lattice, Lattice[x - 1, y], Lattice[x, y - 1]); //napraw indeksy
                                                M_k[Lattice[x - 1, y]] = 0; //wyzeruj nieistniejacy kluster
                                             }
                                        }
                                    }
                                    else //lewe pole puste
                                    {
                                        Lattice[x, y] = Lattice[x - 1, y];
                                        M_k[Lattice[x, y]]++;
                                    }
                                }
                                else //gorne pole puste
                                {

                                    if (Lattice[x, y - 1] > 1) //sprawdz lewe pole
                                    {
                                            Lattice[x, y] = Lattice[x, y - 1];
                                            M_k[Lattice[x, y]]++;
                                    }
                                    else //lewe pole puste - nowy kluster
                                    {
                                        Lattice[x, y] = k;
                                        k++;
                                        M_k[k]++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            M_k[k] = 0; //usun wartosc z klustra o  ostatnim indeksie bo on nie istnieje

            int max = M_k.Max(); //maximum value

            int[] n_M = new int[L * L + 1]; //tablica na liczenie ilosci klustrow o tym samym rozmiarze

            for (int o = 0; o < L * L + 1; o++) //przypisz wartosci poczatkowe tablicy ilosci klustrow
            {
                n_M[o] = 0;
            }

            for (int l = 0; l < L * L + 1; l++) //policz ile mamy klustrow o takim samym rozmiarze
            {

                if (M_k[l] > 0)
                {
                    n_M[M_k[l]]++; //zwieksz licznik klustrow o wielkosci M_k[l]
                }
            }

            int licznik = 0;

            for (int q = 0; q < L * L + 1; q++)
            {
                if (n_M[q] > 0)
                {
                    SaveOut2(q, n_M[q]);
                    licznik++;
                }
            }

            return max;
        }

        public void FixLattice(int[,] Lattice, int index_delete, int index_fix)
        {
            for(int x = 0; x < L; x++)
            {
                for(int y = 0; y < L; y++)
                {
                    if(Lattice[x, y] == index_delete)
                    {
                        Lattice[x, y] = index_fix;
                    }
                }
            }
        }

        public int ClusterCount(int[,] Lattice) //liczenie z szukaniem rodzica
        {
            int[] M_k = new int[L * L + 1];
            for(int i = 0; i < L * L; i++)
            {
                M_k[i] = 0;
            }

            int k = 2;
            M_k[k] = 1;
            int index1;
            int index2;

            for(int x = 0; x < L; x++)
            {

                for(int y = 0; y < L; y++)
                {

                    if (Lattice[x, y] == 1)
                    {

                        if (x == 0) //jestesmy w pierwszym gornym rzedzie
                        {

                            if (y == 0) //jestesmy w lewym gornym rogu - nowy kluster
                            {
                                Lattice[x, y] = k; //przypisz wartosc polu
                                k++; //zwieksz licznik klustra
                                M_k[k]++; //zwieksz licznik obiektow w klustrze
                            }
                            else //nie jestesmy w lewnym gornym rogu
                            {

                                if (Lattice[x, y - 1] > 1) //mamy kluster po lewej
                                {
                                    Lattice[x, y] = Lattice[x, y - 1]; //przypisz wartosc lewego klustra obecnemu polu
                                    M_k[Lattice[x, y]]++; //zwieksz licznik obiektow
                                }
                                else //nie mamy klustra po lewej - nowy kluster
                                {
                                    Lattice[x, y] = k;
                                    k++;
                                    M_k[k]++;
                                }
                            }
                        }
                        else //nie jestesmy w pierwszym gornym rzedzie
                        {

                            if (y == 0) //jestesmy na lewym brzegu
                            {

                                if (Lattice[x - 1, y] > 1) //sprawdz gorne pole
                                {
                                    if (M_k[Lattice[x - 1, y]] < 0)
                                    {
                                        index1 = -FindParent(M_k, M_k[Lattice[x - 1, y]], 0); //znajdz rodzica

                                        Lattice[x, y] = index1;
                                        M_k[index1]++;
                                    }
                                    else
                                    {
                                        Lattice[x, y] = Lattice[x - 1, y];
                                        M_k[Lattice[x, y]]++;
                                    }                                        
                                }
                                else //gorne pole puste - nowy kluster
                                {
                                    Lattice[x, y] = k;
                                    k++;
                                    M_k[k]++;
                                }

                            }
                            else //nie jestesmy na lewym brzegu
                            {

                                if (Lattice[x - 1, y] > 1) //sprawdz gorne pole
                                {

                                    if (Lattice[x, y - 1] > 1) //sprawdz lewe pole
                                    {

                                        if (M_k[Lattice[x - 1, y]] < 0) // gorne pole ma ujemna wartosc w tablicy
                                        {
                                            index1 = -FindParent(M_k, M_k[Lattice[x - 1, y]], 0); //znajdz rodzica

                                            if (M_k[Lattice[x, y - 1]] < 0) //lewe pole ma ujemna wartosc w tablicy
                                            {
                                                index2 = -FindParent(M_k, M_k[Lattice[x, y - 1]], 0);

                                                if (index1 == index2) //obydwa indeksy to te same klustry
                                                {
                                                    Lattice[x, y] = index1;
                                                    M_k[index1]++;
                                                }
                                                else //nie sa tymi samymi klustrami
                                                {
                                                    if (M_k[index1] < M_k[index2]) //gorne pole ma nizszy indeks niz lewe
                                                    {
                                                        Lattice[x, y] = index1;
                                                        M_k[index1] += M_k[index2] + 1; //przerzuc ilosc obiektow na kluster o nizszym indeksie
                                                        M_k[index2] = -index1; //przypisz ujemna wartosc obiektow na kluster o wyzszym indeksie i wskaz na rodzica
                                                    }
                                                    else //lewe pole ma nizszy indeks
                                                    {
                                                        Lattice[x, y] = index2;
                                                        M_k[index2] += M_k[index1] + 1;
                                                        M_k[index1] = -index2;
                                                    }
                                                }
                                            }
                                            else //tylko gorne pole ma ujemna wartosc w tablicy
                                            {

                                                if (index1 == Lattice[x, y - 1]) //obydwa indeksy to te same klustry
                                                {
                                                    Lattice[x, y] = index1;
                                                    M_k[index1]++;
                                                }
                                                else //nie sa tymi samymi klustrami
                                                {

                                                    if (M_k[index1] < M_k[Lattice[x, y - 1]]) //gorne pole ma nizszy indeks niz lewe
                                                    {
                                                        Lattice[x, y] = index1;
                                                        M_k[index1] += M_k[Lattice[x, y - 1]] + 1; //przerzuc ilosc obiektow na kluster o nizszym indeksie
                                                        M_k[Lattice[x, y - 1]] = -index1; //przypisz ujemna wartosc obiektow na kluster o wyzszym indeksie i wskaz na rodzica
                                                    }
                                                    else //lewe pole ma nizszy indeks
                                                    {
                                                        Lattice[x, y] = Lattice[x, y - 1];
                                                        M_k[Lattice[x, y - 1]] += M_k[index1] + 1;
                                                        M_k[index1] = -Lattice[x, y - 1];
                                                    }
                                                }
                                            }
                                        }
                                        else //gorne pole ma dodatnia wartosc w tablicy
                                        {

                                            if (M_k[Lattice[x, y - 1]] < 0) //lewe pole ma ujemna wartosc w tablicy
                                            {
                                                index2 = -FindParent(M_k, M_k[Lattice[x, y - 1]], 0);

                                                if (Lattice[x - 1, y] == index2) //obydwa indeksy to te same klustry
                                                {
                                                    Lattice[x, y] = index2;
                                                    M_k[index2]++;
                                                }
                                                else // nie sa tymi samymi klustrami
                                                {

                                                    if (M_k[Lattice[x - 1, y]] < M_k[Lattice[x, y - 1]]) //gorne pole ma nizszy indeks niz lewe
                                                    {
                                                        Lattice[x, y] = Lattice[x - 1, y];
                                                        M_k[Lattice[x - 1, y]] += M_k[index2] + 1; //przerzuc ilosc obiektow na kluster o nizszym indeksie
                                                        M_k[index2] = -Lattice[x - 1, y]; //przypisz ujemna wartosc obiektow na kluster o wyzszym indeksie i wskaz na rodzica
                                                    }
                                                    else //lewe pole ma nizszy indeks
                                                    {
                                                        Lattice[x, y] = index2;
                                                        M_k[index2] += M_k[Lattice[x - 1, y]] + 1;
                                                        M_k[Lattice[x - 1, y]] = -index2;
                                                    }
                                                }
                                            }
                                            else // obydwa pola maja dodatnia wartosc
                                            {

                                                if (Lattice[x - 1, y] == Lattice[x, y - 1]) //obydwa indeksy to te same klustry
                                                {
                                                    Lattice[x, y] = Lattice[x - 1, y];
                                                    M_k[Lattice[x - 1, y]]++;
                                                }
                                                else //nie sa tymi samymi klustrami
                                                {

                                                    if (Lattice[x - 1, y] < Lattice[x, y - 1]) //gorne pole ma nizszy indeks niz lewe
                                                    {
                                                        Lattice[x, y] = Lattice[x - 1, y];
                                                        M_k[Lattice[x - 1, y]] += M_k[Lattice[x, y - 1]] + 1; //przerzuc ilosc obiektow na kluster o nizszym indeksie
                                                        M_k[Lattice[x, y - 1]] = -Lattice[x - 1, y]; //przypisz ujemna wartosc obiektow na kluster o wyzszym indeksie i wskaz na rodzica
                                                    }
                                                    else //lewe pole ma nizszy indeks
                                                    {
                                                        Lattice[x, y] = Lattice[x, y - 1];
                                                        M_k[Lattice[x, y - 1]] += M_k[Lattice[x - 1, y]] + 1;
                                                        M_k[Lattice[x - 1, y]] = -Lattice[x, y - 1];
                                                    }
                                                }
                                            }
                                        }                                        
                                    }
                                    else //lewe pole puste
                                    {

                                        if (M_k[Lattice[x - 1, y]] < 0)
                                        {
                                            index1 = -FindParent(M_k, M_k[Lattice[x - 1, y]], 0);

                                            Lattice[x, y] = index1;
                                            M_k[index1]++;
                                        }
                                        else
                                        {
                                            Lattice[x, y] = Lattice[x - 1, y];
                                            M_k[Lattice[x, y]]++;
                                        }
                                    }
                                }
                                else //gorne pole puste
                                {

                                    if (Lattice[x, y - 1] > 1) //sprawdz lewe pole
                                    {

                                        if (M_k[Lattice[x, y - 1]] < 0)
                                        {
                                            index2 = -FindParent(M_k, M_k[Lattice[x, y - 1]], 0);

                                            Lattice[x, y] = index2;
                                            M_k[index2]++;
                                        }
                                        else
                                        {
                                            Lattice[x, y] = Lattice[x, y - 1];
                                            M_k[Lattice[x, y]]++;
                                        }
                                    }
                                    else //lewe pole puste - nowy kluster
                                    {
                                        Lattice[x, y] = k;
                                        k++;
                                        M_k[k]++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            M_k[k] = 0; //usun wartosc z klustra o  ostatnim indeksie bo on nie istnieje

            int max = M_k.Max(); //maximum value

            int[] n_M = new int[L * L + 1]; //tablica na liczenie ilosci klustrow o tym samym rozmiarze

            for(int o = 0; o < L * L + 1; o++) //przypisz wartosci poczatkowe tablicy ilosci klustrow
            {
                n_M[o] = 0;
            }

            for (int l = 0; l < L * L + 1; l++) //policz ile mamy klustrow o takim samym rozmiarze
            {
                
                if (M_k[l] > 0)
                {
                    n_M[M_k[l]]++; //zwieksz licznik klustrow o wielkosci M_k[l]
                }
            }

            int licznik = 0;

            for(int q = 0; q < L * L + 1; q++)
            {
                if (n_M[q] > 0)
                {
                    SaveOut2(q, n_M[q]);
                    licznik++;
                }
            }

            return max;
        }

        public int FindParent(int[] M_k, int index, int overflow) //szukanie rodzica
        {

            if (M_k[-index] < 0) //is new cluster a pointer to another parent?
            {
                index = FindParent(M_k, M_k[-index], overflow); //find parent of that cluster
            }

            return index; //return index of positive cluster
        }

        public void IniOut()
        {
            out1 = new int[T, 2];
            out2 = new double[L * L + 1];
        }

        public void SaveOut1(int pflow, int s_max, int count)
        {
            out1[count, 0] = pflow; //is there a path
            out1[count, 1] = s_max; //size of max cluster
        }

        public void WriteOut1(string[] zapis)
        {
            //petla na znalezienie nieuzywanej nazwy pliku
            String lokacjadanych = "";

            for (int l = 0; l < 100; l++)
            {
                String nazwadanych = @"Ave_L" + L + "T" + (T * 4) + "ver" + l + ".txt";
                if (!File.Exists(nazwadanych))
                {
                    lokacjadanych = nazwadanych;
                    break;
                }
            }

            //zapis do pliku
            using (StreamWriter file = new StreamWriter(lokacjadanych, true))
            {
                for(int i = 0;i < 101; i++)
                {
                    try
                    {
                        file.WriteLine(zapis[i]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e.Message);
                    }
                }
            }
        }

        public void SaveOut2(int size, int n_s)
        {
            out2[size] += n_s; //size of cluster
        }

        public void WriteOut2(double[,] tab, int ind)
        {
            //petla na znalezienie nieuzywanej nazwy pliku
            String lokacjadanych = "";

            for (int l = 0; l < 100; l++)
            {
                //String nazwadanych = @"Dist_p" + Math.Round(p, 2, MidpointRounding.AwayFromZero) + "L" + L + "T" + T + "ver" + l + ".txt";
                String nazwadanych = @"Dist_p" + tab[ind, 0].ToString("0.00") + "L" + L + "T" + (T * 4) + "ver" + l + ".txt";
                if (!File.Exists(nazwadanych))
                {
                    lokacjadanych = nazwadanych;
                    break;
                }
            }

             //zapis do pliku
             using (StreamWriter file = new StreamWriter(lokacjadanych, true))
             {
                 for (int i = 1; i < L * L + 1; i++) //write to file
                 {

                     if (tab[ind, i] > 0)
                     {
                         try
                         {
                             //tab[ind, i] = out2[i] / T; //average number of clusters
                             file.WriteLine("{0}  {1}", i, (tab[ind, i]).ToString("0.00"));
                         }
                         catch (Exception e)
                         {
                             Console.WriteLine("Exception: " + e.Message);
                         }
                     }
                 }
             }
        }

        public void ClearOut2()
        {
            for (int i = 0; i < L * L; i++) //clear out2
            {
                out2[i] = 0;
            }
        }

        public void MultiThread() //funkcja na multithread
        {

            int iterations = Convert.ToInt32(pk * 100.0 - p0 * 100.0 + 1) + 1; //ilość zmian prawdopodobieństwa
            zapis = new double[iterations, 5]; //tablica zapisu do pliku
            zapis_klustrow = new double[8, L * L + 1]; int iterator_klustrow = 0;

            for (int ite = 0; ite < iterations; ite++) //pętla na zmiane prawdopodobieństwa
            {

                IniOut(); //zrob nowa tablice zapisu wartości

                for (int i = 0; i < T; i++) //petla na powtarzanie obliczen dla tych samych parametrow
                {
                    int[,] Lttice = MakeLattice(); //stworz plansze
                    int[,] Ltttice = CopyLattice(Lttice); //utworz kopie

                    int s_max = ClusterCount(Lttice); //policz klustry

                    int path = BurningMethod(Ltttice); //spal drzewa

                    SaveOut1(path, s_max, i); //dodaj dane do zapisu
                }

                double pflow = 0; double avr_s = 0;

                for (int j = 0; j < T; j++) //petla na usrednienie
                {

                    if (out1[j, 0] > 0)
                    {
                        pflow += 1;
                    }
                    avr_s += out1[j, 1];
                }

                zapis[ite, 0] = p; //wartość prawdopodobieństwa
                zapis[ite, 1] = pflow / T; //średnia wartość przejścia
                zapis[ite, 2] = avr_s / T; //średnia wartość największego klustra

                //warunek na zapis dystrybucji klustrow: p = 0.2; 0.3; 0.4; 0.5; 0.592746; 0.6; 0.7; 0.8
                if ((p > 0.199 && p < 0.201) || (p > 0.299 && p < 0.301) || (p > 0.399 && p < 0.401) || (p > 0.499 && p < 0.501) || (p == 0.592746) || (p > 0.599 && p < 0.601) || (p > 0.699 && p < 0.701) || (p > 0.799 && p < 0.801))
                {
                    zapis_klustrow[iterator_klustrow, 0] = p;

                    for(int q1 = 1; q1 < L * L + 1; q1++) //petla na przejscie po wszystkich wielkosciach klustrow
                    {
                        zapis_klustrow[iterator_klustrow, q1] = out2[q1];
                    }

                    iterator_klustrow++;
                }

                ClearOut2(); //wyczysc tablice out2

                if (p > 0.589 && p < 0.591) //prawdopodobienstwo jest rowne 0.59
                {
                    p = 0.592746; //ustaw krytyczne prawdopodobienstwo
                }
                else //p nie jest rowne 0.59
                {

                    if (p == 0.592746) //jestesmy w krytycznym prawdopodobienstwie
                    {
                        p = 0.60; //przejdz do 0.6
                    }
                    else //nie jestesmy w krytycznym prawdopodobienstwie
                    {
                        p += dp; //zwieksz prawdopodobienstwo zapelnienia o krok
                    }
                }

                Console.WriteLine("Current thread propability: {0}", p.ToString("0.00"));
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Variables a1 = new Variables(); //nowa klasa na zmienne globalne
            Variables a2 = new Variables();
            Variables a3 = new Variables();
            Variables a4 = new Variables();

            a1.Parameters(); //odczytaj parametry z pliku i ustaw ich wartosci
            a2.Parameters();
            a3.Parameters();
            a4.Parameters();

            a1.T /= 4; //zmień wartość iteracji każdego prawdopodobieństwa na 1/4 bo mamy 4 wątki
            a2.T /= 4;
            a3.T /= 4;
            a4.T /= 4;

            int iterations = Convert.ToInt32(a1.pk * 100.0 - a1.p0 * 100.0 + 1) + 1; //liczba zmian prawdopodobieństwa
            string[] zapis = new string[iterations + 1];
            double pflow = 0; double avr_s = 0; // zmienne zapisu: czy jest trasa od góry do dołu i średnia wielkość max klustra
            double[,] zapis_k = new double[8, a1.L * a1.L + 1];

            Thread z1 = new Thread(a1.MultiThread); //tworzenie watkow
            Thread z2 = new Thread(a2.MultiThread);
            Thread z3 = new Thread(a3.MultiThread);
            Thread z4 = new Thread(a4.MultiThread);

            Console.WriteLine("Starting multiple threads.");

            z1.Start(); //start watkow
            z2.Start();
            z3.Start();
            z4.Start();

            do //petla na zapis
            {
                if (z1.IsAlive == false && z2.IsAlive == false && z3.IsAlive == false && z4.IsAlive == false)
                {
                    Console.WriteLine("Calculations done, saving files!");
                    //zapis do pierwszego pliku (droga od gory do dolu i srednia wielkosc najwiekszego klustra)
                    for (int i = 0; i < iterations; i++)
                    {
                        pflow = (a1.zapis[i, 1] + a2.zapis[i, 1] + a3.zapis[i, 1] + a4.zapis[i, 1]) / 4;
                        avr_s = (a1.zapis[i, 2] + a2.zapis[i, 2] + a3.zapis[i, 2] + a4.zapis[i, 2]) / 4;
                        zapis[i] = a1.zapis[i, 0].ToString("0.000") + "  " + pflow.ToString("0.00") + "  " + avr_s.ToString("0.00"); //zapis do pliku
                    }

                    //zapis do drugiego pliku (ilosc klustrow o okreslonej wielkosci)
                    for (int j = 0; j < 8; j++)
                    {
                        zapis_k[j, 0] = a1.zapis_klustrow[j, 0]; //przerzuć wartość prawdopodobienstwa

                        for(int q = 1; q< a1.L * a1.L + 1; q++) //przerzuć wartość klustrow
                        {
                            zapis_k[j, q] = a1.zapis_klustrow[j, q] + a2.zapis_klustrow[j, q] + a3.zapis_klustrow[j, q] + a4.zapis_klustrow[j, q];
                        }

                        a1.WriteOut2(zapis_k, j); //zapis do pliku
                    }
                    break;
                }

            } while (true);            

            a1.WriteOut1(zapis); //zapis do pliku        
        }        
    }
}
