using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class CityGenratorIntiation : MonoBehaviour {

    int phase = 0;

    bool roadCheck(List<nodeConnection> createdRoads, nodeConnection checkingNode)
    {
        for (int i1 = 0; i1<createdRoads.Count; i1++)
        {
            if ((createdRoads[i1].nodes[0] == checkingNode.nodes[0] && createdRoads[i1].nodes[1] == checkingNode.nodes[1]) || (createdRoads[i1].nodes[0] == checkingNode.nodes[1] && createdRoads[i1].nodes[1] == checkingNode.nodes[0]))
            {
                return false;
            }
        }
        return true;
    }

    //turns radians into degress
    double radToDeg(double rad)
    {
        return rad * (180 / Math.PI);
    }

    //creates grid in scene
    void gridCreator(float height, float width, float xCenter, float yCenter, int maxConnections, double gridRodation, float minDistX, float minDistY, int minNodeDist, int gridId, double strengthDecayVal)
    {
        // gets the starting x and y points of grid
        float xStarting = xCenter - (width * minDistX) / 2;
        float yStarting = yCenter - (height * minDistY) / 2;

        //counter var
        int counter = 1;

        //creates every node on the grid
        for (float y = 0; y < height; y++)
        {
            for (float x = 0; x < width; x++)
            {
                //gets distance form starting postion
                double xTransistion = minDistX * (x+1) - (width * minDistX / 2);
                double yTransistion = minDistY * (y+1) - (height * minDistY / 2);

                //gets angle needed to move
                double angleChange = Math.Atan2(xTransistion, yTransistion)+gridRodation;

                //gets distance from the center after rotation (hypotonus)
                double hyp = Math.Pow(Math.Pow(xTransistion, 2) + Math.Pow(yTransistion, 2), 0.5);

                //updates x postion
                xTransistion = Math.Cos(angleChange) * hyp + xCenter;

                //updates y postion
                yTransistion = Math.Sin(angleChange) * hyp + yCenter;

                //creates game object
                GameObject temp = Instantiate(Resources.Load("streetNode"), new Vector3((float)(xTransistion), 0, (float)(yTransistion)), Quaternion.identity) as GameObject;

                //roates node to make look pretty
                temp.transform.Rotate(0, 90 - (int) radToDeg(gridRodation), 0);

                //prints variables
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

                //counter
                counter = counter + 1;

                //grid proximety

                //asigns node strength from center of grid
                double strength = 1 - Math.Pow(Math.Pow(Math.Abs(x - width / 2), 2) + Math.Pow(Math.Abs(y - height / 2), 2), 0.5) * strengthDecayVal;

                if (strength < 0)
                {
                    strength = 0;
                }

                //assings variables
                tempVariables.gridProximityStrength = 1 - Math.Pow(Math.Pow(Math.Abs(x- width/2),2)+ Math.Pow(Math.Abs(y - height / 2), 2),0.5) * strengthDecayVal;
                tempVariables.gridID = gridId;
                tempVariables.nodeID = (int) counter;
                tempVariables.XLocation = (int) x + 1;
                tempVariables.YLocation = (int) y + 1;
                tempVariables.minNodeDistance = minNodeDist;
                tempVariables.avaliableConnection = maxConnections;

                if (y == 0 || x == 0 || y == height-1 || x == width - 1)
                {
                    tempVariables.outerNode = true;
                }
                else
                {
                    tempVariables.outerNode = false;
                }

                //assigns node node vals
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
        phase = 1;
    }

    //orders nodes based on node quality
    private List<nodeConnection> connections = new List<nodeConnection> { };

    int counter = 0;
    // Update is called once per frame
    void Update () {
        //checks for nodes that are too close
        if (phase == 1)
        {
            foreach (GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
            {
                node.GetComponent<nodeScript>().nodeCheck();
            }
        }
        //connects nodes+
        else if (phase == 2)
        {
            string allNodesInorder = "";
            foreach (GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
            {
                allNodesInorder += " " + node.gameObject.GetComponent<nodeScript>().nodeID;
                Collider[] nodesRaw = Physics.OverlapSphere(node.GetComponent<nodeScript>().transform.position, node.GetComponent<nodeScript>().maxNodeDistance);

                List<GameObject> nodes = new List<GameObject> { };
                for (int i1 = 0; i1 < nodesRaw.Length; i1++)
                {
                    if (nodesRaw[i1].gameObject != node.gameObject)
                    {
                        nodes.Add(nodesRaw[i1].gameObject);
                    }
                }



                if (node.GetComponent<nodeScript>().outerNode)
                {
                    node.GetComponent<nodeScript>().possibleConnections.Add(node.GetComponent<nodeScript>().strongNodeStrengthFilter(node.GetComponent<nodeScript>().otherGridFilter(nodes)));
                    node.GetComponent<nodeScript>().possibleConnections.Add(node.GetComponent<nodeScript>().weakNodeStrengthFilter(node.GetComponent<nodeScript>().parentGridFilter(nodes)));
                }
                else
                {
                    node.GetComponent<nodeScript>().possibleConnections.Add(node.GetComponent<nodeScript>().strongNodeStrengthFilter(nodes));
                }
            }
        }
        // checks common nodes and finalist nodes
        else if (phase == 3)
        {
            foreach (GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
            {
                node.gameObject.GetComponent<nodeScript>().commonNode();

            }
        }
        //creates roads
        else if (phase == 4)
        {

            foreach (GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
            {
                for (int i1 = 0; i1 < node.gameObject.GetComponent<nodeScript>().finalistNode.Count; i1++)
                {

                    if (roadCheck(connections, node.gameObject.GetComponent<nodeScript>().finalistNode[i1]))
                    {
                        connections.Add(node.gameObject.GetComponent<nodeScript>().finalistNode[i1]);
                    }
                }
            }
            connections.OrderBy(connection => connection.connectionQuality);
        }
        else if (phase == 5)
        {
            for (int i1 = 0; i1 < connections.Count; i1++)
            {
                if (connections[i1].nodes[0].GetComponent<nodeScript>().avaliableConnection > 0 && connections[i1].nodes[1].GetComponent<nodeScript>().avaliableConnection > 0)
                {
                    connections[i1].nodes[0].GetComponent<nodeScript>().createRoad(connections[i1].nodes[1]);
                    connections.Remove(connections[i1]);
                }


            }
        }
        // changes roads to the road mesh
        else if (phase == 6)
        {
            if (counter < 3)
            {
                counter++;
                phase = 4;
            }
        }
        else if(phase == 7)
        {
            foreach(GameObject road in GameObject.FindGameObjectsWithTag("road"))
            {
                road.gameObject.GetComponent<roadCheck>().roadMetamorphs();
            }
        }
        //deletes self
        else
        {
            Destroy(this.gameObject);
        }
        phase++;
    }
}
