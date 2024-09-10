using System;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;

Console.Write("Введите путь к исходному файлу");
string? path_initial_file = Console.ReadLine();
Console.Write("Введите путь к файлу, измененному первым программистом");
string? path_change1 = Console.ReadLine();
Console.Write("Введите путь к файлу, измененному вторым программистом");
string? path_change2 = Console.ReadLine();
Console.Write("Введите путь к файлу, куда будет записан итоговый файл");
string? path_result = Console.ReadLine();


//string path_initial_file = "C:\\Users\\artem\\Desktop\\ALL PROG\\NEVLABS\\ORIGINAL.txt";
//string path_change1 = "C:\\Users\\artem\\Desktop\\ALL PROG\\NEVLABS\\PROG1.txt";
//string path_change2 = "C:\\Users\\artem\\Desktop\\ALL PROG\\NEVLABS\\PROG2.txt";
//string path_result = "C:\\Users\\artem\\Desktop\\ALL PROG\\NEVLABS\\RESULT.txt";

List<string> merge_file = new List<string>();

try
{
    string[] initial_file = File.ReadAllLines(path_initial_file);
    string[] change1 = File.ReadAllLines(path_change1);
    string[] change2 = File.ReadAllLines(path_change2);

    int i = 0, j = 0, k = 0;

    while (i <= initial_file.Length - 1 || j <= change1.Length - 1 || k <= change2.Length - 1)
    {

        if (i == initial_file.Length & (j < change1.Length || k < change2.Length)) // если достигнут конец исходного файла, а в каком-то из файлов программистов конец не достигнут, то добавляем новые блоки кода в конечный файл и выходим из цикла
        {
            if (i == initial_file.Length & j < change1.Length)
            {
                for (int p = j; p < change1.Length; p++)
                {
                    if (p == j & p == change1.Length - 1) { merge_file.Add(change1[p] + "            ///// СТРОКА, ДОБАВЛЕННАЯ ПРОГРАММИСТОМ"); }
                    else if (p == j) { merge_file.Add(change1[p] + "            ///// НАЧАЛО ДОБАВЛЕННОГО БЛОКА КОДА "); }
                    else if (p == change1.Length - 1) { merge_file.Add(change1[p] + "            ///// КОНЕЦ ДОБАВЛЕННОГО БЛОКА КОДА"); }
                    else { merge_file.Add(change1[p]); }
                }
            }
            if (i == initial_file.Length & k < change2.Length)
            {
                for (int p = k; p < change2.Length; p++)
                {
                    if (p == k & p == change2.Length - 1) { merge_file.Add(change2[p] + "            ///// СТРОКА, ДОБАВЛЕННАЯ ПРОГРАММИСТОМ"); }
                    else if (p == k) { merge_file.Add(change2[p] + "            ///// НАЧАЛО ДОБАВЛЕННОГО БЛОКА КОДА "); }
                    else if (p == change2.Length - 1) { merge_file.Add(change2[p] + "            ///// КОНЕЦ ДОБАВЛЕННОГО БЛОКА КОДА"); }
                    else { merge_file.Add(change2[p]); }
                }
            }
            j = change1.Length;
            k = change2.Length;
        }
        else // если конец исходного файла не достигнут, то продолжаем сравнение
        {
            if (i > initial_file.Length - 1) { i = initial_file.Length - 1; }
            if (j > change1.Length - 1) { j = change1.Length - 1; }
            if (k > change2.Length - 1) { k = change2.Length - 1; }

            if (initial_file[i].Trim() == change1[j].Trim() & initial_file[i].Trim() == change2[k].Trim()) // строка во всех файлах одинаковая
            {
                merge_file.Add(initial_file[i]);
                j++;
                k++;
            }
            else if (initial_file[i].Trim() == change1[j].Trim() & initial_file[i].Trim() != change2[k].Trim()) // строка в 1 файле соотвествует начальному файлу, строка во 2 файле НЕ соответствует
            {
                Func_Search_Similar_One(initial_file, change2, i, k, out k);
                j++;
                k++;
            }
            else if (initial_file[i].Trim() != change1[j].Trim() & initial_file[i].Trim() == change2[k].Trim()) // строка во 2 файле соответствует начальному файлу, строка в 1 файле НЕ соотвествует
            {
                Func_Search_Similar_One(initial_file, change1, i, j, out j);
                j++;
                k++;
            }
            else if (initial_file[i].Trim() != change1[j].Trim() & initial_file[i].Trim() != change2[k].Trim())
            {
                Func_Search_Similar_Two(initial_file, change1, change2, i, j, k, out j, out k);
            }
            i++;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}



try // Запись массива строк в файл
{
    File.WriteAllLines(path_result, merge_file);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}



void Func_Search_Similar_One(string[] initial_file, string[] changed, int original, int str_changed, out int index)
{
    List<double> temporary_mas = new List<double>();

    for (int p = str_changed; p < changed.Length; p++)
    {
        double similarity = CalculateSimilarity(initial_file[original].Trim(), changed[p].Trim());
        temporary_mas.Add(similarity);
    }

    double maxNumber = temporary_mas.Max();
    index = temporary_mas.IndexOf(maxNumber) + str_changed;

    if (maxNumber > 70) // если есть достаточно схожей строка, то считаем ее удаленной и не добавляем в результирующий файл
    {
        if (index != str_changed) // 
        {
            for (int p = str_changed; p <= index; p++)
            {
                if (p == str_changed & p == index - 1) { merge_file.Add(changed[p] + "            ///// СТРОКА, ДОБАВЛЕННАЯ ПРОГРАММИСТОМ"); }
                else if (p == str_changed) { merge_file.Add(changed[p] + "            ///// НАЧАЛО ДОБАВЛЕННОГО БЛОКА КОДА "); }
                else if (p == index) { merge_file.Add(changed[p] + "            ///// КОНЕЦ ДОБАВЛЕННОГО БЛОКА КОДА"); }
                else { merge_file.Add(changed[p]); }
            }
        }
        else { merge_file.Add(changed[index]); }
    }
    else // если достаточно схожей строки нету, то считаем ее удаленной и не добавляем в результирующий файл
    { 
        merge_file.Add($"            >>> СТРОКА НОМЕР {original+1} (ИСХОДНОЕ СОДЕРЖИМОЕ: {initial_file[original].Trim()}) УДАЛЕНА У ОДНОГО ИЗ ПРОГРАММИСТОВ >>>");
        index = str_changed - 1;
    }
}

void Func_Search_Similar_Two(string[] initial_file, string[] changed1, string[] changed2, int original, int str_changed1, int str_changed2, out int index1, out int index2)
{
    List<double> temporary_mas1 = new List<double>();
    List<double> temporary_mas2 = new List<double>();

    for (int p = str_changed1; p < changed1.Length; p++)
    {
        double similarity = CalculateSimilarity(initial_file[original].Trim(), changed1[p].Trim());
        temporary_mas1.Add(similarity);
    }

    for (int p = str_changed2; p < changed2.Length; p++)
    {
        double similarity = CalculateSimilarity(initial_file[original].Trim(), changed2[p].Trim());
        temporary_mas2.Add(similarity);
    }

    double maxNumber1 = temporary_mas1.Max();
    index1 = temporary_mas1.IndexOf(maxNumber1) + str_changed1;

    double maxNumber2 = temporary_mas2.Max();
    index2 = temporary_mas2.IndexOf(maxNumber2) + str_changed2;

    if (maxNumber1 == 1 & maxNumber2 == 1) // считаем что строка не изменена, оба программиста добавили блоки кода перед ней
    {
        for (int p = str_changed1; p < index1; p++)
        {
            if (p == str_changed1 & p == index1 - 1) { merge_file.Add(changed1[p] + "            ///// СТРОКА, ДОБАВЛЕННАЯ 1 ПРОГРАММИСТОМ"); }
            else if (p == str_changed1) { merge_file.Add(changed1[p] + "            ///// НАЧАЛО БЛОКА КОДА ДОБАВЛЕННОГО 1 ПРОГРАММИСТОМ"); }
            else if (p == index1 - 1) { merge_file.Add(changed1[p] + "            ///// КОНЕЦ БЛОКА КОДА ДОБАВЛЕННОГО 1 ПРОГРАММИСТОМ"); }
            else { merge_file.Add(changed1[p]); }
        }
        for (int p = str_changed2; p < index2; p++)
        {
            if (p == str_changed1 & p == index1 - 1) { merge_file.Add(changed1[p] + "            ///// СТРОКА, ДОБАВЛЕННАЯ 2 ПРОГРАММИСТОМ"); }
            else if (p == str_changed2) { merge_file.Add(changed2[p] + "            ///// НАЧАЛО БЛОКА КОДА ДОБАВЛЕННОГО 2 ПРОГРАММИСТОМ"); }
            else if (p == index2 - 1) { merge_file.Add(changed2[p] + "            ///// КОНЕЦ БЛОКА КОДА ДОБАВЛЕННОГО 2 ПРОГРАММИСТОМ"); }
            else { merge_file.Add(changed2[p]); }
        }
        merge_file.Add(changed1[index1]);
    } 
    else if ((maxNumber1 != 1 || maxNumber2 != 1) & (maxNumber1 > 70 & maxNumber2 < 70 || maxNumber1 < 70 & maxNumber2 > 70)) // считаем что перед строкой обоими программистами были добавлены блоки кода, и текущая строка была удалена одним из программистов
    {
        merge_file.Add($"            >>> СТРОКА НОМЕР {original+1} (ИСХОДНОЕ СОДЕРЖИМОЕ: {initial_file[original].Trim()}) УДАЛЕНА У ОДНОГО ИЗ ПРОГРАММИСТОВ >>>");
        index1 = str_changed1 + 1;
        index2 = str_changed2 + 1;
    }
    else if (maxNumber1 < 70 & maxNumber2 < 70) // КОНФЛИКТ: СТРОКА ИЗМЕНЕНА ПО РАЗНОМУ
    {
        merge_file.Add($"            >>> СТРОКА НОМЕР {original+1} (ИСХОДНОЕ СОДЕРЖИМОЕ: {initial_file[original].Trim()}) ИЗМЕНЕНА У ДВУХ ПРОГРАММИСТОВ ПО РАЗНОМУ >>>");
        index1 = str_changed1 + 1;
        index2 = str_changed2 + 1;
    }
}


double CalculateSimilarity(string s1, string s2)
{
    int maxLen = Math.Max(s1.Length, s2.Length);
    int distance = LevenshteinDistance(s1, s2);
    return (1.0 - (double)distance / maxLen) * 100;
}

int LevenshteinDistance(string s, string t) // метод Ливенштейна - расчет расстояния между строками
{
    int n = s.Length;
    int m = t.Length;
    int[,] d = new int[n + 1, m + 1];

    for (int i = 0; i <= n; i++) d[i, 0] = i;
    for (int j = 0; j <= m; j++) d[0, j] = j;

    for (int i = 1; i <= n; i++)
    {
        for (int j = 1; j <= m; j++)
        {
            int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
            d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
        }
    }

    return d[n, m];
}