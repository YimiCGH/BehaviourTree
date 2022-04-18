﻿namespace BT
{
    [Category("组合","序列（SequenceNode）")]
    [NodeView("BT.CompositeNodeView")]
    public class SequenceNode : CompositeNode
    {
        public enum E_ReturnType
        {
            ChildSuccess, //只要一个成功，就退出，全部失败才返回失败（不会被失败的节点截断执行，会一直执行有成功的节点）
            ChildFailure  //只要一个失败，就退出，全部成功才返回成功（会被失败的节点截断执行）
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
            return ReturnType == E_ReturnType.ChildSuccess ? ReturnWhenChildSuccess() : ReturnWhenChildFailure();
        }     

    

        E_State ReturnWhenChildSuccess()
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
        E_State ReturnWhenChildFailure()
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