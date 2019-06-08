using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiDetection : MonoBehaviour
{
    public GameObject parent;
    
    //gets parrent
    void Start()
    {
        parent = this.transform.parent.gameObject;
    }

    void OnCollisionEnter(Collision collision)
    {
        // on collision check if civ or zomb enters visual range
        if (collision.gameObject.tag != parent.gameObject.tag)
        {
            if(collision.gameObject.tag == "npc-Civ")
            {
                parent.gameObject.GetComponent<zombie>().FOOOOOD.Add(collision.gameObject);
            }
            else if(collision.gameObject.tag == "npc-Zomb")
            {
                parent.gameObject.GetComponent<civilian>().zombieLocation.Add(collision.gameObject);
            }
        }
        
    }
    void OnCollisionExit(Collision collision)
    {
        //removes zomb and civ from vission
        if (collision.gameObject.tag != parent.gameObject.tag)
        {
            if (collision.gameObject.tag == "npc-Zomb")
            {
                parent.gameObject.GetComponent<civilian>().zombieLocation.Remove(collision.gameObject);
            }
            else if (collision.gameObject.tag == "npc-Civ")
            {
                parent.gameObject.GetComponent<zombie>().FOOOOOD.Remove(collision.gameObject);
            }
        }
    }
}
