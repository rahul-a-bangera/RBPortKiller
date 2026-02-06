# Keyboard Shortcuts & Quick Exit

## Quick Reference

| Key Combination | Action | Context |
|----------------|--------|---------|
| **Ctrl+C** | Quick Exit | Any screen |
| **? / ?** | Navigate menu | Port selection / Action menu |
| **Enter** | Select option | Any menu |
| **Page Up/Down** | Fast scroll | Long port lists |

## Quick Exit Feature (Ctrl+C)

### Overview
Press **Ctrl+C** at any time to immediately exit RBPortKiller with a clean shutdown.

### How It Works

1. **Press Ctrl+C** on your keyboard
2. Application catches the interrupt signal
3. Displays **"Exiting..."** message
4. Performs clean shutdown
5. Returns you to the command prompt

### Where It Works

**Port Selection Menu**
- When viewing the list of active ports
- While selecting a port to manage

**Port Action Menu**
- When viewing port details
- When choosing "Kill Process" or "Back to Port List"

**During Operations**
- While loading ports
- While terminating a process

**After Errors**
- When prompted to continue after an error

### Visual Indicator

After the port table, you'll always see:
```
Tip: Press Ctrl+C anytime to exit quickly
```

This reminds you that quick exit is always available.

### Example Usage

#### Scenario 1: Quick Exit from Main Menu
```
????????????????????????????????????????????????????????
? #  ? Port ? Protocol ? ...                           ?
????????????????????????????????????????????????????????

Tip: Press Ctrl+C anytime to exit quickly

Select a port to manage:
? 1. Port 3000 (TCP) - TestPortOpener [PID: 5432]
  ...

[You press Ctrl+C]

Exiting...

Thank you for using RBPortKiller!
```

#### Scenario 2: Exit During Port Action
```
????????????????????????????????????????
?       Selected Port Details          ?
????????????????????????????????????????
? Port: 3000                           ?
? ...                                  ?
????????????????????????????????????????

What would you like to do?
? Kill Process
  Back to Port List

[You press Ctrl+C]

Exiting...

Thank you for using RBPortKiller!
```

### Benefits

? **Fast**: No need to navigate to "Exit" option  
? **Universal**: Works from any screen  
? **Clean**: Proper shutdown, no crashes  
? **Intuitive**: Standard across most CLI tools  
? **Always Available**: Can't get "stuck" in the app  

## Alternative Exit Methods

### Method 1: Select "Exit" from Menu
1. Navigate to the bottom of the port selection menu
2. Select **"Exit"** option
3. Application exits gracefully

**When to use**: When you want to use the mouse or prefer menu navigation

### Method 2: Ctrl+C (Quick Exit)
1. Press **Ctrl+C** from anywhere
2. Application exits immediately

**When to use**: When you want to exit quickly from any screen

## Technical Details

### Signal Handling
- Ctrl+C sends an interrupt signal (SIGINT)
- .NET catches this as `OperationCanceledException`
- Application handles it gracefully in the main loop

### Exception Flow
```csharp
try 
{
    // Main application loop
}
catch (OperationCanceledException)
{
    // Ctrl+C was pressed
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[yellow]Exiting...[/]");
    break;
}
```

### Clean Shutdown
When Ctrl+C is pressed:
1. ? Current operation is cancelled
2. ? Resources are released
3. ? No data corruption
4. ? Proper exit message displayed
5. ? Control returns to shell

## Comparison with Other Tools

| Tool | Exit Method | Speed |
|------|------------|-------|
| **RBPortKiller** | Ctrl+C or Menu | Instant |
| netstat | Ctrl+C | Instant |
| Task Manager | Alt+F4 | Instant |
| TCPView | Alt+F4 | Instant |

RBPortKiller matches the UX of professional tools!

## Best Practices

### When to Use Ctrl+C
? You're done viewing ports  
? You want to exit quickly  
? You're in a hurry  
? You prefer keyboard over mouse  

### When to Use "Exit" Menu Option
? You prefer mouse/navigation  
? You're learning the tool  
? You want to see all menu options  

## Accessibility

### Keyboard-Only Navigation
RBPortKiller is fully keyboard-accessible:
- **Arrow keys**: Navigate menus
- **Enter**: Select options
- **Ctrl+C**: Quick exit
- **No mouse required**

### Screen Reader Friendly
- Clear text descriptions
- Logical navigation order
- Consistent menu structure
- Clean exit messages

## Testing the Quick Exit

### Test 1: Exit from Port Menu
1. Run RBPortKiller
2. View port list
3. Press Ctrl+C
4. ? Should see "Exiting..." and exit cleanly

### Test 2: Exit from Action Menu
1. Run RBPortKiller
2. Select a port
3. View port details
4. Press Ctrl+C
5. ? Should see "Exiting..." and exit cleanly

### Test 3: Exit During Load
1. Run RBPortKiller
2. While "Loading active ports..." spinner is showing
3. Press Ctrl+C
4. ? Should cancel and exit cleanly

### Test 4: Multiple Ctrl+C Presses
1. Run RBPortKiller
2. Press Ctrl+C multiple times rapidly
3. ? Should exit on first press (subsequent presses ignored)

## Troubleshooting

### Issue: Ctrl+C Doesn't Work
**Cause**: Terminal might intercept the signal  
**Solution**: 
- Try running in a different terminal (CMD, PowerShell, Git Bash)
- Check if your terminal emulator has custom Ctrl+C binding

### Issue: Gets "Access Denied" Before Exiting
**Cause**: Normal behavior if you selected a system process (shouldn't happen with filtering)  
**Solution**: Ctrl+C will still exit after the message

### Issue: Exits Too Fast, Can't See Message
**Cause**: Very fast computers might exit before you see the message  
**Solution**: This is fine - the message is there, just very brief

## FAQ

**Q: Is Ctrl+C the same as ESC?**  
A: No, ESC is not currently supported. Use Ctrl+C for quick exit.

**Q: Will Ctrl+C corrupt anything?**  
A: No, it performs a clean shutdown. Safe to use anytime.

**Q: Can I rebind Ctrl+C to something else?**  
A: Not in the current version. Ctrl+C is standard across CLI tools.

**Q: What if I press Ctrl+C while killing a process?**  
A: The kill operation will be cancelled, and RBPortKiller will exit. The target process may or may not be terminated.

**Q: Does Ctrl+C kill the ports I was viewing?**  
A: No, it only exits RBPortKiller. Ports remain open.

**Q: Can I exit without the "Thank you" message?**  
A: Not currently, but it's very brief and professional.

## Future Enhancements

Potential additions:
- ?? ESC key support for exiting
- ?? F10 or Q key for quick exit
- ?? Customizable exit keys
- ?? Silent exit mode (no goodbye message)
- ?? Exit confirmation (optional)

## Summary

? **Ctrl+C provides instant exit from anywhere**  
? **Clean shutdown, no corruption**  
? **Tip displayed on every screen**  
? **Standard CLI behavior**  
? **No need to navigate to Exit menu**  

**Just press Ctrl+C anytime! It's that simple.** ??

---

*Last updated: Current version*  
*Related docs: QUICK_TEST.md, TESTING_GUIDE.md, REFRESH_FEATURE.md*
