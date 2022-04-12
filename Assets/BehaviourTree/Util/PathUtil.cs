using UnityEngine;
namespace BT.Util
{
    public class PathUtil
    {
        public static string ToPrjectPath(string _path) {
            var dataPath = Application.dataPath;
            return "Assets" + _path.Remove(0,dataPath.Length);
        }
    }
}