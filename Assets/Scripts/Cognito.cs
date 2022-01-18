using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine.Networking;

using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;


public class Cognito : MonoBehaviour
{
    // UI Buttons & Input Fields
    public Button LoginButton;
    public Button SignupButton;
    public InputField EmailField;
    public InputField SignupPasswordField;
    public InputField SignupUsernameField;
    public InputField LoginPasswordField;
    public InputField LoginUsernameField;

    // Token Holder
    public static string jwt;
    public static string access_Token;
    public static string id_token;

    public static string _returnAccessToken;

    bool loginSuccessful;

    // Create an Identity Provider
    AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient
        (new Amazon.Runtime.AnonymousAWSCredentials()/*CredentialsManager.credentials*/, CredentialsManager.region );
    
    


    // Start is called before the first frame update
    void Start()
    {
        LoginButton.onClick.AddListener(Login);
        SignupButton.onClick.AddListener(Signup);

        loginSuccessful = false;
    }


    public void Login()
    {
        _ = Login_User();

        // Load Panels
        MenuManager.Instance.Close_Login_Panel();
        MenuManager.Instance.Load_Recommendations_Panel();

    }

    public void Login(string token)
    {
        /*_ = Login_User(token);*/
        Debug.Log("해당 로직 파괴된 겁니다");
    }

    public void Signup()
    {
        _ = Signup_Method_Async();
    }

    //Method that creates a new Cognito user
    private async Task Signup_Method_Async()
    {
        string userName = SignupUsernameField.text;
        string passWord = SignupPasswordField.text;
        string email = EmailField.text;

        SignUpRequest signUpRequest = new SignUpRequest()
        {
            ClientId = CredentialsManager.appClientId,
            Username = userName,
            Password = passWord            
        };

        List<AttributeType> attributes = new List<AttributeType>()
        {
            new AttributeType(){Name = "email", Value = email}
        };

        signUpRequest.UserAttributes = attributes;

        try
        {
            SignUpResponse request = await provider.SignUpAsync(signUpRequest);
            Debug.Log("Sign up worked");

            // Send Login Event
            Events.Call_Signup();
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e);
            return;
        }
    }

    //Method that signs in Cognito user 
    public async Task Login_User()
    {
        string userName = LoginUsernameField.text;
        string passWord = LoginPasswordField.text;

        CognitoUserPool userPool = new CognitoUserPool(CredentialsManager.userPoolId, CredentialsManager.appClientId, provider);
        
        
        CognitoUser _userTest = userPool.GetUser();
        Debug.Log(_userTest.UserID);

        CognitoUser user = new CognitoUser(userName, CredentialsManager.appClientId, userPool, provider);

        InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
        {
            Password = passWord
        };

        try
        {
            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(true);

            GetUserRequest getUserRequest = new GetUserRequest();
            getUserRequest.AccessToken = authResponse.AuthenticationResult.AccessToken; // jwt token - 그중에 access 토큰이라는 거죠

            Debug.Log("User Access Token: " + getUserRequest.AccessToken);
            jwt = getUserRequest.AccessToken;

            // User is logged in
            loginSuccessful = true;

        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e);
            return;
        }

     

        if (loginSuccessful == true) {

            /*string subId = await Get_User_Id();
            CredentialsManager.userid = subId;*/

            // Send Login Event
            Events.Call_Login();

            // Print UserID
            Debug.Log("Response - User's Sub ID from Cognito: " + CredentialsManager.userid);
           
        }
    }

    public void Login_IdentityPool()
    {
        _ = Login_AsyncIdentityPool();
    }

    public async Task Login_AsyncIdentityPool()
    {
        try
        {
            jwt = access_Token;


            Debug.Log(jwt);

            // User is logged in
            loginSuccessful = true;
            /*CredentialsManager.credentials.GetIdentityIdAsync*/
            /*CredentialsManager.credentials.AddLogin("accounts.google.com", access_Token);*/
            /*Debug.Log("credentials Add Login google");*/

           
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e);
            return;
        }



        if (loginSuccessful == true)
        {
            try
            {
                string subId = await Get_User_Id();
                CredentialsManager.userid = subId;

                /*CognitoUserPool userPool = new CognitoUserPool(CredentialsManager.userPoolId, CredentialsManager.appClientId, provider);
                CognitoUser _userTest = userPool.GetUser(CredentialsManager.userid);*/

                /*string IdentityId = CredentialsManager.credentials.GetIdentityId();
                Debug.Log(IdentityId);*/


                string test = "cognito-idp.ap-northeast-2.amazonaws.com/" + CredentialsManager.userPoolId;
                Debug.Log(test);

                //https://docs.aws.amazon.com/ko_kr/cognito/latest/developerguide/getting-credentials.html

                CredentialsManager.credentials.AddLogin(test, id_token);
                string IdentityId = CredentialsManager.credentials.GetIdentityId();
                /*CredentialsManager.credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityClient));*/
                Debug.Log("hello_____1");
                AmazonCognitoIdentityClient client = new AmazonCognitoIdentityClient(CredentialsManager.credentials, CredentialsManager.region);
                Debug.Log("hello_____2");
                Dictionary<string, string> Logins = new Dictionary<string, string>();
                Logins.Add(test, id_token);
                Debug.Log("hello_____3");
                Debug.Log(IdentityId);
                GetCredentialsForIdentityResponse response = await client.GetCredentialsForIdentityAsync(IdentityId, Logins);
                Debug.Log("hello_____4");
                Credentials _responseCredentials = response.Credentials;
                Debug.Log("hello_____5");
                Debug.Log("hello_____" + _responseCredentials.AccessKeyId);
                _returnAccessToken = _responseCredentials.SessionToken;




                Debug.Log(IdentityId);
                /*CredentialsManager.credentials = new CognitoAWSCredentials(CredentialsManager.identityPool, CredentialsManager.region);

                CredentialsManager.credentials.AddLogin(test, id_token);
                CredentialsManager.credentials.AddLogin(test, access_Token);*/


                /*string identityId = CredentialsManager.credentials.GetIdentityId();
                Debug.Log("IdentityId____________________ " + identityId);*/


                /*CredentialsManager.credentials.GetIdentityIdAsync(delegate(amazoncog))*/




                /*CredentialsManager.credentials.AddLogin("accounts.google.com", jwt);*/
                /*CredentialsManager.credentials.AddLogin("accounts.google.com", id_token);*/

                // Send Login Event
                Events.Call_Login();

                // Print UserID
                Debug.Log("Response - User's Sub ID from Cognito: " + CredentialsManager.userid);

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            
        }
    }


    // Gets a User's sub UUID from Cognito -> user pool에서만
    private async Task<string> Get_User_Id()
    {
        Debug.Log("Getting user's id...");

        string subId = "";        

        // token을 이용하여 user data를 받아옴
        // 여기서가 확인할 사항들을 확인함
        // 자격 증명자에서 로그인 토큰을 설정해주는 부분
        Task <GetUserResponse> responseTask =
            provider.GetUserAsync(new GetUserRequest
            {
                AccessToken = jwt
            });
        

        GetUserResponse responseObject = await responseTask;
        Debug.Log("is listen?");

        // Set User ID
        foreach (var attribute in responseObject.UserAttributes)
        {
            if (attribute.Name == "sub")
            {
                subId = attribute.Value;
                break;
            }                        
        }

        return subId;
    }
    
    private async Task Send_Post()
    {
        WWWForm form = new WWWForm();
        form.AddField("IdentityPoolId", CredentialsManager.identityPool);
        form.AddField("Logins", "cognito-idp.ap-northeast-2.amazonaws.com/" + CredentialsManager.userPoolId);

        string requestPath = "https:// cognito-identity.ap-northeast-2.amazonaws.com/";

        UnityWebRequest webRequest = UnityWebRequest.Post(requestPath, form);
        await webRequest.SendWebRequest();

        if(webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("통신 실패");
        }
        else
        {
            Debug.Log("identityID 받아오기 성공");
            IdentityResultType IdentityId = JsonUtility.FromJson<IdentityResultType>(webRequest.downloadHandler.text);

            Debug.Log("indentity Id_____________________ " + IdentityId.IdentityId);
            Debug.Log("값은? " + webRequest.downloadHandler.text);
        }
    }

}

[System.Serializable]
public class IdentityResultType
{
    public string IdentityId;
}



