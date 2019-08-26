using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Custom
{
    public static class Utils
    {
        public static void SetUniqueChildVisible(Transform transform, bool isVisible = true)
        {
            SetChildrenVisible(transform.parent, !isVisible);
            transform.gameObject.SetActive(isVisible);
        }

        public static void SetChildrenVisible(IEnumerable transform, bool isVisible = true)
        {
            foreach (Transform child in transform) child.gameObject.SetActive(isVisible);
        }

        public static IEnumerator Translate(Transform transform, float duration, AnimationCurve curve, Vector3 from,
            Vector3 to)
        {
            var time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(from, to, curve.Evaluate(time / duration));
                yield return null;
            }

            transform.localPosition = to;
            yield return null;
        }

        public static IEnumerator Rotate(Transform transform, float duration, AnimationCurve curve, Vector3 from,
            Vector3 to)
        {
            var time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                transform.localEulerAngles = Vector3.Lerp(from, to, curve.Evaluate(time / duration));
                yield return null;
            }

            transform.localEulerAngles = to;
            yield return null;
        }

        public static IEnumerator Scale(Transform transform, float duration, AnimationCurve curve, Vector3 from,
            Vector3 to)
        {
            var time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                transform.localScale = Vector3.Lerp(from, to, curve.Evaluate(time / duration));
                yield return null;
            }

            transform.localScale = to;
            yield return null;
        }

//        private static T GetComponentInParent<T>(Transform transform) where T : class
//        {
//            while (true)
//            {
//                if (transform.parent == null) return null;
//                var t = transform.parent.GetComponent<T>();
//                if (t != null) return t;
//                transform = transform.parent;
//            }
//        }

//        private static string GetOrdinal(int number)
//        {
//            if( number <= 0 ) return number.ToString();
//            number %= 100;
//            if (number != 11 && number != 12 && number != 13)
//            {
//                number %= 10;
//                if (number  == 1) return "st";
//                if (number == 2) return  "nd";
//                if (number == 3) return  "rd";
//            }
//            return "th";
//        }

        public static IEnumerator ScrollTo(ScrollRect scrollRect, float duration, AnimationCurve curve,
            Vector2 from, Vector2 to)
        {
            scrollRect.verticalNormalizedPosition = 0f;
            yield return new WaitForSeconds(.2f);
            scrollRect.verticalNormalizedPosition = 0f;
//            var time = 0f;
//            while (time < duration)
//            {
//                time += Time.deltaTime;
//                scrollRect.normalizedPosition = Vector2.Lerp(from, to,curve.Evaluate(time / duration));
//                yield return null;
//            }

//            scrollRect.normalizedPosition = to;
//            yield return null;
        }
    }
}