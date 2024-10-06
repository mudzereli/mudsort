using mudsort.Properties;
using System;
using System.IO;

namespace mudsort
{
	public static class Util
	{
        public static void Log(String message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Asheron's Call\" + Globals.PluginName + " log.txt", true))
                {
                    writer.WriteLine(DateTime.Now.ToString() + ": " + message);
                    writer.Close();
                }
            }
            catch
            {
            }
        }

		public static void LogError(Exception ex)
		{
			using (StreamWriter writer = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Asheron's Call\" + Globals.PluginName + " errors.txt", true))
			{
				writer.WriteLine("============================================================================");
				writer.WriteLine(DateTime.Now.ToString());
				writer.WriteLine("Error: " + ex.Message);
				writer.WriteLine("Source: " + ex.Source);
				writer.WriteLine("Stack: " + ex.StackTrace);
				if (ex.InnerException != null)
				{
					writer.WriteLine("Inner: " + ex.InnerException.Message);
					writer.WriteLine("Inner Stack: " + ex.InnerException.StackTrace);
				}
				writer.WriteLine("============================================================================");
				writer.WriteLine("");
				writer.Close();
			}
		}

        public static void WriteToChat(string message)
        {
            try
            {
                Globals.Host.Actions.AddChatText("[" + Globals.PluginName + "]: " + message, 5);
            }
            catch (Exception ex) { LogError(ex); }
        }
        public static void DebugWrite(string message)
        {
            try
            {
                if (Settings.Default.Debug)
                {
                    Globals.Host.Actions.AddChatText("[" + Globals.PluginName + "]: " + message, 5);
                }
            }
            catch (Exception ex) { LogError(ex); }
        }
    }
}
