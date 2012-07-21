﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Polenter.Serialization;

namespace Octgn.Data
{
	public static class SimpleConfig
	{
		/// <summary>
		/// Special case since it's required in Octgn.Data, and Prefs can't go there
		/// </summary>
		public static string DataDirectory
		{
			get
			{
				return ReadValue("datadirectory", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Octgn"));
			}
			set
			{
				WriteValue("datadirectory", value);
			}
		}

		private static string GetPath()
		{
			string p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Octgn", "Config");
			const string f = "settings.xml";
			string fullPath = Path.Combine(p, f);

			if (!Directory.Exists(p))
			{
				Directory.CreateDirectory(p);
			}

			return fullPath;
		}
		public static T ReadValue<T>(string valName,T dValue)
		{
			try
			{
				if(File.Exists(GetPath()))
				{
					var serializer = new SharpSerializer();
					var config = (Hashtable) serializer.Deserialize(GetPath());
					if(config.ContainsKey(valName)) return (T) config[valName];
				}
			}
			catch(Exception e)
			{
				Trace.TraceError("[SimpleConfig]ReadValue Error: " + e.Message);
			}
			return dValue;
		}

		public static string ReadValue(string valName, string d)
		{
			return ReadValue<string>(valName , d);
		}

		/// <summary>
		///   Writes a string value to the Octgn registry
		/// </summary>
		/// <param name="valName"> Name of the value </param>
		/// <param name="value"> String to write for value </param>
		public static void WriteValue(string valName, object value)
		{
			try
			{
				var serializer = new SharpSerializer();
				var config = new Hashtable();
				if (File.Exists(GetPath()))
				{
					config = (Hashtable)serializer.Deserialize(GetPath());
				}
				config[valName] = value;
				serializer.Serialize(config, GetPath());
			}
			catch (Exception e)
			{
				Trace.TraceError("[SimpleConfig]WriteValue Error: " + e.Message);
			}

		}
	}
}