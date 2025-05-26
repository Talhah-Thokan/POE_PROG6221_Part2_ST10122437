using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace CyberBot
{
    // User memory and context management
    public class UserMemory 
    {
        public string Name { get; set; } = "Guest";
        public string FavoriteTopic { get; set; } = "";
        public List<string> TopicsDiscussed { get; set; } = new List<string>();
        public string CurrentTopic { get; set; } = "";
        public string LastSentiment { get; set; } = "neutral";
    }

    class Program
    {
        private static UserMemory userMemory = new UserMemory();
        private static Random random = new Random();
        
        // Enhanced response dictionaries for Part 2
        private static Dictionary<string, List<string>> topicResponses = new Dictionary<string, List<string>>
        {
            ["password"] = new List<string> // password security
            {
                "🔐 Bot: Create strong passwords with 12+ characters, mixing letters, numbers, and symbols. Each account should have a unique password!",
                "🔐 Bot: Password managers like Bitwarden or LastPass can generate and store secure passwords for you. They're a game-changer!",
                "🔐 Bot: Avoid common passwords like '123456' or 'password'. Consider using passphrases like 'Coffee$Runs&Morning2024' instead!"
            },
            ["phishing"] = new List<string> // phishing
            {
                "🎣 Bot: Phishing emails often create urgency ('Your account will be closed!'). Take a moment to verify the sender before clicking anything.",
                "🎣 Bot: Check URLs carefully - phishers use lookalike domains (amaz0n.com vs amazon.com). When in doubt, navigate directly to the website.",
                "🎣 Bot: Legitimate companies rarely ask for passwords via email. If suspicious, contact the company directly through their official channels."
            },
            ["scam"] = new List<string> // scam
            {
                "⚠️ Bot: Common scams include fake tech support calls, romance scams, and prize notifications. If it sounds too good to be true, it probably is!",
                "⚠️ Bot: Scammers often ask for immediate payment or personal info. Legitimate businesses give you time to think and verify their claims.",
                "⚠️ Bot: Trust your instincts! If something feels off, step back and verify independently. Scammers rely on pressure and emotions."
            },
            ["privacy"] = new List<string> // privacy
            {
                "🛡️ Bot: Review your social media privacy settings regularly. Limit who can see your posts, location, and personal information.",
                "🛡️ Bot: Be cautious about what you share online - personal details can be used for identity theft or social engineering attacks.",
                "🛡️ Bot: Use privacy-focused browsers and search engines like DuckDuckGo to reduce tracking of your online activities."
            },
            ["vpn"] = new List<string> // vpn
            {
                "🌐 Bot: VPNs encrypt your internet traffic, making it harder for hackers to intercept your data, especially on public Wi-Fi.",
                "🌐 Bot: Choose reputable VPN providers that don't log your activity. Free VPNs often come with privacy trade-offs.",
                "🌐 Bot: VPNs can also help you bypass geo-restrictions, but their main benefit is protecting your privacy and security online."
            },
            ["malware"] = new List<string> // malware
            {
                "💻 Bot: Keep your antivirus updated and scan regularly. Modern threats evolve quickly, so fresh definitions are crucial!",
                "💻 Bot: Avoid downloading software from unknown sources. Stick to official websites and app stores to minimize malware risk.",
                "💻 Bot: Be cautious with email attachments and USB drives from unknown sources - they're common malware delivery methods."
            }
        };

        // Sentiment detection keywords
        private static Dictionary<string, List<string>> sentimentKeywords = new Dictionary<string, List<string>> // sentiment detection
        {
            ["worried"] = new List<string> { "worried", "scared", "afraid", "nervous", "anxious", "concerned" },
            ["curious"] = new List<string> { "curious", "interested", "want to know", "tell me more", "explain", "how" },
            ["frustrated"] = new List<string> { "frustrated", "annoyed", "confused", "difficult", "hard", "complicated" },
            ["confident"] = new List<string> { "confident", "sure", "understand", "got it", "clear", "easy" }
        };

        // Follow-up conversation triggers
        private static List<string> followUpTriggers = new List<string> // follow-up conversation triggers
        {
            "tell me more", "more info", "elaborate", "explain further", "I'm confused", "I don't understand", "continue", "what else"
        };

        static void Main(string[] args)
        {
            PlayGreetingAudio(); // Plays the greeting audio
            ShowHeader(); // Displays the header with ASCII art and title
            DisplayWelcomeMessage(); // Displays the welcome message for Part 2

            // Get user name and store in memory
            Console.Write("🤖 Bot: Let's start with your name: ");
            string? userInput = Console.ReadLine();
            userMemory.Name = string.IsNullOrWhiteSpace(userInput) ? "Guest" : userInput.Trim(); // if the user input is empty, set the name to "Guest"
            Console.WriteLine($"🤖 Bot: Great to meet you, {userMemory.Name}! 💡"); // greets the user

            ShowIntro(); // Displays the introduction message

            // Main chat loop with enhanced Part 2 features
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n{userMemory.Name}: "); // displays the user's name
                Console.ResetColor();

                string? inputText = Console.ReadLine()?.Trim(); // reads the user's input
                string input = inputText ?? ""; // if the user input is empty, set the input to an empty string

                if (string.IsNullOrWhiteSpace(input)) // if the user input is empty, display a message
                {
                    Console.WriteLine("🤖 Bot: Please type something so I can help you stay secure online!"); // if the user input is empty, display a message
                    continue;
                }

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) || 
                    input.Equals("quit", StringComparison.OrdinalIgnoreCase)) // if the user input is "exit" or "quit", display a farewell message
                {
                    ShowFarewellMessage(); // displays the farewell message
                    break;
                }

                if (input.Equals("help", StringComparison.OrdinalIgnoreCase)) // if the user input is "help", display the help menu
                {
                    ShowHelp(); // displays the help menu
                    continue;
                }

                // Enhanced response system for Part 2
                ProcessUserInput(input);
            }
        }

        static void ProcessUserInput(string input)
        {
            string lowerInput = input.ToLower();
            
            // Detect and store sentiment
            string detectedSentiment = DetectSentiment(lowerInput); // detects the sentiment of the user's input
            userMemory.LastSentiment = detectedSentiment; // stores the sentiment of the user's input

            // Check for follow-up conversation
            if (IsFollowUpRequest(lowerInput)) // if the user input is a follow-up conversation trigger, handle the follow-up conversation
            {
                HandleFollowUp(); // handles the follow-up conversation
                return;
            }

            // Detect favorite topic and store in memory
            DetectAndStoreFavoriteTopic(lowerInput);

            // Enhanced keyword recognition with random responses
            string detectedKeyword = DetectCybersecurityKeyword(lowerInput);
            if (!string.IsNullOrEmpty(detectedKeyword))
            {
                userMemory.CurrentTopic = detectedKeyword;
                if (!userMemory.TopicsDiscussed.Contains(detectedKeyword))
                {
                    userMemory.TopicsDiscussed.Add(detectedKeyword);
                }
                RespondWithRandomTip(detectedKeyword, detectedSentiment); // responds with a random tip based on the detected keyword and sentiment
                return;
            }

            // Handle basic conversation
            if (HandleBasicConversation(lowerInput, detectedSentiment))
            {
                return;
            }

            // Default response with sentiment consideration
            HandleUnrecognizedInput(detectedSentiment);
        }

        static string DetectSentiment(string input)
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

        static bool IsFollowUpRequest(string input)
        {
            return followUpTriggers.Any(trigger => input.Contains(trigger));
        }

        static void HandleFollowUp()
        {
            if (string.IsNullOrEmpty(userMemory.CurrentTopic)) // if the current topic is empty, display a message
            { 
                Console.WriteLine("🤖 Bot: I'd be happy to elaborate! What specific cybersecurity topic would you like to know more about?");
                return;
            }

            Console.WriteLine($"🤖 Bot: Let me share more about {userMemory.CurrentTopic}..."); // shares more about the current topic
            
            // Provide additional context based on current topic
            switch (userMemory.CurrentTopic)
            {
                case "password":
                    Console.WriteLine("🔐 Bot: Here's a pro tip: Enable two-factor authentication (2FA) wherever possible. Even if your password is compromised, 2FA adds an extra security layer!");
                    break;
                case "phishing":
                    Console.WriteLine("🎣 Bot: Advanced tip: Use email filters and report phishing attempts to help protect others. Most email providers have built-in phishing detection you can enable.");
                    break;
                case "privacy":
                    Console.WriteLine("🛡️ Bot: Consider using encrypted messaging apps like Signal for sensitive conversations, and regularly audit what apps have access to your data.");
                    break;
                default:
                    RespondWithRandomTip(userMemory.CurrentTopic, userMemory.LastSentiment);
                    break;
            }
        }

        static void DetectAndStoreFavoriteTopic(string input) // detects and stores the user's favorite topic
        {
            string[] interestPhrases = { "interested in", "care about", "want to learn", "focus on", "worried about" }; // interest phrases
            
            foreach (string phrase in interestPhrases)
            {
                if (input.Contains(phrase))
                {
                    foreach (string topic in topicResponses.Keys) // iterates through the topic responses
                    {
                        if (input.Contains(topic))
                        {
                            userMemory.FavoriteTopic = topic;
                            Console.WriteLine($"🤖 Bot: I've noted that you're interested in {topic} security, {userMemory.Name}!");
                            break;
                        }
                    }
                    break;
                }
            }
        }

        static string DetectCybersecurityKeyword(string input) // detects the cybersecurity keyword in the user's input
        {
            // Enhanced keyword detection for Part 2
            foreach (string keyword in topicResponses.Keys)
            {
                if (input.Contains(keyword))
                {
                    return keyword; // returns the keyword
                }
            }

            // Additional keyword variations
            if (input.Contains("2fa") || input.Contains("two factor") || input.Contains("authentication"))
                return "password"; // Group with password security
            
            if (input.Contains("social engineering") || input.Contains("manipulation")) // if the user input contains "social engineering" or "manipulation", return "scam"
                return "scam";
                
            if (input.Contains("safe browsing") || input.Contains("browser security")) // if the user input contains "safe browsing" or "browser security", return "privacy"
                return "privacy";

            return ""; // returns an empty string if no keyword is detected
        }

        static void RespondWithRandomTip(string keyword, string sentiment)
        {
            if (!topicResponses.ContainsKey(keyword)) // if the keyword is not in the topic responses, return
                return;

            // Select random response from the list
            var responses = topicResponses[keyword];
            string selectedResponse = responses[random.Next(responses.Count)];

            // Adjust response based on sentiment
            string sentimentPrefix = GetSentimentPrefix(sentiment, userMemory.Name);
            
            Console.WriteLine(sentimentPrefix + selectedResponse); // displays the sentiment prefix and selected response

            // Add memory-based personalization
            AddPersonalizedContext(keyword);
        }

        static string GetSentimentPrefix(string sentiment, string name) // gets the sentiment prefix based on the sentiment and name
        {
            return sentiment switch // returns the sentiment prefix based on the sentiment
            {
                "worried" => $"🤗 Bot: I understand you're concerned, {name}. Let me help ease your worries. ",
                "frustrated" => $"😌 Bot: I know this can be confusing, {name}. Let me break it down simply. ",
                "curious" => $"🧠 Bot: Great question, {name}! Your curiosity about security is admirable. ",
                "confident" => $"👍 Bot: You seem to have a good grasp on this, {name}! Here's some additional insight: ",
                _ => ""
            };
        }

        static void AddPersonalizedContext(string currentTopic) // adds personalized context based on the current topic
        {
            // Reference user's favorite topic or previous discussions
            if (!string.IsNullOrEmpty(userMemory.FavoriteTopic) && 
                userMemory.FavoriteTopic != currentTopic && 
                userMemory.TopicsDiscussed.Count > 1)
            {
                Console.WriteLine($"💡 Bot: Since you're also interested in {userMemory.FavoriteTopic}, you might find these topics connect - strong {userMemory.FavoriteTopic} practices often complement {currentTopic} security!");
            }
        }

        static bool HandleBasicConversation(string input, string sentiment) // handles the basic conversation
        {
            if (input.Contains("how are you")) // if the user input contains "how are you", display a response
            {   
                string response = sentiment == "worried" 
                    ? "🤖 Bot: I'm doing well, and I'm here to help you feel more secure online! What's concerning you?"
                    : "🤖 Bot: I'm excellent! Always ready to boost your cyber knowledge. 💥";
                Console.WriteLine(response);
                return true;
            }
            
            if (input.Contains("purpose") || input.Contains("what can you do")) // if the user input contains "purpose" or "what can you do", display a response
            {
                Console.WriteLine("🤖 Bot: I'm designed to educate and empower you with cybersecurity knowledge for Part 2 of your learning journey!"); // displays a response
                if (userMemory.TopicsDiscussed.Count > 0) // if the user has discussed topics, display a response
                {
                    Console.WriteLine($"💭 Bot: We've already covered: {string.Join(", ", userMemory.TopicsDiscussed)}. What else interests you?");
                }
                return true;
            }

            return false;
        }

        static void HandleUnrecognizedInput(string sentiment) // handles the unrecognized input
        {
            List<string> defaultResponses = new List<string> // default responses
            {
                "🤖 Bot: I'm not sure I understand. Can you rephrase that?",
                "🤖 Bot: That's interesting, but I didn't catch a cybersecurity topic. Try asking about passwords, phishing, or privacy!",
                "🤖 Bot: Hmm, I didn't recognize that. Type 'help' to see what I can assist you with!"
            };

            string baseResponse = defaultResponses[random.Next(defaultResponses.Count)]; // selects a random default response
            
            // Add sentiment-aware context
            if (sentiment == "frustrated") // if the sentiment is "frustrated", display a response
            {
                baseResponse += $" I know this can be frustrating, {userMemory.Name}. Let's try a different approach!"; 
            }
            else if (sentiment == "worried") // if the sentiment is "worried", display a response
            {
                baseResponse += " Don't worry - I'm here to help you navigate cybersecurity safely."; // adds a response to the base response
            }

            Console.WriteLine(baseResponse);

            // Suggest topics based on memory
            if (userMemory.TopicsDiscussed.Count > 0) // if the user has discussed topics, display a response
            {
                Console.WriteLine($"💡 Bot: Or we could continue discussing {userMemory.TopicsDiscussed.Last()} if you'd like more details!");
            }
        }

        static void ShowFarewellMessage()
        {
            Console.WriteLine($"🤖 Bot: Thanks for chatting, {userMemory.Name}! Stay safe online! 👋"); // displays a farewell message
            
            if (userMemory.TopicsDiscussed.Count > 0) // if the user has discussed topics, display a response
            {
                Console.WriteLine($"📚 Bot: Remember what we covered today: {string.Join(", ", userMemory.TopicsDiscussed)}"); // displays the topics discussed
            }
            
            if (!string.IsNullOrEmpty(userMemory.FavoriteTopic)) // if the user has a favorite topic, display a response
            {
                Console.WriteLine($"🎯 Bot: Keep focusing on {userMemory.FavoriteTopic} security - you're on the right track!"); // displays a response
            }
        }

        static void PlayGreetingAudio() // plays the greeting audio
        {
            string assetPath = "../assets/greeting.wav"; // sets the asset path to the greeting audio
            
            // Try alternative paths if the main one doesn't exist
            if (!System.IO.File.Exists(assetPath))
            {
                string[] alternativePaths = {
                    "assets/greeting.wav",
                    "../assets-clean/greeting.wav",
                    "../../assets/greeting.wav",
                    "./assets/greeting.wav"
                };
                
                foreach (string path in alternativePaths) 
                {
                    if (System.IO.File.Exists(path))
                    {
                        assetPath = path;
                        break;
                    }
                }
                
                if (!System.IO.File.Exists(assetPath)) // if the asset path does not exist, display a message
                {
                    Console.WriteLine("⚠️  Bot: Audio greeting file not found. Please ensure 'greeting.wav' exists in the 'assets' folder.");
                    return;
                }
            }
            
            try 
            {
                ProcessStartInfo psi; // sets the process start info
                
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // if the operating system is Windows, set the process start info
                {
                    psi = new ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = $"-c (New-Object System.Media.SoundPlayer '{System.IO.Path.GetFullPath(assetPath)}').PlaySync()",
                        RedirectStandardOutput = false,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    psi = new ProcessStartInfo
                    {
                        FileName = "aplay",
                        Arguments = assetPath,
                        RedirectStandardOutput = false,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // if the operating system is macOS, set the process start info
                {
                    psi = new ProcessStartInfo
                    {
                        FileName = "afplay",
                        Arguments = assetPath,
                        RedirectStandardOutput = false,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                }
                else
                {
                    Console.WriteLine("⚠️  Bot: Audio greeting is not supported on this platform.");
                    return;
                }

                Process.Start(psi)?.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Bot: Audio greeting could not be played. {ex.Message}");
            }
        }

        static void ShowHeader() // shows the header
        {
            Console.ForegroundColor = ConsoleColor.Cyan; // sets the foreground color to cyan
            Console.WriteLine(@"
   ___      _                ___       _   
  / __\   _| |__   ___ _ __ / __\ ___ | |_ 
 / / | | | | '_ \ / _ \ '__/__\/// _ \| __|
/ /__| |_| | |_) |  __/ | / \/  \ (_) | |_ 
\____/\__, |_.__/ \___|_| \_____/\___/ \__|
      |___/                                

            👾 THINKINBOT CYBER 👾
                    Part 2
        ");
            Console.ResetColor();
        }

        static void DisplayWelcomeMessage() // displays the welcome message
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("👋 Welcome to ThinkinBot Cyber Part 2 – Enhanced AI Security Ally!");
            Console.WriteLine("--------------------------------------------------");
        }

        static void ShowIntro() // shows the introduction
        {
            Console.WriteLine("\n🤖 Bot: I'm your enhanced cybersecurity assistant for Part 2!");
            Console.WriteLine("🧠 New Features: I now remember our conversations, detect your mood, and provide personalized tips!");
            Console.WriteLine("🔍 Ask me about topics like:");
            Console.WriteLine("   • Password security & 2FA");
            Console.WriteLine("   • Phishing & scam prevention");
            Console.WriteLine("   • Privacy protection");
            Console.WriteLine("   • VPN usage");
            Console.WriteLine("   • Malware protection");
            Console.WriteLine("💬 Try saying things like 'I'm interested in privacy' or 'tell me more' for enhanced interactions!");
            Console.WriteLine("🚪 Type 'exit' to leave the session.");
        }

        static void ShowHelp() // shows the help menu
        {
            Console.WriteLine("\n📘 Bot Help Menu (Part 2 Enhanced):");
            Console.WriteLine("Here are some things you can ask me:");
            Console.WriteLine(" - How are you?");
            Console.WriteLine(" - What's your purpose?");
            Console.WriteLine(" - I'm interested in [topic] (saves your preference!)");
            Console.WriteLine(" - Tell me about passwords");
            Console.WriteLine(" - What is phishing?");
            Console.WriteLine(" - Explain privacy protection");
            Console.WriteLine(" - I'm worried about scams");
            Console.WriteLine(" - Tell me more (continues current topic)");
            Console.WriteLine(" - exit / quit");
            Console.WriteLine("\n💡 Part 2 Features:");
            Console.WriteLine(" - I remember your name and interests");
            Console.WriteLine(" - I detect if you're worried, curious, or frustrated");
            Console.WriteLine(" - I provide random educational tips from my knowledge base");
            Console.WriteLine(" - I can continue conversations on topics you're exploring");
        }
    }
} 