using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq; 

/*TECH TEST PART 1.15
  GLENN KENDALL
  22/07/2021
 */



public class MoveCamera : MonoBehaviour
{
 
    private Touch touch;
    private float rotationSpeed = 0.1f;
    private Vector2 fingerUp;
    private Vector2 fingerDown;
    private string direction;
    private bool detectSwipeOnlyAfterRelease = false;

    private Vector3[] startPos, endPos;

    private float startTime, lerpTime;

    private List<String> labels = new List<string>(); 



    //Start 
    private void Start()
    {
        LoadLabels();
        //check if csv has imported correctly Debug.Log("Labels imported"+labels.Count); 
    }


    //User driven input once per frame - 1 touch, rotate direction  2+touches - expand model 
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.tapCount == 1)
            {
                Rotate();
            }

            else if (touch.tapCount >= 2)
            {
                GameObject model = GameObject.Find("TestModelWithParts");

                Vector3[] startPos = new Vector3[model.transform.childCount];
                Vector3[] endPos = new Vector3[model.transform.childCount];

                int index = 0; 
                foreach (Transform transform in model.transform)
                {
                    startPos[index] = (Vector3)transform.position;
                    Debug.Log("Start pos of part " + index + " : " + startPos[index]);
                    index++;
                 
                 
                //    LerpRoutine(startPos, endPos, startTime, lerpTime);
                }
           
            }
        }
    }



    private void Rotate()
    {
        GameObject model = GameObject.Find("TestModelWithParts"); 
        touch = Input.GetTouch(0);

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

    //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    checkSwipe(ref direction);

                    if (direction.Equals("Y"))
                    {
                        foreach (Transform transform in model.transform)
                        {
                             model.transform.Rotate(new Vector3(x: 1, y: 0, rotationSpeed * Time.deltaTime), Space.Self);
                        }
                    }
                    else
                    {
                        foreach (Transform transform in model.transform)
                        {
                            model.transform.Rotate(new Vector3(x: 0, y: 1, rotationSpeed * Time.deltaTime), Space.Self);
                        }
                    }
                }
            }


    //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                checkSwipe(ref direction);
            }
        }


    //Checks the direction of swipe and returns axix direction 
        void checkSwipe(ref string direction)
        {

            if (verticalMove() > rotationSpeed && verticalMove() > horizontalValMove())
            {
                if (fingerDown.y - fingerUp.y > 0)
                { 
                    direction = "Y";
                }
                else if (fingerDown.y - fingerUp.y < 0)
                {
                 
                    direction = "Y";
                }
                fingerUp = fingerDown;
            }

            else if (horizontalValMove() > rotationSpeed && horizontalValMove() > verticalMove())
            {
                if (fingerDown.x - fingerUp.x > 0)
                {
              
                    direction = "X"; 
                }
                else if (fingerDown.x - fingerUp.x < 0)
                {
                
                    direction = "X";
                }
                fingerUp = fingerDown;
            }

        }


        //Position float return 
        float verticalMove()
        {
            return Mathf.Abs(fingerDown.y - fingerUp.y);
        }

        float horizontalValMove()
        {
            return Mathf.Abs(fingerDown.x - fingerUp.x);
        }
    }




    public Vector3 LerpRoutine(Vector3 startPos, Vector3 endPos, float startTime, float lerpTime=1 )

    { 
        float timeSinceStart = Time.time - startTime;
        float percentageComplete = timeSinceStart / lerpTime;
        var result = Vector3.Lerp(startPos, endPos, percentageComplete);


        return result; 
    }


    //Reads Labels from CSV file 
    private void LoadLabels()
    {
        try
        {
            string path = "Assets/Labels.csv";
            StreamReader fileIn = File.OpenText(path);
            char delim = ',';

            while (!fileIn.EndOfStream)
            {
                string line = fileIn.ReadLine();
                labels = line.Split(delim).ToList(); 
            }

            fileIn.Close();
        }
        catch
        {
            Debug.Log("Cannot find CSV file"); 
        }
    }

}