using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

enum Result
{
   NONE,BANKER,PLAYER,TIE
}
public class LottoBaccaratGameController : MonoBehaviour
{
   

   
    [SerializeField] private GameObject coinPrefab;

    [Header("Cards")]
    public List<Card> allCards = new List<Card>();
    private List<Card> dealerCards = new List<Card>();
    private List<Card> playerCards = new List<Card>();

    [SerializeField] private GameObject cardPrefab;

    

    
    [Header("POSITION")]
    public RectTransform canvasTransform;
    public GameObject betCoinHolder;
    public GameObject startPosition;
    public GameObject coinStartPosition;
    public List<GameObject> playerCardPos;
    public List<GameObject> dealerCardPos;
    public Vector2 spawnAreaMin; 
    public Vector2 spawnAreaMax; 
    public float moveDuration = 0.1f;
    public GameObject lostPos;
    public GameObject winPos;

    [Header("BUTTON")]

    public Button standButton;
    public Button drawButton;
    public Button dealButton;



    [Header("SPRITE")]

    public Sprite cardBack;
    public Sprite[] betCoinChangeSprite;
    public Image betCoinChangeImage;

    [Header("Text")]
    public Text playerTotalWinText;
    public Text playerTotalDrawText;
    public Text playerTotalLostText;
    public Text dealerTotalWinText;
    public Text dealerTotalLostText;
    public Text dealerTotalDrawText;
    public Text playerCardValueSumText;
    public Text dealerCardValueSumText;
    public Text messageText;
    public Text messageShowText;
    public Text totalCoinText;
    public Text totalBetText;


    [Header("GameObject")]
    public GameObject betPlace;
    public GameObject messageShowPanel,infoPanel;
    public Image fadeImage;
    float fadeDuration = 1f;

    int totalCoin = 1000000;
    int totalBet = 0;
    int betCoin = 1000;
    int[] betCoinList = { 1000,5000,10000,15000,20000};
    int playerTotalWin = 0;
    int playerTotalDraw = 0;
    int playerTotalLost = 0;
    int dealerTotalWin = 0;
    int dealerTotalLost = 0;
    int dealerTotalDraw = 0;

    string playerWinKey = "player-win";
    string playerLostKey = "player-lost";
    string playerDrawKey = "player-draw";
    string dealerWinKey = "dealer-win";
    string dealerLostKey = "dealer-lost";
    string dealerDrawKey = "dealer-draw";


    private void Awake()
    {
        ConvertToM_K(totalCoin, totalCoinText);
        ConvertToM_K(totalBet, totalBetText);

        playerTotalWinText.text = PlayerPrefs.GetInt(playerWinKey,0).ToString();
        playerTotalDrawText.text = PlayerPrefs.GetInt(playerDrawKey, 0).ToString();
        playerTotalLostText.text = PlayerPrefs.GetInt(playerLostKey, 0).ToString();
        dealerTotalWinText.text = PlayerPrefs.GetInt(dealerWinKey, 0).ToString();
        dealerTotalLostText.text = PlayerPrefs.GetInt(dealerLostKey, 0).ToString();
        dealerTotalDrawText.text = PlayerPrefs.GetInt(dealerDrawKey, 0).ToString();
    }



    private void Start()
    {
        coinPrefab.GetComponent<Image>().sprite = betCoinChangeSprite[0];
        //DealCards();
        fadeImage.gameObject.SetActive(true);
        FadeIn();
        playerTotalWin = PlayerPrefs.GetInt(playerWinKey, 0);
        playerTotalDraw = PlayerPrefs.GetInt(playerDrawKey, 0);
        playerTotalLost = PlayerPrefs.GetInt(playerLostKey, 0);
        dealerTotalWin = PlayerPrefs.GetInt(dealerWinKey, 0);
        dealerTotalLost = PlayerPrefs.GetInt(dealerLostKey, 0);
        dealerTotalDraw = PlayerPrefs.GetInt(dealerDrawKey, 0);

    }


    void Save()
    {
        
         PlayerPrefs.SetInt(playerWinKey, playerTotalWin);
         PlayerPrefs.SetInt(playerDrawKey, playerTotalDraw);
         PlayerPrefs.SetInt(playerLostKey, playerTotalLost);
         PlayerPrefs.SetInt(dealerWinKey, dealerTotalWin);
         PlayerPrefs.SetInt(dealerLostKey, dealerTotalLost);
         PlayerPrefs.SetInt(dealerDrawKey, dealerTotalDraw);
    }











    private List<int> numberList = new List<int>();

    private void DealCards()
    {
        for(int i = 0; i< allCards.Count; i++)
        {
            numberList.Add(i);
        }

        Shuffle(numberList);

        StartCoroutine(DealCardAnimation());
      

    }


    bool isPlayerDraw = false;
    bool isDealerDraw = false;
    int sumDealerValueCard = 0;
    int sumPlayerValueCard = 0;
    IEnumerator DealCardAnimation()
    {
        
        int j = 0;
        int k = 0;

        for (int i = 0; i < 4; i++)
        {
           


            if (i % 2 == 0)
            {

                playerCards.Add(allCards[numberList[i]]);
                SpawnAndMovePrefab(playerCards[j].sprite, startPosition, playerCardPos[j]) ;

                sumPlayerValueCard += playerCards[j].value;
                
                


                j++;




            }
            else
            {
                dealerCards.Add(allCards[numberList[i]]);

                sumDealerValueCard += dealerCards[k].value;





                SpawnAndMovePrefab(cardBack, startPosition, dealerCardPos[k]);
                // SpawnAndMovePrefab(dealerCards[k].sprite, startPosition, dealerCardPos[k]);
                k++;

            }

            

            yield return new WaitForSeconds(1f);

        }

        standButton.interactable = true;
        drawButton.interactable = true;

        sumPlayerValueCard = CovertValue(sumPlayerValueCard);
        sumDealerValueCard = CovertValue(sumDealerValueCard);

        playerCardValueSumText.text = sumPlayerValueCard.ToString();


        Debug.LogWarning($"[total-playercard value] = [{sumPlayerValueCard}]");
        Debug.LogWarning($"[total-dealercard value] = [{sumDealerValueCard}]");

        //isDealerDraw = sumDealerValueCard <= 5;
        DealerDrawLogic();

        

        if(sumPlayerValueCard > 7 || sumDealerValueCard > 7)
        {
            standButton.interactable = false;
            drawButton.interactable = false;

            yield return new WaitForSeconds(1f);
            AudioController.Instance.PlaySFX("card");
            for (int i = 0; i < 2; i++)
            {
                FlipCard(dealerCardPos[i], dealerCards[i].sprite);
            }

            isDealerDraw = isPlayerDraw = false;
            StartCoroutine( WinCheck());
        }

    }

    void FlipCard(GameObject g,Sprite s)
    {
        LeanTween.scaleX(g, 0, 0.5f / 2)
          .setEase(LeanTweenType.easeInQuad)
          .setOnComplete(() =>
          {
               
               g.transform.GetChild(0).GetComponent<Image>().sprite = s;

               
               LeanTween.scaleX(g, 1, 0.5f / 2)
                  .setEase(LeanTweenType.easeOutQuad);

          });
    }


    void DealerDrawLogic()
    {
        float randomValue = Random.Range(0f, 1f); // Generates a random value between 0.0 and 1.0

        if (sumDealerValueCard <= 3)
        {
            isDealerDraw = true; // 100% draw
        }
        else if (sumDealerValueCard >= 3 && sumDealerValueCard <= 5)
        {
            isDealerDraw = randomValue <= 0.8f; // 80% draw
        }
        else if (sumDealerValueCard > 5 && sumDealerValueCard <= 6)
        {
            isDealerDraw = randomValue <= 0.3f; // 30% draw
        }
        else if (sumDealerValueCard == 7)
        {
            isDealerDraw = randomValue <= 0.1f; // 10% draw
        }
        else
        {
            isDealerDraw = false; // No draw
        }

        Debug.Log("Dealer draw: " + isDealerDraw);
    }

    public void OnClickPlayerDrawOrStand(int i)
    {
        AudioController.Instance.PlaySFX("tap");
        switch (i)
        {
            case 1:
                AnimateButtonPress(drawButton.gameObject);
                isPlayerDraw = true;
                break;
            default:
                AnimateButtonPress(standButton.gameObject);
                isPlayerDraw = false;
                break;
        }
       
        standButton.interactable = false;
        drawButton.interactable = false;
        dealButton.interactable = false;
        StartCoroutine(DrawCardOrNot());
    }
   

    IEnumerator DrawCardOrNot()
    {
        if (isPlayerDraw)
        {
           
            playerCards.Add(allCards[numberList[4]]);
            sumPlayerValueCard += playerCards[2].value;
            SpawnAndMovePrefab(playerCards[2].sprite, startPosition, playerCardPos[2]);
        }
       

        yield return new WaitForSeconds(1f);

        if (isDealerDraw)
        {
            if (isPlayerDraw)
            {
                
                dealerCards.Add(allCards[numberList[5]]);
                sumDealerValueCard += dealerCards[2].value;
                SpawnAndMovePrefab(cardBack, startPosition, dealerCardPos[2]);
            }
            else
            {
                dealerCards.Add(allCards[numberList[4]]);
                sumDealerValueCard += dealerCards[2].value;
                SpawnAndMovePrefab(cardBack, startPosition, dealerCardPos[2]);
            }
            
            yield return new WaitForSeconds(1f);
            AudioController.Instance.PlaySFX("card");
            for (int i = 0; i < 3; i++)
            {
                //dealerCardPos[i].transform.GetChild(0).GetComponent<Image>().sprite = dealerCards[i].sprite;
                FlipCard(dealerCardPos[i], dealerCards[i].sprite);
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < 2; i++)
            {
                //dealerCardPos[i].transform.GetChild(0).GetComponent<Image>().sprite = dealerCards[i].sprite;
                FlipCard(dealerCardPos[i], dealerCards[i].sprite);
            }
        }

        sumPlayerValueCard = CovertValue(sumPlayerValueCard);
        sumDealerValueCard = CovertValue(sumDealerValueCard);

        playerCardValueSumText.text = sumPlayerValueCard.ToString();

        Debug.LogWarning($"[total-playercard value] = [{sumPlayerValueCard}]");
        Debug.LogWarning($"[total-dealercard value] = [{sumDealerValueCard}]");

        StartCoroutine( WinCheck());
    }


    IEnumerator WinCheck()
    {
        playerCardValueSumText.text = sumPlayerValueCard.ToString();
        dealerCardValueSumText.text = sumDealerValueCard.ToString();

        if (sumDealerValueCard > sumPlayerValueCard)
        {
            dealerTotalWin += 1;
            playerTotalLost += 1;
            messageText.text = "dealer win !";

            

            messageShowText.text = $"YOU LOSE {totalBet}";

            for (int i = 0; i<betCoinHolder.transform.childCount; i++)
            {
                AudioController.Instance.PlaySFX("tap");
                LeanTween.moveLocal(betCoinHolder.transform.GetChild(i).gameObject, lostPos.transform.localPosition, moveDuration).setEase(LeanTweenType.easeSpring);
                yield return new WaitForSeconds(0.05f);
                //Destroy(betCoinHolder.transform.GetChild(i).gameObject);
            }
            
            print("DEALER WIN 1");
        }else if (sumDealerValueCard < sumPlayerValueCard)
        {
            totalCoin += totalBet * 2;

            playerTotalWin += 1;
            dealerTotalLost += 1;
            messageText.text = "player win !";

            print("PLAYER WIN 1");

            messageShowText.text = $"YOU WIN {totalBet * 2}";
           


            for (int i = 0; i < betCoinHolder.transform.childCount; i++)
            {
                AudioController.Instance.PlaySFX("coin");
                GameObject spawnedCoin = Instantiate(coinPrefab, new Vector3(Random.Range(-100,100), Random.Range(-100, 100),0), Quaternion.identity);
                spawnedCoin.GetComponent<Image>().sprite = betCoinHolder.transform.GetChild(i).GetComponent<Image>().sprite;
                spawnedCoin.transform.SetParent(lostPos.transform, false);
                yield return new WaitForSeconds(0.05f);
             
                //LeanTween.moveLocal(spawnedCoin, winPos.transform.localPosition, moveDuration).setEase(LeanTweenType.easeSpring);
                //Destroy(betCoinHolder.transform.GetChild(i).gameObject);
            }


            for (int i = 0; i < betCoinHolder.transform.childCount; i++)
            {
                AudioController.Instance.PlaySFX("tap");
                LeanTween.moveLocal(betCoinHolder.transform.GetChild(i).gameObject, winPos.transform.localPosition, moveDuration).setEase(LeanTweenType.easeSpring);
                yield return new WaitForSeconds(0.05f);
                //Destroy(betCoinHolder.transform.GetChild(i).gameObject);
            }
            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < lostPos.transform.childCount; i++)
            {
                AudioController.Instance.PlaySFX("tap");
                LeanTween.moveLocal(lostPos.transform.GetChild(i).gameObject, new Vector3(winPos.transform.localPosition.x, winPos.transform.localPosition.y - 700, winPos.transform.localPosition.z), moveDuration).setEase(LeanTweenType.easeSpring);
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            playerTotalDraw += 1;
            dealerTotalDraw += 1;
            messageText.text = "draw !";
            print("DEALER WIN 1");
            totalCoin += totalBet;

            messageShowText.text = "DRAW";

            for (int i = 0; i < betCoinHolder.transform.childCount; i++)
            {
                AudioController.Instance.PlaySFX("tap");
                LeanTween.moveLocal(betCoinHolder.transform.GetChild(i).gameObject,coinStartPosition.transform.localPosition
                    , moveDuration).setEase(LeanTweenType.easeSpring);
                yield return new WaitForSeconds(0.05f);
               
            }
        }

        ConvertToM_K(totalCoin, totalCoinText);
        ConvertToM_K(totalBet, totalBetText);

        AnimateScoreUpdate(totalBetText);
        AnimateScoreUpdate(totalCoinText);

        ShowWinningAnimation(messageShowText.gameObject);

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < betCoinHolder.transform.childCount; i++)
        {
            
            Destroy(betCoinHolder.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < lostPos.transform.childCount; i++)
        {
            Destroy(lostPos.transform.GetChild(i).gameObject);
        }

        playerTotalWinText.text = playerTotalWin.ToString();
        playerTotalLostText.text = playerTotalLost.ToString();
        playerTotalDrawText.text = playerTotalDraw.ToString();

        dealerTotalWinText.text = dealerTotalWin.ToString();
        dealerTotalLostText.text = dealerTotalLost.ToString();
        dealerTotalDrawText.text = dealerTotalDraw.ToString();

        StartCoroutine(ResetGame());
    }



    IEnumerator ResetGame()
    {
        Save();
        messageShowPanel.SetActive(true);
        messageShowText.gameObject.SetActive(true);
        messageShowText.text = "new round!";
        yield return new WaitForSeconds(1f);

        playerCards.Clear();
        dealerCards.Clear();
        numberList.Clear();



        //destroy player card prefab
        if (isPlayerDraw)
        {
            for(int i = 0; i < 3; i++)
            {
                Destroy(playerCardPos[i].transform.GetChild(0).gameObject);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                Destroy(playerCardPos[i].transform.GetChild(0).gameObject);
            }
        }


        //destroy dealer card prefab
        if (isDealerDraw)
        {
            for (int i = 0; i < 3; i++)
            {
                Destroy(dealerCardPos[i].transform.GetChild(0).gameObject);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                Destroy(dealerCardPos[i].transform.GetChild(0).gameObject);
            }
        }

       

        yield return new WaitForSeconds(1f);
        isGetBet = true;
        messageShowPanel.SetActive(false);
        messageShowText.gameObject.SetActive(false);
        drawButton.interactable = false;
        standButton.interactable = false;
        dealButton.interactable = false;
        sumPlayerValueCard = sumDealerValueCard = 0;

        playerCardValueSumText.text = "0";
        dealerCardValueSumText.text = "";
        messageText.text = "click hear to bet";
        totalBet = 0;
        totalBetText.text = "0K";

        isDealerDraw = isPlayerDraw = false;

       // DealCards();

    }


    private  int CovertValue(int value)
    {

        if (value > 10)
        {
            return value % 10;
        }else if (value == 10)
        {
            return 0;
        }
        else {
            return value;
        }

        
    }


    int state = 0;
    public void OnClickChangeBetCoin(string name)
    {
        AudioController.Instance.PlaySFX("bet-c");
        if (name == ">")
        {
            state += 1;
            
            if(state >= betCoinChangeSprite.Length)
            {
                state = 0;
                betCoinChangeImage.sprite = betCoinChangeSprite[state];
                betCoin = betCoinList[state];
            }
            else
            {
                betCoinChangeImage.sprite = betCoinChangeSprite[state];
                betCoin = betCoinList[state];
            }
        }
        else
        {
            state -= 1;
            
            if(state < 0)
            {
                
                state = betCoinChangeSprite.Length - 1;
                betCoinChangeImage.sprite = betCoinChangeSprite[state];
                betCoin = betCoinList[state];
            }
            else
            {
                betCoinChangeImage.sprite = betCoinChangeSprite[state];
                betCoin = betCoinList[state];
            }
        }

        coinPrefab.GetComponent<Image>().sprite = betCoinChangeSprite[state];
    }

    

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }



    
    



    
    void SpawnAndMovePrefab(Sprite s,GameObject startPos, GameObject endPos)
    {
        AudioController.Instance.PlaySFX("card");
        GameObject c = Instantiate(cardPrefab, startPos.transform.position,Quaternion.Euler(0,0,endPos.transform.localRotation.z));
        c.transform.SetParent(endPos.transform, false);
        c.GetComponent<Image>().sprite = s;
      

        LeanTween.move(c, endPos.transform, 0.5f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.rotate(c, endPos.transform.eulerAngles, 1f).setEase(LeanTweenType.easeInOutQuad);
    }

    public void OnClickDeal()
    {
        isGetBet = false;
        AnimateButtonPress(dealButton.gameObject);
        AudioController.Instance.PlaySFX("tap");
        dealButton.interactable = false;
        DealCards();
        
    }

    bool isGetBet = true;
    public void OnClickGetBet()
    {
        if (isGetBet)
        {
            AnimateButtonPress(betPlace);
            if (totalCoin > 0 && totalCoin >= betCoin)
            {
                AudioController.Instance.PlaySFX("coin");
                totalCoin -= betCoin;
                totalBet += betCoin;
                ConvertToM_K(totalCoin, totalCoinText);
                ConvertToM_K(totalBet, totalBetText);

                AnimateScoreUpdate(totalBetText);
                AnimateScoreUpdate(totalCoinText);

                SpawnCoins();
                dealButton.interactable = true;
            }
        }
    }

    void SpawnCoins()
    {
        //AudioController.Instance.PlayMusic("card");
        Vector2 randomPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

            
        GameObject spawnedCoin = Instantiate(coinPrefab, new Vector3(coinStartPosition.transform.localPosition.x, coinStartPosition.transform.localPosition.y, coinStartPosition.transform.localPosition.z), Quaternion.identity);
        spawnedCoin.transform.SetParent(betCoinHolder.transform, false);

        LeanTween.moveLocal(spawnedCoin, randomPosition, moveDuration).setEase(LeanTweenType.easeSpring);
        
    }





    public Button curCashBTN;
    public void ChangeGuessCointImageButton(Button btn)
    {
        if (btn.GetInstanceID() != curCashBTN.GetInstanceID())
        {
            GameObject blockPanel = btn.transform.GetChild(0).gameObject;
            blockPanel.SetActive(true);

            blockPanel = curCashBTN.transform.GetChild(0).gameObject;
            blockPanel.SetActive(false);
            curCashBTN = btn;
        }
    }

    public static void ConvertToM_K(int coint, Text cointText)
    {
        if (coint < 0)
        {
            coint = -coint;
            if (coint >= 1000000 && coint % 1000000 == 0)
                cointText.text = (coint / 1000000).ToString() + "M";
            else if (coint >= 1000000 && coint % 1000000 != 0)
                cointText.text = (coint / 1000000).ToString() + "M" + " " + ((coint % 1000000) / 1000) + "k";
            else if (coint >= 1000)
                cointText.text = (coint / 1000).ToString() + "K";
            else
                cointText.text = "OK";
        }
        else
        {
            if (coint >= 1000000 && coint % 1000000 == 0)
                cointText.text = (coint / 1000000).ToString() + "M";
            else if (coint >= 1000000 && coint % 1000000 != 0)
                cointText.text = (coint / 1000000).ToString() + "M" + " " + ((coint % 1000000) / 1000) + "k";
            else if (coint >= 1000)
                cointText.text = (coint / 1000).ToString() + "K";
            else
                cointText.text = "OK";
        }

    }

    public void ReloadScene(GameObject game)
    {
        AnimateButtonPress(game);
        AudioController.Instance.PlaySFX("tap");
        FadeOutAndLoadScene("home-scene");

        
    }


    public void AnimateButtonPress(GameObject button)
    {
        LeanTween.scale(button, Vector3.one * 0.9f, 0.1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            LeanTween.scale(button, Vector3.one, 0.1f).setEase(LeanTweenType.easeInOutQuad);
        });
    }

    public void ShowWinningAnimation(GameObject winningText)
    {
        winningText.SetActive(true);
        messageShowPanel.SetActive(true);
        LeanTween.scale(winningText, Vector3.one * 1.1f, 0.5f).setEase(LeanTweenType.easeOutElastic).setOnComplete(() =>
        {
            LeanTween.scale(winningText, Vector3.one, 0.5f).setEase(LeanTweenType.easeInElastic).setOnComplete(() => {
                winningText.SetActive(false);
                messageShowPanel.SetActive(false);
            });
        });
    }

    public void AnimateScoreUpdate(Text scoreText)
    {
        LeanTween.scale(scoreText.gameObject, Vector3.one * 1.2f, 0.3f).setEase(LeanTweenType.easeOutBounce).setOnComplete(() =>
        {
            LeanTween.scale(scoreText.gameObject, Vector3.one, 0.3f).setEase(LeanTweenType.easeInBounce);
        });
    }

    public void OnClickOpenInfoPanel(GameObject game)
    {
        AnimateButtonPress(game);
        AudioController.Instance.PlaySFX("tap");
        LeanTween.scale(infoPanel, new Vector3(1, 1, 1), 1f).setEaseOutQuart();
    }
    public void OnClickCloseInfoPanel(GameObject game)
    {
        AnimateButtonPress(game);
        AudioController.Instance.PlaySFX("tap");
        LeanTween.scale(infoPanel, new Vector3(0, 0, 0), 0.5f).setEaseInQuart();
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
}//end of class
