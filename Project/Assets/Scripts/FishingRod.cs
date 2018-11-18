using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCommon;
using TOOL;
using MoreMountains.NiceVibrations;

public class FishingRod : U3DSingleton<FishingRod>
{
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            cast();
        }
        else if (Input.GetMouseButtonUp(0)) {
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
        EventMachine.SendEvent(EventID.EventID_FishingRod, true);
        GetComponent<Animator>().Play("cast");
    }
    // 收杆
    void leave() {
        EventMachine.SendEvent(EventID.EventID_FishingRod, false);
        GetComponent<Animator>().Play("leave");
    }
    #endregion

    #region Effect
    // 入水
    void OnIntoWater() {
        Common.gIsFishing = true;
        transform.Find("wave_InWater").GetComponent<ParticleSystem>().Play();
        transform.Find("wave").GetComponent<ParticleSystem>().Play();
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }
    // 出水
    void OnOutWater() {
        Common.gIsFishing = false;
        transform.Find("wave_OutWater").GetComponent<ParticleSystem>().Play();
        transform.Find("wave").GetComponent<ParticleSystem>().Stop();
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }
    #endregion
}
