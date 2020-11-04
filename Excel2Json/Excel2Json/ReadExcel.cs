using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Excel2Json
{
    public class ReadExcel
    {

        public ReadExcel()
        {
            List_Excels.Clear();
            mHanderData.Clear();
        }

        List<Excel> List_Excels = new List<Excel>();
        List<ExcelHeader> mHanderData = new List<ExcelHeader>();

        static string IgnoreSheet = "Ann_";
        static int HeaderNameLine = 3;
        static string HeaderName = "3";
        static string DataStarLine = "5";

        public void GetAllExcels()
        {
            List_Excels.Clear();
            string fullxlsx = Program.ExcelPath;
            if (Directory.Exists(fullxlsx))
            {
                DirectoryInfo direction = new DirectoryInfo(fullxlsx);
                FileInfo[] files = direction.GetFiles("*.xlsx", SearchOption.AllDirectories);
                Console.WriteLine("找到 " + files.Length + " 个文件，开始读取");

                if (files.Length != 0)
                {
                    foreach (FileInfo info in files)
                    {
                        string temPath = fullxlsx + "/" + info.Name;
                        Console.WriteLine("正在读取表格：" + temPath);
                        Excel tempExcel = ExcelHelper.LoadExcel(temPath);
                        List_Excels.Add(tempExcel);
                    }
                    string tips = string.Format("读取结束，共读取{0}个文件", List_Excels.Count.ToString());
                    Console.WriteLine(tips);
                }
                else
                {
                    Console.WriteLine("**********************指定文件夹下没有找到Excel文件 **********************************");
                }
            }

        }

        public void GetExcelsSheet()
        {
            if (List_Excels.Count == 0)
                return;

            foreach (Excel tmpExcel in List_Excels)
            {
                ReadExcelSheet(tmpExcel);
            }
        }

          void ReadExcelSheet(Excel curExcel)
        {

            foreach (ExcelTable table in curExcel.Tables)
            {
                // 如果是注释页，就跳过
                if (table.TableName.StartsWith(IgnoreSheet))
                    continue;

                List<ExcelHeader> tmpHeader = ReadHeader(table, HeaderNameLine);
                // 这里要处理一下，可能会造成 后台有线程占用情况了

                Task code = ToCShap.ToCodeCShap(table, tmpHeader);

                Task json = ToJson.ToSimpleJson(table, tmpHeader);
            }
        }

        static List<ExcelHeader> ReadHeader(ExcelTable table, int line)
        {
            List<ExcelHeader> tmpList = new List<ExcelHeader>();
            for (int x = 1; x <= table.NumberOfColumns; x++)
            {
                ExcelHeader excelInfo = new ExcelHeader();
                excelInfo.XIndex = line;
                excelInfo.YIndex = x;
                excelInfo.VariableName = table.GetValue(line, x);
                excelInfo.VariableType = table.GetValue(2, x);
                if (string.IsNullOrEmpty(excelInfo.VariableName))
                {
                    excelInfo.IsAnnotation = true;
                    continue;
                }
                excelInfo.IsAnnotation = false;
                tmpList.Add(excelInfo);
            }
            return tmpList;
        }

    }
}