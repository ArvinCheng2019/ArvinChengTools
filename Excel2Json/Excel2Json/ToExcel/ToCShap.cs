using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Excel2Json
{
    public class ToCShap
    {

        public static string ScriptPath = "Configs/ConfigScripts";
        public static   async Task ToCodeCShap(ExcelTable tableName, List<ExcelHeader> tmpHeader)
        {
            await Task.Delay(100);

            BuildCodeClass(tableName.TableName, tmpHeader);
        }


        public static String ClassComment
        {
            get;
            set;
        }



        static void BuildCodeClass(string tableName, List<ExcelHeader> tmpHeader)
        {
            //-- 创建代码字符串
            StringBuilder sb = new StringBuilder();
            sb.Append("/*自动生成的代码，千万不要修改,如需添加，请联系 Arvin 2019.04.03*/");
            sb.Append("\r\n");
            sb.Append("using System;");
            sb.Append("\r\n");
            sb.Append("using SimpleJson;");
            sb.Append("\r\n");
            sb.Append("using System.Collections;");
            sb.Append("\r\n");
            sb.Append("using UnityEngine;");
            sb.Append("\r\n");
            sb.Append("using System.Collections.Generic;");
            sb.Append("\r\n");
            sb.Append("namespace MonogolyConfig");
            sb.Append("{\r\n");
            sb.AppendLine();
            sb.AppendFormat("public class {0}\r\n{{", tableName);
            sb.AppendLine();

            foreach (ExcelHeader heander in tmpHeader)
            {
                if (heander.IsAnnotation)
                    continue;

                string type = GetType(heander.VariableType.ToLower());
                string lowerField = heander.VariableName.ToLower();
                sb.AppendFormat("\tprivate {0} {1}; // {2}", type, lowerField, heander.VariableName);
                sb.AppendLine();
                sb.AppendFormat("\tpublic {0} {1} {{get{{return {2};}} set{{{3}=value;}}}}", type, heander.VariableName, lowerField, lowerField);
                sb.AppendLine();
                sb.AppendLine();
            }

            sb.Append('}');
            sb.AppendLine();
            sb.AppendLine("// End of Auto Generated Code");


            sb.AppendFormat("public class {0}\r\n{{", tableName + "Manager");
            sb.Append("\r\n");
            sb.AppendFormat("private static {0}Manager instance;", tableName);
            sb.Append("\r\n");
            sb.Append("private static object _lock = new object();");
            sb.Append("\r\n");
            // 做成单例类
            sb.AppendFormat("public static {0}Manager GetInstance()", tableName);

            sb.Append("{\r\n");
            sb.Append("\r\n");
            sb.Append(" if (instance == null){lock (_lock){if (instance == null){");
            sb.Append("\r\n");
            sb.AppendFormat("instance = new {0}Manager();", tableName);
            sb.Append("\r\n");
            sb.Append("}}}return instance;}");

            sb.Append("\r\n");
            // 构造函数
            sb.AppendFormat("private {0}Manager()", tableName);
            sb.Append("{\r\n");
            sb.Append("}\r\n");

            // 这里不管ab，只是管拿到数据后读取和添加
            string keyType = GetFirstIDType(tmpHeader);
            sb.AppendFormat("\tprivate Dictionary<{0},{1}>dict=new Dictionary<{2},{3}>(); \r\n", keyType, tableName, keyType, tableName);
            
            sb.AppendFormat("\t   public Dictionary<{0}, {1}> GetConfigDic()", keyType, tableName);
            sb.Append("{\r\n");
            sb.Append("return dict;");
            sb.Append("}\r\n");


            sb.Append("\r\n");
            sb.AppendFormat("\tpublic  {0} Get{1}Info({2} key)\r\n", tableName, tableName, keyType);
            sb.Append("{\r\n");
            sb.Append(" if(dict.ContainsKey(key))\r\n");
            sb.Append("{\r\n");
            sb.Append("return dict[key];\r\n");
            sb.Append("}\r\n");
            sb.AppendFormat(" \tDebug.LogError({0});\r\n", "\"not has this key\"");
            sb.Append("return null;\r\n");
            sb.Append("}\r\n");

            string parms = "";
            sb.Append("public void ReadData( string configdata)");
            sb.Append("{\r\n");
            sb.Append(" SimpleJson.JsonArray array = SimpleJson.SimpleJson.DeserializeObject(configdata) as JsonArray;");
            sb.Append("\r\n");
            sb.Append(" foreach (JsonObject tmpjson in array)");
            //Path path = new Path();
            sb.Append("{\r\n");
            sb.AppendFormat(" {0} {1} = new {2}(); ", tableName, tableName.ToLower(), tableName);
            foreach (ExcelHeader heander in tmpHeader)
            {
                string type = GetType(heander.VariableType.ToLower());
                parms += string.Format("{0}={1},", heander.VariableName, heander.VariableName);
                switch (type)
                {
                    case "bool":
                        sb.AppendFormat("{0}.{1}=GetBool(tmpjson[{2}{3}{4}].ToString());\r\n", tableName.ToLower(), heander.VariableName, "\"", heander.VariableName, "\"");
                        break;
                    case "int":
                        sb.AppendFormat(" {0}.{1}=GetInt(tmpjson[{2}{3}{4}].ToString());\r\n", tableName.ToLower(), heander.VariableName, "\"", heander.VariableName, "\"");
                        break;
                    case "float":
                        sb.AppendFormat(" {0}.{1} = GetFloat(tmpjson[{2}{3}{4}].ToString());\r\n", tableName.ToLower(), heander.VariableName, "\"", heander.VariableName, "\"");
                        break;
                    case "string":
                        sb.AppendFormat("{0}.{1}=tmpjson[{2}{3}{4}].ToString();\r\n", tableName.ToLower(), heander.VariableName, "\"", heander.VariableName, "\"");
                        break;
                    case "string[]":
                        sb.AppendFormat("{0}.{1}= tmpjson[{2}{3}{4}].ToString().Split(',');\r\n", tableName.ToLower(), heander.VariableName, "\"", heander.VariableName, "\"");
                        break;
                    case "int[]":
                        sb.AppendFormat("{0}.{1}= GetIntArray(tmpjson[{2}{3}{4}].ToString());\r\n", tableName.ToLower(), heander.VariableName, "\"", heander.VariableName, "\"");
                        break;
                    case "Vector3":
                        sb.AppendFormat("{0}.{1}= GetVector3(tmpjson[{2}{3}{4}].ToString());\r\n", tableName.ToLower(), heander.VariableName, "\"", heander.VariableName, "\"");
                        break;
                    default:
                        sb.Append("Debug.LogError(www.error);\r\n");
                        break;
                }
            }

            string key = GetFirstID(tmpHeader);
            sb.AppendFormat("if (dict.ContainsKey({0}.{1}) == false)", tableName.ToLower(), key);
            sb.Append("{\r\n");
            sb.AppendFormat(" dict.Add({0}.{1}, {2});\r\n", tableName.ToLower(), key, tableName.ToLower());
            sb.Append("}\r\n");
            sb.Append("}\r\n");
            sb.AppendFormat("Debug.Log( {0}读取表 {1} Manager结束,共:{2} + dict.Count.ToString());", "\"", tableName, "\"");
            sb.Append("}\r\n");

            sb.Append("private Vector3 GetVector3(string key)\r\n");
            sb.Append("{\r\n");
            sb.Append("Vector3 temp = Vector3.zero;\r\n");
            sb.AppendFormat(" key = key.Replace({0}({1}, {2}{3}).Replace({4}){5}, {6}{7});", "\"", "\"", "\"", "\"", "\"", "\"", "\"", "\"");

            sb.Append("string[] keys = key.Split(',');\r\n");
            sb.Append("  if (keys.Length != 3)\r\n");
            sb.Append("{\r\n");
            sb.AppendFormat(" Debug.LogError({0}{1}{2} + key);", "\"", "string 转 vector3 出错：", "\"");
            sb.Append("return temp;\r\n");
            sb.Append("}\r\n");
            sb.Append("temp.x = float.Parse(keys[0]);");
            sb.Append("\r\n");
            sb.Append("temp.y = float.Parse(keys[1]);");
            sb.Append("\r\n");
            sb.Append("temp.z = float.Parse(keys[2]);");
            sb.Append("\r\n");
            sb.Append("return temp;");
            sb.Append("}\r\n");
            
            sb.Append("\tprivate int GetInt(string key)");
            sb.Append("{\r\n");
            sb.Append("int value = -1;");
            sb.Append("if (int.TryParse(key, out value))");
            sb.Append("{\r\n");
            sb.Append("return value;");
            sb.Append("}\r\n");
            sb.AppendFormat(" Debug.LogError({0}{1}{2} + key);", "\"", "转换 int 数值出错：", "\"");
            sb.Append(" return value;");
            sb.Append("}\r\n");

            sb.Append("\t private bool GetBool(string key)");
            sb.Append("{\r\n");
            sb.Append("bool value = false;");
            sb.Append("if (bool.TryParse(key, out value))");
            sb.Append("{\r\n");
            sb.Append("return value;");
            sb.Append("}\r\n");
            sb.AppendFormat(" Debug.LogError({0}{1}{2} + key);", "\"", "转换 bool 数值出错：", "\"");
            sb.Append("return value;");
            sb.Append("}\r\n");


            sb.Append("\t int[] GetIntArray(string key)");
            sb.Append("{\r\n");
            sb.Append("string[] values = key.Split(',');");
            sb.Append("\r\n");
            sb.Append("int[] intValue = new int[values.Length];");
            sb.Append("\r\n");
            sb.Append("for (int i = 0; i < values.Length; i++)");
            sb.Append("{\r\n");
            sb.Append("intValue[i] = int.Parse(values[i]);");
            sb.Append("}\r\n");
            sb.Append("return intValue;");
            sb.Append("}\r\n");
            sb.Append("\r\n");

            sb.Append("\t float[] GetFloatArray(string key)");
            sb.Append("{\r\n");
            sb.Append("string[] values = key.Split(',');");
            sb.Append("\r\n");
            sb.Append("float[] intValue = new float[values.Length];");
            sb.Append("\r\n");
            sb.Append("for (int i = 0; i < values.Length; i++)");
            sb.Append("{\r\n");
            sb.Append("intValue[i] = float.Parse(values[i]);");
            sb.Append("}\r\n");
            sb.Append("return intValue;");
            sb.Append("}\r\n");


            sb.Append("\t private float GetFloat(string key)");
            sb.Append("{\r\n");
            sb.Append(" float value = 0.0f;");
            sb.Append("if (float.TryParse(key, out value))");
            sb.Append("{\r\n");
            sb.Append("return value;");
            sb.Append("}\r\n");
            sb.AppendFormat(" Debug.LogError({0}{1}{2} + key);", "\"", "转换 float 数值出错：", "\"");
            sb.Append("return value;");
            sb.Append("}\r\n");

            sb.Append("}\r\n");
            sb.Append("}\r\n");


            string filePath = Program.CodePath +"/" + tableName + ".cs"; //  Application.dataPath + "/" + ScriptPath + "/" + tableName + ".cs";
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    using (TextWriter writer = new StreamWriter(file, Encoding.UTF8))
                    {
                        writer.Write(sb.ToString());
                    }

                   // AssetDatabase.Refresh();
                }
                Console.WriteLine("-------------脚本："+tableName+ " 结束-------------");
            }
            catch (Exception)
            {
                Console.WriteLine("保存失败 xxxxxxxxxxxxxxx");
            }
        }

        int[] GetIntArray(string key)
        {
            string[] values = key.Split(',');
            int[] intValue = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                intValue[i] = int.Parse(values[i]);
            }
            return intValue;
        }

        static string GetType(string type)
        {
            string thisType = string.Empty;
            switch (type)
            {
                case "uint32":
                    thisType = "int";
                    break;
                case "uint":
                    thisType = "int";
                    break;
                case "uint32[]":
                    thisType = "int[]";
                    break;
                case "int32[]":
                    thisType = "int[]";
                    break;
                case "string":
                    thisType = "string";
                    break;
                case "vector3":
                    thisType = "Vector3";
                    break;
                case "bool":
                    thisType = "bool";
                    break;
                case "floot":
                    thisType = "floot";
                    break;
                case "string[]":
                    thisType = "string[]";
                    break;
                default:
                    thisType = "string";
                    break;
            }

            return thisType;
        }

        static string GetFirstIDType(List<ExcelHeader> tmpHeader)
        {
            string key = string.Empty;
            foreach (ExcelHeader header in tmpHeader)
            {
                if (header.YIndex == 1)
                {
                    key = header.VariableType;
                    break;
                }
            }

            key = GetType(key);
            return key;
        }

        static string GetFirstID(List<ExcelHeader> tmpHeader)
        {
            string key = string.Empty;
            foreach (ExcelHeader header in tmpHeader)
            {
                if (header.YIndex == 1)
                {
                    key = header.VariableName;
                    break;
                }
            }

            return key;
        }

        public static void SaveCode(string path)
        {

        }

    }

}

