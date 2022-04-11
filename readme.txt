Download:
git clone https://github.com/Yigaelb/ezcalc.git

Compiling:
* Compile with visual studio
* Merge with BigDecimal.dll - inorder of ezcalc to work standalone it has to be marked with BigDecimal
** From YigiCalculator 1.1\bin\Release copy EZcalc.pdb and EZcalc.exe into ILMerge
** In ILMerge delete EZcalcBig.pdb and EZcalcBig.exe
** Execute IlMergeEZcalc.bat
** rename EZcalcBig.exe to EZcalc.exe

Known bugs:
* On first open, version is doubled and calculator minimises