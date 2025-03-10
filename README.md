# "Hand"y Presentation Helper
Presentation helper using a ML model that detects and recognizes various hand gestures (forked from https://github.com/kinivi/hand-gesture-recognition-mediapipe, original version: https://github.com/Kazuhito00/hand-gesture-recognition-using-mediapipe).

## Contributors
* Omar Jaber (https://github.com/omaritojaber)
* Antoine Labbé (https://github.com/Marzzeau)
* Yang Li (https://github.com/yxL05)
* Kevin Wu (https://github.com/ToasterBuilder)

## Requisites
A functional camera (and your permission to use it).

## Todo List
1. Standalone C# app (using WPF) for Office 365 applications.
> Might extend to MacOS / iOS applications in the future using MAUI.
 
2. Chrome extension for Google applications.

## How It Works
1. The user chooses the camera mode, the presentation app they are using, two hand gestures used as signal to enable and disable hand gesture shortcuts respectively, and calibrates the ML model if needed using the GUI.
2. Once the chosen presentation app is in presentation mode, the ML model starts searching for the signal hand gesture.
3. If the chosen "enable shortcuts" hand gesture is detected, the user will be able to use presentation keyboard shortcuts via simple hand gestures.

## User settings to implement
1. Camera mode (selfie or normal)
2. Presentation app (dropdown menu? preconfigured list?)
3. "Toggle" hand gestures
4. Calibration menu
5. Settings to customize sign mapping
