namespace DTAClient.Domain.Singleplayer
{
    public class GlobalVariableCondition
    {
        public GlobalVariableCondition(string globalVariableName, bool enabled)
        {
            GlobalVariableName = globalVariableName;
            Enabled = enabled;
        }

        public string GlobalVariableName { get; }
        public bool Enabled { get; }
    }
}
