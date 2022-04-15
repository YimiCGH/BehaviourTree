using System.Collections;
using UnityEngine;

namespace BT
{
    [Category("组合","筛选（SelectorNode）")]
    public class SelectorNode : CompositeNode
    {
        protected override void OnStart()
        {
            foreach (var child in Children)
            {
                child.Reset();
            }
        }

        protected override void OnStop()
        {
        }

        protected override E_State OnUpdate()
        {
            bool isRunning = false;
            foreach (var child in Children)
            {
                if (child.End)
                {
                    continue;
                }

                var result = child.Update();
                if (result == E_State.Success)
                {
                    return E_State.Success;
                }

                if (result == E_State.Running)
                {
                    isRunning = true;
                }
            }

            if (isRunning)
            {
                return E_State.Running;
            }

            return E_State.Failure;
        }
    }
}