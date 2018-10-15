### GTK on OSX
- `brew install gtk+3

### GTK on Windows (not working yet)
- https://github.com/GtkSharp/GtkSharp/wiki/Installing-Gtk-on-Windows
- install msys2: http://www.msys2.org/
- packman -S mingw-w64-x86_64-gtk3`
- add `;C:\msys64\mingw64\bin` to PATH (Control Panel >> System >> Environment Variables)

### Tried with msys2's different version (also not working)
- Download older version of gtk3
  - http://repo.msys2.org/mingw/x86_64/mingw-w64-x86_64-gtk3-3.22.24-1-any.pkg.tar.xz
- `pacman -U mingw-w64-x86_64-gtk3-3.22.24-1-any.pkg.tar.xz
