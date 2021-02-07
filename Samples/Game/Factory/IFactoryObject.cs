namespace SadConsole.Factory
{
    /// <summary>
    /// Interface that can optionally be implemented by objects created via a <see cref="Factory{TBlueprintConfig, TProduced}"/>.  The <see cref="DefinitionId"/> property
    /// will be automatically set to the ID of the blueprint used to create the object when the Factory's Create function is called.
    /// </summary>
    public interface IFactoryObject
    {
        /// <summary>
        /// The identifier of the <see cref="IBlueprint{TBlueprintConfig, TProduced}"/> that created this object. Do not set manually -- <see cref="Factory{TBlueprintConfig, TProduced}"/>
        /// will automatically set this field when the object is created.
        /// </summary>
        string DefinitionId { get; set; }
    }
}
