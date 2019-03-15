using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;


namespace Coldairarrow.DotNettyRPC
{
	/// <summary>
	/// 解析XML格式化配置文件
	/// </summary>
	public class ServiceTypesConfig
	{
		/// <summary>
		/// 解析XML格式化配置文件
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<ServiceTypeConfig> ParseXmlConfig(string xmlCfgFile)
		{
			if (File.Exists(xmlCfgFile))
			{
				return XElement.Load(xmlCfgFile)
					.XPathSelectElement("//dotNettyRpc")?.Elements()
					.Select(x =>
						new ServiceTypeConfig
						{
							Interface = x.Attribute("interface")?.Value,
							Service = Type.GetType(x.Attribute("service")?.Value ?? "")
						});
			}

			return new List<ServiceTypeConfig>();
		}

		/// <summary>
		/// 解析XML格式配置文件中的服务配置信息，并注册到server中。
		/// </summary>
		/// <param name="server"></param>
		/// <param name="xmlCfgFile"></param>
		public static void RegisterServices(RPCServer server, string xmlCfgFile = null)
		{
			if (string.IsNullOrEmpty(xmlCfgFile))
			{
				xmlCfgFile = Assembly.GetExecutingAssembly().Location + ".config";

				if (!File.Exists(xmlCfgFile))
				{
					xmlCfgFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
						"dotNettyRPC.config");
				}
			}

			foreach (var s in ParseXmlConfig(xmlCfgFile))
			{
				server.RegisterService(s.Interface, s.Service);
			}
		}
	}

	/// <summary>
	/// 服务配置信息
	/// </summary>
	public class ServiceTypeConfig
	{
		/// <summary>
		/// 接口全名，不含程序集名
		/// </summary>
		public string Interface { get; set; }


		/// <summary>
		/// 实现接口的服务类全名，含程序集名
		/// </summary>
		public Type Service { get; set; }
	}
}
