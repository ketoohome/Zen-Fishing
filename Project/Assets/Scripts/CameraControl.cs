using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    float _Angle, _currAngle;
    Vector2 _mousePos, _oldMousePos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (Input.GetMouseButtonDown(0)) {
            _mousePos = Input.mousePosition;
            _oldMousePos = _mousePos;
        }
        if (Input.GetMouseButton(0))
        {
            _mousePos = Input.mousePosition;
            _Angle = (_mousePos - _oldMousePos).x *0.01f;
        }
        if (Input.GetMouseButtonUp(0)) {
            _Angle = 0;
        }
        */
        if (Input.GetMouseButtonDown(0))
        {
            _mousePos = Input.mousePosition;
            _oldMousePos = _mousePos;
        }
        if (Input.GetMouseButton(0))
        {
            _mousePos = Input.mousePosition;
            _Angle = (_mousePos - _oldMousePos).x * 0.02f;
            _oldMousePos = _mousePos;
        }

        _currAngle = Mathf.Lerp(_currAngle,_Angle,Time.deltaTime*2);
        transform.Rotate(0,_currAngle,0);
    }
}
