using System.Collections;
using UnityEngine;

namespace BT
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree BT;

        private BehaviourTree _ins_BT;
        public BehaviourTree GetRunningBT => _ins_BT;
        // Use this for initialization
        void Start()
        {
            _ins_BT = BT.Clone();
            _ins_BT.Init();
        }

        // Update is called once per frame
        void Update()
        {
            _ins_BT.Update();
        }

        void ManualCreateBT() {
            _ins_BT = Instantiate(BT);

            var delay = ScriptableObject.CreateInstance<DelayNode>();



            var logNode0 = ScriptableObject.CreateInstance<LogNode>();
            var logNode1 = ScriptableObject.CreateInstance<LogNode>();
            var logNode2 = ScriptableObject.CreateInstance<LogNode>();
            var logNode3 = ScriptableObject.CreateInstance<LogNode>();


            delay.Child = logNode3;

            /*
             var repeatNode = ScriptableObject.CreateInstance<RepeatNode>();
             repeatNode.Child = logNode;
            */

            logNode0.Message = "Hello 0";
            logNode1.Message = "Hello 1";
            logNode2.Message = "Hello 2";
            logNode3.Message = "Delay Log";

            var sequencer = ScriptableObject.CreateInstance<SequenceNode>();
            
            sequencer.Children.Add(logNode0);
            sequencer.Children.Add(logNode1);
            sequencer.Children.Add(logNode2);
            sequencer.Children.Add(delay);

            _ins_BT.rootNode = sequencer;
        }
    }
}