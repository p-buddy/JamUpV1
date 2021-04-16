using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoonSharp.Interpreter;
using Utility;

namespace Lua
{
    public class LuaInterpreter : ILuaInterpreter
    {
        private Script script;

        private static ISample[] GetSamplesFromTable(Table table)
        {
            IEnumerable<ISample> samples = table.Values.AsObjects<ISample>();
            ISample[] samplesArray = samples as ISample[] ?? samples.ToArray();
            if (samplesArray.First() != null)
            {
                return samplesArray;
            }

            IEnumerable<String> fileLocations = table.Values.AsObjects<string>();
            List<ISample> samplesList = new List<ISample>();
            foreach (string fileLocation in fileLocations)
            {
                samplesList.Add(CreateSample(fileLocation, false));
            }

            return samplesList.ToArray();
        }
        

        public LuaInterpreter()
        {
            UserData.RegisterAssembly();
            LuaScriptHelper.RegisterSimpleAction<int>();
            script = new Script();
            //script.AttachDebugger(new BreakAfterManyInstructionsDebugger());
            script.Options.DebugPrint = s => { Debug.LogWarning(s); };
            script.Globals["Sample"] = (Func<string, bool, ISample>) CreateSample;
            script.Globals["Chord"] = (Func<Table, IChord>) CreateChord;
            script.Globals["Sequencer"] = (Func<Table, bool, ISequencer>) CreateSequencer;
        }

        #region Visible to lua

        private static ISample CreateSample(string filename, bool isMono) => SoundFactory.CreateSample(filename, isMono);

        private static IChord CreateChord(Table tableOfSamples) =>
            SoundFactory.CreateChord(GetSamplesFromTable(tableOfSamples));

        private static ISequencer CreateSequencer(Table tableOfSamples, bool isMono) =>
            SoundFactory.CreateSequencer(GetSamplesFromTable(tableOfSamples), isMono);

        #endregion Visible to lua

        public void Execute(string code)
        {
            script.DoString(code);
        }
    }
}
