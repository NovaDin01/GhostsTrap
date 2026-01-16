    using System;
    using UnityEngine;

    public class Shop : MonoBehaviour
    {
        private Upgrade upgrade;

        private void Update()
        {
            
        }

        public void BuyUpgrade(Upgrade upgrade)
        {
            upgrade.Get();
        }
        
    }
