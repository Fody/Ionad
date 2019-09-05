[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg)](https://gitter.im/Fody/Fody)
[![NuGet Status](https://img.shields.io/nuget/v/Ionad.Fody.svg)](https://www.nuget.org/packages/Ionad.Fody/)


## This is an add-in for [Fody](https://github.com/Fody/Home/)

![Ionad Icon - A black circle with 3 arrows forming an outer circle.](https://raw.github.com/Fody/Ionad/master/package_icon.png)

Ionad replaces static types with your own.


## NuGet installation

Install the [Ionad.Fody NuGet package](https://nuget.org/packages/Ionad.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package Ionad.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Your Code

```csharp
[StaticReplacement(typeof(DateTime))]
public static class DateTimeSubstitute
{
    public static IDateTime Current { get; set; }

    public static DateTime Now { get { return Current.Now; } }
}

public void SomeMethod()
{
    var time = DateTime.Now;
    // ...
}
```


### What gets compiled 

```csharp
public void SomeMethod()
{
    var time = DateTimeSubstitute.Now;
    // ...
}
```


## Icon

[Interchange](https://thenounproject.com/noun/interchange/#icon-No2031) designed by [Laurent Patain](https://thenounproject.com/____Lo) from [The Noun Project](https://thenounproject.com).