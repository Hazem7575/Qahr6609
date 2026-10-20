using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.Database
{
   public static class QuizShow
    {
       public class Question
       {
           public int Index = 0;
           public string question = "";
           public string[] Answers = new string[4];
           // 1..4, the same numbering the client sends (MsgQuizShow answer packet).
           public int RightAnswer;

           public bool CheckAnswer(int Answer)
           {
               return Answer == RightAnswer;
           }
           public string[] GetStr()
           {
               string[] str = new string[5];
               str[0] = question;
               for (int x = 0; x < Answers.Length; x++)
                   str[x + 1] = Answers[x];
               return str;
           }
       }

       public static List<Question> Questions = new List<Question>();

       // quizquestins.ini: one question per line, "question#answer1#answer2#answer3#answer4#right(1-4)",
       // '~' = space (TQ text convention).
       public static void Load()
       {
           Questions = new List<Question>();
           string path = Program.ServerConfig.DbLocation + "quizquestins.ini";
           if (!File.Exists(path))
           {
               MyConsole.WriteLine("Quiz Show: " + path + " not found, the Quiz Show is disabled.");
               return;
           }
           int Index = 0;
           foreach (var raw in File.ReadAllLines(path))
           {
               if (string.IsNullOrWhiteSpace(raw))
                   continue;
               string[] parts = raw.Split('#');
               if (parts.Length < 6)
                   continue;
               int right;
               if (!int.TryParse(parts[5].Trim(), out right) || right < 1 || right > 4)
                   continue;
               Question obj = new Question();
               obj.question = Clean(parts[0]);
               for (int x = 0; x < 4; x++)
                   obj.Answers[x] = Clean(parts[x + 1]);
               if (obj.question.Length == 0)
                   continue;
               obj.RightAnswer = right;
               obj.Index = ++Index;
               Questions.Add(obj);
           }
           MyConsole.WriteLine("Loading: " + Questions.Count + " Quiz Show questions");
       }
       // The client's string list holds at most 255 bytes per string.
       private static string Clean(string text)
       {
           text = text.Replace('~', ' ').Trim();
           if (text.Length > 255)
               text = text.Substring(0, 255);
           return text;
       }

    }
}
