using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class civilian : MonoBehaviour
{

    public GameObject target;
    public GameObject lastNode;

    public int speed = 10;

    public List<GameObject> zombieLocation = new List<GameObject> {};
    // 0 = no object
    // 1 = random
    // 2 = run
    public int aiMode = 0;

    private bool targetReached()
    {
        RaycastHit ObstacleHit;
        if (target)    // make sure we have an objective first or we get a dirty error.
            return (Physics.Raycast(this.transform.position, target.transform.position - this.transform.position, out ObstacleHit, Mathf.Infinity) && ObstacleHit.transform != this.transform && ObstacleHit.transform == target.transform);
        else
            return false;
    }

    private GameObject randomTarget(List<GameObject> nodeConnections)
    {

        List<GameObject> temp = nodeConnections;
        
        temp.Remove(lastNode);

        System.Random rnd = new System.Random();

        int r = rnd.Next(temp.Count);

        return temp[r];
    }

    private GameObject runAi(List<GameObject> nodeConnections)
    {
        List<GameObject> possibleOptions = nodeConnections;

        List<Quaternion> zombieAngle = new List<Quaternion> { };

        for (int i1 = 0; i1 < zombieLocation.Count; i1++)
        {

            Quaternion angle = Quaternion.FromToRotation(Vector3.forward, zombieLocation[i1].transform.position - this.transform.position);

            zombieAngle.Add(angle);

        }

        for (int i1 = possibleOptions.Count-1; i1 >= 0; i1--)
        {
            if (zombieAngle.Any(angle => angle == Quaternion.FromToRotation(Vector3.forward, possibleOptions[i1].transform.position - this.transform.position)))
            {
                possibleOptions.Remove(possibleOptions[i1]);
            }
                 
        }

        return randomTarget(possibleOptions);
    }


    bool forwardSafe()
    {
        Quaternion targetAngle = Quaternion.FromToRotation(Vector3.up, target.transform.position - this.transform.position);

        for (int i1 = 0; i1 < zombieLocation.Count; i1++)
        {
            if(targetAngle == Quaternion.FromToRotation(Vector3.up, zombieLocation[i1].transform.position - this.transform.position))
            {
                return true;
            }
        }

        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(aiMode == 0)
        {
            if (zombieLocation.Count > 0)
            {
                target = runAi(lastNode.GetComponent<nodeScript>().connectedNode);
            }
            else
            {
                target = randomTarget(lastNode.GetComponent<nodeScript>().connectedNode);
            }
            aiMode = 1;
        }
        else if(aiMode == 1)
        {
            if (targetReached())
            {
                lastNode.gameObject.GetComponentInChildren<Renderer>().enabled = false;
                
                aiMode = 0;
                lastNode = target.gameObject;
                Vector3 temp = lastNode.transform.position;
                temp.y = 1.5f;
                this.transform.position = temp;
                lastNode.gameObject.GetComponentInChildren<Renderer>().enabled = true;
            }
            //checking zombie
            if (forwardSafe())
            {
                target = lastNode;
            }
            else
            {
                target.gameObject.GetComponentInChildren<Renderer>().enabled = true;
                Quaternion temp1 = this.transform.rotation;
                Quaternion temp2 = Quaternion.LookRotation(target.transform.position-this.transform.position);
                

                temp2.x = 0;
                temp2.z = 0;
                

                this.transform.rotation = temp2;

                this.transform.position = this.transform.position + transform.forward * speed * Time.deltaTime;
            }
        }
    }


}
