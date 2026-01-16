using UnityEngine;
using Logic;

public static class Util
{
    public static Vector3 ToVector3(Position2 p)
    {
        return new Vector3((float)p.x, 0, (float)p.y);
    }
}