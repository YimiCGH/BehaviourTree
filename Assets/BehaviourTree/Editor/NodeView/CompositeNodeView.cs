using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
namespace BT
{
    public class CompositeNodeView:NodeView
    {
        private List<Port> OutPutPorts;

        protected override void OnInit()
        {
            OutPutPorts = new List<Port>();
        }

        protected override void InitTitleContainer()
        {
            base.InitTitleContainer();
            var addbtn = new Button(() => { AddOutPutPort();});
            addbtn.text = "+";
            titleContainer.Add(addbtn);
        }

        void AddOutPutPort()
        {
            CreateOutputPort("", Port.Capacity.Single, typeof(bool));
        }
    }
}