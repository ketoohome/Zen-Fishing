using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOOL;

public class SchoolFish : U3DSingleton<SchoolFish> {

    /// <summary>
    /// 获取一个目标位置
    /// </summary>
    /// <returns></returns>
    public static TargetPoint GetViewportTarget() {
        TargetPoint target = null;
        TargetPoint[] points = GameObject.FindObjectsOfType<TargetPoint>();
        int num = 0;
        while (target == null && points.Length > 0 && num <10) {
            target = points[Random.Range(0, points.Length)];
            target = IsInViewport(target.transform.position, Camera.main) ? target : null;
            if(target != null) target = (!Common.gIsFishing && target.name == "hook") ? null : target;
            num++;
        }
        return target;
    }

    /// <summary>
    /// 获得一个摄像机内的目标
    /// </summary>
    /// <returns></returns>
    public static TargetPoint GetOutViewportTarget()
    {
        TargetPoint target = null;
        TargetPoint[] points = GameObject.FindObjectsOfType<TargetPoint>();
        int num = 0;
        while (target == null && points.Length > 0 && num <10) { 
            target = points[Random.Range(0, points.Length)];
            target = IsInViewport(target.transform.position, Camera.main) ? null : target;
            if (target != null) target = (!Common.gIsFishing && target.name == "hook") ? null : target;
            num++;
        }
        return target;
    }

    /// <summary>
    /// 获取一个目标
    /// </summary>
    /// <returns></returns>
    public static TargetPoint GetTarget()
    {
        TargetPoint target = null;
        TargetPoint[] points = GameObject.FindObjectsOfType<TargetPoint>();
        target = points[Random.Range(0, points.Length)];
        int num = 0;
        while (!Common.gIsFishing && target.name == "hook" && num < 10) {
            target = points[Random.Range(0, points.Length)];
            num++;
        }
        return target;
    }

    /// <summary>
    /// 目标位置是否在视野范围内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static bool IsInViewport(Vector3 pos,Camera camera) {
        Vector3 viewpos = camera.WorldToViewportPoint(pos);
        Vector3 dir = (pos - camera.transform.position).normalized;
        float dot = Vector3.Dot(camera.transform.forward, dir);
        return (dot > 0 && viewpos.x >= 0 && viewpos.x <= 1 && viewpos.y >= 0 && viewpos.y <= 1);
    }


    void CreateFish() { }
}
