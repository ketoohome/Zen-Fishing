using UnityEngine;
using TOOL;
using GameCommon;
/// <summary>
/// 鱼的属性
/// </summary>
public struct FishAttribute
{
    public int _rar; //稀有，1～100，平均1分钟刷出一条鱼，
    public int _spd; //速度，1～100，
    public int _vit; //耐力，1～100，
    public int _dex; //灵敏，1～100，
    public int _int; //智力
    public string _vib; // 震动效果 TODO ：
    public int _len; //长度
    public int _sce_1; //春季出现率
    public int _sce_2; //夏季出现率
    public int _sce_3; //秋季出现率
    public int _sce_4; //冬季出现率

    public static void GetFishAttributeFromCSV() {
        Data fishdata = DataPool.CreateChildData("FishType","FishType");
        //string path = Application.streamingAssetsPath+"/fish_type";
        string path = "fish_type";
        CSVTool.LoadCsv<Data>(fishdata, path);
        foreach(Data data in fishdata.Children) {
            Debug.Log(data.Value.ToString());        
        }
    }
}

