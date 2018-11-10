using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            cast();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leave();
        }

        float angle = transform.rotation.eulerAngles.y;
        speed = angle - oldangle;
        speed = (Mathf.Abs(speed) < 300) ? speed : oldspeed;
        oldspeed = speed;
        oldangle = angle;
        currspeed = Mathf.Lerp(currspeed, speed,Time.deltaTime*2);
        transform.localRotation = Quaternion.Euler(0,0,-currspeed * 10);
    } float speed,oldspeed, currspeed, oldangle;

    #region Action
    // 甩杆
    void cast() {
        GetComponent<Animator>().Play("cast");
    }

    // 收杆
    void leave() {
        GetComponent<Animator>().Play("leave");
    }
    #endregion
    // 入水
    void OnIntoWater() {
        transform.Find("wave_InWater").GetComponent<ParticleSystem>().Play();
        transform.Find("wave").GetComponent<ParticleSystem>().Play();
    }
    // 出水
    void OnOutWater() {
        transform.Find("wave_OutWater").GetComponent<ParticleSystem>().Play();
        transform.Find("wave").GetComponent<ParticleSystem>().Stop();
    }
}
