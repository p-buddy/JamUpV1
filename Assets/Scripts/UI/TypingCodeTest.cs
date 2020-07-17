using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingCodeTest : MonoBehaviour
{
    public void OnSubmit(string value)
    {
        LuaReader.Instance?.ExecuteLuaString(value);
    }
}
