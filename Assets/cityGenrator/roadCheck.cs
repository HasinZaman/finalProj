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
       // GameObject roadMesh = Instantiate(Resources.Load("road2"), this.transform.position, Quaternion.identity) as GameObject;

       // Vector3 scale = roadMesh.transform.localScale;

       // scale.z = ((connectingNodes[0].transform.position - connectingNodes[1].transform.position) / 2).magnitude; ;

       // roadMesh.transform.localScale = scale;

       // Destroy(this.gameObject);
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
