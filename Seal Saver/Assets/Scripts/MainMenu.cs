using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject loadingScreen;

    public void Start()
    {
        loadingScreen.SetActive(false);
        Branch.initSession(CallbackWithBranchUniversalObject);
    }

    void CallbackWithBranchUniversalObject(BranchUniversalObject buo,
                                            BranchLinkProperties linkProps,
                                            string error)
    {
        if (error != null)
        {
            System.Console.WriteLine("Error : "
                                    + error);
        }
        else if (linkProps.controlParams.Count > 0)
        {
            System.Console.WriteLine("Deeplink params : "
                                    + buo.ToJsonString()
                                    + linkProps.ToJsonString());
        }
    }

    public void OnSignInPress()
    {
        BranchUniversalObject obj = Branch.getFirstReferringBranchUniversalObject();
        BranchLinkProperties link = Branch.getFirstReferringBranchLinkProperties();
        loadingScreen.SetActive(true);
        SceneManager.LoadScene("Login");
    }

    public void QuitApp()
    {
#if !UNITY_IPHONE
        Application.Quit();
#endif
    }
}
