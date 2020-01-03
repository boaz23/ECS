:: This compares the output of your assembler with the output of the assembler from the tools.
:: IMPORTANT: you must save your output as .mc files and the assembler from tools as .hack files and in this folder
echo off
echo.
for /r %%i in (*.asm) do (
    if exist %%~ni.mc (
        echo %%~ni.asm
        java -classpath "%CLASSPATH%;../tools/bin/classes" TextComparer %%~ni.hack %%~ni.mc
    )
    echo.
)
pause