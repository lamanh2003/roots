
using UnityEngine;

namespace Utilities
{
    public static class DataUtilities
    {
        public static string ToJson(this object data, bool isPretty = false)
        {
            return JsonUtility.ToJson(data, isPretty);
        }
    }
}