# Mandatory Assignment 4: Augmented Reality Projects — AR-Mobile

This repository is for **Mandatory Assignment 4: Augmented Reality Projects**. The goal of the assignment is to design and implement a **polished AR experience** that uses **AR features (image targets or spatial mapping)** in a **non-trivial** way and runs on **Android**.

## Project Concept — “The Obelisk”
You can “damage” the world by tapping:

- **Tap 1–2:** Small cracks appear in the detected surface/floor.
- **Tap 3:** The crack becomes large enough to open a **rift/tear** between our world and **the void**.
- From the rift, an **obelisk** emerges — this becomes the core interactive **puzzle object**.
- After solving the puzzle, the **rift expands** and consumes everything, leaving only **the void** visible.

## Core Gameplay / Objective
**Objective:** Solve the obelisk puzzle by rotating its segments into the correct alignment.

- The obelisk has **3 segments**
- Each segment can be **rotated by dragging on the phone screen** 
- When all segments are in the correct position, the puzzle completes and triggers the ending sequence (rift expansion → void)

## AR Technology Use (Assignment Requirements)
This project is designed to meet the mandatory constraints:

- **Augmented Reality is central** to the experience (visual cracks, rift effect, emergence of an AR obelisk).
- Uses **spatial mapping / plane detection / anchors** to place cracks, rift, and the obelisk on real-world surfaces.
- Provides a **closed and polished experience**, including:
  - Clear interaction cues / visual aid (tap to crack, drag to rotate)
  - A defined goal (solve the obelisk)
  - A clear ending (rift expansion and void)

The project uses **Pencil** to **include/exclude** elements in the experience (e.g., controlling visibility or masking of objects/effects as part of the presentation and ending sequence).
**Project Type:** Mobile AR puzzle experience (rift + obelisk segment-rotation puzzle)