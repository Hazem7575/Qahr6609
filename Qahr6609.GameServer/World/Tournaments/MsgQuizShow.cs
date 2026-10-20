using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    /// <summary>
    /// Quiz Show. Every online player gets the client's quiz panel; each question is answered in the panel
    /// (MsgQuizShow 2068 action 2), quitting closes it (action 5). Packet layout taken from the 6609 client:
    ///   action 1 (question): [6] question no. (1-based), [8] first question: seconds per question - 1, later:
    ///     result of the previous answer (1 right, 2 wrong, 0 none), [10] first question: number of questions,
    ///     later: current rank, [12] time spent (s), [14] points, strings @24: question + 4 answers.
    ///   action 3 (result): [6] rank (0 = not ranked), [8] rank, [10] time spent (s), [12] points,
    ///     [14] EXP reward in 1/600 EXP ball (client divides by 600).
    /// Ranking list = MsgGenericRanking type 90000000 (see SendRanking).
    /// The schedule is not in the client's Activity.ini. Official text (client Questinfo.ini quest 11 "Quiz Show"):
    /// "We hold a Quiz Show from Monday through Friday, at 3:00, 9:00, 15:00, 21:00." = the defaults here;
    /// shell.ini [Events] QuizShowTimes / QuizShowDays (1=Monday ... 7=Sunday) override them.
    /// </summary>
    public class MsgQuizShow : ITournament
    {
        public const uint RankingType = 90000000;
        public const int LeadSeconds = 10;
        // Answers that arrive this long after the question closed (network delay) still count.
        public const int AnswerGraceSeconds = 2;
        // EXP is given in the client's unit: 600 = 1 EXP ball.
        public const uint ExpPerCorrectAnswer = 400;
        public static readonly uint[] TopReward = { 900, 600, 300 };

        public static readonly int QuestionCount = Math.Max(1, Math.Min(50, Conquer6609.Framework.Settings.Int("Events", "QuizShowQuestions", 20)));
        public static readonly int QuestionSeconds = Math.Max(10, Math.Min(60, Conquer6609.Framework.Settings.Int("Events", "QuizShowQuestionSeconds", 30)));
        public static readonly int[][] StartTimes = ParseTimes(Conquer6609.Framework.Settings.String("Events", "QuizShowTimes", "03:00,09:00,15:00,21:00"));
        public static readonly DayOfWeek[] StartDays = ParseDays(Conquer6609.Framework.Settings.String("Events", "QuizShowDays", "1,2,3,4,5"));

        public class Participant
        {
            public uint UID;
            public string Name = "";
            public ushort Points;
            public int TimeSpent;
            public int Correct;
            public int Answered;
            public byte LastResult;
            public int AnsweredQuestion;
            public bool GotFirstQuestion;
            // the connection that got the first question (a relogged player needs it again)
            public Client.GameClient Owner;
            public uint ExpReward;
            public int Rank;
        }

        private readonly object SyncRoot = new object();
        private Dictionary<uint, Participant> Participants = new Dictionary<uint, Participant>();
        private HashSet<uint> Quitted = new HashSet<uint>();
        private List<int> UseQuestion = new List<int>();
        private DateTime QuestionStart = DateTime.Now;
        public Database.QuizShow.Question CurrentQuestion = null;
        public int QuestionNumber { get; private set; }
        // Ranking of the last finished quiz (shown by the ranking window when no quiz runs).
        public static Participant[] LastRanking = new Participant[0];

        public ProcesType Process { get; set; }
        public TournamentType Type { get; set; }

        public MsgQuizShow(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

        private static int[][] ParseTimes(string text)
        {
            var list = new List<int[]>();
            foreach (var part in (text ?? "").Split(new[] { ',', ' ', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var hm = part.Split(':');
                int h, m;
                if (hm.Length == 2 && int.TryParse(hm[0], out h) && int.TryParse(hm[1], out m) && h >= 0 && h < 24 && m >= 0 && m < 60)
                    list.Add(new[] { h, m });
            }
            return list.ToArray();
        }
        // 1=Monday ... 7=Sunday (client Activity.ini numbering); 0 is accepted for Sunday too.
        private static DayOfWeek[] ParseDays(string text)
        {
            var list = new List<DayOfWeek>();
            foreach (var part in (text ?? "").Split(new[] { ',', ' ', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int d;
                if (int.TryParse(part, out d) && d >= 0 && d <= 7)
                    list.Add((DayOfWeek)(d % 7));
            }
            return list.ToArray();
        }
        public bool IsStartTime(DateTime now)
        {
            if (Array.IndexOf(StartDays, now.DayOfWeek) < 0)
                return false;
            foreach (var t in StartTimes)
                if (t[0] == now.Hour && t[1] == now.Minute)
                    return true;
            return false;
        }
        public static string TimesText()
        {
            string days = StartDays.Length == 7 ? "daily" : (StartDays.Length == 5 && !StartDays.Contains(DayOfWeek.Saturday) && !StartDays.Contains(DayOfWeek.Sunday)
                ? "Monday-Friday" : string.Join("/", StartDays.Select(d => d.ToString().Substring(0, 3))));
            return days + " " + string.Join(", ", StartTimes.Select(t => t[0].ToString("00") + ":" + t[1].ToString("00")));
        }

        public void Open()
        {
            lock (SyncRoot)
            {
                if (Process != ProcesType.Dead)
                    return;
                if (Database.QuizShow.Questions.Count < QuestionCount)
                {
                    MyConsole.WriteLine("Quiz Show: not enough questions loaded (" + Database.QuizShow.Questions.Count + "/" + QuestionCount + ").");
                    return;
                }
                Participants = new Dictionary<uint, Participant>();
                Quitted = new HashSet<uint>();
                UseQuestion = new List<int>();
                CurrentQuestion = null;
                QuestionNumber = 0;
                QuestionStart = DateTime.Now;
                Process = ProcesType.Idle;
                AddOnlinePlayers();
            }
            MsgSchedules.SendSysMesage("The Quiz Show starts in " + LeadSeconds + " seconds: " + QuestionCount + " questions, " + QuestionSeconds
                + " seconds each. Answer in the quiz panel; the top 3 get extra EXP!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.yellow);
            MsgSchedules.SendSysMesage("The Quiz Show starts in " + LeadSeconds + " seconds: " + QuestionCount + " questions, " + QuestionSeconds
                + " seconds each. Answer in the quiz panel; the top 3 get extra EXP!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.yellow);
        }
        /// <summary>GM stop: ends the quiz without rewards.</summary>
        public void Abort()
        {
            lock (SyncRoot)
            {
                if (Process == ProcesType.Dead)
                    return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var p in Participants.Values)
                    {
                        var client = GetOnline(p);
                        if (client != null)
                            client.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.Action.Result, 0, 0, (ushort)Math.Min(65535, p.TimeSpent), p.Points, 0, null));
                    }
                }
                Process = ProcesType.Dead;
                CurrentQuestion = null;
            }
        }

        private static bool CanTakePart(Client.GameClient client)
        {
            if (client == null || client.Player == null || !client.FullLoading)
                return false;
            if (!client.Player.OnMyOwnServer || client.IsConnectedInterServer())
                return false;
            // same maps the event invitations skip (guild war, jail maps)
            if (client.Player.Map == 1860 || client.Player.Map == 1858 || client.Player.Map == 1038)
                return false;
            return true;
        }
        private void AddOnlinePlayers()
        {
            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (!CanTakePart(client))
                    continue;
                uint uid = client.Player.UID;
                if (Participants.ContainsKey(uid) || Quitted.Contains(uid))
                    continue;
                Participants[uid] = new Participant() { UID = uid, Name = client.Player.Name };
            }
        }
        private static Client.GameClient GetOnline(Participant p)
        {
            Client.GameClient client;
            if (Database.Server.GamePoll.TryGetValue(p.UID, out client) && client != null && client.Player != null)
                return client;
            return null;
        }

        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            lock (SyncRoot)
            {
                if (Process == ProcesType.Dead || !CanTakePart(user))
                    return false;
                if (Participants.ContainsKey(user.Player.UID))
                    return false;
                Quitted.Remove(user.Player.UID);
                var p = new Participant() { UID = user.Player.UID, Name = user.Player.Name };
                Participants[p.UID] = p;
                // joins with the next question (the panel needs a first-question packet)
                return true;
            }
        }
        public void Quit(Client.GameClient user)
        {
            lock (SyncRoot)
            {
                if (Process == ProcesType.Dead)
                    return;
                Participants.Remove(user.Player.UID);
                Quitted.Add(user.Player.UID);
            }
        }
        public bool InTournament(Client.GameClient user)
        {
            lock (SyncRoot)
                return Process != ProcesType.Dead && Participants.ContainsKey(user.Player.UID);
        }

        public void Answer(Client.GameClient user, int questionNumber, int answer)
        {
            bool right;
            uint quizPoints;
            lock (SyncRoot)
            {
                if (Process != ProcesType.Alive || CurrentQuestion == null || questionNumber != QuestionNumber)
                    return;
                Participant p;
                if (!Participants.TryGetValue(user.Player.UID, out p) || p.AnsweredQuestion == QuestionNumber)
                    return;
                double elapsed = (DateTime.Now - QuestionStart).TotalSeconds;
                if (elapsed > QuestionSeconds + AnswerGraceSeconds)
                    return;
                p.AnsweredQuestion = QuestionNumber;
                p.Answered++;
                p.TimeSpent += Math.Max(1, Math.Min(QuestionSeconds, (int)Math.Ceiling(elapsed)));
                right = answer >= 1 && answer <= 4 && CurrentQuestion.CheckAnswer(answer);
                if (right)
                {
                    ushort gain = (ushort)(15 + QuestionNumber);
                    p.Points = (ushort)Math.Min(65535, p.Points + gain);
                    p.Correct++;
                    p.LastResult = 1;
                    p.ExpReward += ExpPerCorrectAnswer;
                    user.Player.QuizPoints += gain;
                }
                else
                {
                    p.Points = (ushort)Math.Min(65535, p.Points + 1);
                    p.LastResult = 2;
                }
                quizPoints = user.Player.QuizPoints;
            }
            if (right)
            {
                user.GainExpBall(ExpPerCorrectAnswer, true, Role.Flags.ExperienceEffect.angelwing);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    user.Player.SendUpdate(stream, quizPoints, MsgServer.MsgUpdate.DataType.QuizPoints);
                }
            }
        }

        public void CheckUp()
        {
            lock (SyncRoot)
            {
                if (Process == ProcesType.Idle)
                {
                    if (DateTime.Now >= QuestionStart.AddSeconds(LeadSeconds))
                    {
                        Process = ProcesType.Alive;
                        NextQuestion();
                    }
                }
                else if (Process == ProcesType.Alive)
                {
                    if (DateTime.Now >= QuestionStart.AddSeconds(QuestionSeconds))
                    {
                        foreach (var p in Participants.Values)
                        {
                            if (p.AnsweredQuestion != QuestionNumber)
                            {
                                p.TimeSpent += QuestionSeconds;
                                p.LastResult = 0;
                            }
                        }
                        if (QuestionNumber >= QuestionCount)
                            Finish();
                        else
                            NextQuestion();
                    }
                }
            }
        }

        private void NextQuestion()
        {
            AddOnlinePlayers();
            CurrentQuestion = GenerateQuestion();
            if (CurrentQuestion == null)
            {
                Finish();
                return;
            }
            QuestionNumber++;
            QuestionStart = DateTime.Now;
            CalculateRank();
            string[] texts = CurrentQuestion.GetStr();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var p in Participants.Values)
                {
                    var client = GetOnline(p);
                    if (client == null)
                        continue;
                    if (!p.GotFirstQuestion || QuestionNumber == 1 || p.Owner != client)
                    {
                        // The panel takes the time per question and the question count only from question 1.
                        client.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.Action.Question, 1, (ushort)(QuestionSeconds - 1), (ushort)QuestionCount
                            , (ushort)Math.Min(65535, p.TimeSpent), p.Points, texts));
                        p.GotFirstQuestion = true;
                        p.Owner = client;
                        if (QuestionNumber != 1)
                        {
                            client.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.Action.Question, (ushort)QuestionNumber, 0, (ushort)p.Rank
                                , (ushort)Math.Min(65535, p.TimeSpent), p.Points, texts));
                        }
                    }
                    else
                    {
                        client.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.Action.Question, (ushort)QuestionNumber, p.LastResult, (ushort)p.Rank
                            , (ushort)Math.Min(65535, p.TimeSpent), p.Points, texts));
                    }
                    p.LastResult = 0;
                }
            }
        }

        private void Finish()
        {
            var ranks = CalculateRank();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                for (int x = 0; x < Math.Min(TopReward.Length, ranks.Length); x++)
                {
                    var top = ranks[x];
                    top.ExpReward += TopReward[x];
                    var client = GetOnline(top);
                    if (client != null)
                        client.GainExpBall(TopReward[x], true, Role.Flags.ExperienceEffect.angelwing);
                }
                foreach (var p in Participants.Values)
                {
                    var client = GetOnline(p);
                    if (client == null)
                        continue;
                    client.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.Action.Result, (ushort)Math.Min(255, p.Rank), (ushort)p.Rank
                        , (ushort)Math.Min(65535, p.TimeSpent), p.Points, (ushort)Math.Min(65535, p.ExpReward), null));
                }
            }
            foreach (var p in Participants.Values)
                p.Owner = null;
            LastRanking = ranks;
            Process = ProcesType.Dead;
            CurrentQuestion = null;

            if (ranks.Length > 0)
            {
                var text = new StringBuilder("The Quiz Show has ended! Champion: " + ranks[0].Name + " (" + ranks[0].Points + " points)");
                if (ranks.Length > 1) text.Append(", 2nd: " + ranks[1].Name + " (" + ranks[1].Points + ")");
                if (ranks.Length > 2) text.Append(", 3rd: " + ranks[2].Name + " (" + ranks[2].Points + ")");
                text.Append(". Next Quiz Show: " + TimesText() + ".");
                MsgSchedules.SendSysMesage(text.ToString(), MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.yellow);
            }
        }

        /// <summary>Ranks the players who answered at least once (points, then less time). Others get rank 0.</summary>
        private Participant[] CalculateRank()
        {
            var ranked = Participants.Values.Where(p => p.Answered > 0)
                .OrderByDescending(p => p.Points).ThenBy(p => p.TimeSpent).ThenBy(p => p.UID).ToArray();
            foreach (var p in Participants.Values)
                p.Rank = 0;
            for (int x = 0; x < ranked.Length; x++)
                ranked[x].Rank = x + 1;
            return ranked;
        }

        private Database.QuizShow.Question GenerateQuestion()
        {
            var array_question = Database.QuizShow.Questions.Where(p => UseQuestion.Contains(p.Index) == false).ToArray();
            if (array_question.Length == 0)
                return null;
            var question = array_question[Program.GetRandom.Next(0, array_question.Length)];
            UseQuestion.Add(question.Index);
            return question;
        }

        /// <summary>MsgGenericRanking (1151) type 90000000: the quiz ranking window. Running quiz, else the last one.</summary>
        public void SendRanking(Client.GameClient user, ServerSockets.Packet stream, bool myRank, ushort page)
        {
            Participant[] list;
            lock (SyncRoot)
                list = Process != ProcesType.Dead ? CalculateRank() : LastRanking;
            var type = (MsgServer.MsgGenericRanking.RankType)RankingType;
            if (myRank)
            {
                var me = list.FirstOrDefault(p => p.UID == user.Player.UID);
                if (me == null)
                    return;// the client asserts on a "my rank" reply without the hero in it
                stream.GenericRankingCreate(MsgServer.MsgGenericRanking.Action.QueryCount, type, 0, 0, me != null ? 1 : 0);
                if (me != null)
                    stream.AddItemGenericRankingCreate(me.Rank, me.Points, me.UID, me.Name);
                user.Send(stream.GenericRankingFinalize());
                return;
            }
            const int max = 10;
            int offset = page * max;
            int count = Math.Max(0, Math.Min(max, list.Length - offset));
            stream.GenericRankingCreate(MsgServer.MsgGenericRanking.Action.Ranking, type, (ushort)Math.Min(65535, list.Length), page, count);
            for (int x = 0; x < count; x++)
            {
                var p = list[offset + x];
                stream.AddItemGenericRankingCreate(offset + x + 1, p.Points, p.UID, p.Name);
            }
            user.Send(stream.GenericRankingFinalize());
        }
    }

    internal static class QuizShowCommands
    {
        [Conquer6609.Framework.Commands.GmCommand("quizshow", Role = Conquer6609.Framework.Commands.CommandRole.GM,
            Help = "Start the Quiz Show now; '@quizshow stop' ends it without rewards.", Usage = "@quizshow [stop]")]
        static void Cmd_quizshow(Client.GameClient client, string[] data)
        {
            var quiz = MsgSchedules.QuizShow;
            if (quiz == null)
                return;
            if (data.Length > 1 && data[1] == "stop")
            {
                quiz.Abort();
                client.SendSysMesage("Quiz Show stopped.");
                return;
            }
            if (quiz.Process != ProcesType.Dead)
            {
                client.SendSysMesage("A Quiz Show is already running (question " + quiz.QuestionNumber + "/" + MsgQuizShow.QuestionCount + ").");
                return;
            }
            if (Database.QuizShow.Questions.Count < MsgQuizShow.QuestionCount)
            {
                client.SendSysMesage("Quiz Show: only " + Database.QuizShow.Questions.Count + " questions loaded (quizquestins.ini).");
                return;
            }
            quiz.Open();
        }
    }
}
