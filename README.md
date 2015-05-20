# ObjectMap
ObjectMap is a simple **IoC container for .NET**. 

[![Build Status](https://travis-ci.org/robinmaben/ObjectMap.svg?branch=master)](https://travis-ci.org/robinmaben/ObjectMap) 

[![NuGet version](https://badge.fury.io/nu/ObjectMap.svg)](http://badge.fury.io/nu/ObjectMap)

It started out as a [practice exercise](http://blog.mabenrob.in/post/objetmap-reinventing-the-wheel-learning-by-synthesis) and an excuse to try out features of the **C# 6 preview**.

**Key Features -**
1. Auto-inject constructor dependencies
2. Auto-inject property dependencies
3. Supports lazy instantiation
4. Lifecycle options Singleton, PerRequest (default being LastCreated)


**Simple (although contrived) usage examples -**

<pre>
<code>
public class Logger : ILogger
{
    public Logger(ILogSettings logSettings)
    {
        
    }
    
    ILogFormat LogFormat{ get; set; }
}

public class LogSettings : ILogSettings
{
    ILogFile LogFile { get; set; }
}

</code>
</pre>

`ObjectMap.Register<ILogFormat, LogFormat>();`

`ObjectMap.Register<ILogSettings>(new LogSettings());`

`ObjectMap.Register<ILogFile>(() => new LogFile()); //Lazy`

`ObjectMap.Register<ILogger, Logger>().Singleton(); //default lifecycle option is .LastCreated()`

`ObjectMap.Register<ILogger, Logger>().PerRequest();`
