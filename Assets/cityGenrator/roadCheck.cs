using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roadCheck : MonoBehaviour {

    public List<GameObject> connectingNodes = new List<GameObject> { };
    
    //creates a node at where two nodes collide
    private void intersectionNodeCreator(Vector3 point, List<GameObject> nodes)
    {
        GameObject temp = Instantiate(Resources.Load("streetNode"), point , Quaternion.identity) as GameObject;

        //assings variables
        temp.GetComponent<nodeScript>().gridProximityStrength = 0;
        temp.GetComponent<nodeScript>().gridID = -1;
        temp.GetComponent<nodeScript>().nodeID = 0;
        temp.GetComponent<nodeScript>().XLocation = 0;
        temp.GetComponent<nodeScript>().YLocation = 0;
        temp.GetComponent<nodeScript>().minNodeDistance = 0;
        temp.GetComponent<nodeScript>().avaliableConnection = 0;

        for (int i1 = 0; i1 < nodes.Count; i1++)
        {
            nodeConnection connection = new nodeConnection();

            connection.nodes[0] = this.gameObject;
            connection.nodes[1] = nodes[i1].gameObject;

            temp.GetComponent<nodeScript>().finalistNode.Add(connection);
            nodes[i1].GetComponent<nodeScript>().finalistNode.Add(connection);
        }

        
    }


    //replaces road with a road mesh
    public void roadMetamorphs()
    {
        GameObject roadMesh = Instantiate(Resources.Load("road2"), this.transform.position, Quaternion.identity) as GameObject;

        Vector3 scale = roadMesh.transform.localScale;

        float nodeDist = Convert.ToSingle(connectingNodes[0].GetComponent<nodeScript>().nodeDist(connectingNodes[0], connectingNodes[1]));

        float changeRatio = nodeDist;

        scale.z = ((connectingNodes[0].transform.position - connectingNodes[1].transform.position) / 3.75f).magnitude;

        Debug.Log(nodeDist + " " + changeRatio + scale.z);

        roadMesh.transform.localScale = scale;

        roadMesh.transform.rotation = Quaternion.FromToRotation(Vector3.forward, connectingNodes[0].transform.position - connectingNodes[1].transform.position);

        for(int i1 =0; i1< connectingNodes.Count; i1++)
        {
            if (connectingNodes[i1].gameObject.GetComponentInChildren<Renderer>().enabled)
            {
                connectingNodes[i1].gameObject.GetComponentInChildren<Renderer>().enabled = false;
            }
        }
        

        Destroy(this.gameObject);
    }

    //checks if roads are colliding with another road
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "road")
        {
            //delete other node
            if (connectingNodes[0].GetComponent<nodeScript>().outerNode && connectingNodes[1].GetComponent<nodeScript>().outerNode)
            {
                collisionInfo.gameObject.GetComponent<roadCheck>().connectingNodes[1].GetComponent<nodeScript>().connectedNode.Remove(collisionInfo.gameObject.GetComponent<roadCheck>().connectingNodes[0]);
                collisionInfo.gameObject.GetComponent<roadCheck>().connectingNodes[0].GetComponent<nodeScript>().connectedNode.Remove(collisionInfo.gameObject.GetComponent<roadCheck>().connectingNodes[1]);
                Destroy(collisionInfo.gameObject);
            }
            //delete self
            else if(collisionInfo.gameObject.GetComponent<roadCheck>().connectingNodes[0].GetComponent<nodeScript>().outerNode && collisionInfo.gameObject.GetComponent<roadCheck>().connectingNodes[1].GetComponent<nodeScript>().outerNode)
            {
                connectingNodes[0].GetComponent<nodeScript>().connectedNode.Remove(connectingNodes[1]);
                connectingNodes[1].GetComponent<nodeScript>().connectedNode.Remove(connectingNodes[0]);
                Destroy(this.gameObject);
            }
            //creates a node at the collision point and connects both nodes to the new node(plan)
            //removes self
            else
            {

                connectingNodes[0].GetComponent<nodeScript>().connectedNode.Remove(connectingNodes[1]);
                connectingNodes[1].GetComponent<nodeScript>().connectedNode.Remove(connectingNodes[0]);
                Destroy(this.gameObject);
            }
        }
    }
}
