# LeftoverChef is a smart app.
core functionality
1. Intelligent inventory matching
Dynamic comparison algorithm: The system will automatically compare the "refrigerator inventory" and "recipe demand" in the SQLite database.

Missing reminder: In the search results, users can click the "Check" button, and the system will immediately calculate and list the missing ingredients in the recipe.

2. Hardware level interactive experience (hardware level UX)
Tactile feedback: When an input error or storage limit is reached, the underlying motor of the system will be called to provide vibration feedback, thereby enhancing interactive perception.

Exception tolerance: This code implements try catch hardware protection to ensure that applications can still run stably in different hardware environments (such as simulators that do not support vibration).

3. Visual storage control
Capacity limit logic: The storage limit for refrigerators and freezers is 30 pieces.

Real time progress bar: Use a dynamic progress bar to display storage ratios and automatically switch colors (green/orange/red) based on remaining space, achieving intuitive capacity monitoring.

4. Social integration
Cross platform sharing: supports one click sharing of selected recipes to WhatsApp and WeChat.

Intelligent link construction: Automatically extract formula names and ingredient information, and construct shared text that complies with social platform protocols.

tech stack
Framework:. NET MAUI£¨C#/XAML£©

Database: SQLite (local persistent storage)

VCS: Git/GitHub (Professional PR Workflow)

Concepts: SDLC, CRUD, asynchronous programming, UI/UX heuristics

Best Practices for Git Version Control
This project strictly follows the professional software development process and aims to achieve the highest standards of academic evaluation:

Branch strategy: All new features are developed on separate feature branches.

Pull Requests: Conduct code review and merging through the PR process to ensure the stability of the main branch.

Conflict Resolution: Able to handle and manually resolve code conflicts to ensure smooth team collaboration.