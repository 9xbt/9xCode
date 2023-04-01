﻿using System;
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
        // Beta 2

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
        static void Main(string[] args)
        {
            Console.ForegroundColor = White;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Trim().StartsWith("/CREATEFILE"))
                {
                    using (FileStream stream = new FileStream(args[i + 1], FileMode.Create))
                    {
                        stream.Close();
                    }
                    Environment.Exit(0);
                }

                else if (args[i].Trim().StartsWith("/CREATEDIR"))
                {
                    Directory.CreateDirectory(args[i + 1]);
                    Environment.Exit(0);
                }

                else if (args[i].Trim().StartsWith("/WRITE"))
                {
                    using (FileStream stream = new FileStream(args[i + 2], FileMode.Open))
                    {
                        var data = args[i + 1];
                        byte[] bytes = Encoding.UTF8.GetBytes(data);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "9xCode Files (.9xc)|*.9xc";
            if (dialog.ShowDialog() == DialogResult.OK)
            {

            run:
                bool SysLib = false, ConsoleLib = false, IOLib = false, TimeLib = false;

                Dictionary<string, bool> Booleans = new Dictionary<string, bool>() { };
                Dictionary<string, int> Integers = new Dictionary<string, int>() { };
                Dictionary<string, string> Strings = new Dictionary<string, string>() { };

                string[] code = File.ReadAllLines(dialog.FileName);

                for (int i = 0; i < code.Length; i++)
                {
                    string line = code[i].Trim();

                    // Comments & Empty lines

                    if (line == string.Empty)
                    {
                        continue;
                    }

                    else if (line.StartsWith(";"))
                    {
                        continue;
                    }

                    else if (line.Contains(";"))
                    {
                        int start = line.IndexOf(';');
                        line = line.Remove(start).Trim();
                    }

                    // Default library

                    if (line.StartsWith("import"))
                    {
                        string sub = line.Substring(7);

                        if (sub.StartsWith("System"))
                        {
                            SysLib = true;
                        }

                        else if (sub.StartsWith("Console"))
                        {
                            ConsoleLib = true;
                        }

                        else if (sub.StartsWith("IO"))
                        {
                            IOLib = true;
                        }

                        else if (sub.StartsWith("Time"))
                        {
                            TimeLib = true;
                        }

                        else
                        {
                            Console.ForegroundColor = Red;
                            Console.WriteLine("Error at line " + (i + 1) + ": Unknown library");
                            Console.ForegroundColor = White;
                            break;
                        }
                    }

                    else if (line.StartsWith("bool"))
                    {
                        string key = line.Substring(5, line.IndexOf(" =") - 5);

                        if (Booleans.ContainsKey(key))
                        {
                            Booleans.Remove(key);
                        }

                        Booleans.Add(key, Convert.ToBoolean(line.Substring(line.IndexOf("= ") + 2)));
                    }

                    else if (line.StartsWith("int"))
                    {
                        string key = line.Substring(4, line.IndexOf(" =") - 4);
                        string sub = line.Substring(line.IndexOf("= ") + 2);

                        if (Integers.ContainsKey(key))
                        {
                            Integers.Remove(key);
                        }

                        if (TimeLib)
                        {
                            if (sub.StartsWith("Seconds"))
                            {
                                Integers.Add(key, DateTime.Now.Second);
                            }
                            else if (sub.StartsWith("Minutes"))
                            {
                                Integers.Add(key, DateTime.Now.Minute);
                            }
                            else if (sub.StartsWith("Hours"))
                            {
                                Integers.Add(key, DateTime.Now.Hour);
                            }
                        }
                        else
                        {
                            if (sub.StartsWith("Seconds") || sub.StartsWith("Minutes") || sub.Equals("Hours"))
                            {
                                Console.ForegroundColor = Red;
                                Console.WriteLine("Error at line " + (i + 1) + ": Time library not imported");
                                Console.ForegroundColor = White;
                                break;
                            }
                        }

                        if (sub.StartsWith("0x"))
                        {
                            line.Remove(0, 2);
                            Integers.Add(key, Convert.ToInt32(line.Substring(line.IndexOf("= ") + 2), 16));
                        }
                        else
                        {
                            Integers.Add(key, Convert.ToInt32(line.Substring(line.IndexOf("= ") + 2)));
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

                            if (line.Substring(line.IndexOf("= ") + 2).StartsWith("Read()"))
                            {
                                string consoleLine = Console.ReadKey(true).Key.ToString();
                                Strings.Add(key, consoleLine);
                            }
                            else if (line.Substring(line.IndexOf("= ") + 2).StartsWith("ReadLine()"))
                            {
                                string consoleLine = Console.ReadLine();
                                Strings.Add(key, consoleLine);
                            }
                            else
                            {
                                Strings.Add(key, line.Substring(line.IndexOf("\"") + 1, line.LastIndexOf("\"") - (line.IndexOf("\"") + 1)));
                            }
                        }
                        else
                        {
                            string key = line.Substring(7);

                            if (Strings.ContainsKey(key))
                            {
                                Strings.Remove(key);
                            }

                            Strings.Add(key, "");
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
                                if (comparation[1] != bolval.ToString().ToLower()) // For booleans we need to make it lower
                                {
                                    i = endif + 1;
                                }
                            }
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
                                if (comparation[1] == bolval.ToString().ToLower()) // For booleans we need to make it lower
                                {
                                    i = endif + 1;
                                }
                            }
                        }
                    }

                    else if (line.StartsWith("endif"))
                    {
                        continue;
                    }

                    // Console library

                    else if (ConsoleLib && line.StartsWith("Write(") || ConsoleLib && line.StartsWith("Print("))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;

                        if (line.Substring(start + 1, end - 1).Contains('+'))
                        {
                            string[] contents = line.Substring(start + 1, end - 1).Split('+');
                            string result = "";
                            foreach (string c in contents)
                            {
                                if (c.Trim() == "Seconds")
                                {
                                    result += DateTime.Now.Second;
                                }
                                else if (c.Trim() == "Minutes")
                                {
                                    result += DateTime.Now.Minute;
                                }
                                else if (c.Trim() == "Hours")
                                {
                                    result += DateTime.Now.Hour;
                                }
                                else if (c.Trim().Contains('"'))
                                {
                                    string thing = c.Trim();
                                    thing = thing.Substring(1, thing.LastIndexOf('"') - 1);
                                    result += thing;
                                }
                                else if (Integers.TryGetValue(c.Trim(), out int intval))
                                {
                                    result += intval;
                                }
                                else if (Strings.TryGetValue(c.Trim(), out string strval))
                                {
                                    result += strval;
                                }
                                else if (Booleans.TryGetValue(c.Trim(), out bool bolval))
                                {
                                    result += bolval;
                                }
                                else
                                {
                                    string thing = c.Trim();
                                    result += thing;
                                }

                                if (!TimeLib)
                                {
                                    if (c.Trim() == "Seconds" || c.Trim() == "Minutes" || c.Trim() == "Hours")
                                    {
                                        Console.ForegroundColor = Red;
                                        Console.WriteLine("Error at line " + (i + 1) + ": Time library not imported");
                                        Console.ForegroundColor = White;
                                        break;
                                    }
                                }
                            }

                            Console.Write(result);
                        }
                        else
                        {
                            string sub = line.Substring(start + 1, end - 1);

                            if (TimeLib && sub == "Seconds")
                            {
                                Console.Write(DateTime.Now.Second);
                            }
                            else if (TimeLib && sub == "Minutes")
                            {
                                Console.Write(DateTime.Now.Minute);
                            }
                            else if (TimeLib && sub == "Hours")
                            {
                                Console.Write(DateTime.Now.Hour);
                            }
                            else if (line.Substring(start + 1, 1) == "\"")
                            {
                                Console.Write(line.Substring(start + 2, end - 3));
                            }
                            else if (Integers.TryGetValue(sub, out int intval))
                            {
                                Console.Write(intval);
                            }
                            else if (Strings.TryGetValue(sub, out string strval))
                            {
                                Console.Write(strval);
                            }
                            else if (Booleans.TryGetValue(sub, out bool bolval))
                            {
                                Console.Write(bolval);
                            }
                            else
                            {
                                Console.Write(line.Substring(start + 1, end - 1));
                            }

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
                        }

                        if (line.StartsWith("Print("))
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
                        Console.ReadKey();
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

                    else if (ConsoleLib && line.StartsWith("CursorX = "))
                    {
                        if (Integers.TryGetValue(line.Substring(10), out int intval))
                        {
                            Console.CursorLeft = intval;
                        }
                        else
                        {
                            try
                            {
                                Console.CursorLeft = Convert.ToInt32(line.Substring(10));
                            }
                            catch
                            {
                                Console.ForegroundColor = Red;
                                Console.WriteLine("Syntax error at line " + (i + 1) + ": Not a valid integer");
                                Console.ForegroundColor = White;
                                break;
                            }
                        }
                    }

                    else if (ConsoleLib && line.StartsWith("CursorY = "))
                    {
                        if (Integers.TryGetValue(line.Substring(10), out int intval))
                        {
                            Console.CursorTop = intval;
                        }
                        else
                        {
                            try
                            {
                                Console.CursorTop = Convert.ToInt32(line.Substring(10));
                            }
                            catch
                            {
                                Console.ForegroundColor = Red;
                                Console.WriteLine("Syntax error at line " + (i + 1) + ": Not a valid integer");
                                Console.ForegroundColor = White;
                                break;
                            }
                        }
                    }

                    else if (ConsoleLib && line.StartsWith("Cursor = "))
                    {
                        if (Booleans.TryGetValue(line.Substring(9), out bool intval))
                        {
                            Console.CursorVisible = intval;
                        }
                        else
                        {
                            try
                            {
                                Console.CursorVisible = Convert.ToBoolean(line.Substring(9));
                            }
                            catch
                            {
                                Console.ForegroundColor = Red;
                                Console.WriteLine("Syntax error at line " + (i + 1) + ": Not a boolean");
                                Console.ForegroundColor = White;
                                break;
                            }
                        }
                    }

                    // IO library

                    else if (IOLib && line.StartsWith("Create"))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;
                        string sub = line.Substring(start + 2, end - 3);

                        if (line.StartsWith("CreateFile("))
                        {
                            try
                            {
                                using (FileStream stream = new FileStream(sub, FileMode.Create))
                                {
                                    stream.Close();
                                }
                            }
                            catch (UnauthorizedAccessException)
                            {
                                using (Process configTool = new Process())
                                {
                                    configTool.StartInfo.FileName = Application.ExecutablePath;
                                    configTool.StartInfo.Arguments = $"/CREATEFILE {sub}";
                                    configTool.StartInfo.Verb = "runas";
                                    configTool.Start();
                                    configTool.WaitForExit();
                                }
                            }
                        }

                        else if (line.StartsWith("CreateDir("))
                        {
                            try
                            {
                                Directory.CreateDirectory(sub);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                using (Process configTool = new Process())
                                {
                                    configTool.StartInfo.FileName = Application.ExecutablePath;
                                    configTool.StartInfo.Arguments = $"/CREATEDIR {sub}";
                                    configTool.StartInfo.Verb = "runas";
                                    configTool.Start();
                                    configTool.WaitForExit();
                                }
                            }
                        }
                    }

                    else if (IOLib && line.StartsWith("WriteFile("))
                    {
                        int start1 = line.IndexOf('(');
                        int end1 = line.IndexOf(',') - start1;
                        string sub1 = line.Substring(start1 + 2, end1 - 3);

                        int start2 = line.IndexOf(',');
                        int end2 = line.IndexOf(')') - start2;
                        string sub2 = line.Substring(start2 + 3, end2 - 4);

                        try
                        {
                            using (FileStream stream = new FileStream(sub1, FileMode.Open))
                            {
                                var data = sub2;
                                byte[] bytes = Encoding.UTF8.GetBytes(data);
                                stream.Write(bytes, 0, bytes.Length);
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            using (Process configTool = new Process())
                            {
                                configTool.StartInfo.FileName = Application.ExecutablePath;
                                configTool.StartInfo.Arguments = $"/WRITE {sub1} {sub2}";
                                configTool.StartInfo.Verb = "runas";
                                configTool.Start();
                                configTool.WaitForExit();
                            }
                        }
                    }

                    // System library

                    else if (SysLib && line.StartsWith("Call"))
                    {
                        int start1 = line.IndexOf('(');
                        int end1 = line.IndexOf(',') - start1;
                        string sub1 = line.Substring(start1 + 1, end1 - 1);

                        int start2 = line.IndexOf(',');
                        int end2 = line.IndexOf(')') - start2;
                        string sub2 = line.Substring(start2 + 3, end2 - 3);

                        Process.Start(sub1.Trim(), sub2.Trim());
                    }

                    else if (SysLib && line.StartsWith("Stop()"))
                    {
                        break;
                    }

                    else if (SysLib && line.StartsWith("Goto("))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;
                        i = Convert.ToInt32(line.Substring(start + 1, end - 1)) - 2;
                    }

                    else if (SysLib && line.StartsWith("Delay("))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;

                        Thread.Sleep(Convert.ToInt32(line.Substring(start + 1, end - 1)));
                    }

                    // Syntax error detection

                    else
                    {
                        Console.ForegroundColor = Red;
                        Console.WriteLine("Syntax error at line " + (i + 1) + ": Unknown function/variable");
                        Console.ForegroundColor = White;
                        break;
                    }
                }

            readagain:
                var endKey = Console.ReadKey(true).Key;
                switch (endKey)
                {
                    case ConsoleKey.R:
                        Console.WriteLine("Debug: Run");
                        goto run;
                    case ConsoleKey.C:
                        Console.Clear();
                        Console.WriteLine("Debug: Clear");
                        goto readagain;
                    case ConsoleKey.E:
                        Console.WriteLine("Debug: Exit");
                        break;
                    default:
                        goto readagain;
                }
            }
        }
    }
}
