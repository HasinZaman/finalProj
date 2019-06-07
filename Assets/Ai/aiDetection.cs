using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiDetection : MonoBehaviour
{
    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent.gameObject;
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
             //   Debug.Log("runn");
                parent.gameObject.GetComponent<zombie>().FOOOOOD.Add(collision.gameObject);
            }
            else if(collision.gameObject.tag == "npc-Zomb")
            {
             //   Debug.Log("Chase");
                parent.gameObject.GetComponent<civilian>().zombieLocation.Add(collision.gameObject);
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
            if (collision.gameObject.tag == "npc-Zomb")
            {
            //    Debug.Log("safe");
                parent.gameObject.GetComponent<civilian>().zombieLocation.Remove(collision.gameObject);
            }
            else if (collision.gameObject.tag == "npc-Civ")
            {
             //   Debug.Log("oh no i lost my food");
                parent.gameObject.GetComponent<zombie>().FOOOOOD.Remove(collision.gameObject);
            }
        }
    }
}
