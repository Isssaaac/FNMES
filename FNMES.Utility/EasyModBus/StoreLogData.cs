using System;
using System.IO;

namespace EasyModbus
{
	public sealed class StoreLogData
	{
		private string filename = null;

		private static volatile StoreLogData instance;

		private static object syncObject = new object();

		public static StoreLogData Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncObject)
					{
						if (instance == null)
						{
							instance = new StoreLogData();
						}
					}
				}
				return instance;
			}
		}

		public string Filename
		{
			get
			{
				return filename;
			}
			set
			{
				filename = value;
			}
		}

		private StoreLogData()
		{
		}

		public void Store(string message)
		{
			if (filename != null)
			{
				using (StreamWriter streamWriter = new StreamWriter(Filename, append: true))
				{
					streamWriter.WriteLine(message);
				}
			}
		}

		public void Store(string message, DateTime timestamp)
		{
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(Filename, append: true))
				{
					streamWriter.WriteLine(timestamp.ToString("dd.MM.yyyy H:mm:ss.ff ") + message);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
