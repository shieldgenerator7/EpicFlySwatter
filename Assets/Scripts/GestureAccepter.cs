﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureAccepter : MonoBehaviour
{

    public int splatsPerTap = 2;//the max amount of flies that can be smashed with one tap

    public GameObject cutGO;//the GameObject that is the cut sprite
    private float baseWidth;//base size of cutGO sprite
    private float baseHeight;

    // Use this for initialization
    void Start()
    {
        //Set the size
        Vector3 size = cutGO.GetComponent<SpriteRenderer>().bounds.size;
        baseWidth = size.x;
        baseHeight = size.y;
    }

    /// <summary>
    /// Processes the tap gesture with the given position in World Space
    /// </summary>
    /// <param name="tapPos"></param>
    public void processTapGesture(Vector2 tapPos)
    {
        int splats = 0;
        foreach (CircleCollider2D cc2d in FindObjectsOfType<CircleCollider2D>())
        {
            if (cc2d.OverlapPoint(tapPos))
            {
                FlyController fc = cc2d.gameObject.GetComponent<FlyController>();
                if (fc)
                {
                    splats++;
                    fc.splat();
                    if (splats == splatsPerTap)
                    {
                        break;
                    }
                }
            }
        }
    }

    public void processDragGesture(Vector2 beginPos, Vector2 endPos)
    {
        //2017-11-20: copied from Stonicorn.TeleportStreakUpdater.position()

        //Set the position
        cutGO.transform.position = new Vector3(beginPos.x, beginPos.y, 1);

        float distance = Vector3.Distance(beginPos, endPos);
        float newSize = distance;
        Vector3 newV = new Vector3(newSize / baseWidth, 1.5f * baseHeight / baseHeight);
        cutGO.transform.localScale = newV;

        //Set the angle
        float angle = AngleSigned(endPos - beginPos, Vector3.left, Vector3.back);
        cutGO.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    //2015-12-21: copied from http://forum.unity3d.com/threads/need-vector3-angle-to-return-a-negtive-or-relative-value.51092/#post-324018
    //originally posted by Tinus
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }
}
