using System.Collections;
using System.Collections.Generic;
using Lua;
using MonoBehaviours;
using UnityEngine;
using UnityEngine.UI;

public class TypingCodeTest : MonoBehaviour
{
    public void OnSubmit(string value)
    {
        GameManager.Instance.TryFetch(out ILuaInterpreter interpreter);
        interpreter.Execute(value);
    }
}
