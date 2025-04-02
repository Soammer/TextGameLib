namespace TextGameLib.Util;

public class Table
{
    private List<string> titles; //表格的标题行
    private readonly List<List<string>> datas; //表格的行数据
    public string Path { get; private set; } //表格文件的路径
    public string Name { get; private set; } //表格文件的名称
    public bool IsDirty { get; private set; } //表格是否被修改过
    public int DataCount => datas.Count; //获取行数
    public int TitleCount => titles.Count; //获取列数

    /// <summary>
    /// 构造函数，初始化标题和数据
    /// </summary>
    public Table()
    {
        titles =
            [
                string.Empty,
                string.Empty
            ];
        datas =
            [
                [
                    string.Empty
                ],
                [
                    string.Empty
                ]
            ];
        Path = string.Empty;
        Name = string.Empty;
    }

    /// <summary>
    /// 构造函数，设置标题和数据
    /// </summary>
    /// <param name="data">CSV格式的数据</param>
    public Table(string data)
    {
        titles =
            [
                string.Empty,
                string.Empty
            ];
        datas =
            [
                [
                    string.Empty
                ],
                [
                    string.Empty
                ]
            ];
        Path = string.Empty;
        Name = string.Empty;
        SetTitleAndData(data);
    }

    /// <summary>
    /// 读取CSV文件
    /// </summary>
    /// <param name="path">CSV文件路径</param>
    /// <returns>读取成功返回true，否则返回false</returns>
    public bool Load(string path)
    {
        Path = path;
        Name = System.IO.Path.GetFileName(path);
        StreamReader streamReader;
        try
        {
            streamReader = File.OpenText(Path);
        }
        catch
        {
            throw new ArgumentException($"找不到CSV文件：{Path}");
        }
        string titleAndData = streamReader.ReadToEnd();
        streamReader.Close();
        streamReader.Dispose();
        SetTitleAndData(titleAndData);
        IsDirty = false;
        return true;
    }

    /// <summary>
    /// 重新加载CSV文件
    /// </summary>
    /// <returns>读取成功返回true，否则返回false</returns>
    public bool Reload()
    {
        return Load(Path);
    }

    /// <summary>
    /// 保存CSV文件
    /// </summary>
    /// <returns>保存成功返回true，否则返回false</returns>
    public bool Save()
    {
        StreamWriter streamWriter = File.CreateText(Path);
        string value = string.Join(",", titles);
        streamWriter.WriteLine(value);
        for (int i = 0; i < datas.Count; i++)
        {
            value = string.Join(",", datas[i]);
            if (i == datas.Count - 1)
            {
                streamWriter.Write(value);
            }
            else
            {
                streamWriter.WriteLine(value);
            }
        }
        streamWriter.Close();
        streamWriter.Dispose();
        IsDirty = false;
        return true;
    }

    /// <summary>
    /// 获取指定行的数据
    /// </summary>
    /// <param name="row">行索引</param>
    /// <returns>返回该行的数据</returns>
    public List<string> GetRow(int row)
    {
        if (row < 0 || row >= datas.Count)
        {
            return null;
        }
        return datas[row];
    }

    /// <summary>
    /// 获取指定列的数据
    /// </summary>
    /// <param name="key">指定列的关键词</param>
    /// <param name="col">指定列的索引</param>
    /// <returns>返回该列的数据</returns>
    public List<string> GetRow(string key, int col = 0)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i][col] == key)
            {
                return datas[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 获取所有匹配关键词的行数据
    /// </summary>
    /// <param name="key">匹配的关键词</param>
    /// <param name="col">指定匹配的列索引</param>
    /// <returns>返回所有匹配的行数据</returns>
    public List<List<string>> GetAllRow(string key, int col = 0)
    {
        List<List<string>> list = [];
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i][col] == key)
            {
                list.Add(datas[i]);
            }
        }
        if (list.Count == 0)
        {
            return null;
        }
        return list;
    }

    /// <summary>
    /// 获取指定列的数据
    /// </summary>
    /// <param name="col">列索引</param>
    /// <returns>返回该列的数据</returns>
    public List<string> GetCol(int col)
    {
        List<string> list = [];
        foreach (List<string> list2 in datas)
        {
            list.Add(list2[col]);
        }
        return list;
    }

    /// <summary>
    /// 获取指定关键词的列数据
    /// </summary>
    /// <param name="key">指定列的关键词</param>
    /// <param name="row">指定列的行索引</param>
    /// <returns>返回该列的数据</returns>
    public List<string> GetCol(string key, int row = 0)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[row][i] == key)
            {
                return GetCol(i);
            }
        }
        return null;
    }

    /// <summary>
    /// 获取指定列的标题
    /// </summary>
    /// <param name="col">列索引</param>
    /// <returns>返回该列的标题</returns>
    public string GetTitle(int col)
    {
        if (col < 0 || col >= titles.Count)
        {
            throw new IndexOutOfRangeException();
        }
        return titles[col];
    }

    /// <summary>
    /// 获取指定标题的索引
    /// </summary>
    /// <param name="key">指定标题</param>
    /// <returns>返回该标题的索引</returns>
    public int GetTitle(string key)
    {
        if (string.IsNullOrEmpty(key) || !titles.Contains(key))
        {
            return -1;
        }
        return titles.IndexOf(key);
    }

    /// <summary>
    /// 获取指定单元格的数据
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <returns>返回指定单元格的数据</returns>
    public string GetData(int row, int col)
    {
        if (row < 0 || col < 0 || row >= datas.Count || col >= titles.Count)
        {
            throw new IndexOutOfRangeException();
        }
        return datas[row][col];
    }

    public int GetRowDataCount(int row)
    {
        if (row < 0 || row >= datas.Count)
        {
            throw new IndexOutOfRangeException();
        }
        return datas[row].Count;
    }

    public void SetTitleAndData(string value)
    {
        value = value.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd(
        [
                '\n'
        ]);
        List<string> list = [.. value.Split(Environment.NewLine.ToCharArray())];
        list ??= [];
        datas.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            List<string> item = [.. list[i].Split(
            [
                    ','
            ])];
            if (i > 0)
            {
                datas.Add(item);
            }
            else
            {
                titles = item;
            }
        }
        IsDirty = true;
    }

    public void SetTitle(int col, string value)
    {
        if (col < 0 || col >= titles.Count)
        {
            throw new IndexOutOfRangeException();
        }
        if (titles[col] != value)
        {
            titles[col] = value;
            IsDirty = true;
        }
    }

    public void SetData(int row, int col, string value)
    {
        if (row < 0 || col < 0 || row >= datas.Count || col >= titles.Count)
        {
            throw new IndexOutOfRangeException();
        }
        if (datas[row][col] != value)
        {
            datas[row][col] = value;
            IsDirty = true;
        }
    }

    public void SetData(int index, bool isRow, string value)
    {
        if (isRow)
        {
            for (int i = 0; i < datas[index].Count; i++)
            {
                if (datas[index][i] != value)
                {
                    datas[index][i] = value;
                    IsDirty = true;
                }
            }
            return;
        }
        List<string> col = GetCol(index);
        for (int j = 0; j < col.Count; j++)
        {
            if (col[j] != value)
            {
                col[j] = value;
                IsDirty = true;
            }
        }
    }

    public void SetData(int index, bool isRow, string value, int start, int end)
    {
        if (start >= end)
        {
            throw new ArgumentException("开始索引不能大于结束索引");
        }
        if (start < 0)
        {
            start = 0;
        }
        if (isRow)
        {
            for (int i = 0; i < datas[index].Count; i++)
            {
                if (start <= i && i <= end && datas[index][i] != value)
                {
                    datas[index][i] = value;
                    IsDirty = true;
                }
            }
            return;
        }
        List<string> col = GetCol(index);
        for (int j = 0; j < col.Count; j++)
        {
            if (start <= j && j <= end && col[j] != value)
            {
                col[j] = value;
                IsDirty = true;
            }
        }
    }

    public void CreateRow()
    {
        List<string> list = [];
        for (int i = 0; i < titles.Count; i++)
        {
            list.Add("");
        }
        datas.Add(list);
        IsDirty = true;
    }

    public void CreateCol()
    {
        titles.Add("");
        foreach (List<string> list in datas)
        {
            list.Add("");
        }
        IsDirty = true;
    }

    public void RemoveRow(int row)
    {
        datas.RemoveAt(row);
        IsDirty = true;
    }

    public void RemoveCol(int col)
    {
        titles.RemoveAt(col);
        foreach (List<string> list in datas)
        {
            list.RemoveAt(col);
        }
        IsDirty = true;
    }

    public void MoveRowUp(int row)
    {
        List<string> item = datas[row];
        datas.RemoveAt(row);
        int num = row - 1;
        if (num < 0)
        {
            datas.Add(item);
        }
        else
        {
            datas.Insert(num, item);
        }
        IsDirty = true;
    }

    public void MoveRowDown(int row)
    {
        int index = row + 2;
        List<string> item = datas[row];
        if (row == datas.Count - 1)
        {
            datas.Insert(0, item);
            datas.RemoveAt(datas.Count - 1);
        }
        else if (row == datas.Count - 2)
        {
            datas.Add(item);
            datas.RemoveAt(row);
        }
        else
        {
            datas.Insert(index, item);
            datas.RemoveAt(row);
        }
        IsDirty = true;
    }

    public void MoveColLeft(int col)
    {
        string item = titles[col];
        titles.RemoveAt(col);
        int num = col - 1;
        if (num < 0)
        {
            titles.Add(item);
        }
        else
        {
            titles.Insert(num, item);
        }
        foreach (List<string> list in datas)
        {
            item = list[col];
            list.RemoveAt(col);
            if (num < 0)
            {
                list.Add(item);
            }
            else
            {
                list.Insert(num, item);
            }
        }
        IsDirty = true;
    }

    public void MoveColRight(int col)
    {
        if (titles.Count > 1)
        {
            int index = col + 2;
            string item = titles[col];
            if (col == titles.Count - 1)
            {
                titles.Insert(0, item);
                titles.RemoveAt(titles.Count - 1);
            }
            else if (col == titles.Count - 2)
            {
                titles.Add(item);
                titles.RemoveAt(col);
            }
            else
            {
                titles.Insert(index, item);
                titles.RemoveAt(col);
            }
            foreach (List<string> list in datas)
            {
                item = list[col];
                if (col == list.Count - 1)
                {
                    list.Insert(0, item);
                    list.RemoveAt(list.Count - 1);
                }
                else if (col == list.Count - 2)
                {
                    list.Add(item);
                    list.RemoveAt(col);
                }
                else
                {
                    list.Insert(index, item);
                    list.RemoveAt(col);
                }
            }
        }
        IsDirty = true;
    }
}