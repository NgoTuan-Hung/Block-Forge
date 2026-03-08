namespace BlockBlast
{
    using System.Collections.Generic;
    using UnityEngine;

    public class IdentityController
    {
        Dictionary<int, GameObject> identities;

        public IdentityController()
        {
            identities = new();
        }

        public void AddIdentity(int id, GameObject obj)
        {
            identities.Add(id, obj);
        }

        public void RemoveIdentity(int id) => identities.Remove(id);

        public GameObject GetIdentity(int id)
        {
            return identities[id];
        }
    }
}
