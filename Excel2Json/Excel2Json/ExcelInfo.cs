using System.Collections;
using System.Collections.Generic;

namespace Excel2Json
{
    public class ExcelHeader 
    {
        public int XIndex;  // 行
        public int YIndex; // 列
        public bool IsAnnotation; // 是否为注释，如果是注释，那么这一列的所有数据都不要读取
        public string VariableName; // 这里是第三行的变量名字
        public string VariableType;// 变量的类型，uint32，float 等
    }

}

