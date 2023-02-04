using UnityEngine;

public class UseProfile : MonoBehaviour
{
    public static int CurrentLevel
    {
        get
        {
            return PlayerPrefs.GetInt("CURRENT_LEVEL", 0);
        }
        set
        {
            PlayerPrefs.SetInt("CURRENT_LEVEL", value);
            PlayerPrefs.Save();
        }
    }
}

