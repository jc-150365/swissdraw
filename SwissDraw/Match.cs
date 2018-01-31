using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwissDraw
{
    public class Match
    {
        private int person1;
        private int person2;
        // result 1:Person1の勝ち　2:Person2の勝ち　0:決着がついていない　他:エラー
        private int result;

        public int Person1 { get => person1; set => person1 = value; }
        public int Person2 { get => person2; set => person2 = value; }
        public int Result { get => result; set => result = value; }

        public Match(int i, int j)
        {
            person1 = i;
            person2 = j;
            result = 0;
        }

        public static Match[] MakeMatch(Dictionary<int, Person> persons, Match[] results)
        {
            /*try
            {*/
                // personsのkeyのみ取り出す
                int[] keys = GetKeyArray(persons);

                // 配列を初期化する
                int matchCount = keys.Length / 2;

                int[][] SplittedKeys = splitPersons(persons, results);

                Match[] matches = MakeMatch1(matchCount, SplittedKeys, persons, results);

            if(matches == null)
            {
                matches = MakeMatch2(matchCount, SplittedKeys, persons, results);
            }
                
                return matches;
            /*}
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);

                return results;
            }*/
        }

        // 「対戦していない」「同じチームじゃない」「勝ち数が同じ」で対戦
        public static Match[] MakeMatch1(int matchCount, int[][] SplittedKeys, Dictionary<int, Person> persons, Match[] results)
        {
            Match[] matches = new Match[matchCount];

            for (int i = 0; i < matchCount; i++)
            {
                // 最小のくじ番号を取得する(使われていないこと)
                int minKey = GetMinimumKey(SplittedKeys, matches);

                // 対応する対戦相手のくじ番号を取得する（使われていない、対戦していない、同じチームじゃない、勝ち数が同じ）
                int versusKey = getVersusKey1(minKey, SplittedKeys, matches, persons, results);

                // versusKey<0なら、対戦相手は見つからなかったため、nullを返す
                if (versusKey < 0)
                {
                    return null;
                }
                //対戦を保存する
                matches[i] = new Match(minKey, versusKey);
            }
            return matches;
        }
        // 「対戦していない」「同じチームじゃない」で対戦
        public static Match[] MakeMatch2(int matchCount, int[][] SplittedKeys, Dictionary<int, Person> persons, Match[] results)
        {
            Match[] matches = new Match[matchCount];

            for (int i = 0; i < matchCount; i++)
            {
                // 最小のくじ番号を取得する(使われていないこと)
                int minKey = GetMinimumKey(SplittedKeys, matches);

                // 対応する対戦相手のくじ番号を取得する（使われていない、対戦していない、同じチームじゃない）
                int versusKey = getVersusKey2(minKey, SplittedKeys, matches, persons, results);

                // versusKey<0なら、対戦相手は見つからなかったため、nullを返す
                if (versusKey < 0)
                {
                    return null;
                }
                //対戦を保存する
                matches[i] = new Match(minKey, versusKey);
            }
            return matches;
        }

        // 対応する対戦相手のくじ番号を取得する
        //（使われていない、対戦していない、同じチームじゃない、勝ち数が同じ）
        public static int getVersusKey1(int minKey, int[][] splittedKeys, Match[] matches,
            Dictionary<int, Person> persons, Match[] results)
        {
            int i = 0;
            bool flag = true;
            while (flag == true)
            {
                if (splittedKeys[i].Contains(minKey) == true)
                {
                    flag = false;
                }
                else
                {
                    i++;
                }
            }

            foreach (int key in splittedKeys[i])
            {
                if (key != minKey)
                {
                    if (containsKey(matches, key) == false)
                    {
                        if (isMatched(results, key, minKey) == false)
                        {
                            if (isSameGroup(persons, key, minKey) == false)
                            {
                                return key;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        // 対応する対戦相手のくじ番号を取得する
        //（使われていない、対戦していない、同じチームじゃない、勝ち数無視）
        public static int getVersusKey2(int minKey, int[][] splittedKeys, Match[] matches,
            Dictionary<int, Person> persons, Match[] results)
        {
            int i = 0;
            bool flag = true;
            while (flag == true)
            {
                if (splittedKeys[i].Contains(minKey) == true)
                {
                    flag = false;
                }
                else
                {
                    i++;
                }
            }
            while (i <= splittedKeys.Length)
            {
                foreach (int key in splittedKeys[i])
                {
                    if (key != minKey)
                    {
                        if (containsKey(matches, key) == false)
                        {
                            if (isMatched(results, key, minKey) == false)
                            {
                                if (isSameGroup(persons, key, minKey) == false)
                                {
                                    return key;
                                }
                            }
                        }
                    }
                }
                i++;
            }
            return -1;
        }

        // 最小のくじ番号を取得する(使われていないこと)
        public static int GetMinimumKey(int[][] keys, Match[] matches)
        {
            int m = 1000;

            foreach (int[] key in keys)
            {
                foreach (int key2 in key)
                {
                    if (m > key2)
                    {
                        if (containsKey(matches, key2) == false)
                        {
                            m = key2;
                        }
                    }
                    if (m != 1000)
                        return m;
                }
                /*
                foreach(Match key2 in matches)
                {
                    if(key != key2.person1)
                    {
                        if(key != key2.person2)
                        {
                            if (key <= m)
                            {
                                m = key;
                            }
                        }
                    }
                }
                */
            }

            return m;

            //throw new NotImplementedException();
        }

        //使われてないメソッド
        public static bool NotUse(int key2, Match[] matches)
        {
            bool b = false;

            foreach (Match mkey in matches)
            {
                if (key2 != mkey.person1)
                {
                    if (key2 != mkey.person2)
                    {
                        b = true;
                    }
                    else
                    {
                        b = false;
                    }
                }
                else
                {
                    b = false;
                }
            }

            return b;
        }

        // 同じグループか調べる（終了）
        public static bool isSameGroup(Dictionary<int, Person> persons, int i, int j)
        {
            String iTeam = persons[i].PersonGroup;
            String jTeam = persons[j].PersonGroup;
            return iTeam.Equals(jTeam);
        }

        //対戦していないメソッド
        public static bool NotVersus(int minKey, int key, Match matches)
        {
            if (matches.person1 == minKey)
            {
                if (matches.person2 == key)
                {
                    return true;
                }
            }
            if (matches.person2 == minKey)
            {
                if (matches.person1 == key)
                {
                    return true;
                }
            }

            return false;
        }

        //勝ち数が同じ人のIDをまとめる（終了）
        public static int[][] splitPersons(Dictionary<int, Person> persons, Match[] results)
        {
            Dictionary<int, int> winCountDic = new Dictionary<int, int>();//ディクショナリーを作る
            int[] keyArray = GetKeyArray(persons);//全キー取得
            int maxWinCount = -1;
            foreach (int i in keyArray)
            {
                int wCount = getWinCount(i, results);//勝ち数取得
                winCountDic.Add(i, wCount);
                if (maxWinCount < wCount)
                {
                    maxWinCount = wCount;
                }
            }

            /**********独自*******************************/
            int m = maxWinCount;

            int mc = maxWinCount + 1;

            int l = keyArray.Length;

            int[][] split = new int[mc][];



            for (int x = 0; x <= m; x++)
            {

                split[x] = makePersonArray(m - x, winCountDic);
                /*
                int v = 0;
                foreach (int i in winCountDic.Keys)
                {
                    if(winCountDic[i] == x)
                    {
                        split[x][v] = winCountDic[i];
                        v++;
                    }
                }
                */
            }
            return split;
            /**************独自**********************/
        }

        // winCountDicの中で、勝数がvの要素の配列を生成する（終了）
        public static int[] makePersonArray(int v, Dictionary<int, int> winCountDic)
        {
            List<int> l = new List<int>();
            foreach (int i in winCountDic.Keys)
            {
                if (winCountDic[i] == v)
                {
                    l.Add(i);
                }
            }
            return l.ToArray();
        }

        //指定されたkeyの勝ち数を調べる（終了）
        public static int getWinCount(int key, Match[] results)
        {
            int winCount = 0;
            foreach (Match result in results)
            {
                if (checkWin(key, result) == true)
                {
                    winCount++;
                }
            }
            return winCount;
        }

        //keyが勝っていればtrue（終了）
        public static Boolean checkWin(int key, Match result)
        {
            if (containsKey(result, key) == true)
            {
                //int count = 0;
                if (result.result == 1)//resultが1か（person1の勝ちか）
                {
                    if (result.person1 == key)
                    {
                        return (true);
                    }
                }
                else if (result.result == 2)//resultが2か（person2の勝ちか）
                {
                    if (result.person2 == key)
                    {
                        return (true);
                    }
                }
            }
            return false;
        }

        // resultsの中に、iとjの対戦があればtrueを返す（終了）
        public static bool isMatched(Match[] results, int i, int j)
        {
            bool b = false;

            foreach (Match m in results)
            {
               if (isMatched(m, i, j) == true)
               {
                    b = true;
                    
               }
            }

            return b;
            
        }

        // mがiとjの対戦ならtrueを返す（終了）
        public static bool isMatched(Match m, int i, int j)
        {
            if (m.person1 == i)
            {
                if (m.person2 == j)
                {
                    return true;
                }
            }

            if (m.person2 == i)
            {
                if (m.person1 == j)
                {
                    return true;
                }
            }

            return false;
        }

        //（終了）
        public static bool containsKey(Match[] matches, int i)
        {
            bool b = false;

            foreach (Match m in matches)
            {
                //nullだったらcontainsKeyを呼ばない
                if (m != null) 
                {
                    if (containsKey(m, i) == true)
                    {
                        b = true;
                    }
                }
            }

            return b;
        }

        //（終了）
        private static bool containsKey(Match m, int i)
        {
            if (m.person1 == i)
            {
                return true;
            }
            if (m.person2 == i)
            {
                return true;
            }
            return false;
        }


        // 2つの配列をマージして1つにする（終了）
        public static Match[] MergeMatch(Match[] results1, Match[] results2)
        {
            int rLength = results1.Length + results2.Length;

            Match[] rArray = new Match[rLength];

            int x = 0;

            for (int i = 0; i < results1.Length; i++)
            {
                rArray[x] = results1[i];
                x++;
            }

            for (int j = 0; j < results2.Length; j++)
            {
                rArray[x] = results2[j];
                x++;
            }

            return rArray;
        }

        // personsのkeyを取り出して配列にする(終了)
        public static int[] GetKeyArray(Dictionary<int, Person> persons)
        {
            return persons.Keys.ToArray();
        }

    }

}
