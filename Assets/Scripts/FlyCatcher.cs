using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCatcher : MonoBehaviour {

    public bool isTop = true;//true: top exit, false: bottom exit

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<FlyController>())
        {
            if ((isTop && collider.gameObject.transform.position.y > transform.position.y)
                || (!isTop && collider.gameObject.transform.position.y < transform.position.y))
            {
                Destroy(collider.gameObject);
            }
        }
    }
}
