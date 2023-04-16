using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.ConsoleColor;

namespace _9xCode
{
    internal static class Program
    {
        // Copyright © 2023 Mobren

        static Dictionary<string, ConsoleColor> StringToConsoleColor = new Dictionary<string, ConsoleColor>()
        {
            { "Black", Black }, { "DarkBlue", DarkBlue },
            { "DarkGreen", DarkGreen }, { "DarkCyan", DarkCyan },
            { "DarkRed", DarkRed },{ "DarkMagenta", DarkMagenta },
            { "DarkYellow", DarkYellow }, { "Gray", Gray },
            { "DarkGray", DarkGray },{ "Blue", Blue },
            { "Green", Green },{ "Cyan", Cyan },
            { "Red", Red }, { "Magenta", Magenta },
            { "Yellow", Yellow }, { "White", White },
        };

        [STAThread]
        static void Main()
        {
            Console.ForegroundColor = White;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "9xCode Files (.9xc)|*.9xc";
            if (dialog.ShowDialog() == DialogResult.OK)
            {

            Run:
                bool SysLib = false, ConsoleLib = false, IOLib = false, TimeLib = false, WinLib = false;

                Dictionary<string, bool> Booleans = new Dictionary<string, bool>() { };
                Dictionary<string, int> Integers = new Dictionary<string, int>() { };
                Dictionary<string, string> Strings = new Dictionary<string, string>() { { "Ver", "b1.1" } };
                Dictionary<string, ConsoleColor> Colors = new Dictionary<string, ConsoleColor>() { };

                string[] code = File.ReadAllLines(dialog.FileName);

                for (int i = 0; i < code.Length; i++)
                {
                    string line = code[i].Trim();

                    #region Globals

                    if (ConsoleLib)
                    {
                        if (Colors.ContainsKey("ForeColor"))
                        {
                            Colors.Remove("ForeColor");
                        }
                        if (Colors.ContainsKey("BackColor"))
                        {
                            Colors.Remove("BackColor");
                        }
                        if (Integers.ContainsKey("CursorX"))
                        {
                            Integers.Remove("CursorX");
                        }
                        if (Integers.ContainsKey("CursorY"))
                        {
                            Integers.Remove("CursorY");
                        }
                        if (Booleans.ContainsKey("Cursor"))
                        {
                            Booleans.Remove("Cursor");
                        }

                        Colors.Add("ForeColor", Console.ForegroundColor);
                        Colors.Add("BackColor", Console.BackgroundColor);
                        Integers.Add("CursorX", Console.CursorLeft);
                        Integers.Add("CursorY", Console.CursorTop);
                        Booleans.Add("Cursor", Console.CursorVisible);
                    }

                    if (TimeLib)
                    {
                        if (Integers.ContainsKey("Millisecond"))
                        {
                            Integers.Remove("Millisecond");
                        }
                        if (Integers.ContainsKey("Second"))
                        {
                            Integers.Remove("Second");
                        }
                        if (Integers.ContainsKey("Minute"))
                        {
                            Integers.Remove("Minute");
                        }
                        if (Integers.ContainsKey("Hour"))
                        {
                            Integers.Remove("Hour");
                        }

                        Integers.Add("Millisecond", DateTime.Now.Millisecond);
                        Integers.Add("Second", DateTime.Now.Second);
                        Integers.Add("Minute", DateTime.Now.Minute);
                        Integers.Add("Hour", DateTime.Now.Hour);
                    }

                    #endregion

                    #region Default library

                    if (line == string.Empty)
                    {
                        continue;
                    }

                    else if (line.StartsWith(";"))
                    {
                        continue;
                    }

                    else if (line.StartsWith("endif"))
                    {
                        continue;
                    }

                    else if (line.Contains(";"))
                    {
                        int start = line.IndexOf(';');
                        line = line.Remove(start).Trim();
                    }

                    if (line.StartsWith("import"))
                    {
                        string sub = line.Substring(7);

                        if (sub.StartsWith("System"))
                        {
                            SysLib = true;
                            continue;
                        }

                        else if (sub.StartsWith("Console"))
                        {
                            ConsoleLib = true;
                            continue;
                        }

                        else if (sub.StartsWith("IO"))
                        {
                            IOLib = true;
                            continue;
                        }

                        else if (sub.StartsWith("Time"))
                        {
                            TimeLib = true;
                            continue;
                        }

                        else if (sub.StartsWith("Windows"))
                        {
                            WinLib = true;
                            continue;
                        }

                        else
                        {
                            Console.ForegroundColor = Red;
                            Console.WriteLine("Error at line " + (i + 1) + ": Unknown library");
                            Console.ForegroundColor = White;
                            break;
                        }
                    }

                    else if (line.StartsWith("Bool"))
                    {
                        if (line.Contains('='))
                        {
                            string key = line.Substring(5, line.IndexOf(" =") - 5);
                            string sub = line.Substring(line.IndexOf("= ") + 2);

                            #region Console library

                            if (!ConsoleLib)
                            {
                                if (key == "Cursor")
                                {
                                    Console.ForegroundColor = Red;
                                    Console.WriteLine("Error at line " + (i + 1) + ": Console library not imported");
                                    Console.ForegroundColor = White;
                                    break;
                                }
                            }

                            if (ConsoleLib && key == "Cursor")
                            {
                                Console.CursorVisible = Convert.ToBoolean(sub);
                                continue;
                            }

                            #endregion

                            if (Booleans.ContainsKey(key))
                            {
                                Booleans.Remove(key);
                            }

                            Booleans.Add(key, Convert.ToBoolean(sub));
                            continue;
                        }
                        else
                        {
                            string key = line.Substring(5);

                            if (Booleans.ContainsKey(key))
                            {
                                Booleans.Remove(key);
                            }

                            Booleans.Add(key, false);
                            continue;
                        }
                    }

                    else if (line.StartsWith("Color"))
                    {
                        if (line.Contains('='))
                        {
                            string key = line.Substring(6, line.IndexOf(" =") - 6);
                            string sub = line.Substring(line.IndexOf("= ") + 2);

                            #region Console library

                            if (!ConsoleLib)
                            {
                                if (key == "ForeColor" || key == "BackColor")
                                {
                                    Console.ForegroundColor = Red;
                                    Console.WriteLine("Error at line " + (i + 1) + ": Console library not imported");
                                    Console.ForegroundColor = White;
                                    break;
                                }
                            }

                            if (ConsoleLib && key == "ForeColor")
                            {
                                if (StringToConsoleColor.TryGetValue(sub, out ConsoleColor colval1))
                                {
                                    Console.ForegroundColor = colval1;
                                }
                                continue;
                            }

                            if (ConsoleLib && key == "BackColor")
                            {
                                if (StringToConsoleColor.TryGetValue(sub, out ConsoleColor colval2))
                                {
                                    Console.BackgroundColor = colval2;
                                }
                                continue;
                            }

                            #endregion

                            if (Colors.ContainsKey(key))
                            {
                                Colors.Remove(key);
                            }

                            if (StringToConsoleColor.TryGetValue(sub, out ConsoleColor colval3))
                            {
                                Colors.Add(key, colval3);
                            }
                            else
                            {
                                Console.ForegroundColor = Red;
                                Console.WriteLine("Syntax error at line " + (i + 1) + ": Not a valid ConsoleColor");
                                Console.ForegroundColor = White;
                                break;
                            }
                            continue;
                        }
                        else
                        {
                            string key = line.Substring(6);

                            if (Booleans.ContainsKey(key))
                            {
                                Booleans.Remove(key);
                            }

                            Booleans.Add(key, false);
                            continue;
                        }
                    }

                    else if (line.StartsWith("Int"))
                    {
                        if (line.Contains('='))
                        {
                            string key = line.Substring(4, line.IndexOf(" =") - 4);
                            string sub = line.Substring(line.IndexOf("= ") + 2);

                            if (Integers.ContainsKey(key))
                            {
                                Integers.Remove(key);
                            }

                            #region Console library

                            if (!ConsoleLib)
                            {
                                if (sub.StartsWith("CursorX") || sub.StartsWith("CursorY"))
                                {
                                    Console.ForegroundColor = Red;
                                    Console.WriteLine("Error at line " + (i + 1) + ": Console library not imported");
                                    Console.ForegroundColor = White;
                                    break;
                                }
                            }

                            if (ConsoleLib && key == "CursorX")
                            {
                                Console.CursorLeft = Convert.ToInt32(sub);
                                continue;
                            }

                            if (ConsoleLib && key == "CursorY")
                            {
                                Console.CursorTop = Convert.ToInt32(sub);
                                continue;
                            }

                            #endregion

                            #region Time library

                            if (!TimeLib)
                            {
                                if (sub.StartsWith("Seconds") || sub.StartsWith("Minutes") || sub.Equals("Hours"))
                                {
                                    Console.ForegroundColor = Red;
                                    Console.WriteLine("Error at line " + (i + 1) + ": Time library not imported");
                                    Console.ForegroundColor = White;
                                    break;
                                }
                            }

                            #endregion

                            if (TimeLib && sub.StartsWith("Seconds"))
                            {
                                Integers.Add(key, DateTime.Now.Second);
                            }
                            else if (TimeLib && sub.StartsWith("Minutes"))
                            {
                                Integers.Add(key, DateTime.Now.Minute);
                            }
                            else if (TimeLib && sub.StartsWith("Hours"))
                            {
                                Integers.Add(key, DateTime.Now.Hour);
                            }
                            else if (sub.StartsWith("0x"))
                            {
                                sub.Remove(0, 2);
                                Integers.Add(key, Convert.ToInt32(sub, 16));
                            }
                            else
                            {
                                Integers.Add(key, Convert.ToInt32(sub));
                            }
                            continue;
                        }
                        else
                        {
                            string key = line.Substring(4);

                            if (Integers.ContainsKey(key))
                            {
                                Integers.Remove(key);
                            }

                            Integers.Add(key, 0);
                            continue;
                        }
                    }

                    else if (line.StartsWith("String"))
                    {
                        if (line.Contains('='))
                        {
                            string key = line.Substring(7, line.IndexOf(" =") - 7);

                            if (Strings.ContainsKey(key))
                            {
                                Strings.Remove(key);
                            }

                            if (!IOLib)
                            {
                                if (line.Substring(line.IndexOf("= ") + 2).StartsWith("OpenFile"))
                                {
                                    Console.ForegroundColor = Red;
                                    Console.WriteLine("Error at line " + (i + 1) + ": IO library not imported");
                                    Console.ForegroundColor = White;
                                    break;
                                }
                            }

                            if (IOLib && line.Substring(line.IndexOf("= ") + 2).StartsWith("ReadFile"))
                            {
                                string thing = line.Substring(line.IndexOf("(") + 1, line.IndexOf(")") - (line.IndexOf("(") + 1));

                                if (Strings.TryGetValue(thing, out string strval))
                                {
                                    thing = strval;
                                }

                                Strings.Add(key, File.ReadAllText(thing));
                            }
                            else if (line.Substring(line.IndexOf("= ") + 2).StartsWith("Read()"))
                            {
                                Strings.Add(key, Console.ReadKey(true).Key.ToString());
                            }
                            else if (line.Substring(line.IndexOf("= ") + 2).StartsWith("ReadLine()"))
                            {
                                Strings.Add(key, Console.ReadLine());
                            }
                            else
                            {
                                Strings.Add(key, line.Substring(line.IndexOf("\"") + 1, line.LastIndexOf("\"") - (line.IndexOf("\"") + 1)));
                            }
                            continue;
                        }
                        else
                        {
                            string key = line.Substring(7);

                            if (Strings.ContainsKey(key))
                            {
                                Strings.Remove(key);
                            }

                            Strings.Add(key, "");
                            continue;
                        }
                    }

                    else if (line.StartsWith("if"))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;

                        string[] comparation;
                        int endif = 0;

                        for (int o = i; o < code.Length; o++)
                        {
                            if (code[o] == "endif")
                            {
                                endif = o;
                                break;
                            }
                        }

                        #region Pain (and its still not perfect)

                        if (line.Contains('='))
                        {
                            comparation = line.Substring(4).Split('=');

                            if (Integers.TryGetValue(comparation[0].Trim(), out int intval))
                            {
                                comparation[1] = comparation[1].Substring(1, comparation[1].Length - 2);
                                if (comparation[1] != intval.ToString())
                                {
                                    i = endif + 1;
                                }
                            }
                            else if (Strings.TryGetValue(comparation[0].Trim(), out string strval))
                            {
                                comparation[1] = comparation[1].Substring(comparation[1].IndexOf('"') + 1, comparation[1].LastIndexOf('"') - 2);
                                if (comparation[1] != strval)
                                {
                                    i = endif + 1;
                                }
                            }
                            else if (Booleans.TryGetValue(comparation[0].Trim(), out bool bolval))
                            {
                                comparation[1] = comparation[1].Substring(1, comparation[1].Length - 2);
                                if (comparation[1] != bolval.ToString().ToLower())
                                {
                                    i = endif + 1;
                                }
                            }
                            continue;
                        }
                        else if (line.Contains('!'))
                        {
                            comparation = line.Substring(4).Split('!');

                            if (Integers.TryGetValue(comparation[0].Trim(), out int intval))
                            {
                                comparation[1] = comparation[1].Substring(1, comparation[1].Length - 2);
                                if (comparation[1] == intval.ToString())
                                {
                                    i = endif + 1;
                                }
                            }
                            else if (Strings.TryGetValue(comparation[0].Trim(), out string strval))
                            {
                                comparation[1] = comparation[1].Substring(comparation[1].IndexOf('"') + 1, comparation[1].LastIndexOf('"') - 2);
                                if (comparation[1] == strval)
                                {
                                    i = endif + 1;
                                }
                            }
                            else if (Booleans.TryGetValue(comparation[0].Trim(), out bool bolval))
                            {
                                comparation[1] = comparation[1].Substring(1, comparation[1].Length - 2);
                                if (comparation[1] == bolval.ToString().ToLower())
                                {
                                    i = endif + 1;
                                }
                            }
                            continue;
                        }

                        #endregion
                    }

                    #endregion

                    #region System library

                    else if (SysLib && line.StartsWith("Call"))
                    {
                        int start1 = line.IndexOf('('), start2 = line.IndexOf(',');
                        int end1 = line.IndexOf(',') - start1, end2 = line.IndexOf(')') - start2;
                        string sub1 = line.Substring(start1 + 1, end1 - 1), sub2 = line.Substring(start2 + 3, end2 - 3);

                        Process.Start(sub1.Trim(), sub2.Trim());
                    }

                    else if (SysLib && line.StartsWith("Stop") && line.Contains("(") && line.Contains(")"))
                    {
                        break;
                    }

                    else if (SysLib && line.StartsWith("Goto"))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;
                        i = Convert.ToInt32(line.Substring(start + 1, end - 1));
                    }

                    else if (SysLib && line.StartsWith("Delay"))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;

                        Thread.Sleep(Convert.ToInt32(line.Substring(start + 1, end - 1)));
                    }

                    #endregion

                    #region Console library

                    else if (ConsoleLib && line.StartsWith("Write") || ConsoleLib && line.StartsWith("Print"))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;

                        if (line.Substring(start + 1, end - 1).Contains('+'))
                        {
                            string[] contents = line.Substring(start + 1, end - 1).Split('+');
                            string result = "";
                            for (int o = 0; o < contents.Length; o++)
                            {
                                string sub = contents[o].Trim();

                                if (!TimeLib)
                                {
                                    if (sub == "Seconds" || sub == "Minutes" || sub == "Hours")
                                    {
                                        Console.ForegroundColor = Red;
                                        Console.WriteLine("Error at line " + (i + 1) + ": Time library not imported");
                                        Console.ForegroundColor = White;
                                        break;
                                    }
                                }

                                if (sub.Contains('"'))
                                {
                                    string thing = sub;
                                    thing = thing.Substring(1, thing.LastIndexOf('"') - 1);
                                    result += thing;
                                }
                                else if (Booleans.TryGetValue(sub, out bool bolval))
                                {
                                    result += bolval;
                                }
                                else if (Integers.TryGetValue(sub, out int intval))
                                {
                                    result += intval;
                                }
                                else if (Colors.TryGetValue(sub, out ConsoleColor colval))
                                {
                                    result += colval;
                                }
                                else if (Strings.TryGetValue(sub, out string strval))
                                {
                                    result += strval;
                                }
                                else
                                {
                                    string thing = sub;
                                    result += thing;
                                }
                            }

                            Console.Write(result);
                        }
                        else
                        {
                            string sub = line.Substring(start + 1, end - 1).Trim();

                            if (!TimeLib)
                            {
                                if (sub == "Seconds" || sub == "Minutes" || sub == "Hours")
                                {
                                    Console.ForegroundColor = Red;
                                    Console.WriteLine("Error at line " + (i + 1) + ": Time library not imported");
                                    Console.ForegroundColor = White;
                                    break;
                                }
                            }

                            if (line.Substring(start + 1, 1) == "\"")
                            {
                                Console.Write(line.Substring(start + 2, end - 3));
                            }
                            else if (Booleans.TryGetValue(sub, out bool bolval))
                            {
                                Console.Write(bolval);
                            }
                            else if (Integers.TryGetValue(sub, out int intval))
                            {
                                Console.Write(intval);
                            }
                            else if (Colors.TryGetValue(sub, out ConsoleColor colval))
                            {
                                Console.Write(colval);
                            }
                            else if (Strings.TryGetValue(sub, out string strval))
                            {
                                Console.Write(strval);
                            }
                            else
                            {
                                Console.Write(line.Substring(start + 1, end - 1));
                            }
                        }

                        if (line.StartsWith("Print"))
                        {
                            Console.Write('\n');
                        }
                    }

                    else if (ConsoleLib && line.Equals("Clear()"))
                    {
                        Console.Clear();
                    }

                    else if (ConsoleLib && line.Equals("Read()"))
                    {
                        Console.ReadKey(true);
                    }

                    else if (ConsoleLib && line.Equals("ReadLine()"))
                    {
                        Console.ReadLine();
                    }

                    else if (ConsoleLib && line.StartsWith("ForeColor = "))
                    {
                        if (StringToConsoleColor.TryGetValue(line.Substring(12), out ConsoleColor clrval))
                        {
                            Console.ForegroundColor = clrval;
                        }
                        else
                        {
                            Console.ForegroundColor = Red;
                            Console.WriteLine("Syntax error at line " + (i + 1) + ": Not a valid color");
                            Console.ForegroundColor = White;
                            break;
                        }
                    }

                    else if (ConsoleLib && line.StartsWith("BackColor = "))
                    {
                        if (StringToConsoleColor.TryGetValue(line.Substring(12), out ConsoleColor clrval))
                        {
                            Console.BackgroundColor = clrval;
                        }
                        else
                        {
                            Console.ForegroundColor = Red;
                            Console.WriteLine("Syntax error at line " + (i + 1) + ": Not a valid color");
                            Console.ForegroundColor = White;
                            break;
                        }
                    }

                    #endregion

                    #region IO library

                    else if (IOLib && line.StartsWith("Mk"))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;
                        string sub = string.Empty;

                        if (Strings.TryGetValue(line.Substring(line.IndexOf("(") + 1, line.IndexOf(")") - (line.IndexOf("(") + 1)), out string strval))
                        {
                            sub = strval;
                        }
                        else
                        {
                            sub = line.Substring(start + 2, end - 3);
                        }

                        if (line.StartsWith("MkFile"))
                        {
                            using (FileStream stream = new FileStream(sub, FileMode.Create))
                            {
                                stream.Close();
                            }
                        }

                        else if (line.StartsWith("MkDir"))
                        {
                            Directory.CreateDirectory(sub);
                        }
                    }

                    else if (IOLib && line.StartsWith("WrFile"))
                    {
                        int start1 = line.IndexOf('('), start2 = line.IndexOf(',');
                        int end1 = line.IndexOf(',') - start1, end2 = line.IndexOf(')') - start2;

                        string sub1 = line.Substring(start1 + 1, end1 - 1).Trim(), sub2 = line.Substring(start2 + 2, end2 - 2).Trim();

                        if (Strings.TryGetValue(sub1, out string strval1))
                        {
                            sub1 = strval1;
                        }

                        if (Strings.TryGetValue(sub2, out string strval2))
                        {
                            sub2 = strval2;
                        }

                        using (FileStream stream = new FileStream(sub1, FileMode.Open))
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(sub2);
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }

                    #endregion

                    #region Windows library

                    else if (WinLib && line.StartsWith("MsgBox"))
                    {
                        int start1 = line.IndexOf('('), start2 = line.IndexOf(',');
                        int end1 = line.IndexOf(',') - start1, end2 = line.IndexOf(')') - start2;

                        string sub1 = line.Substring(start1 + 2, end1 - 3).Trim(), sub2 = line.Substring(start2 + 3, end2 - 4).Trim();

                        if (Strings.TryGetValue(sub1, out string strval1))
                        {
                            sub1 = strval1;
                        }

                        if (Strings.TryGetValue(sub2, out string strval2))
                        {
                            sub2 = strval2;
                        }

                        MessageBox.Show(sub2, sub1);
                    }

                    #endregion

                    else
                    {
                        Console.ForegroundColor = Red;
                        Console.WriteLine("Syntax error at line " + (i + 1) + ": Unknown function/variable");
                        Console.ForegroundColor = White;
                        break;
                    }
                }

                Thread.Sleep(500);
                if (MessageBox.Show("Run again?", "9xCode", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) goto Run;
            }
        }
    }
}
