using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScreenEvent : MonoBehaviour
{
    // The keys this game uses changes what's displayed during it
    public bool usesArrows;
    public bool usesSpace;

    public abstract void EndScreenEvent();
}
