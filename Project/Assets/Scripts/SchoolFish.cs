using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolFish : MonoBehaviour {
    public static SchoolFish It;

    private void Awake()
    {
        It = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 获取一个目标位置
    /// </summary>
    /// <returns></returns>
    public static TargetPoint GetViewportTarget() {
        TargetPoint target = null;
        TargetPoint[] points = GameObject.FindObjectsOfType<TargetPoint>();
        while (target == null && points.Length > 0) {
            target = points[Random.Range(0, points.Length)];
            target = IsInViewport(target.transform.position, Camera.main) ? target : null;
        }
        return target;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static TargetPoint GetOutViewportTarget()
    {
        TargetPoint target = null;
        TargetPoint[] points = GameObject.FindObjectsOfType<TargetPoint>();
        while (target == null && points.Length > 0) {
            target = points[Random.Range(0, points.Length)];
            target = IsInViewport(target.transform.position, Camera.main) ? null : target;
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
