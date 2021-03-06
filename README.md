# CSInputs

Build target must set to x86 to work.

This library lets you send and read mouse and keyboard input from windows.

# Usage Examples

# To Send

Keyboard Inputs:
```cs
SendInput.Keyboard.Send(Enums.KeyboardKeys.F1);
SendInput.Keyboard.Send(Enums.KeyboardKeys.F1,Enums.KeyFlags.Down);
SendInput.Keyboard.Send(Enums.KeyboardKeys.F1,Enums.KeyFlags.Up);
SendInput.Keyboard.SendChar('A');
SendInput.Keyboard.SendString("Hello World!");
```

Mouse Inputs:
```cs
SendInput.Mouse.Send(Enums.MouseKeys.MouseLeft);

SendInput.Mouse.Send(Enums.MouseKeys.MouseLeft,Enums.KeyFlags.Down);
SendInput.Mouse.Send(Enums.MouseKeys.MouseLeft,Enums.KeyFlags.Up);

SendInput.Mouse.Send(Enums.MouseKeys.MouseLeft,Enums.KeyFlags.Down,new Point(150, 123),Enums.MousePositioning.Absolute);
SendInput.Mouse.Send(Enums.MouseKeys.MouseLeft,Enums.KeyFlags.Down,new Point(-5, -30),Enums.MousePositioning.Relative);

SendInput.Mouse.MoveTo(new Point(150, 123), Enums.MousePositioning.Absolute);
```

# To Listen
First you need to instantiate a input listener.
```cs
ReadInput.InputListener listener = new ReadInput.InputListener();
```
Then you can register for keyboard or mouse events to listen inputs.
```cs
listener.KeyboardInputs += Listener_KeyboardInputs;
listener.MouseInputs += Listener_MouseInputs;
```
