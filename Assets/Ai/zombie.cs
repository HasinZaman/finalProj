using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class zombie : MonoBehaviour
{

    //declares varaibles
    public GameObject target;
    public GameObject lastNode;

    public int speed = 15;

    //FOOOOD is just like zombieLocation in civilan.cs but for civilian location
    public List<GameObject> FOOOOOD = new List<GameObject> { };
    // 0 = no object
    // 1 = random
    // 2 = run
    public int aiMode = 0;

    //checks if any the foood index is null
    private void foodCheck()
    {
        for (int i1 = 0; i1 < FOOOOOD.Count; i1++)
        {
            if (FOOOOOD[i1] == null)
            {
                FOOOOOD.Remove(FOOOOOD[i1]);
            }
        }
    }

    //checks if target has been reached
    private bool targetReached()
    {
        RaycastHit ObstacleHit;
        if (target)
            return (Physics.Raycast(this.transform.position, target.transform.position - this.transform.position, out ObstacleHit, Mathf.Infinity) && ObstacleHit.transform != this.transform && ObstacleHit.transform == target.transform);
        else
            return false;
    }

    //selects a random node form a list
    private GameObject randomTarget(List<GameObject> nodeConnections)
    {

        List<GameObject> temp = nodeConnections;

        temp.Remove(lastNode);

        System.Random rnd = new System.Random();

        int r = rnd.Next(temp.Count);

        return temp[r];
    }

    //selects a path with food on it
    private GameObject chaseAi(List<GameObject> nodeConnections)
    {
        List<GameObject> possibleOptions = new List<GameObject> { };

        List<Quaternion> zombieAngle = new List<Quaternion> { };

        for (int i1 = 0; i1 < FOOOOOD.Count; i1++)
        {

            Quaternion angle = Quaternion.LookRotation(FOOOOOD[i1].transform.position - this.transform.position);

            zombieAngle.Add(angle);

        }

        for (int i1 = possibleOptions.Count - 1; i1 >= 0; i1--)
        {
            if (zombieAngle.Any(angle => angle == Quaternion.LookRotation(possibleOptions[i1].transform.position - this.transform.position)))
            {
                possibleOptions.Add(possibleOptions[i1]);
            }

        }
        if (possibleOptions.Count == 0)
        {
            randomTarget(nodeConnections);
        }

        return randomTarget(possibleOptions);
    }
    
    //on collision with civ
    //civ is converted into another zombie
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != this.gameObject.tag)
        {
            if (collisionInfo.gameObject.tag == "npc-Civ")
            {
                Debug.Log("converted");
                GameObject temp = collisionInfo.gameObject;

                GameObject infected = Instantiate(Resources.Load("zombie"), temp.gameObject.transform.position, Quaternion.identity) as GameObject;

                infected.GetComponent<zombie>().lastNode = temp.gameObject.GetComponent<civilian>().lastNode;

                Destroy(temp);
            }
        }
    }

    void Update()
    {
        //gets rid of invalid values in FOOOOOD
        foodCheck();
        try
        {
            
            //checks if a new target needs to found
            if (aiMode == 0)
            {
                if (FOOOOOD.Count > 0)
                {
                    target = chaseAi(lastNode.GetComponent<nodeScript>().connectedNode);
                }
                else
                {
                    target = randomTarget(lastNode.GetComponent<nodeScript>().connectedNode);
                }
                aiMode = 1;
            }
            else if (aiMode == 1)
            {
                //checks target is reached
                if (targetReached())
                {
                    
                    aiMode = 0;
                    lastNode = target.gameObject;
                    Vector3 temp = lastNode.transform.position;
                    temp.y = 1.5f;
                    this.transform.position = temp;
                }
                //moves the zombie forward
                else
                {
                    Quaternion temp1 = this.transform.rotation;
                    Quaternion temp2 = Quaternion.LookRotation(target.transform.position - this.transform.position);


                    temp2.x = 0;
                    temp2.z = 0;


                    this.transform.rotation = temp2;

                    this.transform.position = this.transform.position + transform.forward * speed * Time.deltaTime;
                }
            }
        }
        catch
        {
        }
    }


}
