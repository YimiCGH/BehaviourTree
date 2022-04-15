namespace BT
{
    [Category("组合","赋值（AssignValueNode）")]
    [NodeView("BT.AssignValueNodeView")]
    public class AssignValueNode:CompositeNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override E_State OnUpdate()
        {
            return E_State.Success;
        }
    }
}