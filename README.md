# PersistentCollections

I needed a very simple way of storing the state of my .Net application across restarts.
My requirements were: 
 - very simple to use
 - thread safe
 - no database setup
 - high performance is not required
 
I belive the 
```cs
PersistentDictionary<TKey, TValue> 
```
class should do the job, because it is a drop-in replacement for 
```cs
Dictionary<TKey,TValue>
```
 , it follows the SyncRoot pattern for thread safety and only requires a simple file on the disk.
It is not battle tested yet, but I will be using it soon.
