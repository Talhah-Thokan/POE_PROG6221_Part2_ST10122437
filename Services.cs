using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CyberBot.Models;

namespace CyberBot.Services
{
    // Chat Service - migrated from console app Part 1 & 2
    public class ChatService
    {
        private static Random random = new Random();
        
        // Enhanced response dictionaries from Part 2
        private static Dictionary<string, List<string>> topicResponses = new Dictionary<string, List<string>>
        {
            ["password"] = new List<string>
            {
                "🔐 Bot: Create strong passwords with 12+ characters, mixing letters, numbers, and symbols. Each account should have a unique password!",
                "🔐 Bot: Password managers like Bitwarden or LastPass can generate and store secure passwords for you. They're a game-changer!",
                "🔐 Bot: Avoid common passwords like '123456' or 'password'. Consider using passphrases like 'Coffee$Runs&Morning2024' instead!"
            },
            ["phishing"] = new List<string>
            {
                "🎣 Bot: Phishing emails often create urgency ('Your account will be closed!'). Take a moment to verify the sender before clicking anything.",
                "🎣 Bot: Check URLs carefully - phishers use lookalike domains (amaz0n.com vs amazon.com). When in doubt, navigate directly to the website.",
                "🎣 Bot: Legitimate companies rarely ask for passwords via email. If suspicious, contact the company directly through their official channels."
            },
            ["scam"] = new List<string>
            {
                "⚠️ Bot: Common scams include fake tech support calls, romance scams, and prize notifications. If it sounds too good to be true, it probably is!",
                "⚠️ Bot: Scammers often ask for immediate payment or personal info. Legitimate businesses give you time to think and verify their claims.",
                "⚠️ Bot: Trust your instincts! If something feels off, step back and verify independently. Scammers rely on pressure and emotions."
            },
            ["privacy"] = new List<string>
            {
                "🛡️ Bot: Review your social media privacy settings regularly. Limit who can see your posts, location, and personal information.",
                "🛡️ Bot: Be cautious about what you share online - personal details can be used for identity theft or social engineering attacks.",
                "🛡️ Bot: Use privacy-focused browsers and search engines like DuckDuckGo to reduce tracking of your online activities."
            },
            ["vpn"] = new List<string>
            {
                "🌐 Bot: VPNs encrypt your internet traffic, making it harder for hackers to intercept your data, especially on public Wi-Fi.",
                "🌐 Bot: Choose reputable VPN providers that don't log your activity. Free VPNs often come with privacy trade-offs.",
                "🌐 Bot: VPNs can also help you bypass geo-restrictions, but their main benefit is protecting your privacy and security online."
            },
            ["malware"] = new List<string>
            {
                "💻 Bot: Keep your antivirus updated and scan regularly. Modern threats evolve quickly, so fresh definitions are crucial!",
                "💻 Bot: Avoid downloading software from unknown sources. Stick to official websites and app stores to minimize malware risk.",
                "💻 Bot: Be cautious with email attachments and USB drives from unknown sources - they're common malware delivery methods."
            }
        };

        // Sentiment detection keywords from Part 2
        private static Dictionary<string, List<string>> sentimentKeywords = new Dictionary<string, List<string>>
        {
            ["worried"] = new List<string> { "worried", "scared", "afraid", "nervous", "anxious", "concerned" },
            ["curious"] = new List<string> { "curious", "interested", "want to know", "tell me more", "explain", "how" },
            ["frustrated"] = new List<string> { "frustrated", "annoyed", "confused", "difficult", "hard", "complicated" },
            ["confident"] = new List<string> { "confident", "sure", "understand", "got it", "clear", "easy" }
        };

        // Follow-up conversation triggers from Part 2
        private static List<string> followUpTriggers = new List<string>
        {
            "tell me more", "more info", "elaborate", "explain further", "I'm confused", "I don't understand", "continue", "what else"
        };

        public string ProcessUserInput(string input, UserMemory userMemory)
        {
            string lowerInput = input.ToLower();
            
            // Detect and store sentiment
            string detectedSentiment = DetectSentiment(lowerInput);
            userMemory.LastSentiment = detectedSentiment;

            // Check for follow-up conversation
            if (IsFollowUpRequest(lowerInput))
            {
                return HandleFollowUp(userMemory);
            }

            // Detect favorite topic and store in memory
            DetectAndStoreFavoriteTopic(lowerInput, userMemory);

            // Enhanced keyword recognition with random responses
            string detectedKeyword = DetectCybersecurityKeyword(lowerInput);
            if (!string.IsNullOrEmpty(detectedKeyword))
            {
                userMemory.CurrentTopic = detectedKeyword;
                if (!userMemory.TopicsDiscussed.Contains(detectedKeyword))
                {
                    userMemory.TopicsDiscussed.Add(detectedKeyword);
                }
                return RespondWithRandomTip(detectedKeyword, detectedSentiment, userMemory);
            }

            // Handle basic conversation
            string basicResponse = HandleBasicConversation(lowerInput, detectedSentiment, userMemory);
            if (!string.IsNullOrEmpty(basicResponse))
            {
                return basicResponse;
            }

            // Default response with sentiment consideration
            return HandleUnrecognizedInput(detectedSentiment, userMemory);
        }

        private string DetectSentiment(string input)
        {
            foreach (var sentiment in sentimentKeywords)
            {
                if (sentiment.Value.Any(keyword => input.Contains(keyword)))
                {
                    return sentiment.Key;
                }
            }
            return "neutral";
        }

        private bool IsFollowUpRequest(string input)
        {
            return followUpTriggers.Any(trigger => input.Contains(trigger));
        }

        private string HandleFollowUp(UserMemory userMemory)
        {
            if (string.IsNullOrEmpty(userMemory.CurrentTopic))
            {
                return "🤖 Bot: I'd be happy to elaborate! What specific cybersecurity topic would you like to know more about?";
            }

            return userMemory.CurrentTopic switch
            {
                "password" => "🔐 Bot: Here's a pro tip: Enable two-factor authentication (2FA) wherever possible. Even if your password is compromised, 2FA adds an extra security layer!",
                "phishing" => "🎣 Bot: Advanced tip: Use email filters and report phishing attempts to help protect others. Most email providers have built-in phishing detection you can enable.",
                "privacy" => "🛡️ Bot: Consider using encrypted messaging apps like Signal for sensitive conversations, and regularly audit what apps have access to your data.",
                _ => RespondWithRandomTip(userMemory.CurrentTopic, userMemory.LastSentiment, userMemory)
            };
        }

        private void DetectAndStoreFavoriteTopic(string input, UserMemory userMemory)
        {
            string[] interestPhrases = { "interested in", "care about", "want to learn", "focus on", "worried about" };
            
            foreach (string phrase in interestPhrases)
            {
                if (input.Contains(phrase))
                {
                    foreach (string topic in topicResponses.Keys)
                    {
                        if (input.Contains(topic))
                        {
                            userMemory.FavoriteTopic = topic;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private string DetectCybersecurityKeyword(string input)
        {
            foreach (string keyword in topicResponses.Keys)
            {
                if (input.Contains(keyword))
                {
                    return keyword;
                }
            }

            // Additional keyword variations
            if (input.Contains("2fa") || input.Contains("two factor") || input.Contains("authentication"))
                return "password";
            
            if (input.Contains("social engineering") || input.Contains("manipulation"))
                return "scam";
                
            if (input.Contains("safe browsing") || input.Contains("browser security"))
                return "privacy";

            return "";
        }

        private string RespondWithRandomTip(string keyword, string sentiment, UserMemory userMemory)
        {
            if (!topicResponses.ContainsKey(keyword))
                return "";

            var responses = topicResponses[keyword];
            string selectedResponse = responses[random.Next(responses.Count)];
            string sentimentPrefix = GetSentimentPrefix(sentiment, userMemory.Name);
            
            return sentimentPrefix + selectedResponse + GetPersonalizedContext(keyword, userMemory);
        }

        private string GetSentimentPrefix(string sentiment, string name)
        {
            return sentiment switch
            {
                "worried" => $"🤗 Bot: I understand you're concerned, {name}. Let me help ease your worries. ",
                "frustrated" => $"😌 Bot: I know this can be confusing, {name}. Let me break it down simply. ",
                "curious" => $"🧠 Bot: Great question, {name}! Your curiosity about security is admirable. ",
                "confident" => $"👍 Bot: You seem to have a good grasp on this, {name}! Here's some additional insight: ",
                _ => ""
            };
        }

        private string GetPersonalizedContext(string currentTopic, UserMemory userMemory)
        {
            if (!string.IsNullOrEmpty(userMemory.FavoriteTopic) && 
                userMemory.FavoriteTopic != currentTopic && 
                userMemory.TopicsDiscussed.Count > 1)
            {
                return $"\n\n💡 Bot: Since you're also interested in {userMemory.FavoriteTopic}, you might find these topics connect - strong {userMemory.FavoriteTopic} practices often complement {currentTopic} security!";
            }
            return "";
        }

        private string HandleBasicConversation(string input, string sentiment, UserMemory userMemory)
        {
            if (input.Contains("how are you"))
            {
                return sentiment == "worried" 
                    ? "🤖 Bot: I'm doing well, and I'm here to help you feel more secure online! What's concerning you?"
                    : "🤖 Bot: I'm excellent! Always ready to boost your cyber knowledge. 💥";
            }
            
            if (input.Contains("purpose") || input.Contains("what can you do"))
            {
                string response = "🤖 Bot: I'm designed to educate and empower you with cybersecurity knowledge for Part 3 of your learning journey!";
                if (userMemory.TopicsDiscussed.Count > 0)
                {
                    response += $"\n💭 Bot: We've already covered: {string.Join(", ", userMemory.TopicsDiscussed)}. What else interests you?";
                }
                return response;
            }

            return "";
        }

        private string HandleUnrecognizedInput(string sentiment, UserMemory userMemory)
        {
            List<string> defaultResponses = new List<string>
            {
                "🤖 Bot: I'm not sure I understand. Can you rephrase that?",
                "🤖 Bot: That's interesting, but I didn't catch a cybersecurity topic. Try asking about passwords, phishing, or privacy!",
                "🤖 Bot: Hmm, I didn't recognize that. Type 'help' to see what I can assist you with!"
            };

            string baseResponse = defaultResponses[random.Next(defaultResponses.Count)];
            
            if (sentiment == "frustrated")
            {
                baseResponse += $" I know this can be frustrating, {userMemory.Name}. Let's try a different approach!";
            }
            else if (sentiment == "worried")
            {
                baseResponse += " Don't worry - I'm here to help you navigate cybersecurity safely.";
            }

            if (userMemory.TopicsDiscussed.Count > 0)
            {
                baseResponse += $"\n💡 Bot: Or we could continue discussing {userMemory.TopicsDiscussed.Last()} if you'd like more details!";
            }

            return baseResponse;
        }

        public string GetHelpMessage()
        {
            return @"📘 Bot Help Menu (Part 3 Enhanced):
Here are some things you can ask me:
 - How are you?
 - What's your purpose?
 - I'm interested in [topic] (saves your preference!)
 - Tell me about passwords
 - What is phishing?
 - Explain privacy protection
 - I'm worried about scams
 - Tell me more (continues current topic)

💡 Part 3 Features:
 - I remember your name and interests
 - I detect if you're worried, curious, or frustrated
 - I provide random educational tips from my knowledge base
 - I can continue conversations on topics you're exploring
 - Try saying 'Remind me to...' or 'Add a task for...' for task management!";
        }
    }

    // NLP Service for natural language processing
    public class NLPService
    {
        public NLPCommand ParseCommand(string input)
        {
            var command = new NLPCommand { Command = input };
            string lowerInput = input.ToLower();

            // Task-related commands
            if (Regex.IsMatch(lowerInput, @"(remind me to|add a task|create a task|add task)"))
            {
                command.Action = "AddTask";
                
                // Extract task title
                var taskMatch = Regex.Match(lowerInput, @"(?:remind me to|add a task|create a task|add task)\s+(.+)");
                if (taskMatch.Success)
                {
                    command.Parameters["title"] = taskMatch.Groups[1].Value.Trim();
                }
            }
            else if (Regex.IsMatch(lowerInput, @"(show tasks|view tasks|list tasks|my tasks)"))
            {
                command.Action = "ShowTasks";
            }
            else if (Regex.IsMatch(lowerInput, @"(take quiz|start quiz|quiz me)"))
            {
                command.Action = "StartQuiz";
            }
            else if (Regex.IsMatch(lowerInput, @"(show log|view log|activity log|recent activity)"))
            {
                command.Action = "ShowLog";
            }

            return command;
        }
    }

    // Quiz Service for managing quiz functionality
    public class QuizService
    {
        private List<QuizQuestion> questions = new List<QuizQuestion>
        {
            new QuizQuestion
            {
                Question = "What is the recommended minimum length for a strong password?",
                Options = new List<string> { "6 characters", "8 characters", "12 characters", "20 characters" },
                CorrectAnswerIndex = 2,
                Explanation = "12+ characters provide better security against brute force attacks.",
                Type = QuizQuestionType.MultipleChoice
            },
            new QuizQuestion
            {
                Question = "What does 'phishing' refer to in cybersecurity?",
                Options = new List<string> { "Fishing for compliments", "Attempting to steal personal information", "Network monitoring", "Data backup" },
                CorrectAnswerIndex = 1,
                Explanation = "Phishing is a social engineering attack used to steal user data and credentials.",
                Type = QuizQuestionType.MultipleChoice
            },
            new QuizQuestion
            {
                Question = "Should you use the same password for multiple accounts?",
                Options = new List<string> { "True", "False" },
                CorrectAnswerIndex = 1,
                Explanation = "Using unique passwords for each account prevents credential stuffing attacks.",
                Type = QuizQuestionType.TrueFalse
            },
            new QuizQuestion
            {
                Question = "What does VPN stand for?",
                Options = new List<string> { "Virtual Private Network", "Very Personal Network", "Verified Protection Node", "Virtual Protection Network" },
                CorrectAnswerIndex = 0,
                Explanation = "VPN creates a secure, encrypted connection over the internet.",
                Type = QuizQuestionType.MultipleChoice
            },
            new QuizQuestion
            {
                Question = "Two-factor authentication adds an extra layer of security.",
                Options = new List<string> { "True", "False" },
                CorrectAnswerIndex = 0,
                Explanation = "2FA requires something you know (password) and something you have (phone/token).",
                Type = QuizQuestionType.TrueFalse
            },
            new QuizQuestion
            {
                Question = "Which of these is NOT a common social engineering technique?",
                Options = new List<string> { "Pretexting", "Baiting", "Encryption", "Tailgating" },
                CorrectAnswerIndex = 2,
                Explanation = "Encryption is a security measure, not a social engineering technique.",
                Type = QuizQuestionType.MultipleChoice
            },
            new QuizQuestion
            {
                Question = "It's safe to connect to any public Wi-Fi network.",
                Options = new List<string> { "True", "False" },
                CorrectAnswerIndex = 1,
                Explanation = "Public Wi-Fi can be insecure; use VPN and avoid sensitive activities.",
                Type = QuizQuestionType.TrueFalse
            },
            new QuizQuestion
            {
                Question = "What should you do if you receive a suspicious email?",
                Options = new List<string> { "Click all links to investigate", "Forward it to friends", "Report it as spam/phishing", "Reply asking for verification" },
                CorrectAnswerIndex = 2,
                Explanation = "Report suspicious emails and never click unknown links or attachments.",
                Type = QuizQuestionType.MultipleChoice
            },
            new QuizQuestion
            {
                Question = "Regular software updates help protect against security vulnerabilities.",
                Options = new List<string> { "True", "False" },
                CorrectAnswerIndex = 0,
                Explanation = "Updates often include security patches for newly discovered vulnerabilities.",
                Type = QuizQuestionType.TrueFalse
            },
            new QuizQuestion
            {
                Question = "Which is the most secure way to store passwords?",
                Options = new List<string> { "Write them down on paper", "Save in browser", "Use a password manager", "Memorize all of them" },
                CorrectAnswerIndex = 2,
                Explanation = "Password managers encrypt and securely store all your passwords.",
                Type = QuizQuestionType.MultipleChoice
            }
        };

        public List<QuizQuestion> GetQuestions()
        {
            return questions.OrderBy(x => Guid.NewGuid()).ToList(); // Randomize order
        }

        public string GetResultMessage(int score, int total)
        {
            double percentage = (double)score / total * 100;
            
            return percentage switch
            {
                >= 90 => "🏆 Excellent! You're a cybersecurity expert!",
                >= 80 => "🎉 Great job! You have strong security knowledge!",
                >= 70 => "👍 Good work! You understand the basics well!",
                >= 60 => "📚 Not bad! Consider reviewing some security concepts.",
                _ => "💪 Keep learning! Cybersecurity takes practice."
            };
        }
    }

    // Activity Log Service for tracking user actions
    public class ActivityLogService
    {
        private List<ActivityLogEntry> logEntries = new List<ActivityLogEntry>();
        private const int MaxLogEntries = 50;

        public void LogActivity(string action, string details, string type = "General")
        {
            var entry = new ActivityLogEntry
            {
                Action = action,
                Details = details,
                Type = type,
                Timestamp = DateTime.Now
            };

            logEntries.Insert(0, entry); // Add to beginning for newest first

            // Keep only the most recent entries
            if (logEntries.Count > MaxLogEntries)
            {
                logEntries.RemoveRange(MaxLogEntries, logEntries.Count - MaxLogEntries);
            }
        }

        public List<ActivityLogEntry> GetRecentActivities(int count = 10)
        {
            return logEntries.Take(count).ToList();
        }

        public void ClearLog()
        {
            logEntries.Clear();
        }

        public List<ActivityLogEntry> GetAllActivities()
        {
            return logEntries.ToList();
        }
    }
} 