using UnityEngine;

namespace Controllers
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animation buildingShop;
        [SerializeField] private Animation notification;
        private bool shopActivated = false;
        private bool notificationActivated = false;

        void Start()
        {
            ToggleNotificationVisibility();
        }
        
        public void ToggleShopVisibility()
        {
            if (!shopActivated)
            {
                buildingShop["ShowShop"].time = 0;
                buildingShop["ShowShop"].speed = 1;
                buildingShop.Play("ShowShop");
            }
            else
            {
                // Play backwards
                buildingShop["ShowShop"].time = buildingShop["ShowShop"].length;
                buildingShop["ShowShop"].speed = -1;
                buildingShop.Play("ShowShop");
            }
            shopActivated = !shopActivated;
        }
        
        public void ToggleNotificationVisibility()
        {
            if (!notificationActivated)
            {
                notification["ShowNotification"].time = 0;
                notification["ShowNotification"].speed = 1;
                notification.Play("ShowNotification");
            }
            else
            {
                // Play backwards
                // notification["ShowNotification"].time = buildingShop["ShowNotification"].length;
                // notification["ShowNotification"].speed = -1;
                // notification.Play("ShowNotification");
            }
            notificationActivated = !notificationActivated;
        }
    }
}
