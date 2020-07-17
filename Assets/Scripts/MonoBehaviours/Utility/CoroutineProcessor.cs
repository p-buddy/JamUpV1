using System.Collections;
using System.Collections.Generic;

public class CoroutineProcessor : Singleton<CoroutineProcessor>
{
    private Queue<IEnumerator> actions = new Queue<IEnumerator>();

    public void EnqueCoroutine(IEnumerator coroutine)
    {
        actions.Enqueue(coroutine);
    }

    private IEnumerator Process()
    {
        if (actions.Count > 0)
        {
            yield return StartCoroutine(actions.Dequeue());
        }
        else
        {
            yield return null;
        }
    }

    private void Update()
    {
        if (actions.Count > 0)
        {
            StartCoroutine(Process());
        }
    }
}
