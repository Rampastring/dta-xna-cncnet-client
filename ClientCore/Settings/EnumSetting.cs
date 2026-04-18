using Rampastring.Tools;
using System;

namespace ClientCore.Settings
{
    public class EnumSetting<T> : INISetting<T> where T : Enum
    {
        public EnumSetting(IniFile iniFile, string iniSection, string iniKey, T defaultValue) : base(iniFile, iniSection, iniKey, defaultValue)
        {
        }

        public override void Write()
        {
            IniFile.SetStringValue(IniSection, IniKey, Get().ToString());
        }

        protected override T Get()
        {
            return (T)Enum.Parse(typeof(T), IniFile.GetStringValue(IniSection, IniKey, default(T).ToString()));
        }

        protected override void Set(T value)
        {
            IniFile.SetStringValue(IniSection, IniKey, value.ToString());
        }
    }
}
