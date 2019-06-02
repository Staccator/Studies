using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace ASD
{
    /// <summary>
    /// Klasa drzewa prefiksowego z możliwością wyszukiwania słów w zadanej odległości edycyjnej
    /// </summary>
    public class Lab14_Trie : System.MarshalByRefObject
    {

        // klasy TrieNode NIE WOLNO ZMIENIAĆ!
        private class TrieNode
        {
            public SortedDictionary<char, TrieNode> childs = new SortedDictionary<char, TrieNode>();
            public bool IsWord = false;
            public int WordCount = 0;
        }

        private TrieNode root;

        public Lab14_Trie()
        {
            root = new TrieNode();
        }

        /// <summary>
        /// Zwraca liczbę przechowywanych słów
        /// Ma działać w czasie stałym - O(1)
        /// </summary>
        public int Count { get {

                return root.WordCount;

            } }

        /// <summary>
        /// Zwraca liczbę przechowywanych słów o zadanym prefiksie
        /// Ma działać w czasie O(len(startWith))
        /// </summary>
        /// <param name="startWith">Prefiks słów do zliczenia</param>
        /// <returns>Liczba słów o zadanym prefiksie</returns>
        public int CountPrefix(string startWith)
        {
            //if (!Contains(startWith)) return 0;
            char c = ' ';
            var tempNode = root;
            for (int i = 0; i < startWith.Length; i++)
            {
                c = startWith[i];
                if (tempNode.childs.ContainsKey(c))
                {
                    tempNode = tempNode.childs[c];
                }
                else return 0;
            }
            return tempNode.WordCount;
        }

        /// <summary>
        /// Dodaje słowo do słownika
        /// Ma działać w czasie O(len(newWord))
        /// </summary>
        /// <param name="newWord">Słowo do dodania</param>
        /// <returns>True jeśli słowo udało się dodać, false jeśli słowo już istniało</returns>
        public bool AddWord(string newWord)
        {
            if (Contains(newWord)) return false;
            char c = ' ';
            var tempNode = root;
            for (int i = 0; i < newWord.Length; i++)
            {
                c = newWord[i];
                if (tempNode.childs.ContainsKey(c))
                {
                    tempNode = tempNode.childs[c];
                    if (i == newWord.Length - 1)
                    {
                        if (tempNode.IsWord)
                        {
                            return false;
                        }
                        else
                        {
                            tempNode.IsWord = true;
                            //return true;
                        }
                    }

                }
                else
                {
                    TrieNode newNode = new TrieNode();
                    newNode.childs = new SortedDictionary<char, TrieNode>();
                    //newNode.IsWord = 
                    tempNode.childs.Add(c, newNode);
                    tempNode = tempNode.childs[c];
                    if (i == newWord.Length - 1)
                    {
                        tempNode.IsWord = true;
                        //return true;
                    }
                }
            }
            tempNode = root;
            tempNode.WordCount++;
            for (int i = 0; i < newWord.Length; i++)
            {
                c = newWord[i];
                if (tempNode.childs.ContainsKey(c))
                {
                    tempNode = tempNode.childs[c];
                    tempNode.WordCount++;
                }

            }
            return true;
        }

        /// <summary>
        /// Sprawdza czy podane słowo jest przechowywane w słowniku
        /// Ma działać w czasie O(len(word))
        /// </summary>
        /// <param name="word">Słowo do sprawdzenia</param>
        /// <returns>True jeśli słowo znajduje się w słowniku, wpp. false</returns>
        public bool Contains(string word)
        {
            char c = ' ';
            var tempNode = root;
            for (int i = 0; i < word.Length; i++)
            {
                c = word[i];
                if (tempNode.childs.ContainsKey(c))
                {
                    tempNode = tempNode.childs[c];
                }
                else
                {
                    return false;
                }
            }

            if (tempNode.IsWord) return true;
            else return false;
        }

        /// <summary>
        /// Usuwa podane słowo ze słownika
        /// Ma działać w czasie O(len(word))
        /// </summary>
        /// <param name="word">Słowo do usunięcia</param>
        /// <returns>True jeśli udało się słowo usunąć, false jeśli słowa nie było w słowniku</returns>
        public bool Remove(string word)
        {
            if (!Contains(word)) return false;

            char c = ' ';
            var tempNode = root;
            List<(TrieNode, char)> lista = new List<(TrieNode, char)>();
            lista.Add((tempNode, ' '));
            for (int i = 0; i < word.Length; i++)
            {
                c = word[i];
                tempNode = tempNode.childs[c];
                lista.Add((tempNode, c));
            }
            for (int i = 0; i < lista.Count; i++)
            {
                lista[i].Item1.WordCount--;
            }

            if (lista.Last().Item1.WordCount > 0)
                lista.Last().Item1.IsWord = false;
            else
            {
                lista[lista.Count - 2].Item1.childs.Remove(lista[lista.Count - 1].Item2);
                for (int i = lista.Count - 2; i > 0; i--)
                {
                    var item = lista[i];
                    if (item.Item1.WordCount > 0) break;
                    else
                    {
                        lista[i - 1].Item1.childs.Remove(item.Item2);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Zwraca wszystkie słowa o podanym prefiksie. 
        /// Dla pustego prefiksu zwraca wszystkie słowa ze słownika.
        /// Wynik jest w porządku alfabetycznym.
        /// Ma działać w czasie O(liczba węzłów w drzewie)
        /// </summary>
        /// <param name="startWith">Prefiks</param>
        /// <returns>Wyliczenie zawierające wszystkie słowa ze słownika o podanym prefiksie</returns>
        public List<string> AllWords(string startWith = "")
        {
            if (CountPrefix(startWith) == 0) return new List<string>();
            char c = ' ';
            var tempNode = root;
            for (int i = 0; i < startWith.Length; i++)
            {
                c = startWith[i];
                if (tempNode.childs.ContainsKey(c))
                {
                    tempNode = tempNode.childs[c];
                }
            }

            return allWords(tempNode, startWith);
        }
        private List<string> allWords(TrieNode node, string prefix)
        {
            List<string> words = new List<string>();
            if (node.IsWord) words.Add(prefix);
            foreach (var item in node.childs)
            {
                words.AddRange(allWords(item.Value, prefix + item.Key));
            }
            return words;
        }

        /// <summary>
        /// Wyszukuje w słowniku wszystkie słowa w podanej odległości edycyjnej od zadanego słowa
        /// Wynik jest w porządku alfabetycznym ze względu na słowa (a nie na odległość).
        /// Ma działać optymalnie - tj. niedozwolone jest wyszukanie wszystkich słów i sprawdzenie ich odległości
        /// Należy przeszukując drzewo odpowiednio odrzucać niektóre z gałęzi.
        /// Złożoność pesymistyczna (gdy wszystkie słowa w słowniku mieszczą się w zadanej odległości)
        /// O(len(word) * (liczba węzłów w drzewie))
        /// </summary>
        /// <param name="word">Słowo</param>
        /// <param name="distance">Odległość edycyjna</param>
        /// <returns>Lista zawierająca pary (słowo, odległość) spełniające warunek odległości edycyjnej</returns>
        public List<(string, int)> Search(string word, int distance = 1)
        {
            List<(string, int)> Results = new List<(string, int)>();
            int level = 0;
            int[] tab = Enumerable.Range(0, word.Length + 1).ToArray();
            SearchWords(root,'?', level, tab,"", ref Results,word,distance);
            return Results;
        }

        private void SearchWords(TrieNode node,char c, int level, int[] tab,string actuel_word, ref List<(string, int)> results,string word, int distance)
        {
            if (actuel_word.Length + 1 > word.Length + distance) return;
            string new_word;
            if (level == 0)
            {
                new_word = actuel_word;
                int Levenstein = tab.Last();
                if (Levenstein <= distance)
                {
                    results.Add(("", Levenstein));
                }
            }
            else
            {
                new_word = actuel_word + c;
                tab = CountNewColumn(word, c, level, tab);
                int Levenstein = tab.Last();

                if (Levenstein <= distance && node.IsWord)
                {
                    results.Add((new_word, Levenstein));
                }
            }
            
            foreach(var child in node.childs)
            {
                
                SearchWords(child.Value,child.Key, level + 1, tab.ToArray(), level==0?"":actuel_word + c, ref results,word,distance);
            }
        }

        public int[] CountNewColumn(string word, char c, int c_index, int[] tab)
        {
            int[] result = new int[word.Length + 1];
            result[0] = c_index;
            for (int i = 1; i < word.Length+1; i++)
            {
                int minimum = Math.Min(Math.Min(result[i - 1]+1, tab[i]+1 ) , tab[i-1] + costOfSubstitution(word[i - 1], c ));
                //if (!(i-1 == c_index-1 && word[i-1] == c)) minimum++;
                result[i] = minimum;
            }
            return result;
        }

        public static int costOfSubstitution(char a, char b)
        {
            return a == b ? 0 : 1;
        }
    }
}