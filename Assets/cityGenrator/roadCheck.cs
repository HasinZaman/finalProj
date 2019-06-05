using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roadCheck : MonoBehaviour {

    public List<GameObject> connectingNodes = new List<GameObject> { };

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
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
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "road")
        {
            Debug.Log("group 1: " + collisionInfo.gameObject.GetComponent<roadCheck>().connectingNodes[0].GetComponent<nodeScript>().nodeID+", "+ collisionInfo.gameObject.GetComponent<roadCheck>().connectingNodes[1].GetComponent<nodeScript>().nodeID+ "| group 2: "+connectingNodes[0].GetComponent<nodeScript>().nodeID+", "+connectingNodes[1].GetComponent<nodeScript>().nodeID);
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
            //delete self
            else
            {
                connectingNodes[0].GetComponent<nodeScript>().connectedNode.Remove(connectingNodes[1]);
                connectingNodes[1].GetComponent<nodeScript>().connectedNode.Remove(connectingNodes[0]);
                Destroy(this.gameObject);
            }
        }
    }
}
