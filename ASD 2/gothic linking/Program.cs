using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace gothic_linking
{
    public class Dialog
    {
        public string file_name;
        public string text;
        public string character;
        public string actor;
        public bool favorite = false;

        public Dialog(string file_name, string text, string character = "noone", string actor = "nobody")
        {
            this.file_name = file_name;
            this.text = text;
            this.character = character;
            this.actor = actor;
        }
    }

    static class Program
    {
        public static string FirstStringBetweenChars(this string str,char startchar, char endchar)
        {
            int start = str.IndexOf(startchar);
            int end = str.IndexOf(endchar, start + 1);
            return str.Substring(start + 1, end - start - 1);
        }
        public static Dictionary<string, Dialog> Dialogs = new Dictionary<string, Dialog>();

        static void Main(string[] args)
        {
            Encoding enc = Encoding.GetEncoding("Windows-1250");
            var lines = File.ReadAllLines("filename_text.txt", enc);
            string text = "", filename = "", character = "";
            var di = new DirectoryInfo("C:\\Users\\Andrzej\\Desktop\\Gothic\\SPEECH");
            var names = di.GetFiles().Select(x => x.Name).ToList();
            HashSet<string> firsts = new HashSet<string>();

            for (int i = 0; i < lines.Length; i += 2)
            {
                filename = lines[i + 1].Substring(12).ToUpper();
                text = lines[i].Substring(12);
                if (!names.Contains(filename)) continue;
                var parts = filename.Split('_');
                var first = parts[0];
                character = "noone";
                switch (first)
                {
                    case "INFO":
                        if (parts[1] == "FINDNPC") continue;
                        character = parts[1];
                        break;
                    case "SVM":
                        character = "SMALLTALK";
                        break;
                    case "DIA":
                    case "PC":
                    case "B":
                        character = parts[1];
                        break;
                    case "VLK":
                    case "ORG":
                    case "KDF":
                    case "KDW":
                    case "GRD":
                    case "GUR":
                    case "NOV":
                    case "SLD":
                    case "STT":
                    case "TPL":
                        character = parts[2];
                        break;
                    case "SIT":
                    case "MIS":
                        character = parts[3];
                        break;
                }
                try
                {
                    firsts.Add(parts[0]);
                }
                catch { }
                try { Dialogs.Add(filename, new Dialog(filename, text, character)); }
                catch { }


            }


            Console.WriteLine();
            Dictionary<string, bool> check_which_has_speaker = new Dictionary<string, bool>();
            Console.WriteLine();
            foreach (var key in Dialogs.Keys)
            {
                check_which_has_speaker.Add(key, false);
            }

            HashSet<string> others = new HashSet<string>();

            var attachment_lines = File.ReadAllLines("gothic1.txt", enc);
            for (int i = 0; i < attachment_lines.Length; i += 5)
            {
                string analyzed_str = attachment_lines[i + 1];
                string file_name = analyzed_str.FirstStringBetweenChars('"', '"').ToUpper() + ".WAV";
                string who_speaks = analyzed_str.FirstStringBetweenChars('(', ',');
                if (!Dialogs.ContainsKey(file_name)) continue;

                if (who_speaks == "other" || who_speaks == "hero")
                {
                    Dialogs[file_name].character = "BEZI";
                }
                check_which_has_speaker[file_name] = true;
            }

            //foreach (var item in check_which_has_speaker.Where(x => x.Value==false).Select(y => y.Key) ) {Console.WriteLine(item)};
            HashSet<string> Characters = new HashSet<string>();
            foreach (var dial in Dialogs.Values)
            {
                try
                {
                    Characters.Add(dial.character);
                }
                catch { }
            }
            var smalltalks = Dialogs.Values.Where(d => d.character == "SMALLTALK" && d.file_name.IsProperSmalltalk()).ToList();
            Console.WriteLine(smalltalks.Count());
            //foreach (string str in Characters.ToArray().OrderBy(x=>x)) Console.WriteLine(str);

            Console.WriteLine();
            string[] CharactersTaken = new string[] { "MUD", "DIEGO", "GORN", "MILTEN", "LESTER", "LARES", "XARDAS", "THORUS" };
            List<Dialog> Results = Dialogs.Values.ToList();
            var ToApp = Results.Where(x => CharactersTaken.Contains(x.character)).ToList();
            ToApp.AddRange(smalltalks);
            string[] FilesToTake = ToApp.Select(x => x.file_name).ToArray();

            List<string> contents = new List<string>();
            foreach(Dialog dialog in ToApp)
            {
                string str = $"{dialog.file_name.Substring(0,dialog.file_name.Length-4) + ".mp3"};{dialog.text};{dialog.character};{dialog.actor}";
                contents.Add(str);
            }
            File.WriteAllLines("DialogsInfo.txt", contents);

        }
        public static bool IsProperSmalltalk(this string str)
        {
            var parts = str.Split('_');
            if (parts.Count() != 3) return false;
            if (parts[2].StartsWith("SMALLTALK")) return true;
            return false;

        }
        public static void CopySelectedFiles()
        {
            string[] FilesToTake = new string[0];
            DirectoryInfo Speech = new DirectoryInfo("C:\\Users\\Andrzej\\Desktop\\Gothic\\SPEECH");
            int s = 0;
            foreach (var file in Speech.GetFiles())
            {
                if (FilesToTake.Contains(file.Name))
                {
                    s++;
                    file.CopyTo("C:\\Users\\Andrzej\\Desktop\\FilesToApp\\" + file.Name);
                    System.Threading.Thread.Sleep(3);
                }
            }
        }
    }
}
