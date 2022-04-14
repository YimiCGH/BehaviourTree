using UnityEngine;
namespace BT.Util
{
    public class PathUtil
    {
        public static string ToPrjectPath(string _path) {
            var dataPath = Application.dataPath;
            return "Assets" + _path.Remove(0,dataPath.Length);
        }

        public static string GetProjectPath()
        {
            var dataPath = Application.dataPath;
            var projectPath = dataPath.Substring(0,dataPath.Length - 6);
            return projectPath;
        }
    }
}