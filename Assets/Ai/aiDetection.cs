using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiDetection : MonoBehaviour
{
    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.root.gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("zommbie AAAAA");
        if (parent.tag == "npc-civ")
        {
            parent.GetComponent<civilian>().zombieLocation.Add(collision.gameObject);
        }
        else
        {
            parent.GetComponent<zombie>().FOOOOOD.Add(collision.gameObject);
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {

        Debug.Log("safe");
        if (parent.tag == "npc-civ")
        {
            parent.GetComponent<civilian>().zombieLocation.Remove(collision.gameObject);
        }
        else
        {
            parent.GetComponent<zombie>().FOOOOOD.Remove(collision.gameObject);
        }
    }
}
