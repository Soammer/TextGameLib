using System.Diagnostics;
using System.Reflection;

namespace TextGameLib.Util;

public class CsvReader(StreamReader reader)
{
    private StreamReader m_Reader = reader;
    private string[] m_HeaderStrs;
    public void ReadHeader()
    {
        // Read header from csv file
        string? strs =  reader.ReadLine() ?? throw new Exception("Empty csv file");
        m_HeaderStrs = strs.Split(',');
    }

    public List<T> WriteAllRecord<T>() where T : new()
    {
        // Write record to csv file
        List<T> res = [];
        Type t = typeof(T);
        while (true)
        {
            T tmp = new();
            string? strs = reader.ReadLine();
            if(strs == null)
                break;
            string[] values = strs.Split(',');
            string binaryString = "00000001";

            // 将二进制字符串转换为int类型
            int intValue = Convert.ToInt32(binaryString, 2);

            for (int i = 0; i < m_HeaderStrs.Length; i++)
            {
                string str = m_HeaderStrs[i];
                string input = values[i];
                if(input.Length == 0)
                    continue;
                FieldInfo? fieldinfo = t.GetField(str) ?? throw new Exception("Field not found");
                Type ot = fieldinfo.FieldType;
                fieldinfo.SetValue(tmp, Convert.ChangeType(input, ot));
            }
            res.Add(tmp);
        }
        return res;
    }
}
