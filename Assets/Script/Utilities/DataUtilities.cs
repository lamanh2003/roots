
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Utilities
{
    public static class DataUtilities
    {
        public static string ToJson(this object data, bool isPretty = false)
        {
            return JsonUtility.ToJson(data, isPretty);
        }
        private static Random rng = new Random();  

        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
    }
}