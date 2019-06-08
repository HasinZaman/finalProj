using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class movementObservationScript : MonoBehaviour {

    //delcares all local variables
    private float speed = 30;
    
	
	// checks if the camera needs to move or not
	void Update () {

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += Vector3.back * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift)){
            transform.position += Vector3.down * speed * Time.deltaTime;
        }else if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
    }
}
