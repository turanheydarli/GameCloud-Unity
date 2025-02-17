using System.Collections;
using UnityEngine;

namespace GameCloud
{
    public static class CoroutineExtensions
    {
        public static void ToCoroutine(this IEnumerator enumerator)
        {
            GameCloudCoroutineRunner.Instance.StartCoroutine(enumerator);
        }
    }
}