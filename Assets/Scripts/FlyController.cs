using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : MonoBehaviour {

    public float moveSpeed = 3.0f;//how fast the fly moves across the screen

    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(0, 360)));
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        rb2d.velocity = transform.up * moveSpeed;
        float zrot = transform.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(zrot - 10, zrot + 10)));
    }
}
