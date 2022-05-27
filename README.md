
# ThreadGuard

This code is based on an idea recommended for C# - this could be done better at the C# and compiler level in some cases so voting might help get this in
https://github.com/dotnet/csharplang/discussions/892

Multithreaded code is easy to get wrong. A couple of the main concerns are:

 1. Race conditions due to unprotected variable access are easy to introduce/ hard to detect
 2.  Deadlocks due to lock ordering
 3.  Livelocks, lock contention, etc.


This code addresses some areas of problem #1. It does this by helping to make multithreading clear/defined about which synchronization objects are protecting which code elements more visible. Currently when using locks developers reading others code need to infer what variables are to be protected and by which synchronization objects by looking at all of the variable accessed in a code block between acquiring and release of a lock. If we can make this more visible in code it will be easier to tell what variables/objects should be protected and by which synchronization objects. Also this code has the ability to detect when unprotected access to variables/objects is attempted to catch when these problems happen.


**Goals**

 - Make it easy to visualize protected and unprotected variable access
 - Detect unprotected variable access





