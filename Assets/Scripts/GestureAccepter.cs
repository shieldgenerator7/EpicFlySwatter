using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureAccepter : MonoBehaviour {

    public int splatsPerTap = 2;//the max amount of flies that can be smashed with one tap

	// Use this for initialization
	void Start () {
		
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
}
