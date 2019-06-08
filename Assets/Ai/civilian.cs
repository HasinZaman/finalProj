using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class civilian : MonoBehaviour
{

    // declares varriables
    public GameObject target;
    public GameObject lastNode;

    public int speed = 10;

    //zombie location is a list of every zombie that can be seen
    public List<GameObject> zombieLocation = new List<GameObject> {};
    // 0 = no object
    // 1 = random
    // 2 = run

    public int aiMode = 0;

    //checks if the civ has reached teh target
    private bool targetReached()
    {
        RaycastHit ObstacleHit;
        if (target)
            return (Physics.Raycast(this.transform.position, target.transform.position - this.transform.position, out ObstacleHit, Mathf.Infinity) && ObstacleHit.transform != this.transform && ObstacleHit.transform == target.transform);
        else
            return false;
    }

    //picks a random node from a list of possibe connections at a intersection
    private GameObject randomTarget(List<GameObject> nodeConnections)
    {

        List<GameObject> temp = nodeConnections;
        
        temp.Remove(lastNode);

        System.Random rnd = new System.Random();

        int r = rnd.Next(temp.Count);

        return temp[r];
    }

    //runai finds safe nodes and picks a safe node as a target
    private GameObject runAi(List<GameObject> nodeConnections)
    {
        List<GameObject> possibleOptions = nodeConnections;

        List<Quaternion> zombieAngle = new List<Quaternion> { };

        for (int i1 = 0; i1 < zombieLocation.Count; i1++)
        {
            Quaternion angle = Quaternion.LookRotation(zombieLocation[i1].transform.position - this.transform.position);

            zombieAngle.Add(angle);
            

        }

        for (int i1 = possibleOptions.Count-1; i1 >= 0; i1--)
        {
            if (zombieAngle.Any(angle => angle == Quaternion.LookRotation(possibleOptions[i1].transform.position - this.transform.position)))
            {
                possibleOptions.Remove(possibleOptions[i1]);
            }
                 
        }
        
        return randomTarget(possibleOptions);
    }

    //checks if the current target is safe
    bool forwardSafe()
    {
        Quaternion targetAngle = Quaternion.LookRotation(target.transform.position - this.transform.position);

        for (int i1 = 0; i1 < zombieLocation.Count; i1++)
        {
            if (targetAngle == Quaternion.LookRotation(zombieLocation[i1].transform.position - this.transform.position))
            {
                return true;
            }
            
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        //if ai mode  deals with ai tree
        if(aiMode == 0)
        {
            //finds a new target
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
            //checks if the target is reached
            if (targetReached())
            {
                
                aiMode = 0;
                lastNode = target.gameObject;
                Vector3 temp = lastNode.transform.position;
                temp.y = 1.5f;
                this.transform.position = temp;
            }

            //checking if there is a zombie infornt
            if (forwardSafe())
            {
                target = lastNode;
            }
            //moves the npc forward
            else
            {
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
