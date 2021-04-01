using System;
using System.Linq;
using UnityEngine;
using MoonSharp.Interpreter;
using Utility;

namespace Lua
{
    public class LuaInterpreter : ILuaInterpreter
    {
        private Script script;

        private static ISample[] GetSamplesFromTable(Table table) =>
            table.Values.AsObjects<ISample>().ToArray();

        public LuaInterpreter()
        {
            UserData.RegisterAssembly();
            LuaScriptHelper.RegisterSimpleAction<int>();
            script = new Script();
            //script.AttachDebugger(new BreakAfterManyInstructionsDebugger());
            script.Options.DebugPrint = s => { Debug.LogWarning(s); };
            script.Globals["Sample"] = (Func<string, ISample>) CreateSample;
            script.Globals["Chord"] = (Func<Table, IChord>) CreateChord;
            script.Globals["Sequence"] = (Func<Table, ISequencer>) CreateSequence;
        }

        #region Visible to lua

        private static ISample CreateSample(string filename) => SoundFactory.CreateSample(filename);

        private static IChord CreateChord(Table tableOfSamples) =>
            SoundFactory.CreateChord(GetSamplesFromTable(tableOfSamples));

        private static ISequencer CreateSequence(Table tableOfSamples) =>
            SoundFactory.CreateSequencer(GetSamplesFromTable(tableOfSamples));

        #endregion Visible to lua

        public void Execute(string code)
        {
            script.DoString(code);
        }
    }
}
