using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
public class LoadingScript : MonoBehaviour
{

    public Image fadeImage;
    
    IEnumerator Start()
    {
        fadeImage.gameObject.SetActive(true);
        FadeIn();
        LeanTween.scale(gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.5f).setEaseSpring().setLoopPingPong();
        yield return new WaitForSeconds(3f);
        FadeOutAndLoadScene("home-scene");
    }

    public void FadeIn()
    {
        LeanTween.alpha(fadeImage.rectTransform, 0f, 1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false);
        });
    }

    public void FadeOutAndLoadScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        LeanTween.alpha(fadeImage.rectTransform, 1f, 1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }
}
