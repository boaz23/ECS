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