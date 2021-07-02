# <img src="/package_icon.png" height="30px"> Ionad.Fody

[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg)](https://gitter.im/Fody/Fody)
[![NuGet Status](https://img.shields.io/nuget/v/Ionad.Fody.svg)](https://www.nuget.org/packages/Ionad.Fody/)

Ionad replaces static types with your own.


### This is an add-in for [Fody](https://github.com/Fody/Home/)

**It is expected that all developers using Fody either [become a Patron on OpenCollective](https://opencollective.com/fody/contribute/patron-3059), or have a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-fody?utm_source=nuget-fody&utm_medium=referral&utm_campaign=enterprise). [See Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.**


## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).


### NuGet installation

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

You can also reference methods within the original static class to add defaults to optional parameters. For example:

```csharp
[StaticReplacement(typeof(System.Reactive.Linq.Observable))]
public static class QueryableSubstitute
{
    public static IObservable<IList<TSource>> Delay<TSource>(this IObservable<TSource> source, TimeSpan timeSpan)
    {
        return Linq.Delay<TSource>(source, timeSpan, RxApp.TaskpoolScheduler);
    }
}

public async Task<int> SomeMethod()
{
    return await Observable.Return(1).Delay(TimeSpan.FromSeconds(1)).ToTask();
}
```


### What gets compiled 

```csharp
public async Task<int> SomeMethod()
{
    return await Observable.Return(1).Delay(TimeSpan.FromSeconds(1), RxApp.TaskpoolScheduler).ToTask();
}
```


## Icon

[Interchange](https://thenounproject.com/noun/interchange/#icon-No2031) designed by [Laurent Patain](https://thenounproject.com/____Lo) from [The Noun Project](https://thenounproject.com).
