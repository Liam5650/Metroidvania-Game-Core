using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    /*
        Used by several assets to keep them persistent through scene changes.
    */

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(gameObject.tag);

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
