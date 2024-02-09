using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class HashTable <TKey, TValue>
{
    private int capacity;
    private int size;
    private double loadFactor;
    private KeyValuePair<TKey, TValue>[] buckets;

    public HashTable(int initial = 10, double loadFactor = 0.75)
    {
        this.capacity = initial;
        this.size = 0;
        this.loadFactor = loadFactor;
        this.buckets = new KeyValuePair<TKey, TValue>[capacity];
    }
    #region HashFunctions
    private int Hash(TKey key, int i) //double hash function
    {
        return (Math.Abs(key.GetHashCode() + i * HashSecond(key)) % capacity);
    }
    private int HashSecond(TKey key) //secondary hash function 
    {
        return 1 + (key.GetHashCode() % (capacity - 1));
    }
    #endregion
    #region Resize
    private void resize()
    {
        //Resize hash table when capacity exceeds threshold
        int newCap = 2 * capacity;
        KeyValuePair<TKey, TValue>[] newBuckets = new KeyValuePair<TKey, TValue>[newCap];

        //Rehash existing pairs
        for (int i = 0; i < capacity; i++)
        {
            var pair = buckets[i];
            if (!pair.Equals(default(KeyValuePair<TKey, TValue>)))
            {
                int j = 0;
                while (j < newCap)
                {
                    int index = Hash(pair.Key, j);
                    if (newBuckets[index].Equals(default(KeyValuePair<TKey, TValue>)))
                    {
                        newBuckets[index] = pair;
                        break;
                    }
                    j++;
                }
            }
        }
        capacity = newCap;
        buckets = newBuckets;
    }
    #endregion
    #region InsertPair
    public void insert(TKey key, TValue value)
    {
        int i = 0;
        while (i < capacity)
        {
            int index = Hash(key, i);
            if (index <= buckets.Length) 
            {
                buckets[index] = new KeyValuePair<TKey, TValue>(key, value);
                size++;

                if (((double)size / capacity) > loadFactor)
                {
                    resize();
                }
                return;
            }
            i++;
        }
    }
    #endregion
    #region GetValue
    public TValue Get(TKey key)
    {
        int i = 0;
        while (i < capacity)
        {
            int index = Hash(key, i);
            var pair = buckets[index];
            if (!pair.Equals(default(KeyValuePair<TKey, TValue>)) && pair.Key.Equals(key))
            {
                return pair.Value; 
            }
            i++;
        }
        return default(TValue);
    }
    #endregion
    #region RemovePair
    public string remove(TKey key)
    {
        int i = 0;
        while (i < capacity)
        {
            int index = Hash(key, i);
            var pair = buckets[index];
            if (pair.Equals(default(KeyValuePair<TKey, TValue>)))
            {
                if (pair.Key != null && pair.Key.Equals(key))  //doesn't enter loop, pair = {null, null}
                {
                    buckets[index] = default(KeyValuePair<TKey, TValue>);
                    size++;
                    return "Key-value pair with Key of " + key + " was removed";
                }
            }
            i++;
        }
        return "Key not found";
    }
    #endregion
    #region ConvertToString
    public override string ToString()
    {
        List<string> items = new List<string>();
        for (int i = 0; i < capacity; i++)
        {
            var pair = buckets[i];
            if(!pair.Equals(default(KeyValuePair<TKey, TValue>)))
            {
                items.Add($"({pair.Key}, {pair.Value})");
            }
        }
        return string.Join("\n", items);
    }
    #endregion
}
class Program
{
    static void Main()
    {
        var hashTable = new HashTable<string, string>();

        string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sFile = System.IO.Path.Combine(sCurrentDirectory, "us-contacts.csv");
        ReadDataFromFile(hashTable, sFile);

        Console.WriteLine(hashTable.Get("17"));  //displays blank line

        Console.WriteLine(hashTable.remove("113").ToString());  //displays error message

        Console.WriteLine(hashTable.ToString());
    }
    static void ReadDataFromFile(HashTable<string, string> hashTable, string filepath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filepath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fields = line.Split(',');
                    if (fields.Length >= 2)
                    {
                        if (fields[8].Trim() == "")
                        {
                            Console.WriteLine("--");
                        }
                        string key = fields[8].Trim();
                        string value = string.Join(" ", fields[0].Trim(), fields[1].Trim());

                        hashTable.insert(key, value);
                    }
                    else
                    {
                        Console.WriteLine("Invalid data in file.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading file");
        }
    }
}
