using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class nodeConnection
{
    public GameObject[] nodes = new GameObject[2];

    public int connectionQuality;
}

public class nodeScript : MonoBehaviour {

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

    //colour decider

    void ColourFinder(double srength)
    {
        MeshRenderer gameObjectRenderer = this.GetComponent<MeshRenderer>();

        Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"));

        double[] colourRGB = { Convert.ToInt32(-2.1 * Math.Pow(srength, 2) + 1), Convert.ToInt32(-10 * Math.Pow(srength-1, 2) + 1), Convert.ToInt32(-10 * Math.Pow(srength - 0.6, 2) + 1) };
        for(int colour = 0; colour < colourRGB.Length; colour++)
        {
            if (colourRGB[colour] < 0)
            {
                colourRGB[colour] = 0;
            }
        }
        newMaterial.color = new Color((float) colourRGB[0], (float)colourRGB[1], (float)colourRGB[2]);
        gameObjectRenderer.material = newMaterial;
    }
    public void nodeCheck()
    {
        //gets removes all nodes that are too close
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, minNodeDistance);

        List<Collider> deletedCollider = new List<Collider> { };

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
    //format heiarcy (nodeType, priortys inorder of| next node, prytorties|...)
    //example EOgcPgc|MC
    //Og = other grid, Pg = parent grid, Hs = highest strength, ls = lowest strength, r = random  

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
    public List<GameObject> strongNodeStrengthFilter(List<GameObject> nodePool)
    {
        List<GameObject> newOrderNodes = nodePool.OrderBy(node => node.GetComponent<nodeScript>().gridProximityStrength).ToList();

        return newOrderNodes;
    }
    public List<GameObject> weakNodeStrengthFilter(List<GameObject> nodePool)
    {
        List<GameObject> newOrderNodes = nodePool.OrderBy(node => node.GetComponent<nodeScript>().gridProximityStrength).Reverse().ToList();
        return newOrderNodes;
    }
    //checks if the conection is possible
    public int commonNodeChecker(GameObject nodeInput,int nodeGroup)
    {
        int connectionQuality = -1;
        List<List<GameObject>> temp = nodeInput.GetComponent<nodeScript>().possibleConnections;
        for (int i1 = 0; i1 < temp.Count; i1++)
        {
            if (temp[i1].Any(node => node == this.gameObject))
            {
                connectionQuality = nodeGroup + i1;
                return connectionQuality;
            }
        }
        return connectionQuality;
    }
    
    public void commonNode()
    {
        List<GameObject> uncommonNode = new List<GameObject> { };
        for (int i1 = 0; i1<possibleConnections.Count; i1++)
        {
            for (int i2 = 0; i2 < possibleConnections[i1].Count; i2++)
            {
                int temp = commonNodeChecker(possibleConnections[i1][i2], i1);
                if (temp != -1)
                {
                    nodeConnection connection = new nodeConnection();
                    connection.connectionQuality = temp;
                    connection.nodes[0] = this.gameObject;
                    connection.nodes[1] = possibleConnections[i1][i2];
                    
                    //.Add(connection);
                    possibleConnections[i1][i2].GetComponent<nodeScript>().finalistNode.Add(connection);
                }
                else
                {
                    uncommonNode.Add(possibleConnections[i1][i2]);
                }
            }
        }
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

            connectedNode.Add(connectingNode);
            avaliableConnection -= 1;
            connectingNode.GetComponent<nodeScript>().connectedNode.Add(this.gameObject);
            connectingNode.GetComponent<nodeScript>().avaliableConnection -= 1;

        }
        catch { }
    }
    public void roadConection(Collider[] nodes)
    {
        //finds all closest nodes
        //Collider[] connectingNodes = Physics.OverlapSphere(this.transform.position, maxNodeDistance);
        //list<collider> allconnectingroads = new list<collider> { };
        //gameobject temp;
        //for (int i1 = 0; i1 < nodes.length; i1++)
        //{
        //    try
        //    {
        //        nodescript temp = nodes[i1].gameobject.getcomponent<nodescript>();
        //        if (nodeid != temp.nodeid && temp.existance && existance && connectingnodes.any(p => p.gameobject == nodes[i1]) && nodes[i1].gameobject.getcomponent<nodescript>().connectingnodes.any(p => p.gameobject == this.gameobject))
        //        {

        //            creates road

        //            vector3 postion = (temp.transform.position - this.transform.position) / 2.0f + this.transform.position;

        //            gameobject road = instantiate(resources.load("road"), postion, quaternion.identity) as gameobject;

        //            vector3 scale = road.transform.localscale;// scale it
        //            scale.y = ((this.transform.position - temp.transform.position) / 2).magnitude;
        //            road.transform.localscale = scale;

        //            rotate it
        //            road.transform.rotation = quaternion.fromtorotation(vector3.up, this.transform.position - temp.transform.position);

        //            connectingroads.add(road);
        //            temp.connectingroads.add(road);
        //            road.getcomponent<roadcheck>().connectingnodes.add(this.gameobject);
        //            road.getcomponent<roadcheck>().connectingnodes.add(nodes[i1].gameobject);

        //            nodes[i1].getcomponent<nodescript>().connectingnodes.add(this.gameobject);

        //            connectingnodes.add(nodes[i1].gameobject);


        //        }
        //    }
        //    catch { }
        //}

    }
    // Use this for initialization
    void Start () {
        ColourFinder(gridProximityStrength);
    }

    private void OnDestroy()
    {
        //Debug.Log("destroyed");
    }
    // Update is called once per frame
    //void Update () {

    //}
}
