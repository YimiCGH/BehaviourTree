using UnityEditor;
using UnityEngine;

namespace BT
{
    

    public class SequencerNode : CompositeNode
    {
        public enum E_ReturnType
        {
            ReturnWhenSuccess, //只要一个成功，就退出，全部失败才返回失败（不会被失败的节点截断执行，会一直执行有成功的节点）
            ReturnWhenFailure  //只要一个失败，就退出，全部成功才返回成功（会被失败的节点截断执行）
        };
        public E_ReturnType ReturnType;

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
            return ReturnType == E_ReturnType.ReturnWhenSuccess ? ReturnWhenSuccess() : ReturnWhenFailure();
        }     

    

        E_State ReturnWhenSuccess()
        {
            var child = Children[_current];
            switch (child.Update())
            {
                case E_State.Running:
                    return E_State.Running;
                case E_State.Success:
                    return E_State.Success;
                case E_State.Failure:
                    _current++;
                    break;
            }
            return _current == Children.Count ?  E_State.Failure: E_State.Running;
        }
        E_State ReturnWhenFailure()
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