using UnityEngine;
using UnityEngine.UI;

public class MicUI : MonoBehaviour
{
    [SerializeField] private Image image;

    public void Show(bool v)
    {
        image.enabled = v;
    }
}