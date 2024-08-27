using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    public GameObject infoPanel;
    public Text playerTotalWinText;
    public Text playerTotalDrawText;
    public Text playerTotalLostText;
    public Text dealerTotalWinText;
    public Text dealerTotalLostText;
    public Text dealerTotalDrawText;
    string playerWinKey = "player-win";
    string playerLostKey = "player-lost";
    string playerDrawKey = "player-draw";
    string dealerWinKey = "dealer-win";
    string dealerLostKey = "dealer-lost";
    string dealerDrawKey = "dealer-draw";

    void Start()
    {
        playerTotalWinText.text = PlayerPrefs.GetInt(playerWinKey, 0).ToString();
        playerTotalDrawText.text = PlayerPrefs.GetInt(playerDrawKey, 0).ToString();
        playerTotalLostText.text = PlayerPrefs.GetInt(playerLostKey, 0).ToString();
        dealerTotalWinText.text = PlayerPrefs.GetInt(dealerWinKey, 0).ToString();
        dealerTotalLostText.text = PlayerPrefs.GetInt(dealerLostKey, 0).ToString();
        dealerTotalDrawText.text = PlayerPrefs.GetInt(dealerDrawKey, 0).ToString();

        fadeImage.gameObject.SetActive(true);
        FadeIn();
    }

    public void GoToGame(GameObject game)
    {

        AnimateButtonPress(game);
        AudioController.Instance.PlaySFX("tap");
        FadeOutAndLoadScene("game-scene");
    }

    public void QuitButton(GameObject game)
    {
        AnimateButtonPress(game);
        AudioController.Instance.PlaySFX("tap");
        Application.Quit();
    }



    public void FadeIn()
    {
        LeanTween.alpha(fadeImage.rectTransform, 0f, fadeDuration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false);
        });
    }

    public void FadeOutAndLoadScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        
        LeanTween.alpha(fadeImage.rectTransform, 1f, fadeDuration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    public void OnClickOpenInfoPanel(GameObject game)
    {
        AudioController.Instance.PlaySFX("tap");
        AnimateButtonPress(game);
        LeanTween.scale(infoPanel, new Vector3(1, 1, 1), 1f).setEaseOutQuart();
    }
    public void OnClickCloseInfoPanel(GameObject game)
    {
        AudioController.Instance.PlaySFX("tap");
        AnimateButtonPress(game);
        LeanTween.scale(infoPanel, new Vector3(0, 0, 0), 0.5f).setEaseInQuart();
    }

    public void AnimateButtonPress(GameObject button)
    {
        LeanTween.scale(button, Vector3.one * 0.9f, 0.1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            LeanTween.scale(button, Vector3.one, 0.1f).setEase(LeanTweenType.easeInOutQuad);
        });
    }
}
