using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
namespace BT
{
    public class CompositeNodeView:NodeView
    {
        protected List<Port> OutPutPorts;

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

        protected virtual void AddOutPutPort()
        {
            CreateOutputPort("", Port.Capacity.Single, typeof(bool));
        }
        protected override Port CreateOutputPort(string portName, Port.Capacity capacity, Type type)
        {
            var port = base.CreateOutputPort(portName, capacity, type);
            
            var deleteBtn = new Button(() =>
            {
                GraphView.RemovePort(port);
            }){text = "X"};
            deleteBtn.style.backgroundColor = new StyleColor(new Color(0.7f, 0.2f, 0));
            port.contentContainer.Add(deleteBtn);
            
            return port;
        }
    }
}