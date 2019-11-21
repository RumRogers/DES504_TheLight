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

    public static IEnumerator WaitUntilAndExecute(GameManager.Predicate predicate, GameManager.Callback callback)
    {
        while(!predicate())
        {
            yield return new WaitForSeconds(.5f);
        }

        callback();
    }

    public static Transform FindChildByNameRecursive(Transform node, string name)
    {
        foreach(Transform t in node)
        {
            if(t.name.Equals(name))
            {
                return t;
            }

            Transform res = FindChildByNameRecursive(t, name);
            if(res != null)
            {
                return res;
            }
        }

        return null;
    }
}
