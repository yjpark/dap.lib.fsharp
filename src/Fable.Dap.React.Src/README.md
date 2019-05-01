## Notes

For unknown reasons, after did the Fable.Dap.Context work, fable can't compile my project properly, the problem seems to be order of compiling, if include the dependencies as nuget package, some howe fable compiler either crash with no trace, or throwing out all sort of weird errors that make no sense.

After spent much time try to solve this by trying different ways to organize the library projects, I created a project to include the libs as project dependencies, hope this can make the experiments faster.

But the thing is, that in this way, there is no issue, can compile properly.

So current workaround is instead of add the libs as nuget references in the project, need to add this project as dependencies.

The down side is that have to keep the local project check out with proper version, it's quite annoying.

Need to revisit this after fable 2.0 upgrade, see whether the issue been fixed