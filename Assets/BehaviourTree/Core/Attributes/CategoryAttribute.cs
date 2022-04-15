using System;

namespace BT
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CategoryAttribute:Attribute
    {
        public string CategoryName { get; private set; }
        public string SubName { get; private set; }

        public CategoryAttribute(string _categoryName,string _subName)
        {
            CategoryName = _categoryName;
            SubName = _subName;
        }
    }
}