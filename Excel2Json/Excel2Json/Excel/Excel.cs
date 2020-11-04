using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml;

public class Excel
{
    public List <ExcelTable> Tables = new List<ExcelTable>();

    public Excel()
    {
		
    }

    public Excel(ExcelWorkbook wb)
    {
        for (int i = 0; i < wb.Worksheets.Count; i++)
        {
            ExcelWorksheet sheet = wb.Worksheets[i];
            ExcelTable table = new ExcelTable(sheet);
            Tables.Add(table);
        }
    }

    public void ShowLog() {
        for (int i = 0; i < Tables.Count; i++)
        {
            Tables[i].ShowLog();
        }
    }

    public void AddTable(string name)
    {

        bool isAlready = false;
        foreach (var tmp in Tables)
        {
            if (tmp.TableName == name)
            {
                isAlready = true;
                break;
                
            }
        }

        if ( isAlready )
        {
            return;
        }
        
        ExcelTable table = new ExcelTable();
        table.TableName = name;
        Tables.Add(table);
    }


    public bool HasThisTable( string name )
    {
        bool has = false;
        foreach (var table in Tables)
        {
            if (table.TableName == name)
            {
                has = true;
                break;
            }
        }

        return has;
    }


    public ExcelTable GetTableByName( string name )
    {
        foreach (var table in Tables)
        {
            if (table.TableName == name)
            {
                return table;
            }
        }

        return null;
    }
    
}
