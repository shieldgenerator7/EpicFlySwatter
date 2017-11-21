using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySpawner : MonoBehaviour {

    public GameObject flyPrefab;
    public float fliesPerMinute = 60;

    private float nextFlySpawnTime = 0;
    private float flySpawnCooldown = 0;

	// Use this for initialization
	void Start () {
        flySpawnCooldown = 60 / fliesPerMinute;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextFlySpawnTime)
        {
            nextFlySpawnTime = Time.time + flySpawnCooldown;
            spawnFly();
        }
	}

    void spawnFly()
    {
        GameObject fly = GameObject.Instantiate(flyPrefab);
        fly.transform.position = transform.position;
    }
}
