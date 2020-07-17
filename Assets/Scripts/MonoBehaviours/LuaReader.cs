using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using MoonSharp.Interpreter;
using System.Collections.Generic;

public class LuaReader : Singleton<LuaReader>
{
    #region Visible to lua
    private static Sample CreateSample(string filename)
    {
        return SampleFactory.Instance.CreateSample(filename);
    }

    private static Chord CreateChord(Table tableOfSamples)
    {
        Sample[] samples = GetSamplesFromTable(tableOfSamples);
        return new Chord(samples);
    }

    private static Sequence CreateSequence(Table tableOfSamples)
    {
        Sample[] samples = GetSamplesFromTable(tableOfSamples);
        return new Sequence(samples);
    }

    #endregion
    private Script script;

    public void ExecuteLuaString(string code)
    {
        script.DoString(code);
    }

    private static Sample[] GetSamplesFromTable(Table table) =>
        table.Values.AsObjects<Sample>().ToArray<Sample>();

    private void Initialize()
    {
        UserData.RegisterAssembly();
        LuaScriptHelper.RegisterSimpleAction<int>();
        script = new Script();
        //script.AttachDebugger(new BreakAfterManyInstructionsDebugger());
        script.Options.DebugPrint = s => { Debug.LogWarning(s); };
        script.Globals["create_sample"] = (Func<string, Sample>)CreateSample;
        script.Globals["create_chord"] = (Func<Table, Chord>)CreateChord;
        script.Globals["create_sequence"] = (Func<Table, Sequence>)CreateSequence;
    }

    #region
    private void Awake()
    {
        Initialize();
    }
    #endregion
}
