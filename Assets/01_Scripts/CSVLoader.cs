using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVLoader
{
    /// <summary>
    /// ��ȭ ���� ǥ���ϴ� "����ǥ �ȿ�, ������ ���� �� ������, �� ������ ����"�ϰ� ��ǥ ������ ������.
    /// </summary>
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    /// <summary>
    /// �� �ٲ��� ���Խ� \r or \n or \r\n or \n\r
    /// </summary>
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    /// <summary>
    /// trim�� ���� ���ſ� ��� �Ǵµ� ������ ���� ���� ��� �ش� ������ �����Ѵ�.
    /// </summary>
    static char[] TRIM_CHARS = { ' ' };

    /// <summary>
    /// �ݵ�� file�� Resources ������ ��ġ�ؾ߸� �Ѵ�.
    /// </summary>
    public static List<Dictionary<string, string>> Read(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new System.ArgumentNullException("name");

        // name���� Ȯ���ڸ� ����.
        //name = System.IO.Path.GetFileNameWithoutExtension(name);
        TextAsset data = Resources.Load<TextAsset>(name);
        if (!data) throw new System.Exception(string.Format("Not Find TextAsset : {0}", name));

        // ���ڸ� ����� ���� �ƴ϶�� boxing, unboxing�� �߻��ϴ� object ��� string���� �״�� �̿��ϴ� ���� ����.
        var list = new List<Dictionary<string, string>>();
        // �� ������ text ������ ������.
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        // ù ���� �з��̱� ������ 2�̻� �̾�� �Ѵ�.
        if (lines.Length <= 1) return list;

        // �з��� ��ǥ(,) ������ ������.
        var header = Regex.Split(lines[0], SPLIT_RE);
        // �������� ���� index�� 1.
        for (var i = 1; i < lines.Length; i++)
        {
            // �����͸� ��ǥ(,) ������ ������.
            var values = Regex.Split(lines[i], SPLIT_RE);
            // �ش� ���ο� �����Ͱ� �ϳ��� ���ٸ� �ǳʶڴ�.
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, string>();
            // �з����� �����Ͱ� ���ų� ���� ��츦 ����Ͽ� �ּ����� �������� ����.
            var count = Mathf.Min(header.Length, values.Length);
            for (var j = 0; j < count; j++)
            {
                string value = values[j];
                // ���ڿ��� ���۰� ���� �ִ� TRIM_CHARS�� ����.
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS);
                value = value.Replace("\\", "");
                /*object finalValue = value;
                int n;
                float f;
                if (int.TryParse(value, out n)) finalValue = n;
                else if (float.TryParse(value, out f)) finalValue = f;*/
                value = value.Replace("<br>", "\n"); // ���๮�ڰ� <br>�� ��� \n�� ����.
                value = value.Replace("<c>", ","); // ��ǥ�� <c>�� ��� ","�� ����.

                entry[header[j]] = value; // �з��� ������ ������ �߰�.
            }

            // �з� ���� ������ ������ Dictionary�� List�� �߰�.
            list.Add(entry);
        }
        return list;
    }
}
