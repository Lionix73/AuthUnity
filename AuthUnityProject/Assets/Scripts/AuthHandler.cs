using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using TMPro;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] private string url = "https://sid-restapi.onrender.com";
    [SerializeField] private GameObject panelAuth;
    [SerializeField] private GameObject errorBox;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TMP_InputField inputFieldUsername;
    [SerializeField] private TMP_InputField inputFieldPassword;

    public UserModel[] Leaderboard { get; private set; }

    private GameManager gameManager;

    string token;
    string username;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        panelAuth.SetActive(true);
        errorBox.SetActive(false);
        errorText.text = "";

        token = PlayerPrefs.GetString("token");
        username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(username))
        {
            Debug.Log("No hay token");
        }
        else
        {
            StartCoroutine(GetPerfil());
        }
    }

    public void Login(){
        Credentials credentials = new Credentials();

        credentials.username = inputFieldUsername.text;
        credentials.password = inputFieldPassword.text;

        string postData = JsonUtility.ToJson(credentials);

        StartCoroutine(LoginPost(postData));
    }

    public void Registro(){
        Credentials credentials = new Credentials();

        credentials.username = inputFieldUsername.text;
        credentials.password = inputFieldPassword.text;

        string postData = JsonUtility.ToJson(credentials);

        StartCoroutine(RegistroPost(postData));
    }

    IEnumerator RegistroPost(string postData){
        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url+path, postData);

        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                Debug.Log(www.downloadHandler.text);
                errorBox.SetActive(true);
                errorText.color = Color.black;
                errorText.text = "Usuario registrado correctamente!";
            }
            else if (www.responseCode == 400)
            {
                errorBox.SetActive(true);
                errorText.color = Color.red;
                errorText.text = "Error al registrar. Credenciales ya existentes \nO credenciales invalidas";
            }
            else 
            {
                errorBox.SetActive(true);
                errorText.color = Color.red;
                errorText.text = "Error: " + www.downloadHandler.text;

                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError: " + www.downloadHandler.text;
                Debug.Log(mensaje);
            }
        }
    }

    IEnumerator LoginPost(string postData){
        string path = "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url+path, postData);

        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError){
            Debug.Log(www.error);
        }
        else{
            if(www.responseCode == 200){
                string json = www.downloadHandler.text;

                AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);
                Debug.Log("JSON Response: " + json);

                panelAuth.SetActive(false);

                PlayerPrefs.SetString("token", response.token);
                PlayerPrefs.SetString("username", response.usuario.username);

                StartCoroutine(GetUsers());

                gameManager.NewGame();

                errorBox.SetActive(false);
                errorText.text = "";
            }
            else if(www.responseCode == 400){
                errorBox.SetActive(true);
                errorText.color = Color.red;
                errorText.text = "Error al iniciar sesión: Usuario o contraseña incorrectos";
            }
            else{
                errorBox.SetActive(true);
                errorText.color = Color.red;
                errorText.text = "Error: " + www.downloadHandler.text;

                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError: " + www.downloadHandler.text;
                Debug.Log(mensaje);
            }
        }
    }

    IEnumerator GetPerfil(){
        string path = url + "/api/usuarios/" + username;
        UnityWebRequest www = UnityWebRequest.Get(url + path);
        www.SetRequestHeader("x-token", token);

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError){
            Debug.Log(www.error);
        }
        else{
            if(www.responseCode == 200){
                string json = www.downloadHandler.text;
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);
                Debug.Log(response.token);

                panelAuth.SetActive(false);

                StartCoroutine(GetUsers());

                gameManager.NewGame();
            }
            else{
                Debug.Log("Error al obtener perfil");
            }
        }
    }

    public IEnumerator GetUsers(){
        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Get(url + path);
        www.SetRequestHeader("x-token", token);

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError){
            Debug.Log(www.error);
        }
        else{
            if (www.responseCode == 200)
            {
                string json = www.downloadHandler.text;
                UserList response = JsonUtility.FromJson<UserList>(json);

                if (response.usuarios == null)
                {
                    Debug.LogError("response.users is null");
                }
                else
                {
                    Leaderboard = response.usuarios.OrderByDescending(u => u.data.score).ToArray();
                    
                    /*
                    foreach (var user in response.usuarios)
                    {
                        Debug.Log(user.username + "|" + user.data.score);
                    }
                    */
                    
                    gameManager.UpdateScoreboard(Leaderboard);
                }
            }
        }
    }

    public IEnumerator UpdateData(string data){
        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url + path, data);
        
        www.method = "PATCH";
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("x-token", token);

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError){
            Debug.Log(www.error);
        }
        else{

            if (www.responseCode == 200)
            {
                string json = www.downloadHandler.text;
                UserList response = JsonUtility.FromJson<UserList>(json);
                Debug.Log("Update successful: " + json);   
            }
            else{
                Debug.Log("Error al actualizar datos");

                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError: " + www.downloadHandler.text;
                Debug.Log(mensaje);
            }
        }
    }

    public void UpdateScoreFunction(int score)
    {
        UpdateScore newScore = new UpdateScore
        {
            username = PlayerPrefs.GetString("username"),
            data = new DataUser
            {
                score = score
            }
        };

        string newScoreJson = JsonUtility.ToJson(newScore);
        Debug.Log("Sending JSON: " + newScoreJson);

        StartCoroutine(UpdateData(newScoreJson));
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteKey("username");
        Debug.Log("PlayerPrefs cleared");
    }

    public void ShowLoggedInUser()
    {
        string loggedInUsername = PlayerPrefs.GetString("username");
        string loggedInToken = PlayerPrefs.GetString("token");

        if (!string.IsNullOrEmpty(loggedInUsername) && !string.IsNullOrEmpty(loggedInToken))
        {
            Debug.Log("Logged in as: " + loggedInUsername);
        }
        else
        {
            Debug.Log("No user is currently logged in.");
        }
    }

    public class Credentials{
        public string username;
        public string password;
    }

    public class UpdateScore{
        public string username;
        public DataUser data;
    }

    [System.Serializable]
    public class AuthResponse{
        public UserModel usuario;
        public string token;
    }
}
