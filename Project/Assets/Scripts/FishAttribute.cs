using UnityEngine;
using TOOL;
using GameCommon;
/// <summary>
/// 鱼的属性
/// </summary>
public struct FishAttribute
{
    public uint _typeid; // 种类ID
    public uint _rar; //稀有，1～100，平均1分钟刷出一条鱼，
    public uint _spd; //速度，1～100，
    public uint _vit; //耐力，1～100，
    public uint _dex; //灵敏，1～100，
    public uint _int; //智力
    public uint _len; //长度
    public uint _sce_1; //春季出现率
    public uint _sce_2; //夏季出现率
    public uint _sce_3; //秋季出现率
    public uint _sce_4; //冬季出现率

    /// <summary>
    /// 加载鱼类表格
    /// </summary>
    public static void GetFishAttributeFromCSV() {
        Data fishdata = DataPool.CreateChildData("FishType","FishType");
        //string path = Application.streamingAssetsPath+"/fish_type";
        string path = "fish_type";
        CSVTool.LoadCsv<Data>(fishdata, path);
    }

    public static FishAttribute CreateFishAttribute(Data data) {
        FishAttribute att = new FishAttribute();
        att._typeid = data.GetValue<uint>();
        att._rar = data.FindChild("rar").GetValue<uint>();
        att._spd = data.FindChild("spd").GetValue<uint>();
        att._vit = data.FindChild("vit").GetValue<uint>();
        att._dex = data.FindChild("dex").GetValue<uint>();
        att._int = data.FindChild("int").GetValue<uint>();
        att._len = data.FindChild("len").GetValue<uint>();
        att._sce_1 = data.FindChild("sce1").GetValue<uint>();
        att._sce_2 = data.FindChild("sce2").GetValue<uint>();
        att._sce_3 = data.FindChild("sce3").GetValue<uint>();
        att._sce_4 = data.FindChild("sce4").GetValue<uint>();
        return att;
    }
}

