namespace Meyer.Socrates.Services
{
    using Meyer.Socrates.Data;
    using System;
    using System.Collections.Generic;

    internal class Ms3dmmCollectionResolver: IResolver<uint, Ms3dmmCollection>
    {
        private Dictionary<uint, Ms3dmmCollection> dictionary = new Dictionary<uint, Ms3dmmCollection>();
        static volatile Ms3dmmCollectionResolver singleton;

        public static Ms3dmmCollectionResolver Default => singleton ?? (singleton = new Ms3dmmCollectionResolver());

        private Ms3dmmCollectionResolver()
        {
        }

        public Ms3dmmCollection Resolve(uint collectionID, IProgress<ProgressInfo> progress = null)
        {
            var directories = Ms3dmm.GetCollectionDirectories(collectionID);
            if (!dictionary.TryGetValue(collectionID, out var coll))
                dictionary[collectionID] = coll = new Ms3dmmCollection(collectionID, directories, new ThrottleProgressInfo(progress));
            return coll;
        }

        Ms3dmmCollection IResolver<uint, Ms3dmmCollection>.Resolve(uint input)
        {
            return Resolve(input);
        }
    }
}
