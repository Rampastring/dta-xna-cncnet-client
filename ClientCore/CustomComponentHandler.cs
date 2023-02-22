using System;

namespace ClientCore
{
    public class CustomComponentModifiedEventArgs : EventArgs
    {
        public CustomComponentModifiedEventArgs(string customComponentName)
        {
            CustomComponentName = customComponentName;
        }

        public string CustomComponentName { get; }
    }

    public class CustomComponentHandler
    {
        private static CustomComponentHandler _instance;
        public static CustomComponentHandler Instance 
        { 
            get
            {
                if (_instance == null)
                    _instance = new CustomComponentHandler();

                return _instance;
            } 
        }

        private CustomComponentHandler()
        {
        }

        public event EventHandler<CustomComponentModifiedEventArgs> CustomComponentModified;

        public void BroadcastCustomComponentModifiedEvent(string customComponentName)
        {
            CustomComponentModified?.Invoke(this, new CustomComponentModifiedEventArgs(customComponentName));
        }
    }
}
