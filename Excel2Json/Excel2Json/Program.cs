using System;
using System.IO;
using System.Text;

namespace Excel2Json
{
    class Program
    {
        public static string ExcelPath = string.Empty;
        public static string JsonPath = string.Empty;
       public static string CodePath = string.Empty;

        static void Main(string[] args)
        {
            Console.WriteLine("------------------开始执行--------------------------");
            if ( args.Length == 0 )
            {
                Console.WriteLine("*******从本地文件获取路径*********");
                ReadFiles();
            }
            else
            {
                Console.WriteLine("*******从执行文件获取路径*********");
                GetPathFromArg(args);
            }

            if(!CheckArgs())
            {
                Console.Read();
                return;
            }

            Console.WriteLine("Excel读取路径为：   " + ExcelPath);
            Console.WriteLine("Json导出路径为：    " + JsonPath);
            Console.WriteLine("C#脚本导出路径为：" + CodePath);

            ReadExcel newExcel = new ReadExcel();
            newExcel.GetAllExcels();
            newExcel.GetExcelsSheet();

            Console.Read();
        }


       static bool CheckArgs()
        {
            if( ExcelPath == string.Empty )
            {
                Console.WriteLine("*******Excel 路径出错，程序结束*********");
                return false;
            }

            if (JsonPath == string.Empty)
            {
                Console.WriteLine("*******JsonPath 路径出错，程序结束*********");
                return false;
            }

            if (CodePath == string.Empty)
            {
                Console.WriteLine("*******CodePath 路径出错，程序结束*********");
                return false;
            }

            return true;
        }

        static void GetPathFromArg( string[] args)
        {
            if( args.Length != 3 )
            {
                Console.WriteLine("*******从执行文件参数不正确获取路径*********");
                return;
            }

            ExcelPath = args[0].ToString();
            JsonPath = args[1].ToString();
            CodePath = args[2].ToString();
        }

        static void ReadFiles()
        {
            StreamReader sr = new StreamReader("Path.txt", Encoding.Default);
            String line;
            int index = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("//"))
                    continue;
                index++;
                switch( index )
                {
                    case 1:
                        ExcelPath = line.ToString();
                        break;
                    case 2:
                        JsonPath = line.ToString();
                        break;
                    case 3:
                        CodePath = line.ToString();
                        break;
                }
            }
        }

    }
}
