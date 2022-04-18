using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT
{
    public class WeightPickNodeView:CompositeNodeView
    {
        private WeightPickNode _weightPickNode
        {
            get { return Node as WeightPickNode; }
        }

        protected override void OnInit()
        {
            OutPutPorts = new List<Port>();
        }
        protected override Port CreateOutputPort(string portName, Port.Capacity capacity, Type type)
        {
            var port = InstantiatePort(Orientation.Horizontal, Direction.Output, capacity, type);
            port.portName = portName;
            outputContainer.Add(port);
            outPutPorts.Add(port);            

            var oldLabel = port.contentContainer.Q<Label>("type");
            oldLabel.text = "权重";

            var intField = new IntegerField
            {
                name = string.Empty,
                value = 0
            };
            
            
            intField.RegisterValueChangedCallback(evt =>
            {
                int index = outPutPorts.IndexOf(port);
                _weightPickNode.Weights[index] = evt.newValue;
            });
            
            int index = outPutPorts.Count - 1;
            if (index < _weightPickNode.Weights.Count )
            {
                var value = _weightPickNode.Weights[index];
                intField.SetValueWithoutNotify(value);                
            }



            port.contentContainer.Add(intField);

            var deleteBtn = new Button(() =>
            {
                GraphView.RemovePort(port);
            }){text = "X"};
            deleteBtn.style.backgroundColor = new StyleColor(new Color(0.7f, 0.2f, 0));
            port.contentContainer.Add(deleteBtn);
            
            RefreshPorts();
            RefreshExpandedState();  
            return port;
        }

        public override void RemoveOutputPort(Port _port)
        {
            int index = outPutPorts.IndexOf(_port);
            outPutPorts.RemoveAt(index);
            outputContainer.Remove(_port);
            _weightPickNode.Weights.RemoveAt(index);
            RefreshPorts();
            RefreshExpandedState();
        }
    }
}