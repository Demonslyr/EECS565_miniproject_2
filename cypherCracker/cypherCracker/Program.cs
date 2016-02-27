using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cypherCracker
{
    class Program
    {
        static void Main(string[] args)
        {
            string manCypher = "", manWordLen = "", manKeyLen = "";
            char tmpInput = 'o';
            Console.WriteLine("This program expects a key that is shorter than the length of the first word of the plaintext sentence.\n");
            do
            {
                Console.WriteLine("type 'n' to run the miniproject inputs or type 'y' to provide a custom cyphertext, keylength, and firstWordLength?\n");
                tmpInput = Convert.ToChar(Console.Read());
            }
            while (tmpInput != 'Y' && tmpInput != 'y' && tmpInput != 'N' && tmpInput != 'n');
                if (tmpInput == 'Y' || tmpInput == 'y')
                {
                    Console.WriteLine("Enter teh Cyphertext:\n");
                    manCypher = Console.ReadLine();
                    Console.WriteLine("Enter the length of the first word:\n");
                    manWordLen = Console.ReadLine();
                    Console.WriteLine("Enter the length of the key:\n");
                    manKeyLen = Console.ReadLine();
                }
            string dictionarySrc = @"c:/dict.txt";
            List<string> words = new List<string> { "", "", "MSOKKJ", "OOPCULN", "MTZHZEOQKA", "HUETNMIXVTM", "LDWMEKPOP", "VVVLZWWPBWHZD" };
            List<string> cyphers = new List<string>();
            cyphers.Add("MSOKKJCOSXOEEKDTOSLGFWCMCHSUSGX");
            cyphers.Add("OOPCULNWFRCFQAQJGPNARMEYUODYOUNRGWORQEPVARCEPBBSCEQYEARAJUYGWWYACYWBPRNEJBMDTEAEYCCFJNENSGWAQRTSJTGXNRQRMDGFEEPHSJRGFCFMACCB");
            cyphers.Add("MTZHZEOQKASVBDOWMWMKMNYIIHVWPEXJA");
            cyphers.Add("HUETNMIXVTMQWZTQMMZUNZXNSSBLNSJVSJQDLKR");
            cyphers.Add("LDWMEKPOPSWNOAVBIDHIPCEWAETYRVOAUPSINOVDIEDHCDSELHCCPVHRPOHZUSERSFS");
            cyphers.Add("VVVLZWWPBWHZDKBTXLDCGOTGTGRWAQWZSDHEMXLBELUMO");
            var overWatch = new Stopwatch();
            var dictWatch = new Stopwatch();
            var keyWatch = new Stopwatch();
            List<Task> tasks = new List<Task>();
            List<string> keys = new List<string>();
            List<string> results = new List<string>();
            List<List<string>> dictContainer = new List<List<string>> { new List<string>(), new List<string>() };
            overWatch.Start();
            dictWatch.Start();
            foreach (string entry in File.ReadAllLines(dictionarySrc))
            {
                if (dictContainer.Count <= entry.Length)
                    dictContainer.Add(new List<string> { entry });
                else
                    dictContainer[entry.Length].Add(entry);
            }
            dictWatch.Stop();
            overWatch.Stop();
            Console.WriteLine("Key times");
            overWatch.Start();
            if (tmpInput == 'Y' || tmpInput == 'y')
            {
                keyWatch.Start();
                genKeys(keys, dictContainer, manCypher.ToUpper().Substring(0,Int32.Parse(manWordLen)), Int32.Parse(manKeyLen));
                keyWatch.Stop();
                overWatch.Stop();
                Console.WriteLine(keyWatch.ElapsedMilliseconds);
                keyWatch.Reset();
                overWatch.Start();
                overWatch.Stop();
                Console.WriteLine("Decode Times");
                overWatch.Start();
                for (int i = 0; i < keys.Count; i++)
                {
                    results.Add("");
                    int x = i;
                    tasks.Add(Task.Factory.StartNew(() => results[x] = deCode(keys[x], cyphers[x])));
                }
                Task.WaitAll(tasks.ToArray());
            }
            else
            {
                for (int i = 2; i < cyphers.Count + 2; i++)
                {
                    keyWatch.Start();
                    genKeys(keys, dictContainer, words[i].ToUpper(), i);
                    keyWatch.Stop();
                    overWatch.Stop();
                    Console.WriteLine(keyWatch.ElapsedMilliseconds);
                    keyWatch.Reset();
                    overWatch.Start();

                }
            overWatch.Stop();
            Console.WriteLine("Decode Times");
            overWatch.Start();
            for (int i = 0; i < keys.Count; i++)
            {
                results.Add("");
                int x = i;
                tasks.Add(Task.Factory.StartNew(() => results[x] = deCode(keys[x], cyphers[x])));
            }
            Task.WaitAll(tasks.ToArray());
            }
            overWatch.Stop();
            Console.WriteLine("Dictionary");
            Console.WriteLine(dictWatch.ElapsedMilliseconds);
            Console.WriteLine("Total w/o dicitonary");
            Console.WriteLine(overWatch.ElapsedMilliseconds - dictWatch.ElapsedMilliseconds);
            Console.WriteLine("Total");
            Console.WriteLine(overWatch.ElapsedMilliseconds);
            for (int i = 0; i < keys.Count; i++)
            {
                Console.WriteLine("Key: "+keys[i]+",\nPlaintext: "+results[i]+"\n");
            }
            //1. KS
            //2. JAY
            //3. IWKD
            //4. ZIENF
            //5. HACKER
            //6. NICHOLS
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }

        static string deCode(string key, string cypherText)
        {
            string tmp = "";
            int keyLen = key.Length;
            int msgLen = cypherText.Length;
            var decWatch = Stopwatch.StartNew();
            for (int i = 0; i < msgLen; i++)
            {
                tmp = tmp + ((char)((((int)cypherText[i]) - 65 + (26-(((int)key[i%keyLen])-65))) % 26 + 65));
            }
            decWatch.Stop();
            Console.WriteLine(decWatch.ElapsedMilliseconds);
            return tmp;

        }

        /// <summary>
        /// uses the equation (C+26-P)%26=k
        /// </summary>
        /// <param name="keys">list of generated possible keys</param>
        /// <param name="dict">dictionary containing the word lists</param>
        /// <param name="firstWord">first word in cyphertext</param>
        /// <param name="keylen">length of the key</param>
        /// <returns></returns>
        static bool genKeys(List<string> keys, List<List<string>> dict, string firstWord, int keyLen) {

            foreach (var word in dict[firstWord.Length])
            {
                string tmp = "",test = "";//the key
                for (int i = 0; i<keyLen;i++)
                {
                    tmp = tmp + (char)((((int)firstWord[i]-65 + 26) - ((int)word[i]-65)) % 26+65);
                }                
                if (keyLen < firstWord.Length)
                {
                    test = "";
                    for (int i = 0; i < word.Length; i++)
                    {
                        test = test +( (char)(((int)(tmp[i%keyLen]) - 130 + (int)(word[i])) % 26 + 65));
                    }        
                }
                if(test == firstWord)
                    keys.Add(tmp);
            }
            return true;
        }
    }
}
