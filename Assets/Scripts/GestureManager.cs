﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureManager : MonoBehaviour
{
    //2017-11-20: copied from Stonicorn.GestureManager
    public Camera cam;
    private CameraController cmaController;
    private GestureAccepter gestureAccepter;

    //Settings
    public float dragThreshold = 50;//how far from the original mouse position the current position has to be to count as a drag
    public float holdThreshold = 0.1f;//how long the tap has to be held to count as a hold (in seconds)

    //Gesture Event Methods
    public TapGesture tapGesture;

    //Original Positions
    private Vector3 origMP;//"original mouse position": the mouse position at the last mouse down (or tap down) event
    private Vector3 origMPWorld;
    private float origTime = 0f;//"original time": the clock time at the last mouse down (or tap down) event
    //Current Positions
    private Vector3 curMP;//"current mouse position"
    private Vector3 curMPWorld;//"current mouse position world" - the mouse coordinates in the world
    private float curTime = 0f;
    //Stats
    private int touchCount = 0;//how many touches to process, usually only 0 or 1, only 2 if zoom
    private float maxMouseMovement = 0f;//how far the mouse has moved since the last mouse down (or tap down) event
    private float holdTime = 0f;//how long the gesture has been held for
    private enum ClickState { Began, InProgress, Ended, None };
    private ClickState clickState = ClickState.None;
    //
    public int tapCount = 0;//how many taps have ever been made, including tap+holds that were sent back as taps
    //Flags
    private bool isDrag = false;
    private bool isTapGesture = true;
    private bool isHoldGesture = false;
    public const float holdTimeScale = 0.5f;
    public const float holdTimeScaleRecip = 1 / holdTimeScale;
    public float holdThresholdScale = 1.0f;//the amount to multiply the holdThreshold by


    // Use this for initialization
    void Start()
    {
        cmaController = cam.GetComponent<CameraController>();
        gestureAccepter = GetComponent<GestureAccepter>();

        Input.simulateMouseWithTouches = false;
    }

    // Update is called once per frame
    void Update()
    {
        //
        //Threshold updating
        //
        float newDT = Mathf.Min(Screen.width, Screen.height) / 20;
        if (dragThreshold != newDT)
        {
            dragThreshold = newDT;
        }
        //
        //Input scouting
        //
        if (Input.touchCount > 2)
        {
            touchCount = 0;
        }
        else if (Input.touchCount == 2)
        {
            touchCount = 2;
            if (Input.GetTouch(1).phase == TouchPhase.Began)
            {
                clickState = ClickState.Began;
            }
            else if (Input.GetTouch(1).phase == TouchPhase.Ended)
            {
            }
            else
            {
                clickState = ClickState.InProgress;
            }
        }
        else if (Input.touchCount == 1)
        {
            touchCount = 1;
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                clickState = ClickState.Began;
                origMP = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                clickState = ClickState.Ended;
            }
            else
            {
                clickState = ClickState.InProgress;
                curMP = Input.GetTouch(0).position;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            touchCount = 1;
            if (Input.GetMouseButtonDown(0))
            {
                clickState = ClickState.Began;
                origMP = Input.mousePosition;
            }
            else
            {
                clickState = ClickState.InProgress;
                curMP = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            clickState = ClickState.Ended;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            clickState = ClickState.InProgress;
        }
        else if (Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            touchCount = 0;
            clickState = ClickState.None;
        }

        //
        //Preliminary Processing
        //Stats are processed here
        //
        switch (clickState)
        {
            case ClickState.Began:
                if (touchCount < 2)
                {
                    curMP = origMP;
                    maxMouseMovement = 0;
                    origTime = Time.time;
                    curTime = origTime;
                }
                break;
            case ClickState.Ended: //do the same thing you would for "in progress"
            case ClickState.InProgress:
                float mm = Vector3.Distance(curMP, origMP);
                if (mm > maxMouseMovement)
                {
                    maxMouseMovement = mm;
                }
                curTime = Time.time;
                holdTime = curTime - origTime;
                break;
            case ClickState.None: break;
            default:
                throw new System.Exception("Click State of wrong type, or type not processed! (Stat Processing) clickState: " + clickState);
        }
        curMPWorld = (Vector2)cam.ScreenToWorldPoint(curMP);//cast to Vector2 to force z to 0
        origMPWorld = (Vector2)cam.ScreenToWorldPoint(origMP);//cast to Vector2 to force z to 0


        //
        //Input Processing
        //
        if (touchCount == 1)
        {
            if (clickState == ClickState.Began)
            {
                if (touchCount < 2)
                {
                    //Set all flags = true
                    isDrag = false;
                    isTapGesture = true;
                    isHoldGesture = false;
                }
                gestureAccepter.processTapGesture(curMPWorld);
            }
            else if (clickState == ClickState.InProgress)
            {
                if (maxMouseMovement > dragThreshold)
                {
                    if (!isHoldGesture)
                    {
                        isTapGesture = false;
                        isDrag = true;
                    }
                }
                if (holdTime > holdThreshold * holdThresholdScale)
                {
                    if (!isDrag)
                    {
                        isTapGesture = false;
                        isHoldGesture = true;
                        Time.timeScale = holdTimeScale;
                    }
                }
                if (isDrag)
                {
                    //Check to make sure Merky doesn't get dragged off camera
                    Vector3 delta = cam.ScreenToWorldPoint(origMP) - cam.ScreenToWorldPoint(curMP);
                }
                else if (isHoldGesture)
                {
                    //currentGP.processHoldGesture(curMPWorld, holdTime, false);
                }
            }
            else if (clickState == ClickState.Ended)
            {
                if (isDrag)
                {
                    gestureAccepter.processDragGesture(origMPWorld, curMPWorld);
                }
                else if (isHoldGesture)
                {
                    //currentGP.processHoldGesture(curMPWorld, holdTime, true);
                }
                else if (isTapGesture)
                {
                    tapCount++;
                    adjustHoldThreshold(holdTime, false);
                    //gestureAccepter.processTapGesture(curMPWorld);
                    if (tapGesture != null)
                    {
                        tapGesture();
                    }
                }

                //Set all flags = false
                isDrag = false;
                isTapGesture = false;
                isHoldGesture = false;
                Time.timeScale = 1;
            }
            else
            {
                throw new System.Exception("Click State of wrong type, or type not processed! (Input Processing) clickState: " + clickState);
            }

        }
        else
        {//touchCount == 0 || touchCount >= 2
            if (clickState == ClickState.Began)
            {
            }
            else if (clickState == ClickState.InProgress)
            {                
            }
            else if (clickState == ClickState.Ended)
            {
            }
        }

        //
        //Application closing
        //
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        //
    }

    /// <summary>
    /// Accepts the given holdTime as not a hold but a tap and adjusts holdThresholdScale
    /// Used by outside classes to indicate that a tap gesture was incorrectly classified as a hold gesture
    /// </summary>
    /// <param name="holdTime"></param>
    public void adjustHoldThreshold(float holdTime)
    {
        adjustHoldThreshold(holdTime, true);
    }
    /// <summary>
    /// Used by the GestureManager to adapt hold threshold even when gestures are being classified correctly
    /// Expects tapCount to never be 0 when called directly from GestureManager
    /// </summary>
    /// <param name="holdTime"></param>
    /// <param name="incrementTapCount"></param>
    private void adjustHoldThreshold(float holdTime, bool incrementTapCount)
    {
        if (incrementTapCount)
        {
            tapCount++;
        }
        holdThresholdScale = (holdThresholdScale * (tapCount - 1) + (holdTime / holdThreshold)) / tapCount;
        if (holdThresholdScale < 1)
        {
            holdThresholdScale = 1.0f;//keep it from going lower than the default holdThreshold
        }
    }
    /// <summary>
    /// Returns the absolute hold threshold, including its scale
    /// </summary>
    /// <returns></returns>
    public float getHoldThreshold()
    {
        return holdThreshold * holdThresholdScale;
    }

    /// <summary>
    /// Gets called when a tap gesture is processed
    /// </summary>
    public delegate void TapGesture();
}