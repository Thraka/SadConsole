using System;
using Newtonsoft.Json;
using SadConsole.Renderers;

namespace SadConsole.SerializedTypes;

/// <summary>
/// Converts a <see cref="IRenderer"/> to its <see cref="IRenderer.Name"/> value and back.
/// </summary>
public class RendererJsonConverter : JsonConverter<IRenderer>
{
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, IRenderer? value, JsonSerializer serializer)
    {
        if (value != null)
            writer.WriteValue(value.Name);
    }

    /// <inheritdoc/>
    public override IRenderer? ReadJson(JsonReader reader, Type objectType, IRenderer? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is null)
            return null;

        return GameHost.Instance.GetRenderer((string)reader.Value);
    }
}
