using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    internal class Program
    {
        const int windowHeight = 30;
        const int windowWidth = 120;
        private static string mkDir = Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Title = "FileManager";

            Console.SetWindowSize(windowWidth, windowHeight);
            Console.SetBufferSize(windowWidth, windowHeight);

            DrawConsole(0, 0, windowWidth,18);
            DrawConsole(0, 18, windowWidth, 8);
            UpdateConsole();

            Console.ReadKey(true);
        }
        static void UpdateConsole()
        {
            CursorLock(mkDir, 0, 26, windowWidth, 3);
            EnterCommand(windowWidth);
        }
        static (int  Left, int Top) CursorPosition()
        {
            return (Console.CursorLeft, Console.CursorTop);
        }
        static void EnterCommand (int width)
        {
            (int Left, int Top) = CursorPosition();
            StringBuilder command = new StringBuilder();
            char key;
            do
            {
                key = Console.ReadKey().KeyChar;

                if (key != 8 && key != 13)
                    command.Append(key);

                (int curentLeft, int curentTop) = CursorPosition();

                if (curentLeft == width - 2)
                {
                    Console.SetCursorPosition(curentLeft - 1, Top);
                    Console.Write(" ");
                    Console.SetCursorPosition(curentLeft - 1, Top);
                }
                if (key == (char)8)
                {

                    if (command.Length > 0)
                        command.Remove(command.Length - 1, 1);
                    if (curentLeft >= Left)
                    {
                        Console.SetCursorPosition(curentLeft, Top);
                        Console.Write(" ");
                        Console.SetCursorPosition (curentLeft, Top);    
                    }
                    else
                    {
                         Console.SetCursorPosition(Left, Top);
                    }
                }
            } while (key != (char)13);
            ParseCommand(command.ToString());
        }
        static void ParseCommand(string command)
        {
           string[] commandParams = command.ToLower().Split(' ');
            if (commandParams.Length > 0)
            {
                switch (commandParams[0])
                {
                    case "cd":

                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                          mkDir = commandParams[1];
                        }

                        break;

                    case "ls":

                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                            if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int r))
                            {
                                drawWood(new DirectoryInfo(commandParams[1]), r);
                            }
                            else
                            {
                                drawWood(new DirectoryInfo(commandParams[1]), 1);
                            }
                        }
                        break;
                }
            }
            UpdateConsole();
        }

        static void drawWood(DirectoryInfo dir , int page)
        {
            StringBuilder wood = new StringBuilder();
            outputWood(wood, dir, "", true);
            DrawConsole(0, 0, windowWidth, 18);
            (int curentLeft, int curentTop) = CursorPosition();
            int pageLines = 16;
            string[] Lines = wood.ToString().Split(new char[] { '\n' });
            int pageTotal = (Lines.Length + pageLines - 1) / pageLines;

            if (page > pageTotal)
                page = pageTotal;

            for (int i = (page - 1) * pageLines, counter = 0; i < page * pageLines; i++ , counter++)    
            {
                if (Lines.Length - 1 > i)
                {
                    Console.SetCursorPosition(curentLeft + 1, curentTop + 1 + counter);
                    Console.WriteLine(Lines[i]);
                }
            }
            string footer = $"<{page} из {pageTotal}>";
            Console.SetCursorPosition (windowWidth / 2 - footer.Length / 2,17);
            Console.WriteLine(footer);
        }
        static void outputWood(StringBuilder wood, DirectoryInfo dir, string indent, bool lastDirectory)
        {
            wood.Append(indent);
            if (lastDirectory)
            {
                wood.Append("└");
                indent += " ";
            }
            else
            {
                wood.Append("├");
                indent += "│";
            }

            wood.Append($"{dir.Name}\n");

            //Добавляет отображение файлов
            FileInfo[] Files = dir.GetFiles();
            for (int i = 0; i < Files.Length; i++)
            {
                if (i == Files.Length - 1)
                {
                    wood.Append($"{indent}└{Files[i].Name}\n");
                }
                else
                {
                    wood.Append($"{indent}├{Files[i].Name}\n");
                }
            }

            DirectoryInfo[] Directs = dir.GetDirectories();
            for (int i = 0; i < Directs.Length; i++)
                outputWood(wood, Directs[i], indent, i == Directs.Length - 1); ;
        }
        static void CursorLock(string directory,int x, int y, int width, int height)
        {
            DrawConsole(x, y, width, height);
            Console.SetCursorPosition(x + 1, y + height/2);
            Console.Write($"{directory}");        
        }
        static void DrawConsole(int x, int y, int width, int height)
        {
            //Верхний колонтитул
            Console.SetCursorPosition(x, y);
            Console.Write("╔");
            for (int i = 0; i < width - 2; i++)
                Console.Write("═");
            Console.Write("╗");

            //отображение боковых рамок
            Console.SetCursorPosition(x, y + 1);
            for (int i = 0; i < height - 2; i++)
            {
                Console.Write("║");
                for (int j  = x + 1; j < x + width - 1; j++)
                {
                    Console.Write(" ");
                }
                Console.Write("║");
            }

            //Нижний колонтитул
            Console.Write("╚");
            for (int i = 0; i < width - 2; i++)
                Console.Write("═");
            Console.Write("╝");
            Console.SetCursorPosition(x, y);
        }
    }
}
