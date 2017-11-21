using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySpawner : MonoBehaviour {

    public GameObject flyPrefab;
    public float fliesPerMinute = 60;

    public float hardModeWaitTime = 10;
    public float fliesPerMinuteHardMode = 240;

    private float nextFlySpawnTime = 0;
    private float flySpawnCooldown = 0;

	// Use this for initialization
	void Start () {
        flySpawnCooldown = 60 / fliesPerMinute;
        spawnFly();
        spawnFly();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextFlySpawnTime)
        {
            nextFlySpawnTime = Time.time + flySpawnCooldown;
            int no = 1;
            if (FindObjectsOfType<CircleCollider2D>().Length <= 1)
            {
                no = 10;
            }
            for (int i = 0; i < no; i++)
            {
                spawnFly();
            }
        }
	}

    void spawnFly()
    {
        GameObject fly = GameObject.Instantiate(flyPrefab);
        fly.transform.position = transform.position;
    }
}
