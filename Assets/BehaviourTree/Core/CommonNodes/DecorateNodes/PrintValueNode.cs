namespace BT
{
    [Category("调试","打印子节点返回值（PrintValueNode）")]
    [NodeView("BT.PrintValueNodeView")]
    public class PrintValueNode:DecoratorNode
    {

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {            
        }

        protected override E_State OnUpdate()
        {
            //打印输出子节点的值
            return E_State.Success;
        }
    }
}