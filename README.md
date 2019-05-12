## Dependencies

### DotNet Core

- https://dotnet.microsoft.com/download
- SDK v2.2.106

### DotNet Tools

Need `paket` and `fake` command in path

Recommend to install with dotnet tool

Install the first time

```
dotnet tool install --global paket
dotnet tool install --global fake-cli
```

Update to latest version

```
dotnet tool update --global paket
dotnet tool update --global fake-cli
```

Note: need to add `~/.dotnet/tools` to `PATH`
