# ThinkinBot Cyber Security Chatbot - Part 2

An enhanced console-based AI chatbot that educates users about cybersecurity best practices with advanced interactivity features.

## Part 2 New Features 🧠

### Enhanced Interactivity
- **Memory System**: Remembers your name, favorite topics, and conversation history
- **Sentiment Detection**: Detects emotional tones (worried, curious, frustrated, confident) and adjusts responses
- **Conversation Flow**: Continues topics when you say "tell me more" or ask follow-up questions
- **Personalized Responses**: References your interests and previous discussions

### Advanced Response System
- **Random Educational Tips**: Multiple responses per topic with random selection
- **Keyword Recognition**: Enhanced detection of cybersecurity topics and variations
- **Contextual Follow-ups**: Provides deeper insights based on current conversation topic
- **Smart Error Handling**: Graceful handling of unrecognized input with helpful suggestions

## Core Features

- Friendly conversation-based interface
- Educational content about cybersecurity topics
- Cross-platform audio greeting (Windows, macOS, Linux)
- Colorful console UI with emoji support
- User preference tracking and personalization

## Topics Covered

- **Password Security**: Strong passwords, password managers, 2FA
- **Phishing Prevention**: Email security, URL verification, sender validation
- **Scam Awareness**: Common scams, verification techniques, trust indicators
- **Privacy Protection**: Social media settings, data sharing, privacy tools
- **VPN Usage**: Encryption, provider selection, use cases
- **Malware Protection**: Antivirus, safe downloading, threat prevention

## Enhanced User Experience

### Sentiment-Aware Responses
- **Worried/Anxious**: Comforting tone with reassuring guidance
- **Curious/Interested**: Encouraging responses with detailed explanations
- **Frustrated/Confused**: Simplified explanations with patient tone
- **Confident**: Advanced tips and additional insights

### Memory Features
- Stores your name and uses it throughout the conversation
- Remembers topics you've shown interest in
- Tracks conversation history for context
- Provides personalized farewell messages

### Conversation Examples
```
You: I'm worried about passwords
Bot: 🤗 I understand you're concerned, John. Let me help ease your worries. 
     🔐 Create strong passwords with 12+ characters, mixing letters, numbers, and symbols...

You: Tell me more
Bot: Let me share more about password...
     🔐 Here's a pro tip: Enable two-factor authentication (2FA) wherever possible...

You: I'm interested in privacy
Bot: I've noted that you're interested in privacy security, John!
```

## How to Run

1. Ensure you have .NET 8.0 SDK installed
2. Navigate to the project directory
3. Run `dotnet run` to start the application
4. Alternative: Use `./run.sh` (Unix systems)

## Project Structure

```
CyberBot/
│
├── Program.cs          # Enhanced chatbot with Part 2 features
├── FINAL.csproj        # .NET project file
├── CyberBot.sln        # Solution file
├── run.sh             # Build and run script
├── README.md          # Documentation (this file)
│
└── assets/             # Resources directory
    └── greeting.wav    # Audio greeting file
```

## Technical Enhancements (Part 2)

### Code Architecture
- **UserMemory Class**: Manages user data and conversation context
- **Modular Methods**: Separated concerns with dedicated methods for each feature
- **Dictionary-Based Responses**: Organized responses using data structures
- **LINQ Integration**: Efficient keyword and sentiment detection

### Key Methods
- `ProcessUserInput()`: Main input processing with sentiment and context analysis
- `DetectSentiment()`: Analyzes emotional tone in user input
- `HandleFollowUp()`: Manages conversation continuity
- `RespondWithRandomTip()`: Provides varied educational responses
- `DetectAndStoreFavoriteTopic()`: Captures and stores user interests

### Data Structures
- `Dictionary<string, List<string>>` for topic responses
- `UserMemory` class for session persistence
- `List<string>` for sentiment keywords and follow-up triggers

## Part 2 Requirements Completed ✅

1. **Keyword Recognition**: Enhanced detection of 6+ cybersecurity keywords
2. **Random Responses**: 3+ educational tips per topic with random selection
3. **Conversation Flow**: Context tracking and follow-up handling
4. **Memory & Recall**: User info storage and personalized references
5. **Sentiment Detection**: 4 emotional tone categories with adaptive responses
6. **Error Handling**: Graceful unrecognized input management
7. **Code Optimization**: Modular architecture with clean, commented code

## Author

- Talhah Thokan (ST10122437)

## License

This project is part of a Programming course (PROG6221) Part 2 at Varsity College. 