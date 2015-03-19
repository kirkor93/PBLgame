using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Edytejshyn.Logic
{

    public class EditorLogger
    {

        public delegate void LogHandler(LoggerLevel level, string message);
        public event LogHandler LogEvent = delegate {};

        public void Log(LoggerLevel level, string message)
        {
            Console.WriteLine("{0}: {1}", level, message);
            LogEvent(level, message);
        }
    }

    public enum LoggerLevel
    {
        Debug, Info, Warning, Error
    }

    public static class LoggerExtensions
    {
        public static Color GetColor(this LoggerLevel level, Color defaultColor)
        {
            switch (level)
            {
                case LoggerLevel.Debug:
                    return Color.RoyalBlue;
                case LoggerLevel.Warning:
                    return Color.OrangeRed;
                case LoggerLevel.Error:
                    return Color.Red;
            }
            return defaultColor;
        }
    }
}
