using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TOOL
{
	/// <summary>
	/// CSV表格工具
	/// </summary>
	public class CSVTool {

		/// <summary>
		/// 加载CSV表格，并生成一个数据接口的实例
		/// </summary>
		/// <returns>The csv.</returns>
		/// <param name="path">Path.</param>
		/// <param name="data">Data.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T LoadCsv<T>(T data,string path) where T : class, IData<T> 
		{
			if (path.Length >= 0)
			{
				TextAsset text = Resources.Load<TextAsset>(path);
				if (text != null){
					data.Value = text.name;
					return ParsingCsv<T>(data,text.text);
				}
			}
			
			return null;
		}

		/// <summary>
		/// 解析CSV内容
		/// </summary>
		/// <returns>The csv.</returns>
		/// <param name="content">Content.</param>
		/// <param name="data">Data.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static T ParsingCsv<T>(T data,string content) where T : class, IData<T>
		{
			
			/* CSV文件必须遵循以下格式
			 * --------------------------------------------------------------------------------
			 *	ID			|	Type1			Type2			Type3			Type4	...
			 *---------------------------------------------------------------------------------
			 *	uint		|	format			format			format			format
			 *---------------------------------------------------------------------------------
			 *  00001		|	xxxx			xxxx			xxxx			xxxx
			 *---------------------------------------------------------------------------------
			 *  00002		|	xxxx			xxxx			xxxx			xxxx
			 *---------------------------------------------------------------------------------
			 * ...
			 */

			// 消除回车键
			if (content.EndsWith("\n")){
				content = content.Substring(0, content.Length - 1);
			}

			// 将字符串分组
			string[] line = content.Split(new string[]{"\r\n"}, StringSplitOptions.None);
			string[] names = line[0].Split (","[0]);
			string[] formats = line[1].Split(","[0]);
			for(int i = 2; i<line.Length; i++){
				string[] values = line[i].Split(","[0]);
				T node = data.CreatChildData(values[0].ToString(),uint.Parse(values[0]));
				for(int j = 1; j< values.Length; j++){
					DataTool.ParsingFormat<T>(names[j],formats[j],values[j],node);
					//GameCommon.GameCommon.Log(values[j]);
				}
			}
			return null;
		}
	}
}