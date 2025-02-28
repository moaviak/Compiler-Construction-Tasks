using System;
using System.Collections.Generic;

namespace SymbolTable
{
    /// <summary>
    /// A symbol table implementation using a hash table.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the symbol table.</typeparam>
    /// <typeparam name="TValue">The type of values in the symbol table.</typeparam>
    public class HashSymbolTable<TKey, TValue>
    {
        private class Entry
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public Entry Next { get; set; }

            public Entry(TKey key, TValue value, Entry next = null)
            {
                Key = key;
                Value = value;
                Next = next;
            }
        }

        private Entry[] table;
        private int size;
        private int capacity;
        private const double LoadFactorThreshold = 0.75;

        /// <summary>
        /// Initializes a new instance of the HashSymbolTable class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the hash table.</param>
        public HashSymbolTable(int initialCapacity = 16)
        {
            capacity = initialCapacity;
            table = new Entry[capacity];
            size = 0;
        }

        /// <summary>
        /// Gets the number of key-value pairs in the symbol table.
        /// </summary>
        public int Count => size;

        /// <summary>
        /// Checks if the symbol table is empty.
        /// </summary>
        /// <returns>True if the symbol table is empty, otherwise false.</returns>
        public bool IsEmpty() => size == 0;

        /// <summary>
        /// Computes the hash code for the specified key.
        /// </summary>
        /// <param name="key">The key to hash.</param>
        /// <returns>The hash code.</returns>
        private int Hash(TKey key)
        {
            return Math.Abs(key.GetHashCode()) % capacity;
        }

        /// <summary>
        /// Puts the key-value pair into the symbol table.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
        public void Put(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key cannot be null");

            // Check if resizing is needed
            if ((double)size / capacity >= LoadFactorThreshold)
                Resize(capacity * 2);

            int hash = Hash(key);
            
            // Check if key already exists
            for (Entry e = table[hash]; e != null; e = e.Next)
            {
                if (key.Equals(e.Key))
                {
                    e.Value = value;
                    return;
                }
            }

            // Add new entry
            table[hash] = new Entry(key, value, table[hash]);
            size++;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated with the key.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
        public TValue Get(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key cannot be null");

            int hash = Hash(key);
            
            for (Entry e = table[hash]; e != null; e = e.Next)
            {
                if (key.Equals(e.Key))
                {
                    return e.Value;
                }
            }

            throw new KeyNotFoundException($"Key '{key}' not found in symbol table");
        }

        /// <summary>
        /// Tries to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value associated with the key, if found.</param>
        /// <returns>True if the key was found, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
        public bool TryGet(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key cannot be null");

            int hash = Hash(key);
            
            for (Entry e = table[hash]; e != null; e = e.Next)
            {
                if (key.Equals(e.Key))
                {
                    value = e.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Removes the specified key and its associated value from the symbol table.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the key was found and removed, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key cannot be null");

            int hash = Hash(key);
            
            // If the bucket is empty
            if (table[hash] == null)
                return false;

            // If key is at the head of the chain
            if (key.Equals(table[hash].Key))
            {
                table[hash] = table[hash].Next;
                size--;
                return true;
            }

            // Check the rest of the chain
            for (Entry e = table[hash]; e.Next != null; e = e.Next)
            {
                if (key.Equals(e.Next.Key))
                {
                    e.Next = e.Next.Next;
                    size--;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the symbol table contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the symbol table contains the key, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
        public bool Contains(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key cannot be null");

            int hash = Hash(key);
            
            for (Entry e = table[hash]; e != null; e = e.Next)
            {
                if (key.Equals(e.Key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Resizes the hash table.
        /// </summary>
        /// <param name="newCapacity">The new capacity.</param>
        private void Resize(int newCapacity)
        {
            HashSymbolTable<TKey, TValue> temp = new HashSymbolTable<TKey, TValue>(newCapacity);
            
            for (int i = 0; i < capacity; i++)
            {
                for (Entry e = table[i]; e != null; e = e.Next)
                {
                    temp.Put(e.Key, e.Value);
                }
            }
            
            this.table = temp.table;
            this.capacity = temp.capacity;
        }

        /// <summary>
        /// Gets all keys in the symbol table.
        /// </summary>
        /// <returns>All keys in the symbol table.</returns>
        public IEnumerable<TKey> Keys()
        {
            List<TKey> keys = new List<TKey>();
            
            for (int i = 0; i < capacity; i++)
            {
                for (Entry e = table[i]; e != null; e = e.Next)
                {
                    keys.Add(e.Key);
                }
            }
            
            return keys;
        }

        /// <summary>
        /// Clears the symbol table.
        /// </summary>
        public void Clear()
        {
            Array.Fill(table, null);
            size = 0;
        }
    }

    // Demo program to show usage
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new hash symbol table
            var symbolTable = new HashSymbolTable<string, int>();
            
            // Add some entries
            symbolTable.Put("one", 1);
            symbolTable.Put("two", 2);
            symbolTable.Put("three", 3);
            
            // Retrieve values
            Console.WriteLine($"Value for 'one': {symbolTable.Get("one")}");
            Console.WriteLine($"Value for 'two': {symbolTable.Get("two")}");
            
            // Check if a key exists
            Console.WriteLine($"Contains 'four': {symbolTable.Contains("four")}");
            
            // Try to get a value
            if (symbolTable.TryGet("three", out int value))
            {
                Console.WriteLine($"Value for 'three': {value}");
            }
            
            // Remove a key
            symbolTable.Remove("two");
            Console.WriteLine($"Contains 'two' after removal: {symbolTable.Contains("two")}");
            
            // Display all keys
            Console.WriteLine("All keys:");
            foreach (var key in symbolTable.Keys())
            {
                Console.WriteLine(key);
            }
            
            Console.WriteLine($"Count: {symbolTable.Count}");
        }
    }
}