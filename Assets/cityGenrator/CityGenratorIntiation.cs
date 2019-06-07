﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

// CityGenratorIntiation creates the city
public class CityGenratorIntiation : MonoBehaviour {

    //decalres variables
    int phase = 0;
    //orders nodes based on node quality
    private List<nodeConnection> connections = new List<nodeConnection> { };
    int counter = 0;

    public int npcGen = 60;
    //checks road already exists
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

    //creates grid in the scene
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


	// creates grid
	void Start () {
        //genrates item (Math.PI/6)
        gridCreator(32, 64, 0, 0 , 4, Math.PI/6 , 15, 10, 9, 1, 0.05);
       // gridCreator(50, 30, 30, 50, 4, Math.PI / 4, 20, 30, 19, 2, 0.1);
        //gridCreator(20, 43, 10, 70, 4, Math.PI / 2, 20, 15, 12, 3, 0.08);
        phase = 1;
    }

    
    // Update is called once per frame
    void Update () {
        //checks for nodes that are too close
        if (phase == 1)
        {

            List<GameObject> allNodes = new List<GameObject>{ };
            //gets all nodes that are too close
            foreach (GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
            {
                allNodes.Add(node);
            }
            allNodes.OrderBy(node => node.GetComponent<nodeScript>().gridProximityStrength);

            //destroy nodes that are too close
            //the node with a weaker grid strength will be destroyed in a conflict
            for (int i1 = 0; i1<allNodes.Count; i1++)
            {
                if (allNodes[i1].GetComponent<nodeScript>().existance)
                {
                    allNodes[i1].GetComponent<nodeScript>().nodeCheck();
                }
            }
        }
        //connects nodes
        else if (phase == 2)
        {
            //find all possible connections
            foreach (GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
            {
                Collider[] nodesRaw = Physics.OverlapSphere(node.GetComponent<nodeScript>().transform.position, node.GetComponent<nodeScript>().maxNodeDistance);

                List<GameObject> nodes = new List<GameObject> { };
                for (int i1 = 0; i1 < nodesRaw.Length; i1++)
                {
                    if (nodesRaw[i1].gameObject != node.gameObject)
                    {
                        nodes.Add(nodesRaw[i1].gameObject);
                    }
                }



                //organizes connections by a node heiarchy
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
        // filters out uncommon nodes
        else if (phase == 3)
        {
            foreach (GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
            {
                node.gameObject.GetComponent<nodeScript>().commonNode();

            }
        }
        //gets all possible connections
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
        //creates a road between every node
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
        //connections are destroyed based if roads are intersectings
        // repeats phase 5, 3 times inorder connect nodes that still can connect
        else if (phase == 6)
        {
            if (counter < 3)
            {
                counter++;
                phase = 4;
            }
        }
        //turns the roads into their road mesh
        else if(phase == 7)
        {
            foreach(GameObject road in GameObject.FindGameObjectsWithTag("road"))
            {
                road.gameObject.GetComponent<roadCheck>().roadMetamorphs();
            }
        }
        //deletes all nodes that are not connected
        else if(phase == 8)
        {
            foreach(GameObject node in GameObject.FindGameObjectsWithTag("streetNode"))
            {
                if (node.GetComponent<nodeScript>().connectedNode.Count == 0)
                {
                    Destroy(node);
                }
            }
        }
        else if(phase == 9)
        {
            List<GameObject> allNodes = GameObject.FindGameObjectsWithTag("streetNode").ToList();
            for (int i1 = 0; i1 < 500; i1++)
            {
                System.Random rnd = new System.Random();

                int r = rnd.Next(allNodes.Count);

                GameObject node = allNodes[r];

                Vector3 position = node.transform.position;
                position.y += 1.5f;
                GameObject npc = Instantiate(Resources.Load("civ"), position, Quaternion.identity) as GameObject;

                npc.GetComponent<civilian>().lastNode = node;
                allNodes.Remove(node);
            }
            for (int i1 = 0; i1 < 10; i1++)
            {
                System.Random rnd = new System.Random();

                int r = rnd.Next(allNodes.Count);

                GameObject node = allNodes[r];

                Vector3 position = node.transform.position;
                position.y += 1.5f;
                GameObject npc = Instantiate(Resources.Load("zombie"), position, Quaternion.identity) as GameObject;

                npc.GetComponent<zombie>().lastNode = node;
                allNodes.Remove(node);
            }
        }
        //after the map has been genrated the map deletes itself
        else
        {
            Destroy(this.gameObject);
        }
        phase++;
    }
}
