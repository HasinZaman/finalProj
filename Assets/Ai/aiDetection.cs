using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiDetection : MonoBehaviour
{
    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        //parent = this.transform.root.gameObject;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != parent.gameObject.tag)
        {

            if(collision.gameObject.tag != "Untagged")
            {
            //    Debug.Log(parent.gameObject.tag + ":" + collision.gameObject.tag);
            }
            if(collision.gameObject.tag == "npc-Civ")
            {
                Debug.Log("runn");
                parent.GetComponent<civilian>().zombieLocation.Add(collision.gameObject);
            }
            else if(collision.gameObject.tag == "npc-Zomb")
            {
                Debug.Log("Chase");
                parent.GetComponent<zombie>().FOOOOOD.Add(collision.gameObject);
            }
        }
        
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != parent.gameObject.tag)
        {
            if (collision.gameObject.tag != "Untagged")
            {
            //    Debug.Log(parent.gameObject.tag + ":" + collision.gameObject.tag);
            }
            if (collision.gameObject.tag == "npc-Civ")
            {
                Debug.Log("safe");
                parent.GetComponent<civilian>().zombieLocation.Remove(collision.gameObject);
            }
            else if (collision.gameObject.tag == "npc-Zomb")
            {
                Debug.Log("oh no i lost my food");
                parent.GetComponent<zombie>().FOOOOOD.Remove(collision.gameObject);
            }
        }
    }
}
