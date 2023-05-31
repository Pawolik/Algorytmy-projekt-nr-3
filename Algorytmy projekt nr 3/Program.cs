using System;
using System.Diagnostics;
using System.Threading;

namespace Algorytmy_projekt_nr_3
{
    public class Program
    {
        //Random rnd = new Random(Guid.NewGuid().GetHashCode());

        static Random GetNewRandomInstance()
        {
            return new Random(Guid.NewGuid().GetHashCode());
        }

        // ...
        static void Tester()
        {
            int[] sizes = { 50000, 60000, 70000, 80000, 90000, 100000, 110000, 120000, 130000, 140000, 150000, 160000, 170000, 180000, 190000, 200000 };

            Console.WriteLine("Array Type\tArray Size\tInsertion Sort\tSelection Sort\tHeap Sort\tCocktail Sort");

            Random rnd = GetNewRandomInstance();

            foreach (int size in sizes)
            {
                int[] randomArray = new int[size];
                int[] ascendingArray = new int[size];
                int[] descendingArray = new int[size];
                int[] constantArray = new int[size];
                int[] vShapedArray = new int[size];

                GenerateRandomArray(randomArray, rnd);
                GenerateAscendingArray(ascendingArray);
                GenerateDescendingArray(descendingArray);
                GenerateConstantArray(constantArray, rnd);
                GenerateVShapedArray(vShapedArray);

                long insertionSortTime = MeasureExecutionTime(() => InsertionSort(randomArray));
                long selectionSortTime = MeasureExecutionTime(() => SelectionSort(ascendingArray));
                long heapSortTime = MeasureExecutionTime(() => HeapSort(descendingArray));
                long cocktailSortTime = MeasureExecutionTime(() => CocktailSort(constantArray));

                Console.WriteLine("Random\t\t{0}\t{1}\t\t{2}\t\t{3}\t\t{4}", size, insertionSortTime, selectionSortTime, heapSortTime, cocktailSortTime);
                // Repeat the above Console.WriteLine for the other array types
            }
        }
        // ...
        static void Main(string[] args)
        {
            Thread TesterThread = new Thread(Program.Tester, 8 * 1024 * 1024); // utworzenie wątku
            TesterThread.Start(); // uruchomienie wątku
            TesterThread.Join(); // oczekiwanie na zakończenie wątku
            Console.WriteLine("Tu drukujemy podsumowanie eksperymentu");

            Console.ReadLine();
        }
        // ...

        static void GenerateAscendingArray(int[] t)
        {
            for (int i = 0; i < t.Length; ++i)
                t[i] = i;
        }

        static void GenerateDescendingArray(int[] t)
        {
            for (int i = 0; i < t.Length; ++i)
                t[i] = t.Length - i - 1;
        }

        static void GenerateRandomArray(int[] t, Random rnd, int maxValue = int.MaxValue)
        {
            for (int i = 0; i < t.Length; ++i)
                t[i] = rnd.Next(maxValue);
        }


        static void GenerateConstantArray(int[] t, Random rnd, int maxValue = int.MaxValue)
        {
            int value = rnd.Next(maxValue);
            for (int i = 0; i < t.Length; ++i)
                t[i] = value;
        }

        static void GenerateVShapedArray(int[] t)
        {
            int mid = t.Length / 2;
            for (int i = 0; i < mid; ++i)
                t[i] = i;
            for (int i = mid; i < t.Length; ++i)
                t[i] = t.Length - i - 1;
        }

        static void InsertionSort(int[] t)
        {
            for (uint i = 1; i < t.Length; i++)
            {
                uint j = i; // elementy 0 .. i-1 są już posortowane
                int Buf = t[j]; // bierzemy i-ty (j-ty) element
                while ((j > 0) && (t[j - 1] > Buf))
                { // przesuwamy elementy
                    t[j] = t[j - 1];
                    j--;
                }
                t[j] = Buf; // i wpisujemy na docelowe miejsce
            }
        } /* InsertionSort() */

        static void SelectionSort(int[] t)
        {
            uint k;
            for (uint i = 0; i < (t.Length - 1); i++)
            {
                int Buf = t[i]; // bierzemy i-ty element
                k = i; // i jego indeks
                for (uint j = i + 1; j < t.Length; j++)
                    if (t[j] < Buf) // szukamy najmniejszego z prawej
                    {
                        k = j;
                        Buf = t[j];
                    }
                t[k] = t[i]; // zamieniamy i-ty z k-tym
                t[i] = Buf;
            }
        } /* SelectionSort() */

        static void CocktailSort(int[] t)
        {
            int Left = 1, Right = t.Length - 1, k = t.Length - 1;
            do
            {
                for (int j = Right; j >= Left; j--) // przesiewanie od dołu
                    if (t[j - 1] > t[j])
                    {
                        int Buf = t[j - 1]; t[j - 1] = t[j]; t[j] = Buf;
                        k = j; // zamiana elementów i zapamiętanie indeksu
                    }
                Left = k + 1; // zacieśnienie lewej granicy
                for (int j = Left; j <= Right; j++) // przesiewanie od góry
                    if (t[j - 1] > t[j])
                    {
                        int Buf = t[j - 1]; t[j - 1] = t[j]; t[j] = Buf;
                        k = j; // zamiana elementów i zapamiętanie indeksu
                    }
                Right = k - 1; // zacieśnienie prawej granicy
            }
            while (Left <= Right);
        } /* CocktailSort() */

        static void Heapify(int[] t, uint left, uint right)
        { // procedura budowania/naprawiania kopca
            uint i = left,
            j = 2 * i + 1;
            int buf = t[i]; // ojciec
            while (j <= right) // przesiewamy do dna stogu
            {
                if (j < right) // wybieramy większego syna
                    if (t[j] < t[j + 1]) j++;
                if (buf >= t[j]) break;
                t[i] = t[j];
                i = j;
                j = 2 * i + 1; // przechodzimy do dzieci syna
            }
            t[i] = buf;
        } /* Heapify() */

        static void HeapSort(int[] t)
        {
            uint left = ((uint)t.Length / 2),
            right = (uint)t.Length - 1;
            while (left > 0) // budujemy kopiec idąc od połowy tablicy
            {
                left--;
                Heapify(t, left, right);
            }
            while (right > 0) // rozbieramy kopiec
            {
                int buf = t[left];
                t[left] = t[right];
                t[right] = buf; // największy element
                right--; // kopiec jest mniejszy
                Heapify(t, left, right); // ale trzeba go naprawić
            }
        } /* HeapSort() */

        static long MeasureExecutionTime(Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
