using System.Collections;
using UnityEngine;

namespace BT
{
    public class Condition
    {
        public enum E_ValueType { 
            Const,
            SelfVar,
            Blackboard
        }
        public E_ValueType Value1_Type;
        public string Value1_Name;

        public string CompareType = "==";

        public E_ValueType Value2_Type;
        public string Value2_Name;
    }
}