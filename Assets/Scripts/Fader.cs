﻿using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour {
    //2017-11-20: copied from Stonicorn.Fader

    public float startfade = 1.0f;
    public float endfade = 0.0f;
    public float fadetime = 30;
    public float delayTime = 0f;
    
    private ArrayList srs;
    private float startTime;
    private float duration;

    // Use this for initialization
    void Start ()
    {
        startTime = Time.time + delayTime;
        duration = Mathf.Abs(startfade - endfade);
        srs = new ArrayList();
        srs.Add(GetComponent<SpriteRenderer>());
        srs.Add(GetComponent<CanvasRenderer>());
        srs.AddRange(GetComponentsInChildren<SpriteRenderer>());
        foreach (Collider2D bc in GetComponentsInChildren<Collider2D>())
        {
            Destroy(bc);
            //bc.enabled = false;
        }
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.sortingOrder = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime <= Time.time)
        {
            float t = (Time.time - startTime) / duration;//2016-03-17: copied from an answer by treasgu (http://answers.unity3d.com/questions/654836/unity2d-sprite-fade-in-and-out.html)
            foreach (Object o in srs)
            {
                if (!o)
                {
                    continue;//skip null values
                }
                if (o is SpriteRenderer)
                {
                    SpriteRenderer sr = (SpriteRenderer)o;
                    Color prevColor = sr.color;
                    sr.color = new Color(prevColor.r, prevColor.g, prevColor.b, Mathf.SmoothStep(startfade, endfade, t));
                    if (sr.color.a == endfade)
                    {
                        DestroyObject(gameObject);
                    }
                }
                if (o is CanvasRenderer)
                {
                    CanvasRenderer sr = (CanvasRenderer)o;
                    float newAlpha = Mathf.SmoothStep(startfade, endfade, t);
                    sr.SetAlpha(newAlpha);
                    if (newAlpha == endfade)
                    {
                        DestroyObject(gameObject);
                    }
                }
            }
        }
    }
}
