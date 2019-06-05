using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//node connection is a class that holds 2 speficic caractistics about a connection
//1 - an array of the two connecting nodes
//2 - the quality of the connection between the nodes
//the quality of the connection is based on where the nodes are located in eachothers possible connections list
//as a result mthe most ideal connection has a value of 0 while the a less ideal connection has a larger number
public class nodeConnection
{
    public GameObject[] nodes = new GameObject[2];

    public int connectionQuality;
}

//node script is a class that deals with every node
public class nodeScript : MonoBehaviour {

    //declaration of class variables
    public int nodeID = 0;
    public int gridID = 0;
    public int XLocation = 0;
    public int YLocation = 0;
    public int minNodeDistance = 0;
    public int maxNodeDistance = 0;
    public int avaliableConnection = 0;
    public double gridProximityStrength = 0;
    public bool outerNode = false;



    public bool existance = true;

    public List<List<GameObject>> possibleConnections = new List<List<GameObject>> { };
    public List<nodeConnection> finalistNode = new List<nodeConnection> { };
    public List<GameObject> connectedNode = new List<GameObject> { };


    //colour finder assigns a colour to the node that assoicates with the node strength
    void ColourFinder(double strength)
    {
        MeshRenderer gameObjectRenderer = this.GetComponent<MeshRenderer>();

        Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"));

        //gets the value of each colour value depending the 3 quadratic function
        float[] colourRGB = { Convert.ToSingle(-2.1 * Math.Pow(strength, 2) + 1), Convert.ToSingle(-10 * Math.Pow(strength - 1, 2) + 1), Convert.ToSingle(-10 * Math.Pow(strength - 0.6, 2) + 1) };

        //checks if colour value is less than 0
        for (int colour = 0; colour < colourRGB.Length; colour++)
        {
            if (colourRGB[colour] < 0)
            {
                colourRGB[colour] = 0;
            }
        }
        //assings calculated colour value to material
        newMaterial.color = new Color(colourRGB[0], colourRGB[1], colourRGB[2]);
        gameObjectRenderer.material = newMaterial;
    }
    //checks if a node is too close
    public void nodeCheck()
    {
        //gets nodes that are too close
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, minNodeDistance);

        List<Collider> deletedCollider = new List<Collider> { };

        //checks if this node has a higer nodestrencth
        //if this node has a higher nodestrength compared the nodes in hitColliders then everynode in hitcollider shall be destroyed
        for (int i1 = 0; i1 < hitColliders.Length; i1++)
        {
            try
            {
                nodeScript temp = hitColliders[i1].gameObject.GetComponent<nodeScript>();

                if (nodeID != temp.nodeID)
                {
                    if (gridProximityStrength > temp.gridProximityStrength)
                    {
                        deletedCollider.Add(hitColliders[i1]);
                    }
                    else
                    {
                        existance = false;
                        Destroy(this.gameObject);
                        break;
                    }
                }
            }
            catch
            {
            }
        }
        //destoys all nodes that shouldn't exist by doing 3 things
        //1 - sets node existing node to false
        //2 -  update all adjacent nodes to outer nodes if deleted node is an outer node
        //3 - destroys node
        if (existance)
        {
            for (int i1 = 0; i1 < deletedCollider.Count; i1++)
            {
                deletedCollider[0].gameObject.GetComponent<nodeScript>().existance = false;

                deletedCollider[0].gameObject.GetComponent<nodeScript>().outerNodeUpdate();

                Destroy(deletedCollider[0].gameObject);
            }
        }
    }
    //checks if the node is an outer node and if it is updates all adjacent family nodes as an outer node
    public void outerNodeUpdate()
    {
        if (outerNode)
        {
            outerNode = false;

            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, minNodeDistance);

            List<Collider> deletedCollider = new List<Collider> { };

            for (int i1 = 0; i1 < hitColliders.Length; i1++)
            {
                try
                {
                    nodeScript temp = hitColliders[i1].gameObject.GetComponent<nodeScript>();

                    if (nodeID != temp.nodeID && gridID == temp.gridID)
                    {
                        temp.outerNode = true;
                    }
                }
                catch
                {
                }
            }
        }
    }
    //gets the distance between two nodes using the pythagorian therom
    public double nodeDist(GameObject node1,GameObject node2)
    {
        double xDist = node1.gameObject.transform.position.x - node2.gameObject.transform.position.x;
        double yDist = node1.gameObject.transform.position.y - node2.gameObject.transform.position.y;
        double zDist = node1.gameObject.transform.position.z - node2.gameObject.transform.position.z;

        double xzDist = Math.Pow(Math.Pow(xDist, 2) + Math.Pow(yDist, 2), 0.5);
        double dist = Math.Pow(Math.Pow(xzDist, 2) + Math.Pow(yDist, 2), 0.5);
        return dist;
    }
    
    //functions that organize a list of nodes based on certain charactersitics

    //returns nodes from another node family
    public List<GameObject> otherGridFilter(List<GameObject> nodePool)
    {
        List<GameObject> temp = new List<GameObject> { };
        for(int i1 =0; i1 < nodePool.Count; i1++)
        {
            try
            {
                if (nodePool[i1].GetComponent<nodeScript>().gridID != gridID)
                {
                    temp.Add(nodePool[i1]);
                }
            }
            catch { }
            
        }
        return temp;
    }

    //returns nodes from same node family
    public List<GameObject> parentGridFilter(List<GameObject> nodePool)
    {
        List<GameObject> temp = new List<GameObject> { };
        for (int i1 = 0; i1 < nodePool.Count; i1++)
        {
            try
            {
                if (nodePool[i1].GetComponent<nodeScript>().gridID == gridID)
                {
                    temp.Add(nodePool[i1]);
                }
            }
            catch { }
            
        }
        return temp;
    }

    //organizes nodes based on stongest to weakest
    public List<GameObject> strongNodeStrengthFilter(List<GameObject> nodePool)
    {
        try
        {
            List<GameObject> newOrderNodes = nodePool.OrderBy(node => node.GetComponent<nodeScript>().gridProximityStrength).ToList();
            return newOrderNodes;
        }
        catch
        {
            return new List<GameObject> { };
        }
            
        
    }

    //organizes nodes based weakest to strongest
    public List<GameObject> weakNodeStrengthFilter(List<GameObject> nodePool)
    {
        try
        {
            List<GameObject> newOrderNodes = nodePool.OrderBy(node => node.GetComponent<nodeScript>().gridProximityStrength).Reverse().ToList();
            return newOrderNodes;
        }
        catch
        {
            return new List<GameObject> { };
        }
    }

    // orders nodes based on closest to farthest
    public List<GameObject> closetNodeFilter(List<GameObject> nodePool)
    {
        List<GameObject> newOrderNodes = nodePool.OrderBy(node => nodeDist(node,this.gameObject)).ToList();
        return newOrderNodes;
    }

    // order nodes based farthest to closest
    public List<GameObject> farthestNodeFilter(List<GameObject> nodePool)
    {
        List<GameObject> newOrderNodes = nodePool.OrderBy(node => nodeDist(node, this.gameObject)).Reverse().ToList();
        return newOrderNodes;
    }

    //check if a node exists in a node heiarchy group
    //returns the connection quality
    //outputs
    // -1 means no connection
    //-1 > means their is a connection
    public int commonNodeChecker(GameObject nodeInput,int nodeGroup)
    {
        int connectionQuality = -1;
        try
        {

            List<List<GameObject>> temp = nodeInput.GetComponent<nodeScript>().possibleConnections;
            for (int i1 = 0; i1 < temp.Count; i1++)
            {
                if (temp[i1].Any(node => node == this.gameObject))
                {
                    connectionQuality = nodeGroup + i1;
                    return connectionQuality;
                }
            }
        }
        catch { }
        return connectionQuality;
    }

    //finds all common nodes
    public void commonNode()
    {

        List<GameObject> uncommonNode = new List<GameObject> { };

        // for loop checks if a both nodes can connect
        for (int i1 = 0; i1<possibleConnections.Count; i1++)
        {
            for (int i2 = 0; i2 < possibleConnections[i1].Count; i2++)
            {
                int temp = commonNodeChecker(possibleConnections[i1][i2], i1);
                //checks if a node is a common node or an uncommon node
                if (temp != -1)
                {
                    nodeConnection connection = new nodeConnection();
                    connection.connectionQuality = temp;
                    connection.nodes[0] = this.gameObject;
                    connection.nodes[1] = possibleConnections[i1][i2];
                    
                    possibleConnections[i1][i2].GetComponent<nodeScript>().finalistNode.Add(connection);
                }
                else
                {
                    uncommonNode.Add(possibleConnections[i1][i2]);
                }
            }
        }
        //removes all uncommon node
        for (int i1=0; i1<uncommonNode.Count; i1++)
        {
            for (int i2 =0; i2 < possibleConnections.Count; i2++)
            {
                if (possibleConnections[i2].Any(node => node == uncommonNode[i1]))
                {
                    possibleConnections[i2].Remove(uncommonNode[i1]);
                }
            }
        }
    }

    //creates a road between two nodes
    public void createRoad(GameObject connectingNode)
    {
        try
        {
            nodeScript temp = connectingNode.GetComponent<nodeScript>();

            Vector3 postion = (temp.transform.position - this.transform.position) / 2.0f + this.transform.position;

            GameObject road = Instantiate(Resources.Load("road"), postion, Quaternion.identity) as GameObject;

            Vector3 scale = road.transform.localScale;
            scale.y = ((this.transform.position - temp.transform.position) / 2).magnitude;
            road.transform.localScale = scale;
            
                
            road.transform.rotation = Quaternion.FromToRotation(Vector3.up, this.transform.position - temp.transform.position);

            road.GetComponent<roadCheck>().connectingNodes.Add(this.gameObject);
            road.GetComponent<roadCheck>().connectingNodes.Add(connectingNode);


            connectedNode.Add(connectingNode);
            avaliableConnection -= 1;
            connectingNode.GetComponent<nodeScript>().connectedNode.Add(this.gameObject);
            connectingNode.GetComponent<nodeScript>().avaliableConnection -= 1;

        }
        catch { }
    }


    // Use this for initialization
    void Start () {
        ColourFinder(gridProximityStrength);
    }
}
