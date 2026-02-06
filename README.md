# 🕊️ Project Guvercin

**Project Guvercin** is a unique Unity-based strategy and tactical battle game featuring various pigeon breeds native to Turkey. Each pigeon species (such as Dönek, Taklacı, and Sebap) comes with its own set of attributes and special skills, offering a deep and cultural gameplay experience.

## 🎮 Core Gameplay Mechanics
* **Pigeon Breeds:** Choose from a diverse roster of pigeons including:
  * **Taklacı:** Agile flyers with acrobatic combat skills.
  * **Posta:** High-endurance messengers turned warriors.
  * **Mısıri & Sebap:** Breeds with specialized tactical advantages.
* **Battle System:** Strategic turn-based or real-time encounters managed by a central `BattleManager`.
* **Dynamic Arena System:** Combat takes place in iconic locations like Galata Tower and Maiden's Tower, featuring an `ArenaBackgroundManager` for atmospheric variety.
* **Skill Management:** Each bird has unique "Attack" and "Special Skill" animations and logic.

## 🛠 Technical Features & Architecture
* **Data-Driven Design:** Utilizes `PigeonData` classes to handle species-specific stats and attributes, making it easy to balance and add new birds.
* **Modular Battle Logic:** The `BattleManager` handles the core loop, ensuring decoupled logic between UI interactions and gameplay outcomes.
* **UI/UX Framework:** Custom-built health bars and combat UI using `PigeonButtonController` for a responsive player experience.
* **Pixel Art Visuals:** Custom-designed animations for each state (Idle, Attack, Skill, Die) implemented via Unity’s Animator system.

## 📁 Project Structure
```text
Project-Guvercin/
├── Assets/
│   ├── Scripts/        # Battle logic, Pigeon data, and UI controllers
│   ├── 1Animations/    # State-specific animations for all breeds
│   ├── 1Sprites/       # Original breed-specific pixel art (Aseprite format)
│   ├── SpritesUI/      # Health bars, hitboxes, and button assets
│   └── Scenes/         # Main UI and Arena scenes
