![Example output](https://github.com/brudvik/90s-Demo-Example/blob/master/DemoTest/assets/kab-demo-2025.png)

# Project Overview
This project is a retro-inspired multimedia demo application built with C# 12 and .NET 8, emulating the style and spirit of classic 1990s demoscene productions. It combines real-time 2D/3D graphics, audio visualization, particle effects, and custom text rendering to create a visually rich, interactive experience. The application is structured around a main form (MainForm) that orchestrates all rendering, animation, and audio playback.

# Key Techniques Demonstrated
1. Custom Graphics Rendering
- Double Buffering: Uses an off-screen bitmap (renderBuffer) to draw all graphics before presenting them, reducing flicker and improving visual smoothness.
- Direct GDI+ Drawing: Employs System.Drawing for rendering shapes, lines, ellipses, and images directly onto the graphics buffer.
- Bitmap Font Rendering: Implements a custom bitmap font system for drawing pixel-perfect text, including effects like sine-wave scrolling and color cycling.
2. Real-Time 3D Graphics
- 3D Geometry and Projection: Defines a 3D cube using vertices and edges, applies rotation matrices, and projects 3D points onto a 2D plane for display.
- Dynamic Animation: Continuously updates rotation angles and object positions to create smooth, real-time 3D animation.
3. Audio Visualization
- Music Playback: Integrates a music visualizer that plays an MP3 file and exposes real-time spectrum data.
- Spectrum Analysis: Visualizes audio data as animated bars and scrolling waveforms, providing a direct link between the music and the graphics.
- Waveform History: Maintains a history of amplitude data to create smooth, scrolling waveform effects.
4. Particle Systems and Effects
- Warp and Explosion Effects: Implements two types of particle systems—warp particles for a tunnel effect and explosion particles triggered by orbiter collisions.
- Particle Lifecycle Management: Handles creation, animation, fading, and removal of particles for efficient and visually appealing effects.
5. Procedural Animation
- Starfield Simulation: Generates and animates a field of stars moving across the screen, simulating depth and motion.
- Raster Bars: Draws colored bars with sine wave motion, a classic demoscene visual.
- Orbiters: Animates orbs moving in circular paths around the cube, with collision detection and trails.
6. Visual Effects
- Chromatic Aberration: Applies a post-processing effect that offsets color channels for a retro, glitchy look.
- Camera Shake: Adds screen shake during explosions for dramatic impact.
- White Flash: Overlays a white flash effect when certain events (like collisions) occur.
7. User Interaction
- Keyboard Handling: Listens for the Escape key to allow the user to exit the demo.
8. Performance Considerations
- Efficient Redraws: Uses a timer with a 16ms interval (~60 FPS) for smooth animation.
- Resource Management: Properly disposes of graphics objects and manages memory for bitmaps and buffers.

# Learning Outcomes
By studying and modifying this code, you will learn:
- How to build a real-time multimedia application in C# using WinForms and GDI+.
- Techniques for double-buffered rendering and custom drawing in Windows Forms.
- How to implement and animate 3D objects using basic linear algebra (rotation, projection).
- How to process and visualize audio data in real time.
- How to design and manage particle systems for dynamic visual effects.
- How to create procedural animations and classic demoscene effects (starfields, raster bars, sine scrollers).
- How to apply post-processing effects like chromatic aberration and camera shake.
- How to handle user input and manage application state in a multimedia context.
- Best practices for resource management and performance optimization in graphics-heavy applications.

# Summary
This project is an excellent hands-on introduction to multimedia programming, real-time graphics, and audio visualization in C#. It covers a wide range of classic and modern techniques, making it a valuable resource for learning about graphics, animation, audio processing, and interactive application design.

# Copyright
- The font used is a bitmap font from Zingot Games: https://opengameart.org/content/bitmap-font-pack
- The music used is Complications (1990) by Tomas Danko/Fairlight
