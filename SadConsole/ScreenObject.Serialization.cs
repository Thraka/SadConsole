using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// A generic object processed by SadConsole. Provides parent/child, components, and position.
/// </summary>
public partial class ScreenObject
{
    [DataMember(Name = "Children")]
    private IScreenObject[]? _childrenSerialized;

    [DataMember(Name = "ChildrenLocked")]
    private bool _isChildrenLocked;

    [DataMember(Name = "Components")]
    private IComponent[]? _componentsSerialized;

    /// <summary>
    /// Nothing.
    /// </summary>
    /// <param name="context">Nothing.</param>
    [OnSerializing]
    protected void OnSerializingMethod(StreamingContext context)
    {
        _childrenSerialized = Children.ToArray();
        _componentsSerialized = SadComponents.ToArray();
        _isChildrenLocked = Children.IsLocked;
    }

    [OnSerialized]
    private void OnSerialized(StreamingContext context)
    {
        _childrenSerialized = null;
        _componentsSerialized = null;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        foreach (IScreenObject item in _childrenSerialized!)
            Children.Add(item);

        foreach (IComponent item in _componentsSerialized!)
            SadComponents.Add(item);

        Children.IsLocked = _isChildrenLocked;

        _componentsSerialized = null;
        _childrenSerialized = null;

        UpdateAbsolutePosition();
    }
}
