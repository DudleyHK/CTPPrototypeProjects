using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SceneObject is a wrapper for any shape/ object found in the scene.
/// </summary>
public class SceneObject
{
    public string Name { get; private set; }
    public Vector3 Size { get; private set; }
    public Vector3 Position { get; private set; }


    public SceneObject(string name, Vector3 size, Vector3 position)
    {
        Name = name;
        Size = size;
        Position = position;
    }
}
