using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class nodeScript : MonoBehaviour {

    public int nodeID = 0;
    public int gridID = 0;
    public int XLocation = 0;
    public int YLocation = 0;
    public int minNodeDistance = 0;
    public int maxNodeDistance = 0;
    public int maxConnection = 0;
    public double gridProximityStrength = 0;



    public bool existance = true;

    public List<GameObject> connectingRoads = new List<GameObject> { };
        
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
                Destroy(deletedCollider[0].gameObject);
            }
        }
    }


    private Collider[] priortyConections(List<string> heiarcy, Collider[] nodes)
    {
        Collider[] temp = nodes;

        

        for(int i = 0; i < heiarcy.Count; i++)
        {

        }


        return nodes;
    }

    public void roadConection()
    {
        //finds all closest nodes
        Collider[] connectingNodes = Physics.OverlapSphere(this.transform.position, maxNodeDistance);

        List<Collider> allConnectingRoads = new List<Collider> { };

        for (int i1 = 0; i1 < connectingNodes.Length; i1++)
        {
            try
            {
                nodeScript temp = connectingNodes[i1].gameObject.GetComponent<nodeScript>();
                if (nodeID != temp.nodeID && temp.existance && existance)
                {
                    
                    //creates road

                    Vector3 postion = (temp.transform.position - this.transform.position) / 2.0f + this.transform.position;
                    
                    GameObject road = Instantiate(Resources.Load("road"), postion, Quaternion.identity) as GameObject;

                    Vector3 scale = road.transform.localScale;      // Scale it
                    scale.y = ((this.transform.position - temp.transform.position)/2).magnitude;
                    road.transform.localScale = scale;

                    // Rotate it
                    road.transform.rotation = Quaternion.FromToRotation(Vector3.up, this.transform.position - temp.transform.position);

                    connectingRoads.Add(road);
                    temp.connectingRoads.Add(road);
                    road.GetComponent<roadCheck>().connectingNodes.Add(this.gameObject);
                    road.GetComponent<roadCheck>().connectingNodes.Add(connectingNodes[i1].gameObject);

                }
            }
            catch { }
        }
    }

    // Use this for initialization
    void Start () {
        ColourFinder(gridProximityStrength);

        

       

    }

    private void OnDestroy()
    {
        Debug.Log("destroyed");
    }

    // Update is called once per frame
    //void Update () {

    //}
}
