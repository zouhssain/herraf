using Custom;
using Data;
using UnityEngine;

namespace UI
{
    public class Rater : MonoBehaviour
    {
        
        [SerializeField] private int initialCount = 1;
        [SerializeField] private int remindLaterCount = 2;
        [SerializeField] private string rateUrl;

        private void Awake()
        {
            RateAppShow();
        }

        private void RateAppShow()
        {
            if (CanShowRateIt() && Application.internetReachability != NetworkReachability.NotReachable) 
                Utils.SetChildrenVisible(transform);
            else Destroy(gameObject);
        }

        public void RateIt()
        {
            Values.Rate = 10000;
            Application.OpenURL(rateUrl);
            Destroy(gameObject);
        }

        public void RemindMeLater()
        {
            Values.Rate = remindLaterCount;
            Destroy(gameObject);
        }

        private bool CanShowRateIt()
        {
            return (Values.Rate = (Values.Rate == -1 ? initialCount : Values.Rate) - 1) < 0;
        }
    }
}