namespace SadConsole.Factory
{
    /// <summary>
    /// Base class for a settings object that contains parameters to pass to the Create function of a factory.
    /// </summary>
    public class BlueprintConfig
    {
        /// <summary>
        /// Represents no arguments -- pass as the config parameter to Create if there are no parameters you wish to pass.
        /// </summary>
        public static readonly BlueprintConfig Empty = new BlueprintConfig();
    }
}
