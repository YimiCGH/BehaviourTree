using System.Collections;
using Sirenix.OdinInspector;

namespace BT
{    
    [Category("组合","比较（CompareNode）")]
    public class CompareNode:CompositeNode
    {
        [HorizontalGroup,ValueDropdown("GetCompareString"), LabelText("比较类型")]
        public string CompareType = "==";
     
        
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
        
        IEnumerable GetCompareString = new ValueDropdownList<string>() {
            {"==","=="},
            {"!=","~="},
            {">=",">="},
            {"<=","<="},
            {">",">"},
            {"<","<"},
        };
    }
}