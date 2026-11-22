# FrostyToolsuite
The most advanced modding platform for games running on DICE's Frostbite game engine.

## Setup

1. Download Git https://git-scm.com/download/win.
2. Create an empty folder, go inside it, right click an empty space and hit "Git Bash Here". That should open up a command prompt.
3. Press the green "Code" button in the repository and copy the text under "HTTPS".
4. Type out ``git clone -b 1.0.6 <HTTPS code>`` in the command prompt and hit enter. This should clone the project files into the folder.
5. Open the solution (found under FrostyEditor) with Visual Studio 2019, and make sure the project is set to ``DeveloperDebug`` and ``x64``. Close out of retarget window if prompted.
6. Only build the projects themselves, never the solution.

## License
All content, name, code, and assets, except third-party libraries under `ThirdParty\`, are licensed under the [GPL v3.0](./LICENSE.txt).
