using UnityEditor;
using UnityEngine;

namespace BT
{
    public class SequencerNode : CompositeNode
    {
        int _current;
        protected override void OnStart()
        {
            _current = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override E_State OnUpdate()
        {
            var child = Children[_current];
            switch (child.Update())
            {
                case E_State.Running:
                    return E_State.Running;
                case E_State.Failure:
                    return E_State.Failure;
                case E_State.Success:
                    _current++;
                    break;
            }
            return _current == Children.Count ? E_State.Success : E_State.Running;
        }
    }
}