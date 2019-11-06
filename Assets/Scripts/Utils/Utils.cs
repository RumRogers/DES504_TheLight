using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Color32 SetAlpha(Color c, int alpha)
    {        
        return new Color(c.r, c.g, c.b, alpha);
    }

    public static IEnumerator WaitAndExecute(float seconds, GameManager.Callback callback)
    {
        if(callback == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(seconds);
        callback();
    }
}
