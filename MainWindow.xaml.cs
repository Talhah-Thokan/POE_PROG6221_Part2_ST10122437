using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CyberBot.Models;
using CyberBot.Services;

namespace CyberBot
{
    public partial class MainWindow : Window
    {
        // Core services
        private ChatService chatService;
        private NLPService nlpService;
        private QuizService quizService;
        private ActivityLogService activityLogService;
        
        // Data collections
        private UserMemory userMemory;
        private ObservableCollection<SecurityTask> securityTasks;
        private ObservableCollection<ActivityLogEntry> activityLog;
        
        // Quiz state
        private List<QuizQuestion> currentQuiz;
        private int currentQuestionIndex;
        private int quizScore;
        private QuizQuestion? currentQuestion;
        
        // Reminder timer
        private DispatcherTimer reminderTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            InitializeData();
            InitializeTimers();
            LoadInitialContent();
        }

        private void InitializeServices()
        {
            chatService = new ChatService();
            nlpService = new NLPService();
            quizService = new QuizService();
            activityLogService = new ActivityLogService();
        }

        private void InitializeData()
        {
            userMemory = new UserMemory();
            securityTasks = new ObservableCollection<SecurityTask>();
            activityLog = new ObservableCollection<ActivityLogEntry>();
            
            // Bind collections to UI
            TasksListView.ItemsSource = securityTasks;
            ActivityLogListView.ItemsSource = activityLog;
        }

        private void InitializeTimers()
        {
            // Timer for checking reminders
            reminderTimer = new DispatcherTimer();
            reminderTimer.Interval = TimeSpan.FromMinutes(1); // Check every minute
            reminderTimer.Tick += ReminderTimer_Tick;
            reminderTimer.Start();
        }

        private void LoadInitialContent()
        {
            // Add welcome message to chat
            AddChatMessage("🤖 Bot", "Welcome to ThinkinBot Cyber Security Assistant Part 3! 🚀", true);
            AddChatMessage("🤖 Bot", "I'm your enhanced AI companion with new GUI features including task management, security quizzes, and activity logging!", true);
            AddChatMessage("🤖 Bot", "Try saying things like 'Remind me to update passwords' or 'Start a quiz' for new NLP features!", true);

            // Log initial activity
            activityLogService.LogActivity("Application Started", "ThinkinBot Cyber Part 3 launched", "System");
            RefreshActivityLog();
        }

        #region Chat Functionality (Part 1 & 2 Integration)

        private void SetNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                userMemory.Name = NameTextBox.Text.Trim();
                WelcomeText.Text = $"Hello {userMemory.Name}! Ask me about cybersecurity topics or try the new features in other tabs.";
                
                AddChatMessage(userMemory.Name, $"My name is {userMemory.Name}", false);
                AddChatMessage("🤖 Bot", $"Great to meet you, {userMemory.Name}! 💡", true);
                
                activityLogService.LogActivity("Name Set", $"User set name to: {userMemory.Name}", "Chat");
                RefreshActivityLog();
                
                NameTextBox.Clear();
            }
        }

        private void PlayAudioButton_Click(object sender, RoutedEventArgs e)
        {
            PlayGreetingAudio();
        }

        private void ChatInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string helpMessage = chatService.GetHelpMessage();
            AddChatMessage("🤖 Bot", helpMessage, true);
        }

        private async void SendMessage()
        {
            string userInput = ChatInputBox.Text.Trim();
            if (string.IsNullOrEmpty(userInput)) return;

            // Add user message to chat
            AddChatMessage(userMemory.Name, userInput, false);
            ChatInputBox.Clear();

            // Show typing indicator
            var typingMessage = AddChatMessage("🤖 Bot", "🤖 Bot is thinking...", true);
            
            // Simulate thinking delay
            await Task.Delay(1000);

            // Remove typing indicator
            ChatDisplay.Children.Remove(typingMessage);

            // Process NLP commands first
            var nlpCommand = nlpService.ParseCommand(userInput);
            if (!string.IsNullOrEmpty(nlpCommand.Action))
            {
                string nlpResponse = ProcessNLPCommand(nlpCommand);
                AddChatMessage("🤖 Bot", nlpResponse, true);
            }
            else
            {
                // Process normal chat
                string response = chatService.ProcessUserInput(userInput, userMemory);
                AddChatMessage("🤖 Bot", response, true);
            }

            // Log activity
            activityLogService.LogActivity("Chat Message", $"User: {userInput}", "Chat");
            RefreshActivityLog();

            // Auto-scroll to bottom
            ChatScrollViewer.ScrollToBottom();
        }

        private TextBlock AddChatMessage(string sender, string message, bool isBot)
        {
            var messageBlock = new TextBlock
            {
                Text = $"[{DateTime.Now:HH:mm}] {sender}: {message}",
                Style = isBot ? (Style)Resources["ChatTextStyle"] : (Style)Resources["UserTextStyle"],
                Margin = new Thickness(5, 2, 5, 2)
            };

            ChatDisplay.Children.Add(messageBlock);
            return messageBlock;
        }

        #endregion

        #region NLP Processing (Part 3 Feature)

        private string ProcessNLPCommand(NLPCommand command)
        {
            return command.Action switch
            {
                "AddTask" => ProcessAddTaskCommand(command),
                "ShowTasks" => ProcessShowTasksCommand(),
                "StartQuiz" => ProcessStartQuizCommand(),
                "ShowLog" => ProcessShowLogCommand(),
                _ => "🤖 Bot: I understood your intent but couldn't process that command. Try using the specific tabs for tasks and quizzes!"
            };
        }

        private string ProcessAddTaskCommand(NLPCommand command)
        {
            if (command.Parameters.ContainsKey("title"))
            {
                var task = new SecurityTask
                {
                    Title = command.Parameters["title"],
                    Description = "Added via chat command",
                    ReminderDateTime = DateTime.Now.AddDays(1) // Default reminder for 1 day
                };

                securityTasks.Add(task);
                activityLogService.LogActivity("Task Added", $"Task '{task.Title}' added via NLP", "Task");
                RefreshActivityLog();

                return $"✅ Great! I've added the task '{task.Title}' with a reminder for tomorrow. Check the Tasks tab to manage it!";
            }
            return "🤖 Bot: I'd love to help you add a task! Try saying 'Remind me to [task description]'";
        }

        private string ProcessShowTasksCommand()
        {
            if (securityTasks.Count == 0)
            {
                return "📋 You don't have any tasks yet. Head to the Tasks tab to add some security tasks!";
            }

            var pendingTasks = securityTasks.Where(t => !t.IsCompleted).Count();
            return $"📋 You have {securityTasks.Count} total tasks ({pendingTasks} pending). Check the Tasks tab for full details!";
        }

        private string ProcessStartQuizCommand()
        {
            return "🧠 Ready for a security quiz? Head over to the Quiz tab and click 'Start Quiz' to test your knowledge!";
        }

        private string ProcessShowLogCommand()
        {
            var recentCount = activityLogService.GetRecentActivities(5).Count;
            return $"📊 Your recent activity shows {recentCount} actions. Check the Activity Log tab for full details!";
        }

        #endregion

        #region Task Management (Part 3 Feature)

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskTitleBox.Text))
            {
                MessageBox.Show("Please enter a task title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var task = new SecurityTask
            {
                Title = TaskTitleBox.Text.Trim(),
                Description = TaskDescriptionBox.Text.Trim()
            };

            // Set reminder if date/time provided
            if (TaskReminderDate.SelectedDate.HasValue)
            {
                var reminderDate = TaskReminderDate.SelectedDate.Value;
                TimeSpan reminderTime = TimeSpan.Zero;
                
                // Parse time from TextBox
                if (!string.IsNullOrWhiteSpace(TaskReminderTime.Text) && 
                    TaskReminderTime.Text != "HH:MM" &&
                    TimeSpan.TryParse(TaskReminderTime.Text, out TimeSpan parsedTime))
                {
                    reminderTime = parsedTime;
                }
                
                task.ReminderDateTime = reminderDate.Add(reminderTime);
            }

            securityTasks.Add(task);

            // Clear form
            TaskTitleBox.Clear();
            TaskDescriptionBox.Clear();
            TaskReminderDate.SelectedDate = null;
            TaskReminderTime.Text = "HH:MM";

            // Log activity
            activityLogService.LogActivity("Task Added", $"Task '{task.Title}' created", "Task");
            RefreshActivityLog();

            MessageBox.Show($"Task '{task.Title}' added successfully!", "Task Added", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TasksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = TasksListView.SelectedItem != null;
            CompleteTaskButton.IsEnabled = hasSelection;
            DeleteTaskButton.IsEnabled = hasSelection;
        }

        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is SecurityTask selectedTask)
            {
                selectedTask.IsCompleted = true;
                activityLogService.LogActivity("Task Completed", $"Task '{selectedTask.Title}' marked as completed", "Task");
                RefreshActivityLog();
                MessageBox.Show($"Task '{selectedTask.Title}' completed!", "Task Completed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is SecurityTask selectedTask)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the task '{selectedTask.Title}'?", 
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    securityTasks.Remove(selectedTask);
                    activityLogService.LogActivity("Task Deleted", $"Task '{selectedTask.Title}' deleted", "Task");
                    RefreshActivityLog();
                }
            }
        }

        private void ReminderTimer_Tick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            var dueReminders = securityTasks.Where(t => 
                !t.IsCompleted && 
                t.ReminderDateTime.HasValue && 
                t.ReminderDateTime.Value <= now &&
                t.ReminderDateTime.Value > now.AddMinutes(-1) // Only show once per reminder
            ).ToList();

            foreach (var task in dueReminders)
            {
                MessageBox.Show($"⏰ Reminder: {task.Title}\n\n{task.Description}", 
                    "Security Task Reminder", MessageBoxButton.OK, MessageBoxImage.Information);
                
                activityLogService.LogActivity("Reminder Triggered", $"Reminder for task '{task.Title}'", "Reminder");
            }

            if (dueReminders.Any())
            {
                RefreshActivityLog();
            }
        }

        #endregion

        #region Quiz Management (Part 3 Feature)

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            currentQuiz = quizService.GetQuestions();
            currentQuestionIndex = 0;
            quizScore = 0;
            
            StartQuizPanel.Visibility = Visibility.Collapsed;
            QuestionPanel.Visibility = Visibility.Visible;
            NextQuestionButton.Visibility = Visibility.Visible;
            
            activityLogService.LogActivity("Quiz Started", "User started cybersecurity quiz", "Quiz");
            RefreshActivityLog();
            
            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            if (currentQuestionIndex >= currentQuiz.Count)
            {
                ShowQuizResults();
                return;
            }

            currentQuestion = currentQuiz[currentQuestionIndex];
            QuestionText.Text = $"Question {currentQuestionIndex + 1}: {currentQuestion.Question}";
            QuizScoreText.Text = $"Score: {quizScore}/{currentQuestionIndex} | Question {currentQuestionIndex + 1} of {currentQuiz.Count}";

            // Clear previous answers
            AnswersPanel.Children.Clear();
            FeedbackText.Visibility = Visibility.Collapsed;
            NextQuestionButton.Visibility = Visibility.Collapsed;

            // Add answer options
            for (int i = 0; i < currentQuestion.Options.Count; i++)
            {
                var radioButton = new RadioButton
                {
                    Content = currentQuestion.Options[i],
                    GroupName = "QuizAnswers",
                    Margin = new Thickness(0, 5, 0, 5),
                    Foreground = Brushes.White,
                    FontSize = 14,
                    Tag = i
                };
                radioButton.Checked += QuizAnswer_Checked;
                AnswersPanel.Children.Add(radioButton);
            }
        }

        private void QuizAnswer_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && currentQuestion != null)
            {
                int selectedIndex = (int)radioButton.Tag;
                bool isCorrect = selectedIndex == currentQuestion.CorrectAnswerIndex;

                if (isCorrect)
                {
                    quizScore++;
                    FeedbackText.Text = $"✅ Correct! {currentQuestion.Explanation}";
                    FeedbackText.Foreground = Brushes.LightGreen;
                }
                else
                {
                    FeedbackText.Text = $"❌ Incorrect. The correct answer is: {currentQuestion.CorrectAnswer}\n{currentQuestion.Explanation}";
                    FeedbackText.Foreground = Brushes.LightCoral;
                }

                FeedbackText.Visibility = Visibility.Visible;
                NextQuestionButton.Visibility = Visibility.Visible;

                // Disable all radio buttons after answer
                foreach (RadioButton rb in AnswersPanel.Children.OfType<RadioButton>())
                {
                    rb.IsEnabled = false;
                }

                activityLogService.LogActivity("Quiz Answer", $"Question {currentQuestionIndex + 1}: {(isCorrect ? "Correct" : "Incorrect")}", "Quiz");
            }
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex++;
            ShowCurrentQuestion();
        }

        private void ShowQuizResults()
        {
            QuestionPanel.Visibility = Visibility.Collapsed;
            NextQuestionButton.Visibility = Visibility.Collapsed;
            ResultsPanel.Visibility = Visibility.Visible;

            FinalScoreText.Text = $"Final Score: {quizScore}/{currentQuiz.Count}";
            ResultMessageText.Text = quizService.GetResultMessage(quizScore, currentQuiz.Count);

            activityLogService.LogActivity("Quiz Completed", $"Final score: {quizScore}/{currentQuiz.Count}", "Quiz");
            RefreshActivityLog();
        }

        private void RestartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsPanel.Visibility = Visibility.Collapsed;
            StartQuizPanel.Visibility = Visibility.Visible;
            QuizScoreText.Text = "Score: 0/0 | Question 0 of 10";
        }

        #endregion

        #region Activity Log Management (Part 3 Feature)

        private void RefreshActivityLog()
        {
            activityLog.Clear();
            var activities = activityLogService.GetRecentActivities(10);
            foreach (var activity in activities)
            {
                activityLog.Add(activity);
            }
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the activity log?", 
                "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                activityLogService.ClearLog();
                RefreshActivityLog();
                MessageBox.Show("Activity log cleared.", "Log Cleared", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportLogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allActivities = activityLogService.GetAllActivities();
                var fileName = $"CyberBot_ActivityLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("ThinkinBot Cyber Activity Log");
                    writer.WriteLine($"Generated: {DateTime.Now}");
                    writer.WriteLine($"User: {userMemory.Name}");
                    writer.WriteLine(new string('=', 50));
                    writer.WriteLine();

                    foreach (var activity in allActivities)
                    {
                        writer.WriteLine($"[{activity.Timestamp:yyyy-MM-dd HH:mm:ss}] {activity.Type}: {activity.Action}");
                        writer.WriteLine($"  Details: {activity.Details}");
                        writer.WriteLine();
                    }
                }

                MessageBox.Show($"Activity log exported to:\n{filePath}", "Export Successful", 
                    MessageBoxButton.OK, MessageBoxImage.Information);

                activityLogService.LogActivity("Log Exported", $"Activity log exported to {fileName}", "System");
                RefreshActivityLog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting log: {ex.Message}", "Export Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Audio Functionality (Part 1 Integration)

        private void PlayGreetingAudio()
        {
            try
            {
                string[] possiblePaths = {
                    "assets/greeting.wav",
                    "../assets/greeting.wav",
                    "../../assets/greeting.wav",
                    "./assets/greeting.wav"
                };

                string? audioPath = possiblePaths.FirstOrDefault(File.Exists);

                if (audioPath != null)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        // Use SoundPlayer for Windows
                        var player = new SoundPlayer(Path.GetFullPath(audioPath));
                        player.Play();
                    }
                    else
                    {
                        // Use cross-platform approach for other systems
                        ProcessStartInfo psi = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                            ? new ProcessStartInfo("afplay", audioPath)
                            : new ProcessStartInfo("aplay", audioPath);

                        psi.UseShellExecute = false;
                        psi.CreateNoWindow = true;
                        Process.Start(psi);
                    }

                    AddChatMessage("🤖 Bot", "🔊 Playing audio greeting...", true);
                    activityLogService.LogActivity("Audio Played", "Greeting audio played", "System");
                }
                else
                {
                    AddChatMessage("🤖 Bot", "⚠️ Audio greeting file not found. Please ensure 'greeting.wav' exists in the 'assets' folder.", true);
                }
            }
            catch (Exception ex)
            {
                AddChatMessage("🤖 Bot", $"⚠️ Could not play audio greeting: {ex.Message}", true);
            }

            RefreshActivityLog();
        }

        #endregion

        protected override void OnClosed(EventArgs e)
        {
            reminderTimer?.Stop();
            activityLogService.LogActivity("Application Closed", "ThinkinBot Cyber Part 3 closed", "System");
            base.OnClosed(e);
        }
    }
} 