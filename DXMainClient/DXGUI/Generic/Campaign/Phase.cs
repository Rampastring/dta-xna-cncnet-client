using System;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public class Phase
    {
        public int ID;
        public Action<IStoryDisplay> Enter;
        public Action<IStoryDisplay> Ready;
        public Action<IStoryDisplay> Leave;
        public Action<IStoryDisplay> Left;

        public Phase(int id, Action<IStoryDisplay> enter, Action<IStoryDisplay> ready, Action<IStoryDisplay> leave, Action<IStoryDisplay> left)
        {
            ID = id;
            Enter = enter;
            Ready = ready;
            Leave = leave;
            Left = left;
        }
    }
}
