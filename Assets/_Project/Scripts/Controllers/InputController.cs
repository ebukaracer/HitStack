using UnityEngine;

internal class InputController : MonoBehaviour
{
    public static bool HasClicked
    {
        get
        {
            if (GameManager.Instance.CurrentState != GameState.Playing)
                return false;


            if (Input.GetMouseButton(0))
                return true;

            if (Input.GetMouseButtonUp(0))
                return false;

            return false;


            /* TODO:
#if UNITY_ANDROID && !UNITY_EDITOR

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                return true;

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
                return false;

            return false;
#endif
            */
        }
    }
}
