using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    [System.Serializable]
    public class BTBlackboard :ScriptableObject
    {
        public List<BlackboardProperty> Properties;
    }    
}
