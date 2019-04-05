# Unity_EditorSafeUnscaledTime
Time values respect Unity Editor playmode pausing

### Why

When working on some Time-Freezing gameplay, I found that Unity does not provide any Time member for "actual time length in play mode".

All the provided Time values don't care about Play Mode Pause State, which is a pain in the neck when you want to test your Time code in Play Mode.
