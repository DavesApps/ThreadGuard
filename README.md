
# ThreadGuard

This code is based on an idea recommended for C# - this could be done better at the C# and compiler level in some cases so voting might help get this in (GuardedBy feature)  https://github.com/dotnet/csharplang/discussions/892

Multithreaded code is easy to get wrong. A couple of the main concerns are:

 1. Race conditions due to unprotected variable access are easy to introduce/ hard to detect
 2.  Deadlocks due to lock ordering
 3.  Livelocks, lock contention, etc.


This code addresses some areas of problem #1. It does this by helping to make multithreading clear/defined about which synchronization objects are protecting which code elements more visible. Currently when using locks developers reading others code need to infer what variables are to be protected and by which synchronization objects by looking at all of the variable accessed in a code block between acquiring and release of a lock. If we can make this more visible in code it will be easier to tell what variables/objects should be protected and by which synchronization objects. Also this code has the ability to detect when unprotected access to variables/objects is attempted to catch when these problems happen.


**Goals**

 - Make it easy to visualize protected and unprotected variable access
 - Detect unprotected variable access

This isn't a new idea Java already has something similar.

It's easy to make your multithreaded variable decisions visible and protect your variables from access without the synchronization object being acquired.

Example of creating a Monitor (standard C# lock) and using it to protect a bool variable. You can protect as many variables as you want.

     MonitorGuard _mgc = new MonitorGuard();
     GuardedVar<bool> _tcv = new GuardedVar<bool>(_mgc); // this variable is guarded by the _mgc monitor/lock


Example of using the variable after acquiring the lock

    using (mgc.Acquire())
    {
                _tcv.Value = true;
                
    }

If you use the guarded variable without acquiring the lock first it will throw a configurable exception by default that is GuardFailureException.

With that code it's easy to tell which of your variables need multithreading protection and it will throw an exception when used unprotected.
These are very lightweight checks, so performance should not be impacted.

ThreadGuard also lets you encode your intentions for a lock free variable accessing via Volatile.Read and Volatile.Write methods.  Normally when you do this many cases you can only have one thread writing to the variable and other threads can read.  Sometimes you only use variables like this to tell a thread to exit - typically the only safe usage is for example defaulting a bool to false like IsDone and setting to true (ideally from one thread- though other variants exist).

You can encode your intentions as follows.

     GuardedVolatileVarBool _tgvvIsDone = new GuardedVolatileVarBool(true);
     _tgvvIsDone.Value = false;


            Thread thread1 = new Thread(() =>
            {
                while (_tgvvIsDone.Value)
                {  //do something here
                }
            });
            thread1.Start();
            //do something
            _tgvvIsDone.Value = true; Will cause thread to exit
            thread1.Join();



So with this example it's easy to see variable _tgvvIsDone is protected and if it's used inappropriately like writing to the value from multiple threads it will throw an exception.  It is also best good practice of usint volatile for variable access which it does in that class.  It will even prevent changing the value to something different if you desire (only allowing one write to the value - i.e. one shot flag is often the safest way to use)

**Summary**
So in the above code you can
- Make your intentions clear about the protected variables
- It's clear which synchronization objects are protecting which variables
- Detect impropter access of variables without proper synch object acquisition
- Clarify the intentions of volatile variable access and ensure use in a safer way


So please vote for the idea if you like it (as in the compiler they could detect improper use at compile time for some cases), and in the meantime consider using something like this or these ideas to make your multithreaded code safer.
