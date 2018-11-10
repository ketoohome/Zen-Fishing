using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour {

    public float mTargetAccelerated = 0;
    public Vector3 mOldPos;
	
	// Update is called once per frame
	void Update () {
        mTargetAccelerated = Vector3.Distance(transform.position,mOldPos) * Time.deltaTime;
        mOldPos = transform.position;
    }
}
