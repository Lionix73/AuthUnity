using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using TMPro;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] private string url = "https://sid-restapi.onrender.com";

    string token;
    string username;

    void Start()
    {
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

        credentials.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        credentials.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;

        string postData = JsonUtility.ToJson(credentials);

        StartCoroutine(LoginPost(postData));
    }

    public void Registro(){
        Credentials credentials = new Credentials();

        credentials.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        credentials.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;

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
                StartCoroutine(LoginPost(postData));
            }
                
            else 
            {
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

                GameObject.Find("PanelAuth").SetActive(false);

                PlayerPrefs.SetString("token", response.token);
                PlayerPrefs.SetString("username", response.user.username);

                StartCoroutine(GetUsers());
            }
            else{
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

                GameObject.Find("PanelAuth").SetActive(false);

                StartCoroutine(GetUsers());
            }
            else{
                Debug.Log("Error al obtener perfil");
            }
        }
    }

    IEnumerator GetUsers(){
        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Get(url + path);
        www.SetRequestHeader("x-token", token);

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError){
            Debug.Log(www.error);
        }
        else{
            if(www.responseCode == 200){
                string json = www.downloadHandler.text;
                UserList response = JsonUtility.FromJson<UserList>(json);

                UserModel[] leaderboard = response.users.OrderByDescending(u => u.data.score).Take(3).ToArray();
                foreach(UserModel user in response.users){
                    Debug.Log(user.username+"|"+user.data.score);
                }
            }
            else{
                Debug.Log("Error al obtener usuarios");
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError: " + www.downloadHandler.text;
                Debug.Log(mensaje);
            }
        }
    }

    public class Credentials{
        public string username;
        public string password;
    }

    [System.Serializable]
    public class AuthResponse{
        public UserModel user;
        public string token;
    }

    [System.Serializable]
    public class UserModel{
        public string _id;
        public string username;
        public string estado;
        public DataUser data;
    }

    [System.Serializable]
    public class UserList{
        public UserModel[] users;
    }

    [System.Serializable]
    public class DataUser{
        public int score;
    }
}
