using System.Collections;
using UnityEngine;
using TOOL;
using GameCommon;

/// <summary>
/// 鱼群，创建鱼，指定鱼群的移动方向
/// </summary>
public class SchoolFish : U3DSingleton<SchoolFish> {

    private void Start()
    {
        FishAttribute.GetFishAttributeFromCSV();
        StartCoroutine(OnCreateFish());
    }

    /// <summary>
    /// 创建鱼群
    /// </summary>
    /// <returns>The create fish.</returns>
    IEnumerator OnCreateFish() {
        while (true) {
            // 场景中鱼的数量最多不大雨2条
            if (FindObjectsOfType<Fish>().Length < 2) {
                // 按照稀有程度和季节出现率刷出鱼
                Data fishdata = NumericalTool.RandomChoose<Data>(DataPool.FindChild("FishType"),"rar");
                // 按照季节出现概率判断是否刷出来
                // TODO : 获得当前的季节,获得当前的天气...
                string currSeason = "sce1"; 
                if (NumericalTool.RandomBool(fishdata.FindChild(currSeason).GetValue<uint>())) {
                    Vector3 pos = Vector3.zero;
                    if (GetViewportTargetPos(ref pos,false)) {
                        Fish fish = Instantiate(Resources.Load<Fish>("Fish" + fishdata.GetValue<uint>()), pos, Quaternion.identity);
                        // 给鱼的属性进行赋值
                        fish.SetAttribute(fishdata);
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(1,5));
        }
    }

    /*
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
    */

    /// <summary>
    /// 随机摄像机内的一个目标
    /// </summary>
    /// <returns>The out viewport target.</returns>
    public static bool GetViewportTargetPos(ref Vector3 pos, bool isInViewPort) {
        Vector3 target = GameObject.Find("envrionment").transform.position;
        for(int i = 0;i<20;i++)
        {
            float angle = Random.Range(0,360);
            float temp = Random.Range(2.0f,5.0f);
            target.x += Mathf.Sin(angle)*temp;
            target.z += Mathf.Cos(angle)*temp;
            //target.y += Random.Range(-1.0f,0.0f);
            if (IsInViewport(target, Camera.main) == isInViewPort) {
                pos = target;
                return true;
            }
        }
        return false;
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
}
