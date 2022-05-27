
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
     GuardedVar<bool> _tcv = new GuardedVar<bool>(_mgc);


Example of using the variable after acquiring the lock

    using (mgc.Acquire())
    {
                _tcv.Value = true;
                
    }
