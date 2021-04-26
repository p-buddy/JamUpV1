using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoBehaviours.Utility
{
    public class CoroutineProcessor : Singleton<CoroutineProcessor>
    {
        private const int MaxToDequePerFrame = 1000;
        public bool Processing => actions.Count > 0;
        private Queue<IEnumerator> actions = new Queue<IEnumerator>();
    
        public void EnqueCoroutine(IEnumerator coroutine)
        {
            actions.Enqueue(coroutine);
        }

        private void Update()
        {
            for (int i = 0; i < Math.Min(MaxToDequePerFrame, actions.Count); i++)
            {
                StartCoroutine(actions.Dequeue());
            }
        }
    }
}
