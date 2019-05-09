using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class CityGenratorIntiation : MonoBehaviour {

    double radToDeg(double rad)
    {
        return rad * (180 / Math.PI);
    }

    void gridCreator(float height, float width, float xCenter, float yCenter, int maxConnections, double gridRodation, float minDistX, float minDistY, int minNodeDist, int gridId, double strengthDecayVal)
    {
        float xStarting = xCenter - (width * minDistX) / 2;
        float yStarting = yCenter - (height * minDistY) / 2;

        int counter = 1;
        for (float y = 0; y < height; y++)
        {
            for (float x = 0; x < width; x++)
            {
                double xTransistion = minDistX * (x+1) - (width * minDistX / 2);
                double yTransistion = minDistY * (y+1) - (height * minDistY / 2);

                //gets angle change
                double angleChange = Math.Atan2(xTransistion, yTransistion)+gridRodation;

                double hyp = Math.Pow(Math.Pow(xTransistion, 2) + Math.Pow(yTransistion, 2), 0.5);

                xTransistion = Math.Cos(angleChange) * hyp + xCenter;

                yTransistion = Math.Sin(angleChange) * hyp + yCenter;

                GameObject temp = Instantiate(Resources.Load("streetNode"), new Vector3((float)(xTransistion), 0, (float)(yTransistion)), Quaternion.identity) as GameObject;

                temp.transform.Rotate(0, 90 - (int) radToDeg(gridRodation), 0);

                nodeScript tempVariables = temp.GetComponent<nodeScript>();
//                Debug.Log("x - " + x);
//                Debug.Log("y - " + y);
 //               Debug.Log("xtransistion - " + xTransistion);
  //              Debug.Log("ytransistion - " + yTransistion);
    //            Debug.Log("angle - " + angleChange);
      //          Debug.Log("quadrant - ");
        //        Debug.Log("sin value - " + Math.Sin(angleChange));
          //      Debug.Log("cos value - " + Math.Cos(angleChange));
            //    Debug.Log("sin value - " + Math.Sin(angleChange));
              //  Debug.Log("x local - " + xTransistion * Math.Cos(angleChange));
                //Debug.Log("y local - " + yTransistion * Math.Sin(angleChange));

                counter = counter + 1;

                //grid proximety

                double strength = 1 - Math.Pow(Math.Pow(Math.Abs(x - width / 2), 2) + Math.Pow(Math.Abs(y - height / 2), 2), 0.5) * strengthDecayVal;

                if (strength < 0)
                {
                    strength = 0;
                }

                tempVariables.gridProximityStrength = 1 - Math.Pow(Math.Pow(Math.Abs(x- width/2),2)+ Math.Pow(Math.Abs(y - height / 2), 2),0.5) * strengthDecayVal;
                tempVariables.gridID = gridId;
                tempVariables.nodeID = (int) counter;
                tempVariables.XLocation = (int) x + 1;
                tempVariables.YLocation = (int) y + 1;
                tempVariables.minNodeDistance = minNodeDist;
                tempVariables.maxConnection = maxConnections;

                if(minDistX > minDistY)
                {
                    tempVariables.maxNodeDistance = Convert.ToInt32(minDistX *1.05);
                }
                else
                {
                    tempVariables.maxNodeDistance = Convert.ToInt32(minDistY * 1.05);
                }
            }
        }

        
    }


	// Use this for initialization
	void Start () {


        //genrates item (Math.PI/6)
        gridCreator(32, 64, 0, 0 , 4, Math.PI/6 , 15, 10, 9, 1, 0.05);
        gridCreator(50, 30, 30, 50, 4, Math.PI / 4, 20, 30, 19, 2, 0.1);
        
        foreach (GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
        {
            node.GetComponent<nodeScript>().nodeCheck();
        }

        foreach (GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
        {
         node.GetComponent<nodeScript>().roadConection();
        }
    }

    // Update is called once per frame
    //void Update () {

    //}
}
