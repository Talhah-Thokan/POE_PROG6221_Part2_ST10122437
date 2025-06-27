using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CyberBot.Models
{
    // User memory and context management from Part 2
    public class UserMemory
    {
        public string Name { get; set; } = "Guest";
        public string FavoriteTopic { get; set; } = "";
        public List<string> TopicsDiscussed { get; set; } = new List<string>();
        public string CurrentTopic { get; set; } = "";
        public string LastSentiment { get; set; } = "neutral";
    }

    // Security Task model for Task Assistant feature
    public class SecurityTask : INotifyPropertyChanged
    {
        private string _title = "";
        private string _description = "";
        private string _status = "Pending";
        private DateTime? _reminderDateTime;
        private bool _isCompleted = false;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public DateTime? ReminderDateTime
        {
            get => _reminderDateTime;
            set
            {
                _reminderDateTime = value;
                OnPropertyChanged(nameof(ReminderDateTime));
                OnPropertyChanged(nameof(ReminderText));
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                Status = value ? "Completed" : "Pending";
                OnPropertyChanged(nameof(IsCompleted));
            }
        }

        public string ReminderText
        {
            get
            {
                if (ReminderDateTime.HasValue)
                {
                    return ReminderDateTime.Value.ToString("MM/dd HH:mm");
                }
                return "No reminder";
            }
        }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Quiz Question model for Quiz Game feature
    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; } = "";
        public QuizQuestionType Type { get; set; } = QuizQuestionType.MultipleChoice;

        public string CorrectAnswer => Options.Count > CorrectAnswerIndex ? Options[CorrectAnswerIndex] : "";
    }

    public enum QuizQuestionType
    {
        MultipleChoice,
        TrueFalse
    }

    // Activity Log Entry model for Activity Log feature
    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Action { get; set; } = "";
        public string Details { get; set; } = "";
        public string Type { get; set; } = "";

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {Action}: {Details}";
        }
    }

    // Chat Message model for chat display
    public class ChatMessage
    {
        public string Sender { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsBot { get; set; } = true;
    }

    // NLP Command model for natural language processing
    public class NLPCommand
    {
        public string Command { get; set; } = "";
        public string Action { get; set; } = "";
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
} 