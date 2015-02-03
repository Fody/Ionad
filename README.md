## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Ionad Icon - A black circle with 3 arrows forming an outer circle.](https://raw.github.com/Fody/Ionad/master/Icons/package_icon.png)

Ionad replaces static types with your own.

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/Ionad.Fody.svg?style=flat)](https://www.nuget.org/packages/Ionad.Fody/)

https://nuget.org/packages/Ionad.Fody/

    PM> Install-Package Ionad.Fody

### Your Code

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


### What gets compiled 

    public void SomeMethod()
    {
        var time = DateTimeSubstitute.Now;
        // ...
    }

## Contributors

  * [Cameron MacFarland](https://github.com/distantcam)
  * [Simon Cropp](https://github.com/simoncropp)

## Icon

[Interchange](http://thenounproject.com/noun/interchange/#icon-No2031) designed by [Laurent Patain](http://thenounproject.com/____Lo) from The Noun Project
