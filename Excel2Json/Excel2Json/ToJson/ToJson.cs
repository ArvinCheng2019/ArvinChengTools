using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System.Threading.Tasks;
using System.IO;
using System;

namespace Excel2Json
{
    public class ToJson
    {
        public static string jsonPath = "Configs/Jsons";
        static string jsonName = string.Empty;
        public static async Task ToSimpleJson(ExcelTable table, List<ExcelHeader> tmpHeader)
        {
            await Task.Delay(100);
            jsonName = table.TableName;
            SimpleJson.JsonArray jsonArray = new JsonArray();

            int tableColumns = table.NumberOfColumns; // 列
            int tableRows = table.NumberOfRows; // 行

            // 我是要从第5行来遍历，然后 最大不超过 列数，并且这个list 里面是知道 那一列有数据的

            for (int x = 5; x <= tableRows; x++)
            {
                SimpleJson.JsonObject json = new SimpleJson.JsonObject();
                for (int y = 1; y <= tableColumns; y++)
                {
                    ExcelHeader curHander = GetDataByColumns(tmpHeader, y);
                    if (curHander == null || curHander.IsAnnotation)
                        continue;

                    string value = table.GetValue(x, y);
                    json.Add(curHander.VariableName, value);
                }
                jsonArray.Add(json);
            }

            string jsonData = SimpleJson.SimpleJson.SerializeObject(jsonArray);
            string path = Program.JsonPath + "/" + table.TableName + ".json"; //Application.dataPath + "/" + jsonPath + "/" + table.TableName + ".json";
            SaveJsonFile(path, jsonData);
        }

        public static void SaveJsonFile(string path, string data)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(data);
            using (FileStream fsWrite = new FileStream(path, FileMode.Create))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
                fsWrite.Flush();
                fsWrite.Close();
            }

            Console.WriteLine("--------------表格" + path + "结束------------------");
        }

        static ExcelHeader GetDataByColumns(List<ExcelHeader> tmpHeader, int colums)
        {
            ExcelHeader hander = null;
            foreach (ExcelHeader tmp in tmpHeader)
            {
                if (tmp.YIndex == colums)
                {
                    hander = tmp;
                    break;
                }
            }

            return hander;
        }
    }

}