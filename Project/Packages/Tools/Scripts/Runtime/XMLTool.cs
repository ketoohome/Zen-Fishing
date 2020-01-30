using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace TOOL
{
	/// <summary>
	/// Xml工具，用于存储/读取IData<T>泛型数据
	/// </summary>
	public class XmlTool
	{
		/// <summary>
		/// 将数据转换为xml并存储
		/// </summary>
		/// <param name="data">Data.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void SaveData<T>(T data,string path) where T :class, IData<T>{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.AppendChild(AddData<T>(xmlDoc,data));
			xmlDoc.Save(path);
		}

		/// <summary>
		/// 递归创建(无法写入基础类型意外的数据)
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="xmlDoc">Xml document.</param>
		/// <param name="data">Data.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static XmlElement AddData<T>(XmlDocument xmlDoc,T data) where T :class, IData<T>{
			XmlElement Element = xmlDoc.CreateElement(data.Key);
			Element.SetAttribute("format",data.Value.GetType().ToString());
			Element.SetAttribute("value",data.Value.ToString());

			List<T> children = data.Children;
			for(int i = 0;i<children.Count;i++){
				Element.AppendChild(AddData<T>(xmlDoc,children[i]));
			}
			return Element;
		}

		/// <summary>
		/// 通过路径读取xml格式文件
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="path">Path.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void LoadDataWihtPath<T>(ref T data,string path) where T :class, IData<T>{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(path);
			// 创建根节点
			XmlNode root = xmlDoc.FirstChild;
			data.Key = root.Name;
			string format = root.Attributes["format"].Value;
			string value = root.Attributes["value"].Value;
			DataTool.ParsingFormat<T>(format,value,data);

			// 创建子节点
			foreach(XmlNode node in root.ChildNodes){
				ParsingData<T>(node,data);
			}
		}

		/// <summary>
		/// 通过字符串读取xml格式文件
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="stream">Stream.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void LoadDataWihtStream<T>(ref T data,string stream) where T :class, IData<T>{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(stream);
			// 创建根节点
			XmlNode root = xmlDoc.FirstChild;
			data.Key = root.Name;
			string format = root.Attributes["format"].Value;
			string value = root.Attributes["value"].Value;
			DataTool.ParsingFormat<T>(format,value,data);

			// 创建子节点
			foreach(XmlNode node in root.ChildNodes){
				ParsingData<T>(node,data);
			}
		}


		/// <summary>
		/// 递归解析(无法读取基础类型意外的数据)
		/// </summary>
		/// <returns>The data.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static T ParsingData<T>(XmlNode parentNode,T data) where T :class, IData<T>{

			string format = parentNode.Attributes["format"].Value;
			string value = parentNode.Attributes["value"].Value;
			T childData = DataTool.ParsingFormat<T>(parentNode.Name,format,value,data);
			foreach(XmlNode node in parentNode.ChildNodes){
				ParsingData<T>(node,childData);
			}

			return data;
		}
	}
}