REM Executed by the AppVeyour build service
msbuild Build\Build.proj /p:Configuration=Release /p:Version=%APPVEYOR_BUILD_VERSION%