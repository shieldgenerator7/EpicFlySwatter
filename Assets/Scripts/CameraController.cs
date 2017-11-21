using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    //2017-11-20: copied from Stonicorn.CameraController
    public float scale = 5.0f;//
    public GameObject leftWall;
    public GameObject rightWall;

    private Camera cam;

    private int prevScreenWidth;
    private int prevScreenHeight;    

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        Screen.orientation = ScreenOrientation.Portrait;
        updateOrthographicSize();
    }

    void Update()
    {
        if (prevScreenHeight != Screen.height || prevScreenWidth != Screen.width)
        {
            prevScreenWidth = Screen.width;
            prevScreenHeight = Screen.height;
            updateOrthographicSize();
        }
    }
    
    public void updateOrthographicSize()
    {
        if (Screen.height > Screen.width)//portrait orientation
        {
            cam.orthographicSize = (scale * cam.pixelHeight) / cam.pixelWidth;
        }
        else {//landscape orientation
            cam.orthographicSize = scale;
        }
        Vector2 camSize = cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight)) - cam.ScreenToWorldPoint(Vector2.zero);
        leftWall.transform.localScale = rightWall.transform.localScale = new Vector3(camSize.x / 50, camSize.y);
        leftWall.transform.position = new Vector2(cam.ScreenToWorldPoint(Vector2.zero).x + leftWall.GetComponent<SpriteRenderer>().bounds.extents.x, 0);
        rightWall.transform.position = new Vector2(cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight)).x - rightWall.GetComponent<SpriteRenderer>().bounds.extents.x, 0);
    }

    /// <summary>
    /// Returns whether or not the given position is in the camera's view
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool inView(Vector2 position)
    {
        //2017-10-31: copied from an answer by Taylor-Libonati: http://answers.unity3d.com/questions/720447/if-game-object-is-in-cameras-field-of-view.html
        Vector3 screenPoint = cam.WorldToViewportPoint(position);
        return screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}
