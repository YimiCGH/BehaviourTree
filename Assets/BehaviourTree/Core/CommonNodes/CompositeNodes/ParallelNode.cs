namespace BT
{
    [Category("组合","并行（ParallelNode）")]
    public class ParallelNode : CompositeNode
    {

        public enum E_ReturnType
        {            
            ChildSuccess,  //只要一个成功，就退出，返回成功，只有全部失败才返回失败
            ChildFailure //只要一个失败，就退出，返回失败，只有全部成功才返回成功
        };

        public E_ReturnType ReturnType;

        protected override void OnStart()
        {
            foreach (var child in Children) {
                child.Reset();
            }
        }

        protected override void OnStop()
        {

        }
        //并行执行所有子节点
        protected override E_State OnUpdate()
        {
            return ReturnType == E_ReturnType.ChildFailure ? ReturnWhenChildFailure() : ReturnWhenChildSuccess();
        }

        E_State ReturnWhenChildFailure(){
            bool isRunning = false;
            foreach (var child in Children)
            {
                if (child.End) {
                    continue;
                }

                var result = child.Update();
                if (result == E_State.Failure){
                    return E_State.Failure;
                }

                if (result == E_State.Running){
                    isRunning = true;
                }
            }

            if (isRunning){
                return E_State.Running;
            }

            return E_State.Success;
        }

        E_State ReturnWhenChildSuccess()
        {
            bool isRunning = false;
            foreach (var child in Children)
            {
                if (child.End){
                    continue;
                }

                var result = child.Update();
                if (result == E_State.Success){
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