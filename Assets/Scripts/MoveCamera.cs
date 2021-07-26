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

    public GameObject model; 
    private Touch touch;
    private float rotationSpeed = 0.1f;
    private Vector2 fingerUp;
    private Vector2 fingerDown;
    private string direction;
    private bool detectSwipeOnlyAfterRelease = false;

    

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
                LerpRoutine(); 
                


            }
        }
    }



    private void Rotate()
    {
   
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




    private void LerpRoutine()

    {
        Vector3[] startPositionOuter = new Vector3[3];
        Vector3[] endPositionOuter = new Vector3[3];
        

    //Debug.Log("Size of model  :" + model.transform.childCount);
        
        int counter = 0; 
        foreach (Transform part in model.transform)
        {
            while (counter < 3)
            {
                float desiredDuration = 3f;
                float elapsedTime = 0;

                startPositionOuter[counter] = part.transform.position;
                endPositionOuter[counter] = part.transform.position = new Vector3(x: part.position.x+10f, y: part.position.y, z: part.position.z);
                Debug.Log("Start pos: " + startPositionOuter[counter]);
                Debug.Log("End pos: " + endPositionOuter[counter]);

                elapsedTime += Time.deltaTime;
                float percentageComplete = elapsedTime / desiredDuration;
                Debug.Log("Moving position : " + counter); 
                part.transform.position = Vector3.Lerp(startPositionOuter[counter], endPositionOuter[counter], percentageComplete);

                //KEEP THIS TO CHECK CHILD OF A CHILD Debug.Log(part.transform.childCount);
                counter++;

            }
        }

        
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