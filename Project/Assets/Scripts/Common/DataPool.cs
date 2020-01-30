using System.Collections.Generic;
using TOOL;

namespace GameCommon
{
    public class DataPool : U3DSingleton<DataPool> 
    {
        Data rootData = null;
        private void Awake()
        {
            rootData = new Data();
            rootData.Value = "DataRoot";
            rootData.Key = "DataRoot";
        }

        public List<Data> Children { get { return rootData.Children; } }
        public string Key { get { return rootData.Key; } }

        public static Data FindChild(params string[] keys) 
        {
            if (Instance.rootData != null) 
            {
                return Instance.rootData.FindChild(keys);
            }
            else 
            {
                return null;
            }
        }

        public static void DeleteChild(Data data) 
        {
            if(Instance.rootData != null) 
            {
                Instance.rootData.DeleteChildData(data.Key); 
            }
        }

        public static void DeleteChild(string key)
        {
            if (Instance.rootData != null)
            {
                Instance.rootData.DeleteChildData(key);
            }
        }

        public static Data CreateChildData(string key, object value, bool allowExist = false) 
        {
            if(Instance.rootData != null) 
            {
                Data data = Instance.rootData.FindChild(key);
                if (data != null) 
                {
                    if (allowExist)
                    {
                        data.Value = value;
                        return data;
                    }
                    else return null;
                }
                return Instance.rootData.CreatChildData(key,value);
            }
            else {
                return null;
            }
        }
    }

    public class Data : BaseData<Data> { }
}