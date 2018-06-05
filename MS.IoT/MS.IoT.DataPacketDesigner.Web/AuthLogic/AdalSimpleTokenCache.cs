using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Web;

namespace MS.IoT.DataPacketDesigner.Web
{
    /// <summary>
    /// Class helper to store a token into cache
    /// </summary>
    public class AdalSimpleTokenCache : TokenCache
    {
        private static readonly object SessionLock = new object();
        private readonly string CacheId = string.Empty;
        private string UserObjectId = string.Empty;

        public AdalSimpleTokenCache(string userId)
        {
            UserObjectId = userId;
            CacheId = UserObjectId + "_TokenCache";

            AfterAccess = AfterAccessNotification;
            BeforeAccess = BeforeAccessNotification;
            Load();
        }

        public void Load()
        {
            lock (SessionLock)
            {
                if (HttpContext.Current != null)
                {
                    Deserialize((byte[])HttpContext.Current.Session[CacheId]);
                }
            }
        }

        public void Persist()
        {
            lock (SessionLock)
            {
                // reflect changes in the persistent store
                HttpContext.Current.Session[CacheId] = Serialize();
                // once the write operation took place, restore the HasStateChanged bit to false
                HasStateChanged = false;
            }
        }

        // Empties the persistent store.
        public override void Clear()
        {
            base.Clear();
            HttpContext.Current.Session.Remove(CacheId);
        }

        public override void DeleteItem(TokenCacheItem item)
        {
            base.DeleteItem(item);
            Persist();
        }

        // Triggered right before ADAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        // Triggered right after ADAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (HasStateChanged)
            {
                Persist();
            }
        }

    }
}