using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static System.ConsoleColor;

namespace _9xCode
{
    internal static class Program
    {
        // Beta 1

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "9xCode Files (.9xc)|*.9xc";
            if (dialog.ShowDialog() == DialogResult.OK)
            {

            run:
                bool SysLib = false, ConsoleLib = false;

                Dictionary<string, bool> Booleans = new Dictionary<string, bool>() { };
                Dictionary<string, int> Integers = new Dictionary<string, int>() { };
                Dictionary<string, string> Strings = new Dictionary<string, string>() { };
            
                string[] code = File.ReadAllLines(dialog.FileName);

                for (int i = 0; i < code.Length; i++)
                {
                    string line = code[i].Trim();

                    if (line.StartsWith(";"))
                    {
                        continue;
                    }

                    if (line.StartsWith("import"))
                    {
                        if (line.EndsWith("System"))
                        {
                            SysLib = true;
                        }

                        if (line.EndsWith("System.*"))
                        {
                            SysLib = true; ConsoleLib = true;
                        }

                        if (line.EndsWith("System.Console"))
                        {
                            ConsoleLib = true;
                        }
                    }

                    if (line.StartsWith("Bool"))
                    {
                        string key = line.Substring(8, line.IndexOf(" =") - 8);

                        if (Booleans.ContainsKey(key))
                        {
                            Booleans.Remove(key);
                        }

                        Booleans.Add(key, Convert.ToBoolean(line.Substring(line.IndexOf("= ") + 2)));
                    }

                    if (line.StartsWith("Int"))
                    {
                        string key = line.Substring(8, line.IndexOf(" =") - 8);

                        if (Integers.ContainsKey(key))
                        {
                            Integers.Remove(key);
                        }

                        if (line.Substring(line.IndexOf("= ") + 2).StartsWith("0x"))
                        {
                            line.Remove(0, 2);
                            Integers.Add(key, Convert.ToInt32(line.Substring(line.IndexOf("= ") + 2), 16));
                        }
                        else
                        {
                            Integers.Add(key, Convert.ToInt32(line.Substring(line.IndexOf("= ") + 2)));
                        }
                    }

                    if (line.StartsWith("String"))
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

                    if (line.StartsWith("if"))
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;

                        string statement = line.Substring(start + 1, end - 1);
                        string[] comparation = { };

                        int endif = 0;

                        for (int o = i; o < code.Length; o++)
                        {
                            if (code[o] == "endif")
                            {
                                endif = o;
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
                        if (line.Contains('!'))
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

                    if (ConsoleLib)
                    {
                        if (line.StartsWith("Write") || line.StartsWith("Print"))
                        {
                            int start = line.IndexOf('(');
                            int end = line.IndexOf(')') - start;

                            if (line.Substring(start + 1, end - 1).Contains('+'))
                            {
                                string[] contents = line.Substring(start + 1, end - 1).Split('+');
                                string result = "";
                                foreach (string c in contents)
                                {
                                    if (c.Trim().Contains('"'))
                                    {
                                        string thing = c.Trim();
                                        thing = thing.Substring(1, thing.LastIndexOf('"') - 1);
                                        result += thing;
                                    }
                                    else if (!c.Trim().Contains('"'))
                                    {
                                        string thing = c.Trim();
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
                                }

                                Console.Write(result);
                            }
                            else
                            {
                                if (line.Substring(line.IndexOf('(') + 1, 1) == "\"")
                                {
                                    Console.Write(line.Substring(start + 2, end - 3));
                                }
                                else if (Integers.TryGetValue(line.Substring(start + 1, end - 1), out int intval))
                                {
                                    Console.Write(intval);
                                }
                                else if (Strings.TryGetValue(line.Substring(start + 1, end - 1), out string strval))
                                {
                                    Console.Write(strval);
                                }
                                else if (Booleans.TryGetValue(line.Substring(start + 1, end - 1), out bool bolval))
                                {
                                    Console.Write(bolval);
                                }
                                else
                                {
                                    Console.Write(line.Substring(start + 1, end - 2));
                                }
                            }

                            if (line.StartsWith("Print"))
                            {
                                Console.Write('\n');
                            }
                        }

                        if (line.Equals("Clear()"))
                        {
                            Console.Clear();
                        }

                        if (line.Equals("Read()"))
                        {
                            Console.ReadKey();
                        }

                        if (line.Equals("ReadLine()"))
                        {
                            Console.ReadLine();
                        }

                        if (line.StartsWith("ForeColor = "))
                        {
                            if (StringToConsoleColor.TryGetValue(line.Substring(12), out ConsoleColor clrval))
                            {
                                Console.ForegroundColor = clrval;
                            }
                            else
                            {
                                Console.WriteLine("Error at line" + i + ": Not a console color!");
                                break;
                            }
                        }

                        if (line.StartsWith("BackColor = "))
                        {
                            if (StringToConsoleColor.TryGetValue(line.Substring(12), out ConsoleColor clrval))
                            {
                                Console.BackgroundColor = clrval;
                            }
                            else
                            {
                                Console.WriteLine("Error at line" + i + ": Not a console color!");
                                break;
                            }
                        }

                        if (line.StartsWith("CursorX = "))
                        {
                            if (Integers.TryGetValue(line.Substring(10), out int intval))
                            {
                                Console.CursorLeft = intval;
                            }
                            else
                            {
                                Console.CursorLeft = Convert.ToInt32(line.Substring(10));
                            }
                        }

                        if (line.StartsWith("CursorY = "))
                        {
                            if (Integers.TryGetValue(line.Substring(10), out int intval))
                            {
                                Console.CursorTop = intval;
                            }
                            else
                            {
                                Console.CursorTop = Convert.ToInt32(line.Substring(10));
                            }
                        }
                    }
                    else
                    {
                        if (line.StartsWith("Print") || line.Equals("Clear()") || line.StartsWith("ForeColor = ") || line.StartsWith("BackColor = ") || line.StartsWith("CursorX = ") || line.StartsWith("CursorY = "))
                        {
                            Console.WriteLine("Error at line " + i + ": Library not imported!");
                            break;
                        }
                    }

                    if (line.Equals("Stop()") && SysLib)
                    {
                        break;
                    }

                    else if (line.StartsWith("Goto") && SysLib)
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;
                        int lineToGoTo = Convert.ToInt32(line.Substring(start + 1, end - 1));

                        i = lineToGoTo;
                    }

                    else if (line.StartsWith("Delay") && SysLib)
                    {
                        int start = line.IndexOf('(');
                        int end = line.IndexOf(')') - start;

                        Thread.Sleep(Convert.ToInt32(line.Substring(start + 1, end - 1)));
                    }
                    else if (!SysLib && line.Equals("Pause()") || line.Equals("Stop()") || line.StartsWith("Goto") || line.StartsWith("Delay"))
                    {
                        Console.WriteLine("Error at line " + i + ": Library not imported!");
                        break;
                    }

                    if (line.Contains(";"))
                    {
                        continue;
                    }
                }

            readagain:
                var endKey = Console.ReadKey(true).Key;
                switch (endKey)
                {
                    case ConsoleKey.R:
                        goto run;
                    case ConsoleKey.C:
                        Console.Clear();
                        goto readagain;
                    case ConsoleKey.Escape:
                        break;
                    default:
                        goto readagain;
                }
            }
        }
    }
}
