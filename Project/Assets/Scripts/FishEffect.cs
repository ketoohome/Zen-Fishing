using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEffect : MonoBehaviour {

    Vector3 _TargetScale, _CurrScale, _Scale;
    Transform _Fish;
    private void Start()
    {
        _Fish = transform.GetChild(0);
        _Scale = _Fish.localScale;
        _TargetScale = _Scale;
        _CurrScale = _Scale;
    }

    // Update is called once per frame
    void Update() {
        if (Camera.main == null) return;
        transform.LookAt(Camera.main.transform);

        updateSize();
    }

    void updateSize()
    {
        _CurrPoint = Camera.main.WorldToScreenPoint(transform.position);
        if (_CurrPoint.x - _OldPoint.x > 1) _TargetScale = new Vector3(-_Scale.x, _Scale.y, _Scale.z);
        else if(_CurrPoint.x - _OldPoint.x < -1) _TargetScale = new Vector3(_Scale.x, _Scale.y, _Scale.z);
        _OldPoint = _CurrPoint;

        _CurrScale = Vector3.Lerp(_CurrScale,_TargetScale,Time.deltaTime*5);
        _Fish.localScale = _CurrScale;

    } Vector3 _OldPoint, _CurrPoint;
    
}
