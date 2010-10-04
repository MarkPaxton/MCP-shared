using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

namespace mcp
{
   public class ItemExpiredEventArgs
   {
      public ItemExpiredEventArgs(ArrayList items)
      {
         _items = items;
      }
      private ArrayList _items;

      public ArrayList Items
      {
         get { return _items; }
      }
   }

   public delegate void ItemExpiredEventHandler(ItemExpiredEventArgs args);

   public class ObjectCache
   {
      System.Threading.Timer cacheTimer = null;
      TimerCallback timerTimeoutCallback;
      Hashtable items = null;
      Hashtable expiries = null;

      long cacheTime = 0;

      Object cacheLock = new Object();

      public event ItemExpiredEventHandler ItemExpired;

      /// <summary>
      /// Object cache caches an object for a given cache time and raises an event when it's
      /// expired.
      /// </summary>
      /// <param name="cacheTime">Time to cache objects in micro seconds (1000 = 1sec)</param>
      public ObjectCache(long cacheTime)
      {
         items = new Hashtable();
         expiries = new Hashtable();
         timerTimeoutCallback = new TimerCallback(itemExpired);
         this.cacheTime = cacheTime;
      }


      ~ObjectCache()
      {
         if (cacheTimer != null)
         {
            cacheTimer.Dispose();
            cacheTimer = null;
         }
      }

      public object GetItem(object key)
      {
         object toReturn = null;
         lock (cacheLock)
         {
            toReturn = items[key];
         }
         return toReturn;
      }

      public void AddItem(object item, object key)
      {
         lock (cacheLock)
         {
            items[key] = item;
            expiries[key] = UtcNowMicroS() + cacheTime;
            updateTime();
         }
      }

      public void FreshenItem(object key)
      {
         lock (cacheLock)
         {
            if (items.ContainsKey(key))
            {
               expiries[key] = UtcNowMicroS() + cacheTime;
               updateTime();
            }
         }
      }

      private void updateTime()
      {
         if (expiries.Count > 0)
         {
            long[] times = new long[expiries.Count];
            expiries.Values.CopyTo(times, 0);
            Array.Sort(times);
            
            long time = ((long)times[0]) - UtcNowMicroS();

            if (time < 0)
            {
               time = 0;
            }
            if (cacheTimer == null)
            {
               cacheTimer = new System.Threading.Timer(itemExpired, null, time, Timeout.Infinite);
            }
            else
            {
               cacheTimer.Change(time, Timeout.Infinite);
            }
         }
         else
         {
            if (cacheTimer != null)
            {
               cacheTimer.Dispose();
               cacheTimer = null;
            }
         }
      }

      /// <summary>
      /// Return the current time in microsecongs not Ticks
      /// </summary>
      /// <returns></returns>
      private long UtcNowMicroS()
      {
         // Need to convert ticks to microSeconds
         // 1 tick = 100 nanoSeconds
         // 1000 nano seconds = 1 microSeconds
         long ticks = DateTime.UtcNow.Ticks;
         long microS = Convert.ToInt64(ticks / 1000);
         return microS;
      }

      private void itemExpired(object ob)
      {
         lock (cacheLock)
         {
            object[] keys = new object[expiries.Count];
            expiries.Keys.CopyTo(keys, 0);
            ArrayList removedItems = new ArrayList();
            foreach (object key in keys)
            {
               if (((long)expiries[key]) < UtcNowMicroS())
               {
                  removedItems.Add(items[key]);
                  items.Remove(key);
                  expiries.Remove(key);
               }
            }
            updateTime();

            if (removedItems.Count > 0)
            {
               ItemExpired(new ItemExpiredEventArgs(removedItems));
            }
         }

      }

   }
}
