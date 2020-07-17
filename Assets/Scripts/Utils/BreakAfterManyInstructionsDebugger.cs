using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;

public class BreakAfterManyInstructionsDebugger : IDebugger
{
	public class InfiniteLoopException : Exception
	{
		public InfiniteLoopException()
        {
        }

		public InfiniteLoopException(int numberOfInstructions) : base($"Infinite loop suspected. Number of instructions: {numberOfInstructions}")
		{
		}
	}

	int m_InstructionCounter = 0;
	List<DynamicExpression> m_Dynamics = new List<DynamicExpression>();

	public void SetSourceCode(SourceCode sourceCode)
	{
	}

	public void SetByteCode(string[] byteCode)
	{
	}

	public bool IsPauseRequested()
	{
		return true;
	}

	public bool SignalRuntimeException(ScriptRuntimeException ex)
	{
		return false;
	}

	public DebuggerAction GetAction(int ip, SourceRef sourceref)
	{
		m_InstructionCounter += 1;

		if ((m_InstructionCounter % 1000) == 0)
			Console.Write(".");

		if (m_InstructionCounter > 50)
			throw new InfiniteLoopException(m_InstructionCounter);

		return new DebuggerAction()
		{
			Action = DebuggerAction.ActionType.StepIn,
		};
	}

	public void SignalExecutionEnded()
	{
	}

	public void Update(WatchType watchType, IEnumerable<WatchItem> items)
	{
	}

	public List<DynamicExpression> GetWatchItems()
	{
		return m_Dynamics;
	}

	public void RefreshBreakpoints(IEnumerable<SourceRef> refs)
	{
	}

    public DebuggerCaps GetDebuggerCaps()
    {
        throw new NotImplementedException();
    }

    public void SetDebugService(DebugService debugService)
    {
        throw new NotImplementedException();
    }
}
