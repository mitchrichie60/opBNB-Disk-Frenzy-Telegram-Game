using System.Collections;
using UnityEngine;
using Thirdweb;
using Thirdweb.Unity;
using TMPro;
using System.Numerics;
using System;
using UnityEngine.SceneManagement;

public class BlockchainManager : MonoBehaviour
{
    private const string playerTokenString = "PlayerToken";
    public TMP_Text logText;
    public TMP_Text balanceText;

    public string Address { get; private set; }
    public static BigInteger ChainId = 204;

    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button claimTokenButton;
    public UnityEngine.UI.Button getBalanceButton;

    string customSmartContractAddress = "0x0B305B9487cb76990E5eC7cB4E4647F31B37E32F";
    string abiString = "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"currentPoints\",\"type\":\"uint256\"}],\"name\":\"DiskPointsIncreased\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"newTotalPoints\",\"type\":\"uint256\"}],\"name\":\"TotalPointsUpdated\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"getDiskPoints\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"points\",\"type\":\"uint256\"}],\"name\":\"increaseDiskPoints\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalDiskPoints\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

    int tokenAmount = 1;

    string notEnoughToken = " BNB";

    private void Start()
    {
        int currentTokens = PlayerPrefs.GetInt(playerTokenString, 0);
        balanceText.text = "PowerUp Owned: " + currentTokens.ToString();
    }

    public void SwitchToMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void AddPlayerToken()
    {
        int currentTokens = PlayerPrefs.GetInt(playerTokenString, 0);
        currentTokens++;
        PlayerPrefs.SetInt(playerTokenString, currentTokens);
        PlayerPrefs.Save();
        currentTokens = PlayerPrefs.GetInt(playerTokenString, 0);
        balanceText.text = "PowerUp Owned: " + currentTokens.ToString();
        Debug.Log("PlayerToken updated to: " + currentTokens);
    }

    private void HideAllButtons()
    {
        playButton.interactable = false;
        claimTokenButton.interactable = false;
        getBalanceButton.interactable = false;
    }

    private void ShowAllButtons()
    {
        playButton.interactable = true;
        claimTokenButton.interactable = true;
        getBalanceButton.interactable = true;
    }

    private void UpdateStatus(string messageShow)
    {
        logText.text = messageShow;
    }

    private void BoughtSuccessFully()
    {
        AddPlayerToken();
        UpdateStatus("Got 1 PowerUp");
    }
    IEnumerator WaitAndExecute()
    {
        Debug.Log("Coroutine started, waiting for 3 seconds...");
        yield return new WaitForSeconds(3f);
        Debug.Log("3 seconds have passed!");
        BoughtSuccessFully();
        ShowAllButtons();
    }

    private async void Claim1Token()
    {
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var contract = await ThirdwebManager.Instance.GetContract(
           customSmartContractAddress,
           ChainId,
           abiString
       );
        var address = await wallet.GetAddress();
        await ThirdwebContract.Write(wallet, contract, "increaseDiskPoints", 0, address, 1);

        var result = ThirdwebContract.Read<int>(contract, "getDiskPoints", address);
        Debug.Log("result: " + result);
    }

    public async void GetTokens()
    {
        HideAllButtons();
        UpdateStatus("Getting 1 PowerUp...");
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var balance = await wallet.GetBalance(chainId: ChainId);
        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
        Debug.Log("balanceEth1: " + balanceEth);
        if (float.Parse(balanceEth) <= 0f)
        {
            UpdateStatus("Not Enough" + notEnoughToken);
            ShowAllButtons();
            return;
        }
        StartCoroutine(WaitAndExecute());
        try
        {
            Claim1Token();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred during the transfer: {ex.Message}");
        }
    }
}
