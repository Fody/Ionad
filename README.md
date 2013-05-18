## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Ionad replaces static types with your own.

## Nuget 

Nuget package http://nuget.org/packages/Ionad.Fody 

To Install from the Nuget Package Manager Console 
    
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

## Icon

[Interchange](http://thenounproject.com/noun/interchange/#icon-No2031) designed by [Laurent Patain](http://thenounproject.com/____Lo) from The Noun Project
